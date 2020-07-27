@echo off

echo Creating copy of DynamiPong project as client instance.

set SOURCEFOLDER=%~dp0DynamiPong
set TARGETFOLDER=%~dp0DynamiPongClient

REM mkdir "%TARGETFOLDER%

robocopy "%SOURCEFOLDER%" "%TARGETFOLDER%" /MIR
rmdir /Q /S "%TARGETFOLDER%\Assets"
rmdir /Q /S "%TARGETFOLDER%\Packages"
rmdir /Q /S "%TARGETFOLDER%\ProjectSettings"

mklink /D "%TARGETFOLDER%\Assets" "%SOURCEFOLDER%\Assets"
mklink /D "%TARGETFOLDER%\Packages" "%SOURCEFOLDER%\Packages"
mklink /D "%TARGETFOLDER%\ProjectSettings" "%SOURCEFOLDER%\ProjectSettings"

pause