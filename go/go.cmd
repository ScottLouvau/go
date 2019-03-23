@ECHO OFF

:: Run Engine to get paths
"%~dp0\goEngine.exe" %* > %TEMP%\go\LastRun.log

:: Echo output
TYPE %TEMP%\go\LastRun.log

:: Read first line and change directory (C# Environment.set_CurrentDirectory doesn't work)
SET /P Target=< %TEMP%\go\LastRun.log
PUSHD %Target%