# FixSQLMSI

This tool is to extend the capability of “FindSQLInstalls.vbs script” from below article:
https://support.microsoft.com/en-us/help/969052

This tool has a friendly user interface to tell you what SQL server setup related MSI/MSP files are missing or mismatched in cached directory c:\windows\installer. It enables you recover the missing/corrupt/mismatched cached MSI/MSP files of SQL server product by one click. You can also get detailed information about the cached MSI/MSP files in the tool report. If you download service pack or CU from website you need to use X option to extract the downloaded package to extract those MSI/MSP files to the folder, to feed the tool for MSI/MSP meta data. Detailed readme please see the readme document inside the  FixSQLMSI folder.

I would recommend you  use Windows Installer Cache Verifier Package, as directed in KB article 2667628, to verify the cached SQL MSI/MSP files if you use this tool to fix MSI/MSP files.

The binaries can be downloaded in Release tab or below:
https://github.com/suyouquan/SQLSetupTools/releases/download/v1.3/FixSQLMSI.V1.3.2.zip

Note that you need NET 4.5 to run above version.

If you don't have NET 4.5 installed you can try below version:
https://github.com/suyouquan/SQLSetupTools/releases/download/v1.4/FixSQLMSI.V1.4.For.NET.3.5.zip

This version requires NET 3.5 installed on the machine but this version runs a little bit slow.

