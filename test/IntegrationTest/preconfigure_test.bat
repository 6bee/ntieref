@echo off
::
::
echo.
echo.
echo  ***  RESET DATABASE  ***
echo.
del /Q ".\IntegrationTest\IntegrationTest.Server.Host\App_Data\*"
xcopy ".\data\NORTHWND.MDF" ".\IntegrationTest\IntegrationTest.Server.Host\App_Data\" /Y
xcopy ".\data\NORTHWND_log.ldf" ".\IntegrationTest\IntegrationTest.Server.Host\App_Data\" /Y
::
::
echo.
echo.
pause