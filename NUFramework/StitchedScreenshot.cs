using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using OpenQA.Selenium;


namespace SpecflowBDD.TAF.NUFramework
{
    public class StitchedScreenshot
    {
        /// <summary>
        /// Workaround for creating a screenshot of the whole browser screen in Chromedriver.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="filename"></param>
        /// <param name="imageFormat"></param>
        public void Save(IWebDriver driver, string filename, ImageFormat imageFormat)
        {
            Bitmap stitchedImage = null;
            var js = (IJavaScriptExecutor)driver;
            {
                js.ExecuteScript("return window.scrollTo(0,0)");

                // Get the Total Size of the Document
                var totalWidth = (int)((long)js.ExecuteScript("return document.body.clientWidth"));
                var totalHeight = (int)((long)js.ExecuteScript("return document.body.clientHeight"));
                // Get the Size of the Viewport
                var viewportWidth = (int)((long)js.ExecuteScript("return document.documentElement.clientWidth"));
                var viewportHeight = (int)((long)js.ExecuteScript("return document.documentElement.clientHeight"));

                // Split the Screen in multiple Rectangles
                var rectangles = new List<Rectangle>();
                // Loop until the Total Height is reached
                for (var i = 0; i < totalHeight; i += viewportHeight)
                {
                    var newHeight = viewportHeight;
                    // Fix if the Height of the Element is too big
                    if (i + viewportHeight > totalHeight)
                    {
                        newHeight = totalHeight - i;
                    }
                    // Loop until the Total Width is reached
                    for (var ii = 0; ii < totalWidth; ii += viewportWidth)
                    {
                        var newWidth = viewportWidth;
                        // Fix if the Width of the Element is too big
                        if (ii + viewportWidth > totalWidth)
                        {
                            newWidth = totalWidth - ii;
                        }

                        // Create and add the Rectangle
                        var currRect = new Rectangle(ii, i, newWidth, newHeight);
                        rectangles.Add(currRect);
                    }
                }

                // Build the Image
                stitchedImage = new Bitmap(totalWidth, totalHeight);
                // Get all Screenshots and stitch them together
                var previous = Rectangle.Empty;
                foreach (var rectangle in rectangles)
                {
                    // Calculate the Scrolling (if needed)
                    if (previous != Rectangle.Empty)
                    {
                        var xDiff = rectangle.Right - previous.Right;
                        var yDiff = rectangle.Bottom - previous.Bottom;
                        // Scroll
                        //selenium.RunScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        Thread.Sleep(100);
                    }

                    // Take Screenshot
                    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                    // Build an Image out of the Screenshot
                    Image screenshotImage;
                    using (var memStream = new MemoryStream(screenshot.AsByteArray))
                    {
                        screenshotImage = Image.FromStream(memStream);
                    }

                    // Calculate the Source Rectangle
                    var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);

                    // Copy the Image
                    using (var g = Graphics.FromImage(stitchedImage))
                    {
                        g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }

                    // Set the Previous Rectangle
                    previous = rectangle;
                }
                stitchedImage.Save(filename, imageFormat);
            }
        }

    }
}
