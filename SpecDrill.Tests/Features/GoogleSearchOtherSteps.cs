//using SomeTests.PageObjects.Test002;
//using SpecDrill.SpecFlow;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TechTalk.SpecFlow;

//namespace SpecDrill.Tests.Features
//{
//    [Binding]
//    class GoogleSearchOtherSteps : UiSpecFlowBase
//    {
//        public GoogleSearchOtherSteps(ScenarioContext scenarioContext, FeatureContext featureContext)
//            : base(scenarioContext, featureContext) { }

//        [Then(@"ReOpen search page")]
//        public void ReOpenSearchPage()
//        {
//            var gsp = Browser.Open<GoogleSearchPage>();
//            gsp.TxtSearch.SendKeys("bye!");
//        }
//    }
//}
