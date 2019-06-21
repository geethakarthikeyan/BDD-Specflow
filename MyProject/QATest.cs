using SpecflowBDD.TAF.MyProject.Properties;
using SpecflowBDD.TAF.NUFramework;
using NUnit.Framework;
using System;
using System.IO;


namespace SpecflowBDD.TAF.MyProject
{
    [TestFixture]
    public class QATest : WebTest
    {
        public string UltimateQAUrl { get; set; }
        public readonly string SolutionDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..");

        public void Initialize()
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            try
            {
                UltimateQAUrl = Settings.Default.url;
                log.Info(String.Format("ultimateqa_url = {0}", UltimateQAUrl));
            }
            catch (Exception ex)
            {
                log.Error("Failed to configure test environment. " + ex.Message);
                throw ex;
            }

        }
    }
}
