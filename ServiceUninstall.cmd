@echo off
setlocal enabledelayedexpansion

set SERVICE_NAME=SustoAmigo
set SERVICE_PATH=%~dp0SustoAmigo.exe

echo Parando servico %SERVICE_NAME%...
net stop %SERVICE_NAME% 2>nul

echo.
echo Desinstalando servico %SERVICE_NAME%...

REM Desinstala o serviço usando o próprio executável (Topshelf)
"%SERVICE_PATH%" uninstall

echo.
echo Servico desinstalado com sucesso!
pause
