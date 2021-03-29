SET Arbor.X.MSBuild.NuGetRestore.Enabled=true

CALL dotnet arbor-build

EXIT /B %ERRORLEVEL%