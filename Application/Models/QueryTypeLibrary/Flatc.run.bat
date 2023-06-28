set flatc=%~dp0/../../../ExternalTools/flatbuffers/flatc.exe
call %flatc% --csharp %~dp0/Query.fbs
pause
