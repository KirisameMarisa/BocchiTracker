echo
set /p Unity="where is unity dll dir? : "

copy /B /Y %Unity%\UnityEditor.dll %~dp0\UnityEditor.dll
copy /B /Y %Unity%\UnityEngine.dll %~dp0\UnityEngine.dll

pause
