﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Test000;
using SpecDrill;

namespace SomeTests
{
    [TestClass]
    public class SpecDrillTests : TestBase
    {
        [TestMethod]
        public void ShouldOpenBrowserWhenHomepageIsOpened()
        {
            var virtualStoreLoginPage = Browser.Open<Test000LoginPage>();

            Wait.Until(() => virtualStoreLoginPage.IsLoadCompleted);

            virtualStoreLoginPage.TxtUserName.SendKeys("cosmin");
            virtualStoreLoginPage.TxtPassword.SendKeys("abc123");

            virtualStoreLoginPage.DdlCountry.SelectByValue("md");
            Assert.AreEqual("Moldova", virtualStoreLoginPage.DdlCountry.SelectedOptionText);

            virtualStoreLoginPage.DdlCity.SelectByText("Chisinau");
            Assert.AreEqual("Chisinau", virtualStoreLoginPage.DdlCity.SelectedOptionText);
            
            virtualStoreLoginPage.DdlCountry.SelectByIndex(1);
            Assert.AreEqual("Romania", virtualStoreLoginPage.DdlCountry.SelectedOptionText);
            
            var homePage = virtualStoreLoginPage.BtnLogin.Click();
            
            Assert.AreEqual("Virtual Store - Home", Browser.PageTitle);
        }

        //TODO: Create Hover tests on css hover menu with at least 2 levels
    }
}