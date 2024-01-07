using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace BocchiTracker
{
    public class BocchiTrackerLogHook : IDisposable
    {
        private readonly int LOG_MAX_BUFFER = 256;
        private int bufferID = 0;
        private List<List<string>> logBuffer;

        public BocchiTrackerLogHook() 
        {
            Application.logMessageReceived += OnRecived;
            this.logBuffer = new List<List<string>> { new List<string>(), new List<string>() };
        }

        private void OnRecived(string logText, string stackTrace, LogType type)
        {
            this.logBuffer[bufferID].Add(logText);
        }

        public bool GetLogBuffer(out List<string> outMessages)
        {
            //!< 閾値を超えた
            outMessages = null;
            if (this.logBuffer[bufferID].Count > LOG_MAX_BUFFER)
            {
                outMessages = this.logBuffer[bufferID].ToList();
                int previous_id = bufferID;
                Interlocked.Exchange(ref bufferID, (bufferID + 1) % 2);
                this.logBuffer[previous_id].Clear();
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Application.logMessageReceived -= OnRecived;
        }
    }
}
