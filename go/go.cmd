@ECHO OFF
:: NOTE: Batch harness required to handle directory changes because 
:: C# Environment.set_CurrentDirectory doesn't persist after process ends.

:: Don't redirect output for indexing
IF "%1"=="--index" (
  "%~dp0\goEngine.exe" %*
  GOTO :EOF
)

IF NOT EXIST "%LocalAppData%\go" ("%LocalAppData%\go")

:: Run Engine to get paths
"%~dp0\goEngine.exe" %* > %LocalAppData%\go\LastRun.log

:: Echo output
TYPE %LocalAppData%\go\LastRun.log

:: Read first line from output 
SET /P FirstLine=< %LocalAppData%\go\LastRun.log

:: Split acronym and path
FOR /F "tokens=1-3 delims=|" %%A in ("%FirstLine%") do (
  SET Acronym=%%B
  SET Target=%%C
  SET Color=%%A
)

:: Catch error case
IF "%Target%"=="" (GOTO :EOF)

:: Set title and current directory
TITLE "%Acronym% %Target%"
PUSHD %Target%
COLOR %Color%