@echo off
rem Copyright (c) Wave Coorporation 2016. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe"

call :CompileShader SSAO vs vsSSAO
call :CompileShader SSAO ps psSSAO
call :CompileShader SSAO ps psSSAOCombine
call :CompileShader SSAO ps psOnlyAO

echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /E %3 /Fo %3.fxo  
echo.
echo %fxc%
%fxc% || set error=1
exit /b

