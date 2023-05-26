del *.pdb
del *.vshost.exe*
del *.manifest
del TestApp.exe
del *.ini
del *.txt
rename ServerSetup.xml ServerSetup.xml.bak
del *.xml
rename ServerSetup.xml.bak ServerSetup.xml
rmdir /s /q Const
rmdir /s /q DataBase
rmdir /s /q DX
rmdir /s /q ErrLog
rmdir /s /q Log
rmdir /s /q Res
rmdir /s /q SX
rmdir /s /q System
rmdir /s /q Tmp
rmdir /s /q Wc
rmdir /s /q Log
start CreateShortCut.vbs
pause