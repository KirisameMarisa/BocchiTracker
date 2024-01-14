#ifndef BOCCHIAPI_H
#define BOCCHIAPI_H

#include <stdio.h>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/templates/list.hpp>

using namespace godot;

class BocchiAPI : public Object
{
    GDCLASS(BocchiAPI, Object);

public:
    BocchiAPI();
    ~BocchiAPI();

protected:
    static void _bind_methods();

public:
    static BocchiAPI *get_instance();

public:
    PackedByteArray createPlayerPosition(const Vector3& inPlayerPosition, const String& inStage) ;
    PackedByteArray createApplicationBasicInformation(uint32_t inPID, const String& inAppName, const String& inArgs, const String& inPlatform, const String& inLogFilepath);
    PackedByteArray create_screenshot_data(uint32_t inWidth, uint32_t inHeight, const PackedByteArray& inScreenshotData);
    PackedByteArray create_log_data(const Array& inData);

public:
    Dictionary resolve_request_query(const PackedByteArray& inPacket);

private:
    static BocchiAPI *instance;
};

#endif