using BoDi;
using SpecflowBDD.TAF.NUFramework;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;
using SpecflowBDD.TAF.MyProject.Properties;


namespace SpecflowBDD.TAF.MyProject.Setup
{
    /// <summary>
    /// Setup for Test Initiation
    /// </summary>
    [Binding]
    class ApplicationRunSetup : QATest
    {
        private IObjectContainer objectContainer;
        private WebDriverWait wait;
        
        public ApplicationRunSetup(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        [BeforeScenario(Order = 0)]
        public void InitialSetup()
        {
            log = Log.Instance;
            log.Info("=================Starting initial setup=================");
            Initialize();

            WebTest wt = new WebTest();
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Set("obj", wt);
            wt.SetUp();
            driver = wt.driver;

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Convert.ToInt16(Settings.Default.timeout_Sec)));

            objectContainer.RegisterInstanceAs<IWebDriver>(driver);
            objectContainer.RegisterInstanceAs<WebDriverWait>(wait);
            objectContainer.RegisterInstanceAs(log);

            driver.Navigate().GoToUrl(UltimateQAUrl);
            driver.Manage().Window.Maximize();
        }

        [BeforeScenario(Order = 20)]
        public void ULTIMATEQAData()
        {
            var scenarioContext = objectContainer.Resolve<ScenarioContext>();

            if (!scenarioContext.ScenarioInfo.Tags.Contains("NoData"))
            {
                var dataId = scenarioContext.ScenarioInfo.Tags.First();

                Console.WriteLine("Test data: " + dataId);

                var defaultDataFilePath = Path.Combine(SolutionDirectory, "Data", dataId + ".json");
                Console.WriteLine("Test data file path: " + defaultDataFilePath);

                var testData = File.ReadAllText(defaultDataFilePath);
                var json = JObject.Parse(testData);

                objectContainer.RegisterInstanceAs<JObject>(json, "ultimateqaData");
            }
        }

        [AfterStep(Order = 0)]
        public void CaptureScreenshot()
        {
            var scenarioContext = objectContainer.Resolve<ScenarioContext>();

            if (scenarioContext.TestError != null)
            {
                var screenshotFilename = scenarioContext.ScenarioInfo.Title + ".jpg";
                var screesnhotFileDir = Path.Combine(SolutionDirectory, "Reports");
                var screenshotFilePath = Path.GetFullPath(Path.Combine(screesnhotFileDir, screenshotFilename));

                try
                {
                    var screenshot = new StitchedScreenshot();
                    screenshot.Save(driver, screenshotFilePath, ImageFormat.Jpeg);
                }
                catch
                {
                    var screenshot = new DesktopScreenshot();
                    screenshot.CaptureScreenToFile(screenshotFilePath, ImageFormat.Jpeg);
                }
            }
        }

        [AfterScenario]
        public void TearDwn()
        {
            WebTest wt = (WebTest)TestContext.CurrentContext.Test.Properties.Get("obj");
            wt.TearDown();
        }
    }

}
