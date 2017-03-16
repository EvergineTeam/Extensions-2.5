@echo off
rem Copyright (c) Wave Coorporation 2017. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe"

call :CompileShader ToneMapping ps psLinear
call :CompileShader ToneMapping ps psSimpleReinhard
call :CompileShader ToneMapping ps psLumaBasedReinhard
call :CompileShader ToneMapping ps psWhitePreservingLumaBasedReinhard
call :CompileShader ToneMapping ps psRombinDaHouse
call :CompileShader ToneMapping ps psPhotography
call :CompileShader ToneMapping ps psFilmic
call :CompileShader ToneMapping ps psUncharted2

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

