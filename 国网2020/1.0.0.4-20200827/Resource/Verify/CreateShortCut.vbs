set WshShell = WScript.CreateObject("WScript.Shell" )
set fso=WScript.CreateObject("Scripting.FileSystemObject")
strDesktop = WshShell.SpecialFolders("AllUsersDesktop" )
strCurrentDirectory=WshShell.CurrentDirectory

'' ɾ�������Ѵ��ڿ�ݷ�ʽ
IF ( fso.FileExists( strDesktop & "\��½У��.lnk") =true) THEN
	fso.DeleteFile strDesktop & "\��½У��.lnk" , true 
END IF

IF (fso.FileExists (strDesktop & "\��½Э��.lnk")=true) THEN
	fso.DeleteFile strDesktop & "\��½Э��.lnk" , true  
END IF

IF (fso.FileExists (strDesktop & "\��½����.lnk")=true) THEN
	fso.DeleteFile strDesktop & "\��½����.lnk" , true 
END IF

'' ���..\..\ClouBoot.exe �Ƿ���ڣ�������洴�����ݷ�ʽ

parentDir=fso.GetParentFolderName(strCurrentDirectory)
parentDir=fso.GetParentFolderName(parentDir)
strClouBoot=parentDir & "\ClouBoot.exe"

IF fso.FileExists(strClouBoot) then
	XiaoBiao= strClouBoot
ELSE
	XiaoBiao= strCurrentDirectory & "\ClientFrame.exe"
	MsgBox strClouBoot, 0, "�ļ�������"
END IF
set oShellLink = WshShell.CreateShortcut(strDesktop & "\��½У��.lnk" )
oShellLink.TargetPath = XiaoBiao
oShellLink.WindowStyle = "1"
oShellLink.Description = "��½У��"
oShellLink.WorkingDirectory = strCurrentDirectory
oShellLink.Save

set oShellLink = WshShell.CreateShortcut(strDesktop & "\��½Э��.lnk" )
oShellLink.TargetPath = strCurrentDirectory & "\CLMeterProtocolSetup.exe"
oShellLink.WindowStyle = "1"
oShellLink.Description = "��½Э��"
oShellLink.WorkingDirectory = strCurrentDirectory
oShellLink.Save

set oShellLink = WshShell.CreateShortcut(strDesktop & "\��½����.lnk" )
oShellLink.TargetPath = strCurrentDirectory & "\CLDC_DataManager.exe"
oShellLink.WindowStyle = "1"
oShellLink.Description = "��½����"
oShellLink.WorkingDirectory = strCurrentDirectory
oShellLink.Save