@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\NuGet.proj" /v:minimal /maxcpucount /nodeReuse:false /p:Version=1.5.0 %*
pause