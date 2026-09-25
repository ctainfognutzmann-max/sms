# Autenticação da API

Todas as rotas que começam com `/api` exigem uma chave no cabeçalho `X-Api-Key`.

Em desenvolvimento, utilize a chave definida em `appsettings.Development.json`:

```http
GET /api/GetExamesList HTTP/1.1
Host: localhost:5215
X-Api-Key: aghos-dev-c4e6f1a09d3248b28e75c6d3f4a96b10
```

Requisições sem chave, ou com chave inválida, recebem `401 Unauthorized`.

Para produção, configure uma ou mais chaves fora de arquivos versionados por variáveis de ambiente. Os índices permitem rotação de chaves:

```powershell
$env:ApiKey__Keys__0 = "uma-chave-aleatoria-longa"
$env:ApiKey__Keys__1 = "nova-chave-durante-a-rotacao"
```

O aplicativo não inicia se nenhuma chave estiver configurada. No Swagger, clique em **Authorize** e informe o valor da chave.

## Agendamentos por período

```http
GET /api/AgendamentosPorPeriodo?dataInicial=01/01/2026&dataFinal=31/01/2026&idHospital=1 HTTP/1.1
X-Api-Key: sua-chave
```

As datas devem estar no formato `DD/MM/AAAA`. `idHospital` é opcional; sem ele, a rota retorna os exames de todos os hospitais no intervalo informado.
