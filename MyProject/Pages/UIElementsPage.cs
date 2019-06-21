using SpecflowBDD.TAF.NUFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;


namespace SpecflowBDD.TAF.MyProject.Pages
{
    class UIElementsPage : BasePage
    {
        #region Locators
        private readonly By RaiseButton = By.XPath("//button[text()='Raise']");
        private readonly By NameTextBox = By.Id("et_pb_contact_name_0");
        private readonly By EmailTextBox = By.Name("et_pb_contact_email_0");
        private readonly By EmailMeButton = By.XPath("//button[text()='Email Me!']");
        private readonly By ThanksMsg = By.ClassName("et-pb-contact-message");
        private readonly By RadioButtons = By.XPath("//input[@type='radio']");
        private readonly By FemaleRadioButton = By.XPath("//input[@type='radio' and @value='female']");
        private readonly By BikeCheckBox = By.XPath("//input[@type='checkbox' and @value='Bike']");
        private readonly By DropDownElement = By.XPath("//div[@class='et_pb_blurb_description']/select");

        #endregion

        #region Locator Methods

        #endregion

        public UIElementsPage(IWebDriver driver, WebDriverWait wait) : base(driver)
        {
            this.driver = driver;
            this.wait = wait;
        }

        #region IWebElements

        #endregion

        #region Methods
        public void clickRaiseButton()
        {
            Find(RaiseButton).Click();
        }

        public void NavigateToUltimateQA()
        {
            driver.Navigate().GoToUrl("https://www.ultimateqa.com/simple-html-elements-for-automation/");
        }

        public void FillNameTextBox(string name)
        {
            Enter(Find(NameTextBox), name);
        }

        public void FillEmailTextBox(string email)
        {
            Enter(Find(EmailTextBox), email);
        }

        public void ClickEmailMEButton()
        {
            Find(EmailMeButton).Click();
        }

        public bool VerifySuccessMessage()
        {
            return IsElementVisible(ThanksMsg);
        }
        #endregion
        public void selectFemaleRB()
        {
            SelectRB(FindElements(RadioButtons), "value", "Female");
        }

        public bool IsFemaleRBSelected()
        {
            bool status = false;
            status = Find(FemaleRadioButton).Selected;
            return status;
        }

        public void SelectBikeCheckBox()
        {
            Find(BikeCheckBox).Click();
        }

        public bool IsBikeChkBoxSelected()
        {
            bool status = false;
            status = Find(BikeCheckBox).Selected;
            return status;
        }

        public void SelectOpelFromDropdown()
        {
            SelectWhenClickable(Find(DropDownElement), "Opel");
            Thread.Sleep(2000);
        }

        public void DropdownOption()
        {
            string DropDownValue = Find(DropDownElement).GetAttribute("value").ToString();
            Assert.AreEqual(DropDownValue, "opel");
        }
    }

}
