; Installer for DynamiPong
; ver. 0.1

; -------------------------------

; Overview

; The name of the installer
Name "DynamiPong"

; The file to write
OutFile "DynamiPong-0.1.exe"

; The default installation directory
InstallDir $PROGRAMFILES\DynamiPong

; Modern UI
!include "MUI2.nsh"

; Icons
!define MUI_ICON icon.ico
!define MUI_UNICON unicon.ico

; -------------------------------

; Pages

; Installer
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

; Uninstaller
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

;--------------------------------

; Installer

Section "Install"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File /r ..\DynamiPong\bin\*
  File .\readme.txt
  
  ; Create desktop shortcut
  CreateShortcut "$DESKTOP\DynamiPong.lnk" "$INSTDIR\DynamiPong.exe"
  
  ; Create server shortcut
  CreateShortcut "$INSTDIR\DynamiPongLANServer.lnk" "$INSTDIR\DynamiPong.exe" "-batchmode -address localNetwork"
  
  ; create the uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
SectionEnd

;--------------------------------
 
; Uninstaller

Section "Uninstall"
 
  ; Delete desktop shortcut
  Delete "$DESKTOP\DynamiPong.lnk"

  ; Delete the directory recursively
  RMDir /r $INSTDIR

SectionEnd