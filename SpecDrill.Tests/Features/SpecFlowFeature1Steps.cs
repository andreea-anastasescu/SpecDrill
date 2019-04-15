//using System;
//using TechTalk.SpecFlow;
//using SpecDrill.SpecFlow.MsTest;
//using SomeTests.PageObjects.Test001;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using SpecDrill;

//namespace SomeTests.SpecFlow.Tests
//{
//    [Binding]
//    public class SpecFlowFeature1Steps : SpecFlowTestBase
//    {
//        Test001CalculatorPage calculatorPage = null;
//        [Given(@"user opens the calculator page")]
//        public void GivenUserOpensTheCalculatorPage()
//        {
//           calculatorPage =  Browser.Open<Test001CalculatorPage>();
//        }
        
//        [Given(@"I have entered (.*) into operand (.*)")]
//        public void GivenIHaveEnteredIntoOperand(int operandValue, int operand)
//        {
//            if (operand == 1)
//                    calculatorPage.TxtOperand1.SendKeys(operandValue.ToString());
                    
//            if (operand == 2)
//                    calculatorPage.TxtOperand2.SendKeys(operandValue.ToString());
//        }
        
//        [When(@"I press add")]
//        public void WhenIPressAdd()
//        {
//            calculatorPage.BtnAdd.Click();
//        }
        
//        [Then(@"the result should be (.*) in result field")]
//        public void ThenTheResultShouldBeInResultField(int expectedResult)
//        {
//            Assert.AreEqual(expectedResult, Int32.Parse(calculatorPage.TxtResult.GetAttribute("value")));
//        }
//    }
//}
