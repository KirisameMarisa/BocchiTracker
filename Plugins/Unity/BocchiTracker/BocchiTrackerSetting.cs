using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BocchiTracker
{
    public class BocchiTrackerSetting : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string serverAddress = "";
        public string ServerAddress => serverAddress;

        [SerializeField]
        private int serverPort = 8888;
        public int ServerPort => serverPort;
    }
}
