# SQL Setup ToolSuite
There are three tools:


-[FixMissingMSI Version 2.2](#fixmissingmsi-version-22)

A tool to fix missing MSI/MSP in Windows Installer cache

Note: Please use the latest one:
https://github.com/suyouquan/SQLSetupTools/releases/download/V2.2.1/FixMissingMSI_V2.2.1_For_NET45.zip



-[Product Browser Version 2.2](#product-browser-22)

A tool to browse what products are installed and their properties and patch information



-[SQL Registry Viewer 2.2](#sql-registry-viewer-22)

A tool to view all SQL product related keys in registry just like you do with RegEdit.exe



# FixMissingMSI Version 2.2

FixMissingMSI is a tool to fix missing/mismatched MSI/MSP files in widnows installer cache folder, especially for SQL product this tool extends the capability of “FindSQLInstalls.vbs script” from below article:
https://support.microsoft.com/en-us/help/969052

This tool has a friendly user interface to tell you what setup related MSI/MSP files are missing or mismatched in cached directory c:\windows\installer. It enables you recover the missing/corrupt/mismatched cached MSI/MSP files of products by one click. You can also get detailed information about the cached MSI/MSP files in the tool report. If you download service pack or CU from website you need to use X option to extract the downloaded package to extract those MSI/MSP files to the folder, to feed the tool for MSI/MSP meta data. Detailed readme please see the readme document inside the FixMissingMSI folder.

I would recommend you  use Windows Installer Cache Verifier Package, as directed in KB article 2667628, to verify the cached  MSI/MSP files if you use this tool to fix MSI/MSP files.

The binaries can be downloaded in Release tab or below:
https://github.com/suyouquan/SQLSetupTools/releases/download/V2.2/FixMissingMSI_V2.2ForNET45.zip

Note that you need NET 4.5 to run above version.
If you don't have .NET 4.5 installed please download .NET 3.5 version here:
https://github.com/suyouquan/SQLSetupTools/releases/download/V2.2/FixMissingMSI_V2.2ForNET35.zip


Screenshot:
![screenshot](https://user-images.githubusercontent.com/35096859/35314819-939ae972-0103-11e8-8e32-f0f9bcc7475e.png)


# Product Browser 2.2
A tool to browse what products are installed and their properties and patch information
![productbrowser_v1 2](https://user-images.githubusercontent.com/35096859/35320665-8edb720a-011f-11e8-9417-1dd2f568fabe.png)
You can download it here:
https://github.com/suyouquan/SQLSetupTools/releases/download/V2.2/ProductBrowser_V2.2NET45.zip
https://github.com/suyouquan/SQLSetupTools/releases/download/V2.2/ProductBrowser_V2.2NET35.zip


# SQL Registry Viewer 2.2
A tool to view all SQL product related keys in registry just like you do with RegEdit.exe
https://github.com/suyouquan/SQLSetupTools/releases/download/V2.2/SQLRegViewer_v2.2NET3.5.zip
https://github.com/suyouquan/SQLSetupTools/releases/download/V2.2/SQLRegViewer_V2.2NET4.5.zip

![sqlregistryviewer_v1 0](https://user-images.githubusercontent.com/35096859/35322758-61b34efe-0126-11e8-980e-611a7cb4b1c9.png)


What is new in Version 2.2

Comparing to version 1.0, version 2.2 has below advantages:

1.Cached meta you scannedIf you ever scan your SP/CU folder, their meta will be cached for next run. This can save the scan time significantly. 

2.Pre-processed the SQL meta files so loading time will be in seconds instead of minutes, and the size of the tool reduces from 40MB to 4MB.

3.Use multi-threading technology to load registry keys asynchronously and in parallel. This help reduce the time to scan the registry keys. With these technologies now the tool runs 2 times faster.

4.Put some scan task to background so you can get the UI sooner to browse the keys.

5.If you export the keys the result detailed text file will have remark about what the keys belongs to. For example, below entry tells you the key is for “SQL Server 2008 R2 Management Studio”:

[SQL Server 2008 R2 Management Studio]
[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components\6E63FE9FCFA1D2244BD1045FE2A00E7F]"AB3CB1820BCF65042B6B105D760D8DC8"  

6. You can save cleanup script now to cleanup SQL server from your machine.


