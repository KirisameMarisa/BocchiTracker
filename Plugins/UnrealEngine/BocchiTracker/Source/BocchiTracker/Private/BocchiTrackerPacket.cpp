#include "BocchiTrackerPacket.h"

#include "flatbuffers/flatbuffers.h"
#include "Query_generated.h"

TArray<uint8> CreatePacketHelper::CreatePlayerPosition(const FVector &inPlayerPosition, const FString &inStage)
{
    flatbuffers::FlatBufferBuilder builder;

    // Create PlayerPosition object
    auto stage = builder.CreateString(TCHAR_TO_UTF8(*inStage));
    float x = inPlayerPosition.X;
    float y = inPlayerPosition.Y;
    float z = inPlayerPosition.Z;
    
    auto playerPosition = BocchiTracker::ProcessLinkQuery::Queries::CreatePlayerPosition(
        builder, x, y, z, stage
    );

    // Create Packet object
    auto packet = BocchiTracker::ProcessLinkQuery::Queries::CreatePacket(
        builder, BocchiTracker::ProcessLinkQuery::Queries::QueryID::QueryID_PlayerPosition, playerPosition.Union()
    );

    builder.Finish(packet);

    // Convert FlatBuffers data to TArray<uint8>
    TArray<uint8> PacketData;
    PacketData.Append(reinterpret_cast<const uint8_t*>(builder.GetBufferPointer()), builder.GetSize());

    // Prepend packet size
    int32 PacketSize = PacketData.Num();
    TArray<uint8> FinalPacketData;
    FinalPacketData.Append(reinterpret_cast<const uint8_t*>(&PacketSize), sizeof(int32));
    FinalPacketData.Append(PacketData);

    return FinalPacketData;
}

TArray<uint8> CreatePacketHelper::CreateApplicationBsicInformation()
{
    int32 Pid = FPlatformProcess::GetCurrentProcessId();
    FString AppName = FApp::GetProjectName();
    FString Args = FCommandLine::Get();
    FString Platform = FPlatformProperties::IniPlatformName();

    flatbuffers::FlatBufferBuilder builder;

    // Create AppBasicInfo object
    auto appName = builder.CreateString(TCHAR_TO_UTF8(*AppName));
    auto args = builder.CreateString(TCHAR_TO_UTF8(*Args));
    auto platform = builder.CreateString(TCHAR_TO_UTF8(*Platform));

    auto appBasicInfo = BocchiTracker::ProcessLinkQuery::Queries::CreateAppBasicInfo(
        builder, Pid, appName, args, platform
    );

    // Create Packet object
    auto packet = BocchiTracker::ProcessLinkQuery::Queries::CreatePacket(
        builder, BocchiTracker::ProcessLinkQuery::Queries::QueryID::QueryID_AppBasicInfo, appBasicInfo.Union()
    );

    builder.Finish(packet);

    // Convert FlatBuffers data to TArray<uint8>
    TArray<uint8> PacketData;
    PacketData.Append(reinterpret_cast<const uint8_t*>(builder.GetBufferPointer()), builder.GetSize());

    // Prepend packet size
    int32 PacketSize = PacketData.Num();
    TArray<uint8> FinalPacketData;
    FinalPacketData.Append(reinterpret_cast<const uint8_t*>(&PacketSize), sizeof(int32));
    FinalPacketData.Append(PacketData);

    return FinalPacketData;
}

TArray<uint8> CreatePacketHelper::CreateScreenshotData(int inWidth, int inHeight, const TArray<uint8>& inData)
{
    flatbuffers::FlatBufferBuilder builder;

    // Create ScreenshotData object
    auto dataOffset = builder.CreateVector(inData.GetData(), inData.Num());
    auto screenshotData = BocchiTracker::ProcessLinkQuery::Queries::CreateScreenshotData(
        builder, inWidth, inHeight, dataOffset
    );

    // Create Packet object
    auto packet = BocchiTracker::ProcessLinkQuery::Queries::CreatePacket(
        builder, BocchiTracker::ProcessLinkQuery::Queries::QueryID_ScreenshotData, screenshotData.Union()
    );

    builder.Finish(packet);

    // Convert FlatBuffers data to TArray<uint8>
    TArray<uint8> PacketData;
    PacketData.Append(reinterpret_cast<const uint8_t*>(builder.GetBufferPointer()), builder.GetSize());

    // Prepend packet size
    int32 PacketSize = PacketData.Num();
    TArray<uint8> FinalPacketData;
    FinalPacketData.Append(reinterpret_cast<const uint8_t*>(&PacketSize), sizeof(int32));
    FinalPacketData.Append(PacketData);

    return FinalPacketData;
}
