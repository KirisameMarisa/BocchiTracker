#include "register_types.h"

#include <gdextension_interface.h>

#include <godot_cpp/classes/physics_server2d_manager.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/core/defs.hpp>
#include <godot_cpp/godot.hpp>
#include <godot_cpp/classes/engine.hpp>
#include <godot_cpp/variant/callable.hpp>

#include "bocchi_godot.h"

using namespace godot;

static bocchi_godot* bocchiGodot;

void initialize_bocchi_tracker_module(ModuleInitializationLevel p_level) 
{
    if (p_level == MODULE_INITIALIZATION_LEVEL_SCENE)
    {
        ClassDB::register_class<bocchi_godot>();
        bocchiGodot = memnew(bocchi_godot);
        Engine::get_singleton()->register_singleton("bocchi_godot", bocchi_godot::get_singleton());
    }	
}

void uninitialize_bocchi_tracker_module(ModuleInitializationLevel p_level) {
    if (p_level == MODULE_INITIALIZATION_LEVEL_SCENE)
    {
        Engine::get_singleton()->unregister_singleton("bocchi_godot");
        memdelete(bocchiGodot);
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
