@ECHO OFF
"C:\Code\go\go\bin\Release\netcoreapp2.1\win-x64\go.exe" %* > %TEMP%\go\LastRun.log
TYPE %TEMP%\go\LastRun.log
SET /P Target=< %TEMP%\go\LastRun.log
PUSHD %Target%