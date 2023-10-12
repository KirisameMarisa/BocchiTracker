#ifndef BOCCHIGODOT_H
#define BOCCHIGODOT_H

#include <stdio.h>
#include <godot_cpp/classes/ref_counted.hpp>
#include <godot_cpp/core/class_db.hpp>

using namespace godot;

class bocchi_godot : public RefCounted
{
    GDCLASS(bocchi_godot, Object);

    static bocchi_godot *singleton;

protected:
    static void _bind_methods();

public:
    static bocchi_godot *get_singleton();

    bocchi_godot();
    ~bocchi_godot();

public:
    PackedByteArray createPlayerPosition(const Vector3& inPlayerPosition, const String& inStage) ;
    PackedByteArray createApplicationBasicInformation(int32_t inPID, const String& inAppName, const String& inArgs, const String& inPlatform);
};

#endif