using SpecflowBDD.TAF.MyProject.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace SpecflowBDD.TAF.MyProject.Steps
{
    [Binding]
    class SampleUIElementsCheckSteps
    {
        private readonly UIElementsPage uIElementsPage;
        public SampleUIElementsCheckSteps(UIElementsPage uIElementsPage)
        {
            this.uIElementsPage = uIElementsPage;
        }
        [Given(@"Navigate to UltimateQA Website")]
        public void GivenNavigateToUltimateQAWebsite()
        {
            uIElementsPage.NavigateToUltimateQA();
        }

        [When(@"Click on Raise Button")]
        public void WhenClickOnRaiseButton()
        {
            uIElementsPage.clickRaiseButton();
        }

        [Then(@"The UltimateQA Website homepage should be opened")]
        public void ThenTheUltimateQAWebsiteHomepageShouldBeOpened()
        {
            Assert.AreEqual(uIElementsPage.PageTitle(), "Home - Ultimate QA");
        }

        [When(@"The user clicks on Email me button")]
        public void WhenTheUserClicksOnEmailMeButton()
        {
            uIElementsPage.ClickEmailMEButton();
        }

        [Then(@"The User gets Thank you message")]
        public void ThenTheUserGetsThankYouMessage()
        {
            Assert.IsTrue(uIElementsPage.VerifySuccessMessage());
        }

        [When(@"The user Selects Female Radio Button")]
        public void WhenTheUserSelectsFemaleRadioButton()
        {
            uIElementsPage.selectFemaleRB();
        }

        [Then(@"The user select bike check box")]
        public void ThenTheUserSelectBikeCheckBox()
        {
            uIElementsPage.SelectBikeCheckBox();
        }

        [Then(@"The user verifies that Female Radio button got selected")]
        public void ThenTheUserVerifiesThatFemaleRadioButtonGotSelected()
        {
            Assert.IsTrue(uIElementsPage.IsFemaleRBSelected());
        }

        [Then(@"The user verifies the bike checkbox got selected")]
        public void ThenTheUserVerifiesTheBikeCheckboxGotSelected()
        {
            Assert.IsTrue(uIElementsPage.IsBikeChkBoxSelected());
        }
        [When(@"The user Selects opel value from dropdown")]
        public void WhenTheUserSelectsOpelValueFromDropdown()
        {
            uIElementsPage.SelectOpelFromDropdown();
        }

        [Then(@"The user verifies that selected dropdown value is opel")]
        public void ThenTheUserVerifiesThatSelectedDropdownValueIsOpel()
        {
            //   uIElementsPage.DropdownOption();
        }

    }
}
