echo off

set MSBuild="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe"
set Plugins="../../Plugins/"
set UnrealEngine="%Plugins%UnrealEngine"

echo Build flatbuffers...
pushd flatbuffers
call cmake -G "Visual Studio 17" -DCMAKE_BUILD_TYPE=Release
call %MSBuild% FlatBuffers.sln -t:flatbuffers /p:Configuration=Release
call %MSBuild% FlatBuffers.sln -t:flatc /p:Configuration=Release

echo Copying flatbuffers to UnrealEngine directory...
xcopy "include" "%UnrealEngine%/ThirdParty/flatbuffers/include" /S /Y /I
xcopy "Release" "%UnrealEngine%/ThirdParty/flatbuffers/lib/Win64/Release" /S /Y /I
popd 

