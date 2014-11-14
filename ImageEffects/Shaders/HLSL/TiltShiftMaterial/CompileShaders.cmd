@echo off
rem Copyright (c) Wave Coorporation 2014. All rights reserved.

setlocal
set error=0

ver | findstr /i "6\.1\." > nul
IF %ERRORLEVEL% EQU 0 goto ver_Win7
ver | findstr /i "6\.2\." > nul
IF %ERRORLEVEL% EQU 0 goto ver_Win8

:ver_Win7
set fxcpath="C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x86\fxc.exe"
goto compile

:ver_Win8
set fxcpath="C:\Program Files (x86)\Windows Kits\8.0\bin\x86\fxc.exe"
goto compile

:compile
call :CompileShader TiltShift vs vsFastBlur
call :CompileShader TiltShift ps psFastBlur
call :CompileShader TiltShift ps psTiltShift

echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /I Structures.fxh /E %3 /Fo %1%3.fxo  
echo.
echo %fxc%
%fxc% || set error=1
exit /b

