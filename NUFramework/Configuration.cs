using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Threading;


namespace SpecflowBDD.TAF.NUFramework
{
    /// <summary>
    /// Helper class for managing configuration settings.
    /// </summary>
    public class Configuration
    {
        public static int TimeoutSeconds { get { return Convert.ToInt16(GetSetting("timeout_sec")); } }

        /// <summary>
        /// Get configuration setting. Environment variables override app.config.
        /// Environment-specifc settings override appSettings.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSetting(string name)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }

            string environmentName = Environment.GetEnvironmentVariable("DEF.ENVIRONMENT");
            if (String.IsNullOrEmpty(environmentName))
            {
                environmentName = ConfigurationManager.AppSettings["environment"];
            }
            if (environmentName != null)
            {
                if (name == "environment")      // special case - environment will never exist in a section
                {
                    return environmentName;
                }
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                var sectionName = "environment" + textInfo.ToTitleCase(environmentName);
                var environmentSection = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
                if (environmentSection == null)
                {
                    throw new ArgumentException(String.Format("No configuration section for environment {0} [{1}]", environmentName, sectionName));
                }
                if (environmentSection[name] != null)
                {
                    return environmentSection[name].ToString();
                }
            }
            return ConfigurationManager.AppSettings[name];
        }

        /// <summary>
        /// Return configuration setting for the specified environment.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEnvironmentSetting(string environment, string name)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            var sectionName = "environment" + textInfo.ToTitleCase(environment);
            var environmentSection = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            if (environmentSection == null)
            {
                throw new ArgumentException(String.Format("No configuration section for environment {0} [{1}]", environment, sectionName));
            }
            if (environmentSection[name] == null)
            {
                throw new ArgumentException(String.Format("No such configuration property '{0}' environment {1}", name, environment));
            }
            return environmentSection[name].ToString();
        }
    }


}
