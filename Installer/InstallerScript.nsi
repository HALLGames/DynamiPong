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

; -------------------------------

; Modern UI

!include "MUI2.nsh"

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

;--------------------------------

; Pages

; Page directory
; Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; Installer

Section "Install"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File /r ..\DynamiPong\bin\*
  
  ; Create shortcut
  CreateShortcut "$DESKTOP\DynamiPong.lnk" "$INSTDIR\DynamiPong.exe"
  
  ; create the uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
SectionEnd

;--------------------------------
 
; Uninstaller

Section "Uninstall"
 
  ; Delete shortcut
  Delete "$DESKTOP\DynamiPong.lnk"

  ; Delete the directory recursively
  RMDir /r $INSTDIR

SectionEnd