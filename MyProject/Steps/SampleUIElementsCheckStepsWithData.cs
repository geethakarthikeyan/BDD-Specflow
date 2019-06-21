using BoDi;
using SpecflowBDD.TAF.MyProject.Model;
using SpecflowBDD.TAF.MyProject.Pages;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace SpecflowBDD.TAF.MyProject.Steps
{
    [Binding]
    class SampleUIElementsCheckStepsWithData
    {
        private readonly UIElementsPage uIElementsPage;
        private readonly UserDetails userDetails;
        public SampleUIElementsCheckStepsWithData(IObjectContainer container, UIElementsPage uIElementsPage)
        {
            this.uIElementsPage = uIElementsPage;
            JObject json = container.Resolve<JObject>("ultimateqaData");
            userDetails = ((JObject)json["FillNameAndEmail"]).ToObject<UserDetails>();
        }
        [When(@"The user fills Name and Email address")]
        public void WhenTheUserFillsNameAndEmailAddress()
        {
            uIElementsPage.FillNameTextBox(userDetails.Name);
            uIElementsPage.FillEmailTextBox(userDetails.Email);
        }
    }
}
