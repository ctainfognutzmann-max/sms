@echo off
setlocal EnableExtensions
title Atualizar projeto no GitHub

REM Executa sempre a partir da pasta onde este arquivo esta salvo.
cd /d "%~dp0"

where git >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERRO: Git nao foi encontrado neste terminal.
    echo Instale-o em https://git-scm.com/download/win e abra o arquivo novamente.
    goto :end
)

git rev-parse --is-inside-work-tree >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERRO: Esta pasta nao e um repositorio Git.
    goto :end
)

git remote get-url origin >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERRO: O remoto 'origin' ainda nao foi configurado.
    echo Use: git remote add origin URL_DO_REPOSITORIO
    goto :end
)

for /f "delims=" %%B in ('git branch --show-current') do set "BRANCH=%%B"
if not defined BRANCH (
    echo.
    echo ERRO: Nao foi possivel identificar a branch atual.
    goto :end
)

echo.
echo Adicionando alteracoes...
git add -A
if errorlevel 1 goto :giterror

git diff --cached --quiet
if not errorlevel 1 (
    echo Nenhuma alteracao para enviar.
    goto :end
)

echo Criando commit...
git commit -m "Atualizacao automatica"
if errorlevel 1 goto :giterror

echo Enviando a branch %BRANCH% para o GitHub...
git push -u origin "%BRANCH%"
if errorlevel 1 goto :pusherror

echo.
echo Pronto: projeto atualizado no GitHub.
goto :end

:giterror
echo.
echo ERRO: nao foi possivel preparar ou criar o commit.
goto :end

:pusherror
echo.
echo ERRO: o commit foi criado, mas o envio ao GitHub falhou.
echo Verifique se voce esta autenticado na conta com permissao para o repositorio.

:end
echo.
pause
endlocal
