@echo off
start "INTEGRATION TEST WEB SERVER" "%~dp0\Tools\CassiniDev4-console.exe" /path:"%~dp0\IntegrationTest\IntegrationTest.Server.Host" /port:5000 /portMode:Specific
start "START INTEGRATION TEST" "%~dp0\Tools\NUnit-2.6.3\bin\nunit.exe" "%~dp0\Tests\UnitTests\bin\Debug\UnitTests.dll"
