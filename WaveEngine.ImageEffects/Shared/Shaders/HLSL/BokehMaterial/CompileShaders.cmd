@echo off
rem Copyright (c) Wave Coorporation 2018. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe"

call :CompileShader Bokeh ps psCoCMap

call :CompileShader Bokeh ps psHorizontalBlur LOW
call :CompileShader Bokeh ps psHorizontalBlur MEDIUM
call :CompileShader Bokeh ps psHorizontalBlur HIGH

call :CompileShader Bokeh ps psDiagonalBlurCombine LOW
call :CompileShader Bokeh ps psDiagonalBlurCombine MEDIUM
call :CompileShader Bokeh ps psDiagonalBlurCombine HIGH

echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
if [%4]==[] goto NotParam

set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /D %4 /E %3 /Fo %3_%4.fxo
goto End

: NotParam
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /E %3 /Fo %3.fxo

:End
echo.
echo %fxc%
%fxc% || set error=1
exit /b

