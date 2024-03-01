
//!< Copyright (c) 2023 Yuto Arita

using UnityEngine;

namespace BocchiTracker
{
    public class BocchiTrackerSetting : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string serverAddress = "127.0.0.1";
        public string ServerAddress => serverAddress;

        [SerializeField]
        private int serverPort = 8888;
        public int ServerPort => serverPort;

        [SerializeField]
        private int websocketPort = serverPort + 1;
        public int WebSocketPort => websocketPort;

        [SerializeField]
        private Camera captureCamera = null;
        public Camera CaptureCamera => captureCamera;
    }
}
