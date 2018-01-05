# FixSQLMSI

This tool is to extend the capability of “FindSQLInstalls.vbs script” from below article:
https://support.microsoft.com/en-us/help/969052

This tool has a friendly user interface to tell you what SQL server setup related MSI/MSP files are missing or mismatched in cached directory c:\windows\installer. It enables you recover the missing/corrupt/mismatched cached MSI/MSP files of SQL server product by one click. You can also get detailed information about the cached MSI/MSP files in the tool report. If you download service pack or CU from website you need to use X option to extract the downloaded package to extract those MSI/MSP files to the folder, to feed the tool for MSI/MSP meta data. Detailed readme please see the readme document inside the  FixSQLMSI folder.

I would recommend you  use Windows Installer Cache Verifier Package, as directed in KB article 2667628, to verify the cached SQL MSI/MSP files if you use this tool to fix MSI/MSP files.

The binaries can be downloaded in Release tab or here:https://github.com/suyouquan/SQLSetupTools/files/1605235/FixSQLMSI.v1.2.zip
