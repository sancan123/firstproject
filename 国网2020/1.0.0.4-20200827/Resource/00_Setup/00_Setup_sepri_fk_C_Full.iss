; 脚本用 Inno Setup 脚本向导生成。
; 查阅文档获取创建 INNO SETUP 脚本文件详细资料!

[Setup]
AppName=sepri_fk_C
AppVerName=sepri_fk_C
AppVersion=1.0.0.3
VersionInfoVersion=1.0.0.3
VersionInfoTextVersion=1, 0, 0, 3
AppPublisher=南方电网科学研究院有限责任公司
AppPublisherURL=http://www.csg.cn
AppSupportURL=http://www.csg.cn
AppUpdatesURL=http://www.csg.cn
DefaultDirName=D:\sepri_fk_C\
DefaultGroupName=sepri_fk_C
OutputDir=.\00_setup\
OutputBaseFilename=sepri_fk_C_1.0.0.3
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chi"; MessagesFile: "compiler:Default.isl"

[Files]
;View
Source: "..\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\*.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\*.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\*.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\*.ini"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\DogFile\*"; DestDir: "{app}\DogFile"; Flags: ignoreversion
Source: "..\DataBase\*"; DestDir: "{app}\DataBase"; Flags: ignoreversion

Source: "..\images\*"; DestDir: "{app}\images"; Flags: ignoreversion
Source: "..\Language\*"; DestDir: "{app}\Language"; Flags: ignoreversion
Source: "..\Reports\*"; DestDir: "{app}\Reports"; Flags: ignoreversion
Source: "..\ReportTemplate\*"; DestDir: "{app}\ReportTemplate"; Flags: ignoreversion
Source: "..\Skin\*"; DestDir: "{app}\Skin"; Flags: ignoreversion
Source: "..\xml\*.xml"; DestDir: "{app}\xml"; Flags: ignoreversion
;Verify
Source: "..\Verify\*.dll"; DestDir: "{app}\Verify"; Flags: ignoreversion
Source: "..\Verify\*.vbs"; DestDir: "{app}\Verify"; Flags: ignoreversion
Source: "..\Verify\*.ini"; DestDir: "{app}\Verify"; Flags: ignoreversion
Source: "..\Verify\*.xml"; DestDir: "{app}\Verify"; Flags: ignoreversion
Source: "..\Verify\*.bat"; DestDir: "{app}\Verify"; Flags: ignoreversion
Source: "..\Verify\*.exe"; DestDir: "{app}\Verify"; Flags: ignoreversion
Source: "..\Verify\*.config"; DestDir: "{app}\Verify"; Flags: ignoreversion

Source: "..\Verify\Encryption\*"; DestDir: "{app}\Verify\Encryption"; Flags: ignoreversion
Source: "..\Verify\Encryption\DLL_SERVER_SOUTH\Card\*"; DestDir: "{app}\Verify\Encryption\DLL_SERVER_SOUTH\Card"; Flags: ignoreversion
Source: "..\Verify\Encryption\DLL_SERVER_SOUTH\*"; DestDir: "{app}\Verify\Encryption\DLL_SERVER_SOUTH"; Flags: ignoreversion

Source: "..\Verify\Log\*"; DestDir: "{app}\Verify\Log"; Flags: ignoreversion
Source: "..\Verify\Log\Thread\*"; DestDir: "{app}\Verify\Log\Thread"; Flags: ignoreversion
Source: "..\Verify\ErrLog\*"; DestDir: "{app}\Verify\ErrLog"; Flags: ignoreversion

Source: "..\Verify\Pic\*"; DestDir: "{app}\Verify\Pic"; Flags: ignoreversion
Source: "..\Verify\Pic\BllDescription\*"; DestDir: "{app}\Verify\Pic\BllDescription"; Flags: ignoreversion
Source: "..\Verify\Pic\Button\*"; DestDir: "{app}\Verify\Pic\Button"; Flags: ignoreversion
Source: "..\Verify\Pic\ClientBack\*"; DestDir: "{app}\Verify\Pic\ClientBack"; Flags: ignoreversion
Source: "..\Verify\Pic\ClientButton\*"; DestDir: "{app}\Verify\Pic\ClientButton"; Flags: ignoreversion
Source: "..\Verify\Pic\ClientButton\blueberry\*"; DestDir: "{app}\Verify\Pic\ClientButton\blueberry"; Flags: ignoreversion
Source: "..\Verify\Pic\Detection\*"; DestDir: "{app}\Verify\Pic\Detection"; Flags: ignoreversion
Source: "..\Verify\Pic\Detection\Light\*"; DestDir: "{app}\Verify\Pic\Detection\Light"; Flags: ignoreversion
Source: "..\Verify\Pic\Detection\V90Style\*"; DestDir: "{app}\Verify\Pic\Detection\V90Style"; Flags: ignoreversion
Source: "..\Verify\Pic\StateBar\*"; DestDir: "{app}\Verify\Pic\StateBar"; Flags: ignoreversion
Source: "..\Verify\Pic\SysIcons\*"; DestDir: "{app}\Verify\Pic\SysIcons"; Flags: ignoreversion
Source: "..\Verify\Pic\UI\*"; DestDir: "{app}\Verify\Pic\UI"; Flags: ignoreversion
Source: "..\Verify\Pic\UI\UI_Client\*"; DestDir: "{app}\Verify\Pic\UI\UI_Client"; Flags: ignoreversion
Source: "..\Verify\Pic\images\*"; DestDir: "{app}\Verify\Pic\images"; Flags: ignoreversion

Source: "..\Verify\System\*"; DestDir: "{app}\Verify\System"; Flags: ignoreversion

Source: "..\Verify\Xml\*"; DestDir: "{app}\Verify\Xml"; Flags: ignoreversion
Source: "..\Verify\Wav\*"; DestDir: "{app}\Verify\Wav"; Flags: ignoreversion
Source: "..\Verify\zh-CHS\*"; DestDir: "{app}\Verify\zh-CHS"; Flags: ignoreversion

Source: "..\Verify\AppConfigs\*"; DestDir: "{app}\Verify\AppConfigs"; Flags: ignoreversion

Source: "..\Verify\DLLFile\DeviceManufacturers\Clou\*"; DestDir: "{app}\Verify\DLLFile\DeviceManufacturers\Clou"; Flags: ignoreversion
Source: "..\Verify\DLLFile\DeviceManufacturers\HyHpu\*"; DestDir: "{app}\Verify\DLLFile\DeviceManufacturers\HyHpu"; Flags: ignoreversion
Source: "..\Verify\DLLFile\DeviceManufacturers\GeNing\*"; DestDir: "{app}\Verify\DLLFile\DeviceManufacturers\GeNing"; Flags: ignoreversion
Source: "..\Verify\DLLFile\*"; DestDir: "{app}\Verify\DLLFile"; Flags: ignoreversion

Source: "..\Verify\Parameter\*"; DestDir: "{app}\Verify\Parameter"; Flags: ignoreversion

; 注意: 不要在任何共享系统文件中使用“Flags: ignoreversion”

[Icons]

Name: "{userdesktop}\sepri_fk_C"; Filename: "{app}\sepri_fk_C.exe"; WorkingDir: "{app}"
; Tasks: desktopicon

Name: "{group}\sepri_fk_C"; Filename: "{app}\sepri_fk_C.exe"
Name: "{group}\{cm:UninstallProgram,程序}"; Filename: "{app}\unins000.exe"

