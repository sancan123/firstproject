set WshShell = WScript.CreateObject("WScript.Shell" )
set fso=WScript.CreateObject("Scripting.FileSystemObject")
strDesktop = WshShell.SpecialFolders("AllUsersDesktop" )
strCurrentDirectory=WshShell.CurrentDirectory

'' 删除可能已存在快捷方式
IF ( fso.FileExists( strDesktop & "\科陆校表.lnk") =true) THEN
	fso.DeleteFile strDesktop & "\科陆校表.lnk" , true 
END IF

IF (fso.FileExists (strDesktop & "\科陆协议.lnk")=true) THEN
	fso.DeleteFile strDesktop & "\科陆协议.lnk" , true  
END IF

IF (fso.FileExists (strDesktop & "\科陆报表.lnk")=true) THEN
	fso.DeleteFile strDesktop & "\科陆报表.lnk" , true 
END IF

'' 检查..\..\ClouBoot.exe 是否存在，如果储存创建其快捷方式

parentDir=fso.GetParentFolderName(strCurrentDirectory)
parentDir=fso.GetParentFolderName(parentDir)
strClouBoot=parentDir & "\ClouBoot.exe"

IF fso.FileExists(strClouBoot) then
	XiaoBiao= strClouBoot
ELSE
	XiaoBiao= strCurrentDirectory & "\ClientFrame.exe"
	MsgBox strClouBoot, 0, "文件不存在"
END IF
set oShellLink = WshShell.CreateShortcut(strDesktop & "\科陆校表.lnk" )
oShellLink.TargetPath = XiaoBiao
oShellLink.WindowStyle = "1"
oShellLink.Description = "科陆校表"
oShellLink.WorkingDirectory = strCurrentDirectory
oShellLink.Save

set oShellLink = WshShell.CreateShortcut(strDesktop & "\科陆协议.lnk" )
oShellLink.TargetPath = strCurrentDirectory & "\CLMeterProtocolSetup.exe"
oShellLink.WindowStyle = "1"
oShellLink.Description = "科陆协议"
oShellLink.WorkingDirectory = strCurrentDirectory
oShellLink.Save

set oShellLink = WshShell.CreateShortcut(strDesktop & "\科陆报表.lnk" )
oShellLink.TargetPath = strCurrentDirectory & "\CLDC_DataManager.exe"
oShellLink.WindowStyle = "1"
oShellLink.Description = "科陆报表"
oShellLink.WorkingDirectory = strCurrentDirectory
oShellLink.Save