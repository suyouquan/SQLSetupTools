# FixMissingMSI

Originally this tool (Old name FixSQLMSI) is to extend the capability of “FindSQLInstalls.vbs script” from below article:
https://support.microsoft.com/en-us/help/969052

I renamed the tool to SQLMissingMSI and add support so it can fix missing/mismatched MSI/MSP files in installer cache folder for any product installed on the machine.

This tool has a friendly user interface to tell you what setup related MSI/MSP files are missing or mismatched in cached directory c:\windows\installer. It enables you recover the missing/corrupt/mismatched cached MSI/MSP files of products by one click. You can also get detailed information about the cached MSI/MSP files in the tool report. If you download service pack or CU from website you need to use X option to extract the downloaded package to extract those MSI/MSP files to the folder, to feed the tool for MSI/MSP meta data. Detailed readme please see the readme document inside the FixMissingMSI folder.

I would recommend you  use Windows Installer Cache Verifier Package, as directed in KB article 2667628, to verify the cached  MSI/MSP files if you use this tool to fix MSI/MSP files.

The binaries can be downloaded in Release tab or below:


Note that you need NET 4.5 to run above version.

If you don't have NET 4.5 installed you can try below version:


This version requires NET 3.5 installed on the machine but this version runs a little bit slow.

Screenshot:

