
//!< Copyright (c) 2023 Yuto Arita

using UnityEngine;

namespace BocchiTracker
{
    /// <summary>
    /// Monitors the player's position and sends updates to the BocchiTrackerSystem if the player's movement distance exceeds a certain threshold.
    /// </summary>
    public class BocchiTrackerPlayerPositionUpdater : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float distanceThreshold = 100.0f; // Minimum distance to trigger a position update
        public float DistanceThreshold => distanceThreshold;

        [SerializeField]
        private string stage = "";
        public string Stage => stage;

        private BocchiTrackerSystem system;
        private Vector3 previousPosition;

        void Start()
        {
            // Find the BocchiTrackerSystem instance in the scene
            system = FindObjectOfType<BocchiTrackerSystem>();
        }

        void Update()
        {
            if (system == null)
                return;

            if (!system.IsConnect())
                return;

            Vector3 currentPosition = transform.position;

            // Calculate the distance between the current position and the previous position
            float distance = Vector3.Distance(currentPosition, previousPosition);

            // Check if the movement distance exceeds the threshold
            if (distance >= DistanceThreshold)
            {
                // Create a player position packet and send it to the BocchiTrackerSystem
                var playerPositionPacket = CreatePacketHelper.CreatePlayerPosition(currentPosition, Stage);
                system.BocchiTrackerSendPacket(playerPositionPacket);

                // Update the previous position to the current position
                previousPosition = currentPosition;
            }

            if (system.JumpRequest.TryDequeue(out var request))
            {
                transform.position = request.Location;
            }
        }
    }
}
