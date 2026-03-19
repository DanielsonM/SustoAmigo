@echo off
set SERVICE_NAME=SustoAmigo
set SERVICE_DISPLAY_NAME="SustoAmigo"
set SERVICE_DESCRIPTION=""

sc create %SERVICE_NAME% binPath= "CAMINHO" start=auto
sc description %SERVICE_NAME% %SERVICE_DESCRIPTION%
sc config %SERVICE_NAME% DisplayName= %SERVICE_DISPLAY_NAME%
net start %SERVICE_NAME%