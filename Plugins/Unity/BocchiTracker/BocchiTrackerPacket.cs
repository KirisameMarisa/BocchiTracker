
//!< Copyright (c) 2023 Yuto Arita

using System.Collections.Generic;
using UnityEngine;
using BocchiTracker.ProcessLinkQuery.Queries;
using System.Xml.Linq;
using Google.FlatBuffers;

namespace BocchiTracker
{
    /// <summary>
    /// Helper class for creating various data packets using FlatBuffers.
    /// </summary>
    public static class CreatePacketHelper
    {
        /// <summary>
        /// Creates a data packet for player position information.
        /// </summary>
        /// <param name="inPlayerPosition">The player's position vector.</param>
        /// <param name="inStage">The current stage or environment identifier.</param>
        /// <returns>The created data packet.</returns>
        public static List<byte> CreatePlayerPosition(Vector3 inPlayerPosition, string inStage)
        {
            var builder = new Google.FlatBuffers.FlatBufferBuilder(1024);

            // Create PlayerPosition object
            var stage = builder.CreateString(inStage);
            var playerPosition = PlayerPosition.CreatePlayerPosition(builder, inPlayerPosition.x, inPlayerPosition.y, inPlayerPosition.z, stage);

            // Create Packet object
            var packet = Packet.CreatePacket(builder, QueryID.PlayerPosition, playerPosition.Value);

            builder.Finish(packet.Value);

            // Convert FlatBuffers data to byte array
            byte[] packetData = builder.SizedByteArray();

            // Prepend packet size
            int packetSize = packetData.Length;
            List<byte> finalPacketData = new List<byte>();
            finalPacketData.AddRange(System.BitConverter.GetBytes(packetSize));
            finalPacketData.AddRange(packetData);

            return finalPacketData;
        }

        /// <summary>
        /// Creates a data packet for application basic information.
        /// </summary>
        /// <returns>The created data packet.</returns>
        public static List<byte> CreateApplicationBasicInformation()
        {
            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            string appName = Application.productName;
            string args = System.Environment.CommandLine;
            string platform = Application.platform.ToString();

            var builder = new Google.FlatBuffers.FlatBufferBuilder(1024);

            // Create AppBasicInfo object
            var appNameOffset = builder.CreateString(appName);
            var argsOffset = builder.CreateString(args);
            var platformOffset = builder.CreateString(platform);

            var appBasicInfo = AppBasicInfo.CreateAppBasicInfo(builder, pid, appNameOffset, argsOffset, platformOffset);

            // Create Packet object
            var packet = Packet.CreatePacket(builder, QueryID.AppBasicInfo, appBasicInfo.Value);

            builder.Finish(packet.Value);

            // Convert FlatBuffers data to byte array
            byte[] packetData = builder.SizedByteArray();

            // Prepend packet size
            int packetSize = packetData.Length;
            List<byte> finalPacketData = new List<byte>();
            finalPacketData.AddRange(System.BitConverter.GetBytes(packetSize));
            finalPacketData.AddRange(packetData);

            return finalPacketData;
        }

        /// <summary>
        /// Creates a data packet for screenshot information.
        /// </summary>
        /// <param name="inWidth">The width of the screenshot image.</param>
        /// <param name="inHeight">The height of the screenshot image.</param>
        /// <param name="inData">The raw data of the screenshot image.</param>
        /// <returns>The created data packet.</returns>
        public static List<byte> CreateScreenshotData(int inWidth, int inHeight, byte[] inData)
        {
            var builder = new Google.FlatBuffers.FlatBufferBuilder(1024);

            // Create ScreenshotData object
            var dataOffset = ScreenshotData.CreateDataVector(builder, inData);
            var screenshotData = ScreenshotData.CreateScreenshotData(builder, inWidth, inHeight, dataOffset);

            // Create Packet object
            var packet = Packet.CreatePacket(builder, QueryID.ScreenshotData, screenshotData.Value);

            builder.Finish(packet.Value);

            // Convert FlatBuffers data to byte array
            byte[] packetData = builder.SizedByteArray();

            // Prepend packet size
            int packetSize = packetData.Length;
            List<byte> finalPacketData = new List<byte>();
            finalPacketData.AddRange(System.BitConverter.GetBytes(packetSize));
            finalPacketData.AddRange(packetData);

            return finalPacketData;
        }

        public static List<byte> CreateLogData(List<string> inLog)
        {
            var builder = new Google.FlatBuffers.FlatBufferBuilder(1024);

            // Create ScreenshotData object
            var logOffsets = new StringOffset[inLog.Count];
            for (int i = 0; i < inLog.Count; i++)
            {
                logOffsets[i] = builder.CreateString(inLog[i]);
            }
            var dataOffset = LogData.CreateLogVector(builder, logOffsets);
            var logdata = LogData.CreateLogData(builder, dataOffset);

            // Create Packet object
            var packet = Packet.CreatePacket(builder, QueryID.LogData, logdata.Value);

            builder.Finish(packet.Value);

            // Convert FlatBuffers data to byte array
            byte[] packetData = builder.SizedByteArray();

            // Prepend packet size
            int packetSize = packetData.Length;
            List<byte> finalPacketData = new List<byte>();
            finalPacketData.AddRange(System.BitConverter.GetBytes(packetSize));
            finalPacketData.AddRange(packetData);

            return finalPacketData;
        }
    }
}