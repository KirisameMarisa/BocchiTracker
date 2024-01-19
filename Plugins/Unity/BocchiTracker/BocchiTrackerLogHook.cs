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
        private readonly int LOG_MAX_BUFFER = 2048;
        private int bufferID = 0;
        private List<string> logBuffer;

        public BocchiTrackerLogHook() 
        {
            Application.logMessageReceived += OnRecived;
            this.logBuffer = new List<string> { "", "" };
        }

        private void OnRecived(string logText, string stackTrace, LogType type)
        {
            this.logBuffer[bufferID] += logText + "\n";
        }

        public bool GetLogBuffer(out string outMessages)
        {
            //!< 閾値を超えた
            outMessages = null;
            if (this.logBuffer[bufferID].Length > LOG_MAX_BUFFER)
            {
                outMessages = this.logBuffer[bufferID];
                int previous_id = bufferID;
                Interlocked.Exchange(ref bufferID, (bufferID + 1) % 2);
                this.logBuffer[previous_id] = "";
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
