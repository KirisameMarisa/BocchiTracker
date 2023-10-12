#include "bocchi_godot.h"
#include "Query_generated.h"

#include <string>

bocchi_godot* bocchi_godot::singleton = nullptr;

std::string GODOT_TO_STD_STRING(const String inGodotStr) {
    return std::string(inGodotStr.utf8().get_data());
}

void bocchi_godot::_bind_methods()
{
    ClassDB::bind_method(D_METHOD("createPlayerPosition"), &bocchi_godot::createPlayerPosition);
    ClassDB::bind_method(D_METHOD("createApplicationBasicInformation"), &bocchi_godot::createApplicationBasicInformation);
}

bocchi_godot* bocchi_godot::get_singleton()
{
    return singleton;
}

bocchi_godot::bocchi_godot()
{
    ERR_FAIL_COND(singleton != nullptr);
    singleton = this;
}

bocchi_godot::~bocchi_godot()
{
    ERR_FAIL_COND(singleton != this);
    singleton = nullptr;
}

PackedByteArray bocchi_godot::createPlayerPosition(const Vector3& inPlayerPosition, const String& inStage) 
{
    flatbuffers::FlatBufferBuilder builder;

    // Create PlayerPosition object
    auto stageOffset = builder.CreateString(GODOT_TO_STD_STRING(inStage));
    auto playerPositionOffset = BocchiTracker::ProcessLinkQuery::Queries::CreatePlayerPosition(
        builder, inPlayerPosition.x, inPlayerPosition.y, inPlayerPosition.z, stageOffset
    );

    // Create Packet object
    auto packet = BocchiTracker::ProcessLinkQuery::Queries::CreatePacket(
        builder, BocchiTracker::ProcessLinkQuery::Queries::QueryID::QueryID_PlayerPosition, playerPositionOffset.Union()
    );

    builder.Finish(packet);

    // Convert FlatBuffers data to Array
    const uint8_t* bufferPointer = builder.GetBufferPointer();
    int32_t bufferSize = builder.GetSize();

    PackedByteArray packetData;
    packetData.resize(bufferSize);

    memcpy(packetData.ptrw(), bufferPointer, bufferSize);

    // Prepend packet size
    int32_t packetSize = packetData.size();
    PackedByteArray finalPacketData;
    finalPacketData.resize(packetSize + sizeof(int32_t));

    memcpy(finalPacketData.ptrw(), reinterpret_cast<const uint8_t*>(&packetSize), sizeof(int32_t));
    memcpy(finalPacketData.ptrw(), packetData.ptr(), packetSize);

    return finalPacketData;
}

PackedByteArray bocchi_godot::createApplicationBasicInformation(int32_t inPID, const String& inAppName, const String& inArgs, const String& inPlatform) 
{
    flatbuffers::FlatBufferBuilder builder;

    // Create AppBasicInfo object
    auto appNameOffset = builder.CreateString(GODOT_TO_STD_STRING(inAppName));
    auto argsOffset = builder.CreateString(GODOT_TO_STD_STRING(inArgs));
    auto platformOffset = builder.CreateString(GODOT_TO_STD_STRING(inPlatform));

    auto appBasicInfoOffset = BocchiTracker::ProcessLinkQuery::Queries::CreateAppBasicInfo(
        builder, inPID, appNameOffset, argsOffset, platformOffset
    );

    // Create Packet object
    auto packet = BocchiTracker::ProcessLinkQuery::Queries::CreatePacket(
        builder, BocchiTracker::ProcessLinkQuery::Queries::QueryID::QueryID_AppBasicInfo, appBasicInfoOffset.Union()
    );

    builder.Finish(packet);

    // Convert FlatBuffers data to Array
    const uint8_t* bufferPointer = builder.GetBufferPointer();
    int32_t bufferSize = builder.GetSize();
    
    PackedByteArray packetData;
    packetData.resize(bufferSize);
    memcpy(packetData.ptrw(), bufferPointer, bufferSize);

    // Prepend packet size
    int32_t packetSize = packetData.size();
    PackedByteArray finalPacketData;
    finalPacketData.resize(packetSize + sizeof(int32_t));

    memcpy(finalPacketData.ptrw(), reinterpret_cast<const uint8_t*>(&packetSize), sizeof(int32_t));
    memcpy(finalPacketData.ptrw(), packetData.ptr(), packetSize);

    return finalPacketData;
}