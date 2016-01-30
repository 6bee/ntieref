@echo off
::
::
echo.
echo.
echo  ***  BUILD N-TIER ENTITY FRAMEWORK  ***
echo.
::%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\..\..\src\N-Tier Entity Framework\N-Tier Entity Framework.sln" /v:minimal /maxcpucount /nodeReuse:false /target:Rebuild /p:Configuration=Debug %*
::
::
echo.
echo.
echo  ***  BUILD TEST SOLUTION  ***
echo.
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\IntegrationTest.sln" /v:minimal /maxcpucount /nodeReuse:false /target:Rebuild /p:Configuration=Debug;VisualStudioVersion=12.0 %*
::
::
echo.
echo.
pause