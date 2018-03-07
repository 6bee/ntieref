@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\N-Tier Entity Framework VSIX Package.v12.sln" /target:Rebuild /verbosity:minimal /maxcpucount /nodeReuse:false /property:Configuration=Release;VsixVersion=1.9;NugetPackageVersion=1.9.0;VisualStudioVersion=12.0;BuildSilverlight=true %*
pause
