@echo off
::
::
echo.
echo.
echo  ***  RESET DATABASE  ***
echo.
del /Q ".\IntegrationTest\Server\IntegrationTest.Server.Host\App_Data\*"
xcopy ".\IntegrationTest\Lib\NORTHWND.MDF" ".\IntegrationTest\Server\IntegrationTest.Server.Host\App_Data\" /Y
xcopy ".\IntegrationTest\Lib\NORTHWND_log.ldf" ".\IntegrationTest\Server\IntegrationTest.Server.Host\App_Data\" /Y
::
::
echo.
echo.
pause