using System;
using System.Collections.Generic;
using System.IO;


namespace SpecflowBDD.TAF.NUFramework
{

    /// <summary>
    /// Capture run time data.
    /// </summary>
    public class CaptureData
    {
        string testId;
        Dictionary<string, string> data;

        public bool HasData { get { return data.Count > 0; } }
        public string TestId { get { return testId; } set { testId = value; } }

        public CaptureData()
        {
            this.testId = null;
            data = new Dictionary<string, string>();
        }

        public CaptureData(string testId) : this()
        {
            this.testId = testId;
        }

        public void Save(string name, string value)
        {
            data.Add(name, value);
        }

        public string Load(string name)
        {
            return data[name];
        }

        public void Export(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, true))
            {
                if (!String.IsNullOrEmpty(testId))
                {
                    sw.WriteLine("TestId={0}", testId);
                }
                foreach (KeyValuePair<string, string> entry in data)
                {
                    sw.WriteLine("{0}={1}", entry.Key, entry.Value);
                }
            }
        }
    }


}
