set flatc=%~dp0../../../ExternalTools/flatbuffers/Release/flatc.exe
call %flatc% --csharp	    %~dp0/Query.fbs
call %flatc% --jsonschema   %~dp0/Query.fbs
pause
