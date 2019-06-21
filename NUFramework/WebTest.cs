using SpecflowBDD.TAF.NUFramework;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow;


namespace SpecflowBDD.TAF.NUFramework
{
    public class WebTest
    {

        public Log log;
        protected Configuration configuration;
        public IWebDriver driver;
        protected CaptureData captureData;
        protected bool iterationMode;
        protected bool iterationModeAbortOnFail;
        public int iterations;
        public int currentIteration;
        public TestContext TestContext { get; set; }
        public bool LeaveBrowserOpen { get; set; }

        public WebTest()
        {
        }


        public void SetUp()
        {
            log = Log.Instance;
            log.Info(String.Format("=================Starting Test {0}=================", TestContext.CurrentContext.Test.Name));
            iterationMode = false;
            iterationModeAbortOnFail = false;
            iterations = GetIterations();
            currentIteration = 1;
            captureData = new CaptureData();
            LaunchBrowser();
        }


        public void TearDown()
        {
            if (!iterationMode)
            {
                Cleanup(0, TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed);
            }
        }

        /// <summary>
        /// Capture screen shot and close down browser.
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="hasPassed"></param>
        public void Cleanup(int iteration, bool hasPassed)
        {
            log.Info("=======================End Of Test======================");
            string testName = TestContext.CurrentContext.Test.Name;
            if (iteration > 0)
            {
                testName += "_" + iteration;
            }
            if (!hasPassed)
            {
                var screenshotFilename = testName + ".jpg";
                var screesnhotFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Reports");
                var screenshotFilePath = Path.GetFullPath(Path.Combine(screesnhotFileDir, screenshotFilename));

                TakeStitchedScreenShot(screenshotFilePath);
            }
            if (!LeaveBrowserOpen)
            {
                CloseBrowser();
            }
            if (captureData.HasData)
            {
                captureData.TestId = testName;
                captureData.Export(TestContext.CurrentContext.Test.Name + ".txt");
            }
        }

        /// <summary>
        /// Launch the browser defined by the 'browser' configuration setting.
        /// </summary>
        public void LaunchBrowser()
        {
            var browser = "";
            browser = TAF.NUFramework.Properties.Settings.Default.browser;
            driver = GetWebDriver(browser);
        }

        public void CloseBrowser()
        {
            if (this.driver != null)
            {
                this.driver.Quit();
                this.driver.Dispose();
            }
        }

        public IWebDriver GetWebDriver(string browser)
        {
            //string driverFolder = Path.Combine(GetRootFolder(), "drivers"); // TODO: change to get driver from NUGet package
            string driverFolder = GetBinFolder();
            log.Info(String.Format("Browser={0}", browser));
            switch (browser.ToLower())
            {
                case "chrome":
                    return new ChromeDriver(driverFolder);
                case "ie":
                    return new InternetExplorerDriver(driverFolder);
                case "headless_chrome":
                    ChromeOptions options = new ChromeOptions();
                    options.AddArguments("headless");
                    options.AddArguments("window-size=1200x800");
                    return new ChromeDriver(driverFolder, options);
                default:
                    throw new ArgumentException("Invalid browser -- " + browser);
            }
        }

        public string GetRootFolder()
        {
            return new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).Parent.Parent.Parent.FullName;
        }

        public string GetBinFolder()
        {
            return new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName;
        }
        public string GetArchivedDownloadsFolder()
        {
            return String.Format(@"{0}\Downloads\{1}\{2}", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), TestContext.CurrentContext.Test.Name, currentIteration);
        }

        public void TakeScreenShot(string name)
        {
            Screenshot screenShot = ((ITakesScreenshot)driver).GetScreenshot();
            var filename = name + ".jpg";
            log.Info(String.Format("Taking screenshot: {0}", filename));
            screenShot.SaveAsFile(filename, ScreenshotImageFormat.Jpeg);
        }

        public void TakeStitchedScreenShot(string filepath)
        {
            log.Info(String.Format("Taking stitched screenshot: {0}", filepath));
            try
            {
                var screenshot = new StitchedScreenshot();
                screenshot.Save(driver, filepath, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                log.Warning("Failed to take screenshot: " + ex.Message);
            }
        }

        /// <summary>
        /// Get the number of iterations for a bulk run, as defined by the DEF.ITERATIONS environment variable.
        /// </summary>
        /// <returns></returns>
        public int GetIterations()
        {
            string value = Environment.GetEnvironmentVariable("DEF.ITERATIONS");
            if (!String.IsNullOrEmpty(value))
            {
                return Int32.Parse(value);
            }
            return 1;
        }

        /// <summary>
        /// Run the specified scenario in bulk mode, for the configured number of iterations.
        /// </summary>
        /// <param name="instance">object containing the scenario method</param>
        /// <param name="methodName">scenario method name</param>
        /// <param name="testId">testId, which is passed as a parameter to <paramref name="methodName"/></param>
        public void Execute(object instance, string methodName, string testId)
        {
            log.Info(String.Format("RunScenario: name={0}, test={1}", methodName, testId));
            var method = instance.GetType().GetMethod(methodName);
            object[] parameters = new object[] { testId };

            if (iterations == 1)
            {
                method.Invoke(instance, parameters);
                return;
            }

            iterationMode = true;
            bool passed;
            int passCount = 0;
            for (int i = 1; i <= iterations; ++i)
            {
                passed = true;
                currentIteration = i;
                log.Info("iteration: " + i);
                if (i > 1)
                {
                    LaunchBrowser();
                }
                try
                {
                    method.Invoke(instance, parameters);
                    ++passCount;
                }
                catch (TargetInvocationException ex)
                {
                    log.Error(ex.InnerException.Message);
                    log.Error(ex.InnerException.StackTrace);
                    passed = false;
                }
                Cleanup(i, passed);
                if (!passed && iterationModeAbortOnFail)
                {
                    Assert.Fail(String.Format("Aborting after {0}/{1} iterations", i, iterations));
                }
            }
            Assert.AreEqual(iterations, passCount, String.Format("{0}/{1} iterations failed", iterations - passCount, iterations));
        }

    }
}
