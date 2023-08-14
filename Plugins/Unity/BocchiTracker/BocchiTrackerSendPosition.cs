using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BocchiTracker
{
    public class BocchiTrackerSendPosition : MonoBehaviour
    {
        private BocchiTrackerSystem system;
        private Vector3 previousPosition;

        void Start()
        {
            system = FindObjectOfType<BocchiTrackerSystem>();
        }

        public void Send(Vector3 inTrackedPosition, string inStage)
        {
            Vector3 currentPosition = inTrackedPosition;

            float distance = Vector3.Distance(currentPosition, previousPosition);
            if (distance >= 100.0f)
            {
                var playerPositionPacket = CreatePacketHelper.CreatePlayerPosition(currentPosition, inStage);
                system.BocchiTrackerSendPacket(playerPositionPacket);
                previousPosition = currentPosition;
            }
        }
    }
}
