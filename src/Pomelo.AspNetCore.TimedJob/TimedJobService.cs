using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Pomelo.AspNetCore.TimedJob.Jobs;

namespace Pomelo.AspNetCore.TimedJob
{
    public class TimedJobService
    {
        private IAssemblyLocator locator { get; set; }

        private IServiceProvider services { get; set; }

        public Dictionary<string, bool> JobStatus { get; private set; } = new Dictionary<string, bool>();

        private List<TypeInfo> JobTypeCollection { get; set; } = new List<TypeInfo>();

        private int lcm { get; set; } = 1000;

        public TimedJobService(IAssemblyLocator locator, IServiceProvider services)
        {
            this.services = services;
            this.locator = locator;
            var asm = locator.GetAssemblies();
            var intervals = new List<int>();
            foreach(var x in asm)
            {
                // 查找基类为Job的类
                var types = x.DefinedTypes.Where(y => y.BaseType == typeof(Job)).ToList();
                foreach (var y in types)
                {
                    JobTypeCollection.Add(y);
                    // 遍历类中public方法
                    foreach (var z in y.DeclaredMethods)
                    {
                        if (z.GetCustomAttribute<NonJobAttribute>() == null)
                        {
                            JobStatus.Add(y.FullName + '.' + z.Name, false);
                            var invoke = z.GetCustomAttribute<InvokeAttribute>();
                            if (invoke != null)
                            {
                                intervals.Add(invoke.Interval);
                            }
                        }
                    }
                }
            }
        }

        public bool Execute(string identifier)
        {
            var typename = identifier.Substring(0, identifier.LastIndexOf('.'));
            var function = identifier.Substring(identifier.LastIndexOf('.'));
            var type = JobTypeCollection.Single(x => x.FullName == typename);
            var argtypes = type.GetConstructors().First().GetGenericArguments().Select(x => services.GetService(x)).ToArray();
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
            method.Invoke(job, paramtypes);
            JobStatus[identifier] = false;
            return true;
        }
    }
}
