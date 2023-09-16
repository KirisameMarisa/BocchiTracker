#include "register_types.h"

#include <gdextension_interface.h>

#include <godot_cpp/classes/physics_server2d_manager.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/core/defs.hpp>
#include <godot_cpp/godot.hpp>
#include <godot_cpp/variant/callable.hpp>

#include "flatbuffers/flatbuffers.h"
#include "Query_generated.h"

using namespace godot;

void initialize_physics_server_box2d_module(ModuleInitializationLevel p_level) {
	
}

void uninitialize_physics_server_box2d_module(ModuleInitializationLevel p_level) {
	if (p_level != MODULE_INITIALIZATION_LEVEL_SERVERS) {
		return;
	}
}

extern "C" {

// Initialization.

GDExtensionBool GDE_EXPORT physics_server_box2d_library_init(const GDExtensionInterfaceGetProcAddress p_get_proc_address, GDExtensionClassLibraryPtr p_library, GDExtensionInitialization *r_initialization) {
	godot::GDExtensionBinding::InitObject init_obj(p_get_proc_address, p_library, r_initialization);

	init_obj.set_minimum_library_initialization_level(MODULE_INITIALIZATION_LEVEL_SERVERS);

	return init_obj.init();
}
}
