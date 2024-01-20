from pathlib import Path
import subprocess
import shutil
import config
import platform

def Build():
    print("Build C++ flatbuffers...")
    if platform.system() == "Windows":
        RunMsBuild()
    else:
        RunUnix()

    print("Copying flatbuffers to UnrealEngine directory...")
    include_path = config.cFlatBuffersPath / "include"
    unreal_engine_include_path = config.cUnrealEnginePath / "ThirdParty" / "flatbuffers" / "include"
    shutil.copytree(include_path, unreal_engine_include_path.resolve(), dirs_exist_ok=True)

    if platform.system() == "Windows":
        release_lib_path = config.cFlatBuffersPath / "Release"
        unreal_engine_lib_path = config.cUnrealEnginePath / "ThirdParty" / "flatbuffers" / "lib" / "Win64" / "Release"
    else:
        release_lib_path = config.cFlatBuffersPath / "build" / "lib"
        unreal_engine_lib_path = config.cUnrealEnginePath / "ThirdParty" / "flatbuffers" / "lib" / "Unix" / "Release"

    unreal_engine_lib_path.resolve().mkdir(exist_ok=True)
    shutil.copytree(release_lib_path.resolve(), unreal_engine_lib_path.resolve(), dirs_exist_ok=True)

def RunMsBuild():
    subprocess.call(["cmake", "-G", "Visual Studio 17", "-DCMAKE_BUILD_TYPE=Release"], cwd=config.cFlatBuffersPath)
    subprocess.call(["msbuild", config.cFlatBuffersPath / "FlatBuffers.sln", "-t:flatbuffers", "/p:Configuration=Release"], cwd=config.cFlatBuffersPath)
    subprocess.call(["msbuild", config.cFlatBuffersPath / "FlatBuffers.sln", "-t:flatc", "/p:Configuration=Release"], cwd=config.cFlatBuffersPath)
    
def RunUnix():
    subprocess.call(["cmake", "-G", "Unix Makefiles", "-DCMAKE_BUILD_TYPE=Release"], cwd=config.cFlatBuffersPath)
    subprocess.call(["make", "-j"], cwd=config.cFlatBuffersPath)


if __name__ == '__main__':
    Build()