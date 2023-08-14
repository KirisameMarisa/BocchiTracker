using BocchiTracker.ProcessLinkQuery.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BocchiTracker
{
    public static class CreatePacketHelper
    {
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
    }
}
