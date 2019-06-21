using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;


namespace SpecflowBDD.TAF.NUFramework
{

    public class BasePage : WebTest
    {
        protected new Log log;
        protected new IWebDriver driver;
        protected WebDriverWait wait;

        #region Locators        
        protected By LinkToNextPart = By.PartialLinkText("Next -");
        protected By ContinueLink = By.PartialLinkText("Continue");
        #endregion

        public enum YesNo { Yes, No, Undefined };

        public BasePage(IWebDriver driver)
        {
            this.driver = driver;
            log = Log.Instance;
        }

        /// <summary>
        /// Enter value into edit field if value is not empty.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void Enter(IWebElement field, string value)
        {
            if (String.IsNullOrEmpty(value) || field.GetAttribute("class") == "disabled")
            {
                return;
            }
            field.SendKeys(value);
        }

        /// <summary>
        /// Enter date into three separate elements for day, month and year.
        /// </summary>
        /// <param name="date">Date string delimitted by '/' or space</param>
        /// <param name="dateDay"></param>
        /// <param name="dateMonth"></param>
        /// <param name="dateYear"></param>
        public void EnterSplitDate(string date, IWebElement dateDay, IWebElement dateMonth, IWebElement dateYear)
        {
            if (String.IsNullOrEmpty(date))
            {
                return;
            }
            char[] separators = { '/', ' ' };
            string[] dates = date.Split(separators);
            dateDay.SendKeys(dates[0]);
            dateMonth.SendKeys(dates[1]);
            dateYear.SendKeys(dates[2]);
        }

        /// <summary>
        /// If value is not empty, clear field and enter value.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void ClearAndEnter(IWebElement field, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return;
            }
            field.Clear();
            field.SendKeys(value);
        }

        /// <summary>
        /// Wait for element to be found and visible. Reports time taken to log.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public IWebElement Find(By by)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var element = wait.Until(x => x.FindElement(by));
            element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
            stopwatch.Stop();
            log.Info(String.Format("Found element [{0}] after {1}ms", by.ToString(), stopwatch.ElapsedMilliseconds));
            return element;
        }

        /// <summary>
        /// Wait for elements to be found and visible. Reports time taken to log.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public IList<IWebElement> FindElements(By by)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
            var elements = wait.Until(x => x.FindElements(by));
            stopwatch.Stop();
            log.Info(String.Format("Found element [{0}] after {1}ms", by.ToString(), stopwatch.ElapsedMilliseconds));
            return elements;
        }

        /// <summary>
        /// Find the first matching child element.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="by"></param>
        /// <returns>null if no element found</returns>
        public IWebElement FindElementIfExists(IWebElement parent, By by)
        {
            var elements = parent.FindElements(by);
            return (elements.Count >= 1) ? elements.First() : null;
        }

        /// <summary>
        /// Determine if text is present anywhere on the web page.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool PageContainsText(string text)
        {
            return driver.FindElement(By.TagName("body")).Text.Contains(text);
        }

        /// <summary>
        /// Static wait method. Should only be used as a temporary work-around for synchronisation issues.
        /// </summary>
        /// <param name="milliseconds">Wait time</param>
        public void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Wait for browser ready state and any Ajax (JQuery) events to complete.
        /// </summary>
        public void WaitForBrowser()
        {
            Thread.Sleep(10);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var js = (IJavaScriptExecutor)driver;
            Func<IWebDriver, bool> readyCondition = webDriver => (bool)js.ExecuteScript("return (document.readyState == 'complete' && jQuery.active == 0)");
            wait.Until(readyCondition);
            stopwatch.Stop();
            log.Info(String.Format("WaitForBrowser waited {0}ms", stopwatch.ElapsedMilliseconds));
        }


        /// <summary>
        /// Wait for the specified file to appear in the downloads folder.
        /// </summary>
        public void WaitForDownload(string filename)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!File.Exists(filename) && stopwatch.ElapsedMilliseconds < wait.Timeout.TotalMilliseconds)
            {
                System.Threading.Thread.Sleep(100);
            }
            stopwatch.Stop();
            if (File.Exists(filename))
            {
                log.Info(String.Format("WaitForDownload waited {0}ms for {1}", stopwatch.ElapsedMilliseconds, filename));
            }
            else
            {
                throw new TimeoutException(String.Format("WaitForDownload file {0} did not appear within {1}ms", filename, stopwatch.ElapsedMilliseconds));
            }
        }

        /// <summary>
        /// Wait for ASP.NET postback event to complete.
        /// </summary>
        public void WaitForPostback()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var js = (IJavaScriptExecutor)driver;
            Func<IWebDriver, bool> readyCondition = webDriver => (bool)js.ExecuteScript("return Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack() == false");
            wait.Until(readyCondition);
            stopwatch.Stop();
            log.Info(String.Format("WaitForPostback waited {0}ms", stopwatch.ElapsedMilliseconds));
        }

        /// <summary>
        /// Click an element if it appeares after the specified number of seconds.
        /// </summary>
        /// <param name="by"></param>
        /// <param name="timeoutSeconds"></param>
        public void ClickIfExist(By by, int timeoutSeconds)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var waitForExist = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                var element = waitForExist.Until(x => x.FindElement(by));
                waitForExist.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element)).Click();
                stopwatch.Stop();
                log.Info(String.Format("ClickIfExist(). Clicked optional element [{0}] after {1}ms", by.ToString(), stopwatch.ElapsedMilliseconds));
            }
            catch
            {
                stopwatch.Stop();
                log.Info(String.Format("ClickIfExist(). Optional element skipped [{0}] after {1}ms", by.ToString(), stopwatch.ElapsedMilliseconds));
            }
        }
        /// <summary>
        /// Method to click on elements when clickable 
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public void ClickWhenClickable(By by)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by)).Click();
        }

        /// <summary>
        /// Method to click on elements when clickable 
        /// </summary>
        /// <param name="webelement"></param>
        /// <returns></returns>
        public void ClickWhenClickable(IWebElement element)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element)).Click();
        }

        /// <summary>
        /// Method to select the element based on value when clickable 
        /// </summary>
        public void SelectWhenClickable(IWebElement element, string value)
        {
            new SelectElement(wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element))).SelectByText(value);
        }

        /// <summary>
        /// Method to type value when enabled
        /// </summary>
        /// <param name="IWebElement"></param>
        /// <returns></returns>
        public void SendKeysWhenEnabled(IWebElement element, string value)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element)).SendKeys(value);
        }

        /// <summary>
        /// Method to type value if not empty
        /// </summary>
        /// <param name="IWebElement"></param>
        /// <returns></returns>
        public void SendKeysIfNotEmpty(By by, string value)
        {
            if (String.IsNullOrEmpty(value))
                return;
            SendKeysWhenEnabled(Find(by), value);
        }

        /// <summary>
        /// Method to fetch date value
        /// </summary>
        /// <param name="IWebElement"></param>
        /// <returns></returns>
        public string GetListingDate(string statement)
        {
            string strEnd = Regex.Replace(statement, "^.*[0-9]* *day[s]", "");
            string strStart = Regex.Replace(statement, "[0-9]* *day[s].*", "");
            string days0 = Regex.Replace(statement, strStart, "");
            string days1 = Regex.Replace(days0, "days *" + strEnd, "");
            return String.Format("{0:dd/MM/yyyy}", DateTime.Today.AddDays(Convert.ToInt16(days1)));
        }

        /// <summary>
        /// Method to fetch Date and time stamp value
        /// </summary>
        /// <param name="IWebElement"></param>
        /// <returns></returns>
        public string GetTimestampCode(DateTime timeStamp)
        {
            return timeStamp.Day.ToString() + timeStamp.Month.ToString() + timeStamp.Year.ToString()
               + timeStamp.Hour.ToString() + timeStamp.Minute.ToString() + timeStamp.Second.ToString();
        }

        /// <summary>
        /// Click the required element based on a value of 'yes' or 'no'.
        /// </summary>
        /// <param name="yesOrNo"></param>
        /// <param name="yesElement"></param>
        /// <param name="noElement"></param>
        /// <returns></returns>
        public YesNo ClickYesNo(string yesOrNo, IWebElement yesElement, IWebElement noElement)
        {
            if (String.IsNullOrEmpty(yesOrNo))
            {
                return YesNo.Undefined;
            }
            switch (yesOrNo.ToLower())
            {
                case "yes":
                case "y":
                    yesElement.Click();
                    return YesNo.Yes;
                case "no":
                case "n":
                    noElement.Click();
                    return YesNo.No;
                default:
                    throw new ArgumentException("Invalid yes/no value -- " + yesOrNo);
            }
        }

        /// <summary>
        /// Click on a radio button given the attribute and the option
        /// </summary>
        /// <param name="radioButtons">radio buttons as a list of IWebElements</param>
        /// <param name="attribute">attribute that needs to be compared</param>
        /// <param name="radioOption">radio button option to click</param>
        public void SelectRB(IList<IWebElement> radioButtons, string attribute, string radioOption)
        {
            int optionCount = radioButtons.Count();
            if (radioOption != null)
            {
                for (int counter = 0; counter < optionCount; counter++)
                {
                    String radioButtontext = radioButtons.ElementAt(counter).GetAttribute(attribute);
                    if (radioButtontext.ToLower().Equals(radioOption.ToLower()))
                    {
                        radioButtons.ElementAt(counter).Click();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Clicks an IWebElement given a list
        /// </summary>
        /// <param name="wElements">list of IWebElements</param>
        /// <param name="text">option to click</param>
        public void ClickWebElement(IList<IWebElement> wElements, string text)
        {
            int listCount = wElements.Count();
            if (text != null)
            {
                for (int counter = 0; counter < listCount; counter++)
                {
                    if (wElements.ElementAt(counter).Text.ToLower().Equals(text.ToLower()))
                    {
                        wElements.ElementAt(counter).Click();
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Method to Hover and Click Menu Item
        /// </summary>
        /// <param name="IWebElement"></param>
        /// <returns></returns>
        public void HoverAndClickMenuItem(IWebElement menuItem, IWebElement iwe)
        {
            Actions action = new Actions(driver);
            action.MoveToElement((IWebElement)menuItem).MoveToElement((IWebElement)iwe).Click().Build().Perform();
        }

        /// <summary>
        /// Determine if an element is visible.
        /// </summary>
        /// <param name="by">by</param>
        /// <returns>bool</returns>
        public bool IsElementVisible(By by)
        {
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to fetch Page Title
        /// </summary>
        public string PageTitle()
        {
            return driver.Title;
        }

        /// <summary>
        /// Method to verify Page Title
        /// </summary>
        public bool VerifyPageTitle(string title)
        {
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TitleContains(title));
        }

        /// <summary>
        /// Method to click ok on Alert popup
        /// </summary>
        public void AlertPopupAccept()
        {
            driver.SwitchTo().Alert().Accept();

        }

        
        /// <summary>Return download status of a file</summary>
        /// <returns>boolean</returns>
        public bool VerifyFileDownload(string name)
        {
            var downloadFile = Path.Combine(GetArchivedDownloadsFolder(), name);

            for (int i = 0; i < 700; i++)
            {
                if (File.Exists(downloadFile))
                {
                    return true;
                }
                Thread.Sleep(20);
            }
            return false;

        }

        /// <summary>
        /// Download to an archived folder after deleting any previously existing file/s
        /// </summary>
        /// <returns></returns>
        public void Download(IWebElement downloadElement, string filename, string downloadArchiveFolder)
        {
            var downloadsFolder = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads");
            var downloadFile = Path.Combine(downloadsFolder, filename);
            //  var alternateDownloadsFolder = DatacomTAF.UltimateQA.Properties.UltimateQAUser.Default.downloadsPath;
            var alternatedownloadFile = "";

            if (File.Exists(downloadFile))
            {
                File.Delete(downloadFile);
            }
            downloadElement.Click(); // Event that triggers the download
            try
            {
                WaitForDownload(downloadFile);
            }
            catch (TimeoutException)
            {
                downloadFile = alternatedownloadFile;
                WaitForDownload(downloadFile);
            }
            Directory.CreateDirectory(downloadArchiveFolder);
            File.Copy(downloadFile, Path.Combine(downloadArchiveFolder, filename), true);
            File.Delete(downloadFile);  // keep the users download folder clean
        }

        /// <summary>Display status of second element when first element is clicked</summary>
        /// <returns>boolean</returns>
        public bool ClickAndDisplay(By elm1, By elm2)
        {
            Find(elm1).Click();
            return Find(elm2).Displayed;
        }

        /// <summary>Selection of Checkbox using JS Executor </summary>
        public void ClickChkBoxUsingJS(By locator, YesNo yesNo)
        {
            IWebElement elm = Find(locator);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            if (yesNo == YesNo.No && elm.Selected)
            {
                js.ExecuteScript("arguments[0].click()", elm); //if checkbox already selected then clears checkbox
            }

            if (yesNo == YesNo.Yes && !elm.Selected)
            {
                js.ExecuteScript("arguments[0].click()", elm); //if checkbox not selected then selects checkbox
            }
        }


        /// <summary>Selection of Radio button using JS Executor </summary>
        public YesNo ClickYesNoUsingJS(string yesOrNo, IWebElement yesElement, IWebElement noElement)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            if (String.IsNullOrEmpty(yesOrNo))
            {
                return YesNo.Undefined;
            }
            switch (yesOrNo.ToLower())
            {
                case "yes":
                case "y":
                    js.ExecuteScript("arguments[0].click()", yesElement);
                    return YesNo.Yes;
                case "no":
                case "n":
                    js.ExecuteScript("arguments[0].click()", noElement);
                    return YesNo.No;
                default:
                    throw new ArgumentException("Invalid yes/no value -- " + yesOrNo);
            }
        }

    }
}
