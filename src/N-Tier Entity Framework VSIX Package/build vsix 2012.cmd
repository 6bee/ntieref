@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\N-Tier Entity Framework VSIX Package (VS2012).sln" /v:minimal /maxcpucount /nodeReuse:false %*
pause