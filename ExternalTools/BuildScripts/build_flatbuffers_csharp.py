from pathlib import Path
import subprocess
import shutil
import config

def Build():
    print("Cmake C# flatbuffers")
    subprocess.call(["cmake", "-G", "Visual Studio 17", "-DCMAKE_BUILD_TYPE=Release"], cwd=config.cFlatBuffersPath)

    print("Build C# flatbuffers")
    flatbuffers_csproj_path = config.cFlatBuffersPath / "net" / "FlatBuffers" / "Google.FlatBuffers.csproj"
    subprocess.call(["dotnet", "build", flatbuffers_csproj_path.resolve(), "-c", "Release"], cwd=config.cFlatBuffersPath)

    print("Copying flatbuffers to Unity directory...")
    artifact = config.cFlatBuffersPath / "net" / "FlatBuffers" / "bin" / "Release" / "netstandard2.1"
    unity_external_packages_path = config.cUnityPath / "project" / "Assets" / "BocchiTracker" / "ExternalPackages" / "flatbuffers"
    shutil.copytree(artifact, unity_external_packages_path.resolve(), dirs_exist_ok=True)

if __name__ == '__main__':
    Build()