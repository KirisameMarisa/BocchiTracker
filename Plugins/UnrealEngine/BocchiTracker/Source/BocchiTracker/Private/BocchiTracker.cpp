// Copyright Epic Games, Inc. All Rights Reserved.

#include "BocchiTracker.h"
#include "BocchiTrackerSettings.h"
#include "Developer/Settings/Public/ISettingsModule.h"

#define LOCTEXT_NAMESPACE "FBocchiTrackerModule"

void FBocchiTrackerModule::StartupModule()
{
	if (ISettingsModule* SettingsModule = FModuleManager::GetModulePtr<ISettingsModule>("Settings"))
	{
		SettingsModule->RegisterSettings("Project", "Plugins", "BocchiTracker",
				LOCTEXT("RuntimeSettingsName", "BocchiTracker"),
				LOCTEXT("RuntimeSettingsDescription", "Configure BocchiTracker Plugin"),
				GetMutableDefault<UBocchiTrackerSettings>());
	}
}

void FBocchiTrackerModule::ShutdownModule()
{
	if (ISettingsModule* SettingsModule = FModuleManager::GetModulePtr<ISettingsModule>("Settings"))
	{
		SettingsModule->UnregisterSettings("Project", "Plugins", "BocchiTracker");
	}
}

#undef LOCTEXT_NAMESPACE
	
IMPLEMENT_MODULE(FBocchiTrackerModule, BocchiTracker)