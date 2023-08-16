from pathlib import Path
import subprocess
import shutil
import Config

def Build():
    print("Build C# flatbuffers")
    dotnet_path = "dotnet"
    flatbuffers_csproj_path = Path("flatbuffers") / "net" / "FlatBuffers" / "Google.FlatBuffers.csproj"
    subprocess.call([dotnet_path, "build", flatbuffers_csproj_path, "-c", "Release"])

    print("Copying flatbuffers to Unity directory...")
    artifact = Path("flatbuffers") / "net" / "FlatBuffers" / "bin" / "Release" / "netstandard2.1"
    unity_thirdparty_path = Config.cUnityPath / "ThirdParty" / "flatbuffers"
    unity_plugin_artifact_path = Config.cUnityPath / "Artifact"
    shutil.copytree(artifact, unity_thirdparty_path, dirs_exist_ok=True)
    shutil.copytree(artifact, unity_plugin_artifact_path, dirs_exist_ok=True)

if __name__ == '__main__':
    Build()