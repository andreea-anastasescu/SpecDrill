using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects;
using SpecDrill;
using System;
using System.Diagnostics;
using SpecDrill.MsTest;
using System.Windows.Markup;
using SpecDrill.Secondary.Ports.AutomationFramework;
using NUnit.Framework;
using SpecDrill.NUnit3;

namespace SomeTests
{
    public class ShadowDomPage : WebPage
    {
        [Find(By.CssSelector, "div#l0shadow>>>")]
        public ShadowDomControlL0 L0ShadowRoot { get; set; }

        [Find(By.CssSelector, "div#l0shadow>>>ul#list li")]
        public ListElement<WebControl> AlphaList { get; set;}
    }
    public class ShadowDomControlL0 : WebControl
    {
        public ShadowDomControlL0(IElement? parent, IElementLocator locator) : base(parent, locator) { }

        [Find(By.CssSelector, "div#l1regular")]
        public IElement DivL1Regular { get; set; } //present only in 1st level shadow dom


        [Find(By.CssSelector, "div#l1shadow>>>")]
        public ShadowDomControlL1 L1ShadowRoot { get; set; }
    }
    public class ShadowDomControlL1 : WebControl
    {
        public ShadowDomControlL1(IElement? parent, IElementLocator locator) : base(parent, locator) { }


        [Find(By.CssSelector, "div#l2regular")]
        public IElement DivL2Regular { get; set; } //present only in 2nd level shadow dom

    }
    [TestFixture]
    public class ShadowDomTests : NUnitBase
    {
        [Test]
        //[ExpectedException(typeof(TimeoutException))]
        public void ShouldCorrectlyIdentifyRegularElementInShadowDom()
        {
            var shadowDomPage = Browser.Open<ShadowDomPage>();
            var element = ElementFactory.Create(null,
                   ElementLocatorFactory.Create(SpecDrill.Secondary.Ports.AutomationFramework.By.CssSelector, "div#l0shadow >>> div#l1regular"));
            element.IsAvailable.Should().BeTrue();
            element.Text.Should().Be("regular2");
        }
        [Test]
        public void ShouldCorrectlyIdentifyShadowRoot()
        {
            var shadowDomPage = Browser.Open<ShadowDomPage>();
            var element = ElementFactory.Create(null,
                   ElementLocatorFactory.Create(SpecDrill.Secondary.Ports.AutomationFramework.By.CssSelector, "div#l0shadow >>>"));
            element.NativeElementSearchResult().Elements.Should().NotBeEmpty();
        }
        [Test]
        public void ShouldCorrectlyIdentifyRegularElementIn2ndLevelShadowDom()
        {
            var shadowDomPage = Browser.Open<ShadowDomPage>();
            var element = ElementFactory.Create(null,
                   ElementLocatorFactory.Create(SpecDrill.Secondary.Ports.AutomationFramework.By.CssSelector, "div#l0shadow >>> div#l1shadow >>> div#l2regular"));
            element.IsAvailable.Should().BeTrue();
            element.Text.Should().Be("regular3");
        }

        [Test]
        public void ShouldCorrectlyIdentifyElementsInMultipleShadowDomLevelsForWebControls()
        {
            var shadowDomPage = Browser.Open<ShadowDomPage>();
            shadowDomPage.L0ShadowRoot.DivL1Regular.IsAvailable.Should().BeTrue();
            shadowDomPage.L0ShadowRoot.DivL1Regular.Text.Should().Be("regular2");
            shadowDomPage.L0ShadowRoot.L1ShadowRoot.DivL2Regular.IsAvailable.Should().BeTrue();
            shadowDomPage.L0ShadowRoot.L1ShadowRoot.DivL2Regular.Text.Should().Be("regular3");
        }

        [Test]
        public void ShouldCorrectlyAccessListInShadowDom()
        {
            var shadowDomPage = Browser.Open<ShadowDomPage>();
            shadowDomPage.AlphaList.Count.Should().Be(3);
            shadowDomPage.AlphaList[1].Text.Should().Be("A");
            shadowDomPage.AlphaList[2].Text.Should().Be("B");
            shadowDomPage.AlphaList[3].Text.Should().Be("C");
        }

    }
}