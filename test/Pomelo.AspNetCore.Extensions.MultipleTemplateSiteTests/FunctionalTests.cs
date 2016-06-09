using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.AspNetCore.Mvc.Testing;
using Xunit;

namespace Pomelo.AspNetCore.Extensions.MultipleTemplateSiteTests
{
    public class FunctionalTests : MvcTestFixture<MultiTemplateSite.Startup>
    {
        [Fact]
        public async void default_template_test()
        {
            var res = await Client.GetAsync("/");
            var ret = await res.Content.ReadAsStringAsync();
            Assert.True(ret.IndexOf("<!-- This is default template -->") >= 0);
        }

        [Fact]
        public async void cookie_template_test()
        {
            Client.DefaultRequestHeaders.Add("Cookie", "ASPNET_TEMPLATE=Test");
            var res = await Client.GetAsync("/");
            var ret = await res.Content.ReadAsStringAsync();
            Assert.True(ret.IndexOf("<!-- This is test template -->") >= 0);
        }
    }
}
