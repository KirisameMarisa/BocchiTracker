using UnrealBuildTool;
using System.IO;

public class flatbuffers : ModuleRules
{
	public flatbuffers(ReadOnlyTargetRules Target) : base(Target)
	{
		Type = ModuleType.External;
		PublicSystemIncludePaths.Add(Path.Combine(ModuleDirectory, "include"));

		string LibraryPath = Path.Combine(ModuleDirectory, "lib");

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			PublicAdditionalLibraries.Add(Path.Combine(LibraryPath, "Win64", "Release", "flatbuffers.lib"));
		}
		// else if (Target.Platform == UnrealTargetPlatform.Mac)
		// {
		// 	PublicAdditionalLibraries.Add(Path.Combine(LibraryPath, "Mac", "Release", "libzip.a"));
		// }
		// else if (Target.IsInPlatformGroup(UnrealPlatformGroup.Unix))
		// {
		// 	PublicAdditionalLibraries.Add(Path.Combine(LibraryPath, "Unix", Target.Architecture.LinuxName, "Release", "libzip.a"));
		// }
	}
}


