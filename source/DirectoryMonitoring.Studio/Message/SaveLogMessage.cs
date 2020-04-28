using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryMonitoring.Studio.Message
{
    internal class SaveLogMessage
    {
        public SaveLogMessage(string savePath)
        {
            SavePath = savePath;
        }

        public string SavePath { get; }
    }
}
