using System;
using Pomelo.AspNetCore.Localization;
using Pomelo.AspNetCore.Localization.Filters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPomeloLocalization(this IServiceCollection self, Action<MemoryCultureSet> InitCultureSource = null)
        {
            var set = new MemoryCultureSet();
            InitCultureSource?.Invoke(set);
            self.AddMemoryCache();
            self.AddContextAccessor();
            self.AddSingleton<ICultureSet>(set);
            self.AddScoped<ICultureProvider, DefaultCultureProvider>();
            self.AddScoped<IStringReader, DefaultStringReader>();
            self.AddSingleton<ITranslator, NonTranslator>();
            self.AddSingleton<ITranslatedCaching, MemoryTranslatedCaching>();
            self.AddScoped<ITranslatorDisabler, DefaultTranslatorDisabler>();
            return self;
        }

        public static IServiceCollection AddBaiduTranslator(this IServiceCollection self)
        {
            return self.AddSingleton<ITranslator, BaiduTranslator>();
        }

        public static IMvcBuilder AddDynamicLocalizer(this IMvcBuilder self)
        {
            return self.AddMvcOptions(x => x.Filters.Add(typeof(LocalizationFilter)));
        }
    }
}
