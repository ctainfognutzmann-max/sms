@echo off
setlocal EnableExtensions

set "SCRIPT_DIR=%~dp0"
set "PROJECT=%SCRIPT_DIR%webApiAghos\webApiAghos.csproj"
set "CONFIGURATION=Release"
set "OUTPUT_PATH=%SCRIPT_DIR%publish"

if not "%~1"=="" set "CONFIGURATION=%~1"
if not "%~2"=="" set "OUTPUT_PATH=%~2"

if not exist "%PROJECT%" (
    echo ERRO: Projeto nao encontrado em "%PROJECT%".
    exit /b 1
)

where dotnet >nul 2>&1
if errorlevel 1 (
    echo ERRO: O SDK do .NET 10 nao foi encontrado no PATH.
    exit /b 1
)

echo.
echo Publicando webApiAghos...
echo Projeto: "%PROJECT%"
echo Configuracao: %CONFIGURATION%
echo Destino: "%OUTPUT_PATH%"
echo.

if exist "%OUTPUT_PATH%" (
    echo Limpando pasta de destino...
    rmdir /s /q "%OUTPUT_PATH%"
    if errorlevel 1 (
        echo ERRO: Nao foi possivel limpar a pasta de destino.
        exit /b 1
    )
)

dotnet publish "%PROJECT%" --configuration "%CONFIGURATION%" --output "%OUTPUT_PATH%" --no-self-contained
if errorlevel 1 (
    echo.
    echo ERRO: Falha ao gerar a publicacao.
    exit /b 1
)

echo.
echo Publicacao concluida com sucesso em:
echo "%OUTPUT_PATH%"

endlocal
