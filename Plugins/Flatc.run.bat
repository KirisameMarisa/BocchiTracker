set flatc=%~dp0/../ExternalTools/flatbuffers/flatc.exe
set fbs=%~dp0/../Application/Models/ProcessLinkQuery/Query.fbs
call %flatc% --python -o Python %fbs% 
call %flatc% --cpp -o UE4 %fbs%
call %flatc% --cpp -o UE5 %fbs% 
call %flatc% --cpp -o Godot %fbs% 
call %flatc% --csharp -o Unity %fbs% 
pause
