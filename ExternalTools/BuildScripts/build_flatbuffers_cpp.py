from pathlib import Path
import subprocess
import shutil
import Config
import platform

def Build():
    print("Build C++ flatbuffers...")
    if platform.system() == "Windows":
        RunMsBuild()
    else:
        RunUnix()

    print("Copying flatbuffers to UnrealEngine directory...")
    include_path = Path("flatbuffers") / "include"
    unreal_engine_include_path = Config.cUnrealEnginePath / "ThirdParty" / "flatbuffers" / "include"
    shutil.copytree(include_path, unreal_engine_include_path, dirs_exist_ok=True)

    release_lib_path = Path("flatbuffers") / "Release"
    if platform.system() == "Windows":
        unreal_engine_lib_path = Config.cUnrealEnginePath / "ThirdParty" / "flatbuffers" / "lib" / "Win64" / "Release"
    else:
        unreal_engine_lib_path = Config.cUnrealEnginePath / "ThirdParty" / "flatbuffers" / "lib" / "Unix" / "Release"
    shutil.copytree(release_lib_path, unreal_engine_lib_path, dirs_exist_ok=True)

def RunMsBuild():
    msbuild_path = Path("C:\\") / "Program Files" / "Microsoft Visual Studio" / "2022" / "Community" / "Msbuild" / "Current" / "Bin" / "MSBuild.exe"
    if not msbuild_path.exists():
        print("not exist MSBuild.exe, should install VisualStudio build tools.")
        return

    subprocess.call(["cmake", "-G", "Visual Studio 17", "-DCMAKE_BUILD_TYPE=Release"])
    subprocess.call([msbuild_path, Path("flatbuffers") / "FlatBuffers.sln", "-t:flatbuffers", "/p:Configuration=Release"])
    subprocess.call([msbuild_path, Path("flatbuffers") / "FlatBuffers.sln", "-t:flatc", "/p:Configuration=Release"])
    
def RunUnix():
    subprocess.call(["cmake", "-G", "Unix Makefiles", "-DCMAKE_BUILD_TYPE=Release"])
    subprocess.call(["make", "-j"])


if __name__ == '__main__':
    Build()