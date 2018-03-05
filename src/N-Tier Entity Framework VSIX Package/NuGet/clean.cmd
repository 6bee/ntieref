@ECHO OFF
SETLOCAL
PATH=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin;%programfiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin;%ProgramFiles(x86)%\MSBuild\14.0\Bin;%SystemRoot%\Microsoft.NET\Framework\v4.0.30319;%PATH%

MSBuild.exe "%~dp0\NuGet.proj" /v:minimal /maxcpucount /nodeReuse:false /target:Clean %*

ENDLOCAL
PAUSE
