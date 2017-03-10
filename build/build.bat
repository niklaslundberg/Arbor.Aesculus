SET OldPath=%~dp0

SET Arbor.X.MSBuild.NuGetRestore.Enabled=true

CD ..

dotnet restore


REM CD src
REM CD Arbor.Aesculus.Core
REM dotnet restore

CD "%OldPath%"

CALL Build.exe

EXIT /B %ERRORLEVEL%