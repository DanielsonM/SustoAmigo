@echo off
set APP_PATH=C:\Users\Usuario\AppData\Local\MicrosoftTools\Windows Service Widget.exe

:: Cria tarefa para iniciar junto com o Windows (boot)
schtasks /create /tn "ServiceWidgetsStart" /tr "\"%APP_PATH%\"" /sc onstart /rl highest /f

:: Cria tarefa para iniciar ao logon do usuário
schtasks /create /tn "ServiceWidgetsLogon" /tr "\"%APP_PATH%\"" /sc onlogon /rl highest /f

echo As duas tarefas foram criadas com sucesso!
pause