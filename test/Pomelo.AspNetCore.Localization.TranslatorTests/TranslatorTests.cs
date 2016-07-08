using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Pomelo.AspNetCore.Localization;

namespace Pomelo.AspNetCore.Localization.TranslatorTests
{
    public class TranslatorTests
    {
        [Fact]
        public async void baidu_translator_test()
        {
            var disabler = new Mock<ITranslatorDisabler>();
            disabler.Setup(x => x.IsDisabled())
                .Returns(false);
            var translator = new BaiduTranslator(disabler.Object);
            var result = await translator.TranslateAsync("en", "zh", "Hello");
            Assert.Equal("你好", result);
        }
    }
}
