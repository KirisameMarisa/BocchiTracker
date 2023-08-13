set flatc=%~dp0/../ExternalTools/flatbuffers/Release/flatc.exe
set fbs=%~dp0/../Application/Models/ProcessLinkQuery/Query.fbs
call %flatc% --python -o Python %fbs% 
call %flatc% --cpp -o UnrealEngine/BocchiTracker/Source/BocchiTracker/Private %fbs%
call %flatc% --cpp -o Godot %fbs% 
call %flatc% --csharp -o Unity %fbs% 
pause
