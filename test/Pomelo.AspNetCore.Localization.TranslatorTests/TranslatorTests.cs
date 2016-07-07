using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Pomelo.AspNetCore.Localization;

namespace Pomelo.AspNetCore.Localization.TranslatorTests
{
    public class TranslatorTests
    {
        [Fact]
        public async void baidu_translator_test()
        {
            var translator = new BaiduTranslator();
            var result = await translator.TranslateAsync("en", "zh", "Hello");
            Assert.Equal("你好", result);
        }
    }
}
