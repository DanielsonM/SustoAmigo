@echo off
set APP_PATH="C:\caminho..."

:: Cria tarefa para iniciar junto com o Windows (boot)
schtasks /create /tn "SustoAmigoStart" /tr %APP_PATH% /sc onstart /rl highest /f

:: Cria tarefa para iniciar ao logon do usuário
schtasks /create /tn "SustoAmigoLogon" /tr %APP_PATH% /sc onlogon /rl highest /f

echo As duas tarefas foram criadas com sucesso!
pause