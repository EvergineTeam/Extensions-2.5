@echo off
rem Copyright (c) Wave Coorporation 2018. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe"

call :CompileShader MotionBlur ps psMotionBlur LOW
call :CompileShader MotionBlur ps psMotionBlur MEDIUM
call :CompileShader MotionBlur ps psMotionBlur HIGH

echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /I Structures.fxh /D %4 /E %3 /Fo %3_%4.fxo  
echo.
echo %fxc%
%fxc% || set error=1
exit /b

