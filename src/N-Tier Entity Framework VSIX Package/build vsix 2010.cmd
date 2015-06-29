@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\N-Tier Entity Framework VSIX Package.v10.sln" /target:Rebuild /verbosity:minimal /maxcpucount /nodeReuse:false /property:Configuration=Release;VsixVersion=1.8;NugetPackageVersion=1.8.0-alpha002;VisualStudioVersion=12.0 %*
pause