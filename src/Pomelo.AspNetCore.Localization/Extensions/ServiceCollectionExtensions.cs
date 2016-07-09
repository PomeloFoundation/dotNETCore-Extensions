using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Pomelo.AspNetCore.Localization;
using Pomelo.AspNetCore.Localization.Filters;
using Pomelo.AspNetCore.Localization.Listeners;

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
            self.AddScoped<IEntityStateListener, LocalizationEntityStateListener>();
            self.Configure<MvcOptions>(x => x.Filters.Add(typeof(LocalizationFilter)));
            self.Configure<MvcOptions>(x => x.Filters.Add(typeof(DbContextModelBindingFilter)));
            return self;
        }

        public static IServiceCollection AddBaiduTranslator(this IServiceCollection self)
        {
            return self.AddSingleton<ITranslator, BaiduTranslator>();
        }

    }
}
