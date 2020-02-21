Sobre
Objetivo
Esta aplicação de exemplo, permite a realização de transações bancárias (saque, depósito e transferência). Além disso, é possível consultar informações de todas as operações realizadas.

Tecnologias utilizadas:

.Net Core 3.1
Web api Core
Swagger
MongoDb
Mongo Express
XUnit
Docker
Instalação
Link do projeto: https://github.com/JGomes22/DigitalBank

Clone o projeto em: https://github.com/JGomes22/DigitalBank.git

Realize o download do Docker: https://www.docker.com/get-started

Para executar a aplicação localmente, navegue até o diretório onde foi realizado o clone, pelo CMD (prompt de comando).
Exemplo: cd C:\Temp\GitHub\DigitalBank

Execute o comando: docker-compose up —build -d

Apis disponíveis em http://localhost:5000/swagger
Obs: Para testar a api de autenticação utilize as credenciais abaixo:
user: admin
password: password

Mongo Express disponível em http://localhost:8081
