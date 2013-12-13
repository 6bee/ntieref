@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\N-Tier Entity Framework VSIX Package.v10.sln" /v:minimal /maxcpucount /nodeReuse:false /property:VisualStudioVersion=12.0 %*
pause