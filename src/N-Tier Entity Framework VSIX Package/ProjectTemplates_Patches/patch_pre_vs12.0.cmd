@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\patch_pre_vs12.0.proj" /v:minimal /maxcpucount /nodeReuse:false %*
pause