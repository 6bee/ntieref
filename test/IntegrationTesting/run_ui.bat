@echo off
start "INTEGRATION TEST WEB SERVER" "%~dp0\Tools\CassiniDev4-console.exe" /path:"%~dp0\IntegrationTest\Server\IntegrationTest.Server.Host" /port:5000 /portMode:Specific
start "START INTEGRATION TEST" "%~dp0\Tools\NUnit 2.5.10\bin\net-2.0\nunit.exe" "%~dp0\IntegrationTest\UnitTests\UnitTests\bin\Debug\UnitTests.dll"
