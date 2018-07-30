@echo off
rem Copyright (c) Wave Coorporation 2018. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe"

call :CompileShader ScreenOverlay ps psScreenOverlayAdditive
call :CompileShader ScreenOverlay ps psScreenOverlayMultiply
call :CompileShader ScreenOverlay ps psScreenOverlayScreenBlend
call :CompileShader ScreenOverlay ps psScreenOverlayOverlay 
call :CompileShader ScreenOverlay ps psScreenOverlayAlphaBlend
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

