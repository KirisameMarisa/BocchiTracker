#include "BocchiAPI.h"
#include "Query_generated.h"

#include <string>

using namespace BocchiTracker::ProcessLinkQuery::Queries;

BocchiAPI* BocchiAPI::instance = nullptr;

VARIANT_ENUM_CAST(QueryID)

std::string GODOT_TO_STD_STRING(const String inGodotStr) {
    return std::string(inGodotStr.utf8().get_data());
}

void BocchiAPI::_bind_methods()
{
    ClassDB::bind_method(D_METHOD("create_player_position",                 "player_position", "stage"), 
        &BocchiAPI::createPlayerPosition);
    ClassDB::bind_method(D_METHOD("create_application_basic_information",   "pid", "app_name", "args", "platform"), 
        &BocchiAPI::createApplicationBasicInformation);
    ClassDB::bind_method(D_METHOD("create_screenshot_data",                 "width", "height", "screenshot_data"), 
        &BocchiAPI::create_screenshot_data);
    ClassDB::bind_method(D_METHOD("resolve_request_query",                  "packet"), 
        &BocchiAPI::resolve_request_query);

    BIND_ENUM_CONSTANT(QueryID_NONE)
    BIND_ENUM_CONSTANT(QueryID_AppBasicInfo)
    BIND_ENUM_CONSTANT(QueryID_PlayerPosition)
    BIND_ENUM_CONSTANT(QueryID_ScreenshotData)
    BIND_ENUM_CONSTANT(QueryID_ScreenshotRequest)
    BIND_ENUM_CONSTANT(QueryID_JumpRequest)
    BIND_ENUM_CONSTANT(QueryID_IssueesRequest)
}

BocchiAPI* BocchiAPI::get_instance()
{
    return instance;
}

BocchiAPI::BocchiAPI()
{
    ERR_FAIL_COND(instance != nullptr);
    instance = this;
}

BocchiAPI::~BocchiAPI()
{
    ERR_FAIL_COND(instance != this);
    instance = nullptr;
}

PackedByteArray BocchiAPI::createPlayerPosition(const Vector3& inPlayerPosition, const String& inStage) 
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
    memcpy(finalPacketData.ptrw() + sizeof(int32_t), packetData.ptr(), packetSize);

    return finalPacketData;
}

PackedByteArray BocchiAPI::createApplicationBasicInformation(uint32_t inPID, const String& inAppName, const String& inArgs, const String& inPlatform) 
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
    memcpy(finalPacketData.ptrw() + sizeof(int32_t), packetData.ptr(), packetSize);

    return finalPacketData;
}

PackedByteArray BocchiAPI::create_screenshot_data(uint32_t inWidth, uint32_t inHeight, const PackedByteArray &inScreenshotData)
{
    flatbuffers::FlatBufferBuilder builder;

    // Create ScreenshotData object
    auto dataOffset = builder.CreateVector(inScreenshotData.ptr(), inScreenshotData.size());
    auto screenshotData = BocchiTracker::ProcessLinkQuery::Queries::CreateScreenshotData(
        builder, inWidth, inHeight, dataOffset
    );

    // Create Packet object
    auto packet = BocchiTracker::ProcessLinkQuery::Queries::CreatePacket(
        builder, BocchiTracker::ProcessLinkQuery::Queries::QueryID_ScreenshotData, screenshotData.Union()
    );

    builder.Finish(packet);

    // Convert FlatBuffers data to Array
    const uint8_t* bufferPointer = builder.GetBufferPointer();
    int32_t bufferSize = builder.GetSize();

    // Convert FlatBuffers data to TArray<uint8>
    PackedByteArray packetData;
    packetData.resize(bufferSize);
    memcpy(packetData.ptrw(), bufferPointer, bufferSize);

    // Prepend packet size
    int32_t packetSize = packetData.size();
    PackedByteArray finalPacketData;
    finalPacketData.resize(packetSize + sizeof(int32_t));
 
    memcpy(finalPacketData.ptrw(), reinterpret_cast<const uint8_t*>(&packetSize), sizeof(int32_t));
    memcpy(finalPacketData.ptrw() + sizeof(int32_t), packetData.ptr(), packetSize);
    
    return finalPacketData;
}

Dictionary BocchiAPI::resolve_request_query(const PackedByteArray& inPacket)
{
    Dictionary result;

    const BocchiTracker::ProcessLinkQuery::Queries::Packet* packet
        = BocchiTracker::ProcessLinkQuery::Queries::GetPacket(inPacket.ptr());
    
    int32_t id = packet->query_id_type();

    result["query_id"] = id;
    switch (id)
    {

    case BocchiTracker::ProcessLinkQuery::Queries::QueryID_ScreenshotRequest:
        {
            break;
        }

    case BocchiTracker::ProcessLinkQuery::Queries::QueryID_IssueesRequest:
        {
            const auto data = packet->query_id_as_IssueesRequest();
            Array issues;
            for (int i = 0; i < data->issues()->Length(); ++i)
            {
                const auto issue = data->issues()->Get(i);

                Dictionary v;
                v["id"]         = String(issue->id()->c_str());
                v["summary"]    = String(issue->summary()->c_str());
                v["assign"]     = String(issue->assign()->c_str());
                v["status"]     = String(issue->status()->c_str());
                v["stage"]      = String(issue->stage()->c_str());
                v["location"]   = Vector3(issue->location()->x(), issue->location()->y(), issue->location()->z());
                issues.append(v);
            }
            result["issues"] = issues;
        }
        break;

    case BocchiTracker::ProcessLinkQuery::Queries::QueryID_JumpRequest:
        {
            const auto data = packet->query_id_as_JumpRequest();
            result["stage"] = data->stage();
            result["location"] = Vector3(data->location()->x(), data->location()->y(), data->location()->z());
        }
        break;
    
    default:
        break;

    }

    return result;
}
