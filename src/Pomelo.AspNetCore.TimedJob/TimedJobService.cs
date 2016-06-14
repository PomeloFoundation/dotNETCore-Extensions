using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AspNetCore.TimedJob.Jobs;

namespace Pomelo.AspNetCore.TimedJob
{
    public class TimedJobService
    {
        private IAssemblyLocator locator { get; set; }

        private IServiceProvider services { get; set; }

        private IDynamicTimedJobProvider dynamicJobs { get; set; }

        private ILogger logger { get; set; }

        public Dictionary<string, bool> JobStatus { get; private set; } = new Dictionary<string, bool>();

        public Dictionary<string, Timer> JobTimers { get; private set; } = new Dictionary<string, Timer>();

        private List<TypeInfo> JobTypeCollection { get; set; } = new List<TypeInfo>();

        private int lcm { get; set; } = 1000;

        public TimedJobService(IAssemblyLocator locator, IServiceProvider services)
        {
            this.services = services;
            this.locator = locator;
            this.logger = services.GetService<ILogger>();
            this.dynamicJobs = services.GetService<IDynamicTimedJobProvider>();
            var asm = locator.GetAssemblies();
            foreach (var x in asm)
            {
                // 查找基类为Job的类
                var types = x.DefinedTypes.Where(y => y.BaseType == typeof(Job)).ToList();
                foreach (var y in types)
                {
                    JobTypeCollection.Add(y);
                }
            }
            StartHardTimers();
            if (dynamicJobs != null)
                StartDynamicTimers();
        }

        private void StartHardTimers()
        {
            foreach (var x in JobTypeCollection)
            {
                foreach (var y in x.DeclaredMethods)
                {
                    if (y.GetCustomAttribute<NonJobAttribute>() == null)
                    {
                        JobStatus.Add(x.FullName + '.' + y.Name, false);
                        var invoke = y.GetCustomAttribute<InvokeAttribute>();
                        if (invoke != null && invoke.IsEnabled)
                        {
                            int delta = Convert.ToInt32((invoke._begin - DateTime.Now).TotalMilliseconds);
                            if (delta < 0)
                            {
                                delta = delta % invoke.Interval;
                                if (delta < 0)
                                    delta += invoke.Interval;
                            }
                            Task.Factory.StartNew(() =>
                            {
                                var timer = new Timer(t => {
                                    Execute(x.FullName + '.' + y.Name);
                                }, null, delta, invoke.Interval);
                                JobTimers.Add(x.FullName + '.' + y.Name, timer);
                            });
                        }
                    }
                }
            }
        }

        private void StartDynamicTimers()
        {
            var jobs = dynamicJobs.GetJobs();
            foreach (var x in jobs)
            {
                // 如果Hard Timer已经启动则注销实例
                if (JobTimers.ContainsKey(x.Id))
                {
                    JobTimers[x.Id].Dispose();
                    JobStatus[x.Id] = false;
                    JobTimers.Remove(x.Id);
                    JobStatus.Remove(x.Id);
                }
                int delta = Convert.ToInt32((x.Begin - DateTime.Now).TotalMilliseconds);
                if (delta < 0)
                {
                    delta = delta % x.Interval;
                    if (delta < 0)
                        delta += x.Interval;
                }
                Task.Factory.StartNew(() =>
                {
                    var timer = new Timer(t => {
                        Execute(x.Id);
                    }, null, delta, x.Interval);
                    JobTimers.Add(x.Id, timer);
                });
            }
        }

        public void RestartDynamicTimers()
        {
            var jobs = dynamicJobs.GetJobs();
            foreach (var x in jobs)
            {
                if (JobTimers.ContainsKey(x.Id))
                {
                    JobTimers[x.Id].Dispose();
                    JobStatus[x.Id] = false;
                    JobTimers.Remove(x.Id);
                    JobStatus.Remove(x.Id);
                }
            }
            StartDynamicTimers();
        }

        public bool Execute(string identifier)
        {
            var typename = identifier.Substring(0, identifier.LastIndexOf('.'));
            var function = identifier.Substring(identifier.LastIndexOf('.') + 1);
            var type = JobTypeCollection.SingleOrDefault(x => x.FullName == typename);
            if (type == null)
            {
                throw new NotImplementedException(typename + "." + function);
            }
            var argtypes = type.GetConstructors()
                .First()
                .GetParameters()
                .Select(x =>
                {
                    if (x.ParameterType == typeof(IServiceProvider))
                        return services;
                    else
                        return services.GetService(x.ParameterType);
                })
                .ToArray();
            var job = Activator.CreateInstance(type.AsType(), argtypes);
            var method = type.GetMethod(function);
            var paramtypes = method.GetParameters().Select(x => services.GetService(x.ParameterType)).ToArray();
            var invokeAttr = method.GetCustomAttribute<InvokeAttribute>();
            lock (this)
            {
                if (invokeAttr != null && invokeAttr.SkipWhileExecuting && JobStatus[identifier])
                    return false;
                JobStatus[identifier] = true;
            }
            try
            {
                if (logger != null)
                    logger.LogInformation("Invoking " + identifier + "...");
                method.Invoke(job, paramtypes);
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.LogError(ex.ToString());
            }
            JobStatus[identifier] = false;
            return true;
        }

        public List<string> GetJobFunctions()
        {
            var ret = new List<string>();
            foreach (var x in JobTypeCollection)
            {
                foreach (var y in x.DeclaredMethods)
                {
                    if (y.GetCustomAttribute<NonJobAttribute>() == null)
                    {
                        ret.Add(x.FullName + '.' + y.Name);
                    }
                }
            }
            return ret;
        }
    }
}
