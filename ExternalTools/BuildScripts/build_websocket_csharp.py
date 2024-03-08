from pathlib import Path
import subprocess
import shutil
import config

def Build():
    print("Build C# websocket-sharp")
    flatbuffers_csproj_path = config.cWebSocketSharpPath / "websocket-sharp" / "websocket-sharp.csproj"
    subprocess.call(["dotnet", "build", flatbuffers_csproj_path.resolve(), "-c", "Release"], cwd=config.cFlatBuffersPath)

    print("Copying flatbuffers to Unity directory...")
    artifact = config.cWebSocketSharpPath / "websocket-sharp" / "bin" / "Release" / "netstandard2.0"
    unity_external_packages_path = config.cUnityPath / "project" / "Assets" / "BocchiTracker" / "ExternalPackages" / "websocket-sharp"
    shutil.copytree(artifact, unity_external_packages_path.resolve(), dirs_exist_ok=True)

if __name__ == '__main__':
    Build()