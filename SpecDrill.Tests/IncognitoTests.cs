using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill;
using SpecDrill.MsTest;
using System;

namespace SomeTests
{
    [TestClass]
    public class IncognitoTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        public void ShouldConfirmBrowserIsInIncognitoMode()
        {
            Browser.ExecuteJavascript(@"
                var fs = window.RequestFileSystem || window.webkitRequestFileSystem;
                if (!fs) alert('CHECK FAILED');
                else
                {
                    fs(window.TEMPORARY, 100, () => { alert('FALSE') }, () => { alert('TRUE') });
                }
            ");

            Wait.NoMoreThan(TimeSpan.FromSeconds(2)).Until(() => Browser.IsAlertPresent);
            Browser.IsAlertPresent.Should().BeTrue();
            Browser.Alert.Text.Should().Contain("TRUE");
            Browser.Alert.Accept();
            Browser.IsAlertPresent.Should().BeFalse();
        }
    }
}
