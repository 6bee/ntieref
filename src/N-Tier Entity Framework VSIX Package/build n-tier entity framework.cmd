@ECHO OFF
ECHO.
ECHO RESTORE NUGET PACKAGES
"%~dp0..\..\tools\NuGet\NuGet.exe" restore "%~dp0..\N-Tier Entity Framework\N-Tier Entity Framework.sln"
ECHO.
ECHO BUILD N-TIER ENTITY FRAMEWORK
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" "%~dp0..\N-Tier Entity Framework\N-Tier Entity Framework.sln" /target:Rebuild /verbosity:minimal /maxcpucount /nodeReuse:false /property:Configuration=Release %*
ECHO.
pause