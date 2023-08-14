echo
set /p UnityDLL="where is unity dll dir? : "

copy /B /Y %UnityDLL%\UnityEditor.dll %~dp0\UnityEditor.dll
copy /B /Y %UnityDLL%\UnityEngine.dll %~dp0\UnityEngine.dll

pause
