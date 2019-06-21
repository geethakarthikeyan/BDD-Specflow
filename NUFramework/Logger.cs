using NLog;


namespace SpecflowBDD.TAF.NUFramework
{

    /// <summary>
    /// Generic logging functionality (singleton).
    /// </summary>
    public class Log
    {
        private Logger nlog { get; set; }
        private static Log instance;

        private Log()
        {
            nlog = LogManager.GetCurrentClassLogger();
        }

        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log();
                }
                return instance;
            }
        }

        public void Error(string message)
        {
            nlog.Error(message);
        }

        public void Info(string message)
        {
            nlog.Info(message);
        }

        public void Warning(string message)
        {
            nlog.Warn(message);
        }
    }
}
