SET OldPath=%~dp0

CD ..

dotnet restore


REM CD src
REM CD Arbor.Aesculus.Core
REM dotnet restore

CD "%OldPath%"

CALL Build.exe

EXIT /B %ERRORLEVEL%