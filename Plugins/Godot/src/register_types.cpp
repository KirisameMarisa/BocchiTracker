#include "register_types.h"

#include <gdextension_interface.h>

#include <godot_cpp/classes/physics_server2d_manager.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/core/defs.hpp>
#include <godot_cpp/godot.hpp>
#include <godot_cpp/classes/engine.hpp>
#include <godot_cpp/variant/callable.hpp>

#include "BocchiAPI.h"

using namespace godot;

static BocchiAPI* bocchiAPI = nullptr;

void initialize_bocchi_tracker_module(ModuleInitializationLevel p_level) 
{
    if (p_level == MODULE_INITIALIZATION_LEVEL_SCENE)
    {
        ClassDB::register_class<BocchiAPI>();
        bocchiAPI = memnew(BocchiAPI);
        Engine::get_singleton()->register_singleton("bocchi_api", BocchiAPI::get_instance());
    }
}

void uninitialize_bocchi_tracker_module(ModuleInitializationLevel p_level) 
{
    if (p_level == MODULE_INITIALIZATION_LEVEL_SCENE)
    {
        Engine::get_singleton()->unregister_singleton("bocchi_api");
        memdelete(bocchiAPI);
    }
}

extern "C" 
{
	GDExtensionBool GDE_EXPORT bocchi_tracker_library_init(const GDExtensionInterfaceGetProcAddress p_get_proc_address, GDExtensionClassLibraryPtr p_library, GDExtensionInitialization *r_initialization) 
	{
		godot::GDExtensionBinding::InitObject init_obj(p_get_proc_address, p_library, r_initialization);

		init_obj.register_initializer(initialize_bocchi_tracker_module);
		init_obj.register_terminator(uninitialize_bocchi_tracker_module);
		init_obj.set_minimum_library_initialization_level(MODULE_INITIALIZATION_LEVEL_SCENE);

		return init_obj.init();
	}
}
