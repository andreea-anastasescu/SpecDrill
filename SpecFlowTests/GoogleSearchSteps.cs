using System;
using TechTalk.SpecFlow;

namespace SpecFlowTests
{
    [Binding]
    public class GoogleSearchSteps
    {
        [Given(@"I have entered ""(.*)"" into Google search")]
        public void GivenIHaveEnteredIntoGoogleSearch(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I press Search button")]
        public void WhenIPressSearchButton()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"You should get a ""(.*)"" entry in search results")]
        public void ThenYouShouldGetAEntryInSearchResults(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
