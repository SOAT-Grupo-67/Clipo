# Clipo Service - FIAP X (Hack SOAT10)

Este repositório implementa o **serviço principal de processamento de vídeos** do projeto **Sistema de Processamento de Vídeos - FIAP X**, desenvolvido como parte do desafio do curso de Arquitetura de Software da FIAP.  

Ele é responsável por **receber vídeos, processá-los em frames e disponibilizar os resultados** para os usuários autenticados, além de integrar-se com serviços de armazenamento em nuvem e envio de notificações.  

---

## 📌 Funcionalidades

- Upload e processamento de vídeos em múltiplos formatos  
- Conversão de vídeos em **frames** (1 frame por segundo, configurável)  
- Geração de **pacote ZIP** com todos os frames extraídos  
- Upload automático do resultado para **S3 (ou compatível)**  
- Listagem de vídeos processados por usuário  
- Consulta do **status e progresso (%) de processamento** de um vídeo  
- Listagem de vídeos na fila de processamento  
- Envio de notificações (e-mail) em caso de falha ou conclusão  
- Acompanhamento do processamento via **Hangfire Dashboard** em `/hangfire`  

---

## 🏗️ Arquitetura

O serviço segue princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, estruturado da seguinte forma:  

- **Clipo.Api** → Camada de entrada (controllers e endpoints REST)  
- **Clipo.Application** → Casos de uso (upload, conversão, integração com S3, envio de e-mails)  
- **Clipo.Domain** → Entidades e regras de negócio centrais (vídeo, status, progresso)  
- **Infra** → Integrações com banco de dados, S3 e demais serviços externos  
- **Hangfire** → Responsável pelo agendamento e execução assíncrona dos jobs de processamento  

---

## ⚙️ Tecnologias

- **.NET 9 / C#**  
- **ASP.NET Web API**  
- **Entity Framework Core**  
- **Docker + Docker Compose**  
- **Hangfire** (jobs assíncronos + dashboard em `/hangfire`)  
- **FFmpeg + FFProbe** (análise e extração de frames)  
- **S3 Storage (MinIO ou AWS S3)**  
- **xUnit / NUnit** (testes unitários)  

---

## 🔄 Fluxo de Processamento

1. **Upload do vídeo**  
   - O usuário envia um vídeo via endpoint `/videos/convert`.  
   - O arquivo é salvo temporariamente no diretório `uploads/`.  

2. **Criação do Job**  
   - É criada uma entrada em banco (`VideoStatus`) com status **Pending**.  
   - O job é enfileirado no **Hangfire** para execução assíncrona.  

3. **Execução do Job (Processamento)**  
   - O status muda para **Processing**.  
   - O serviço baixa/atualiza a versão mais recente do **FFmpeg**.  
   - O vídeo é analisado pelo **FFProbe** para calcular duração e número de frames.  
   - Frames são extraídos (1 frame por segundo) e salvos em `frames/{videoId}`.  
   - **O progresso (%) é atualizado continuamente no banco**, permitindo que o usuário consulte em tempo real.  

4. **Geração do pacote ZIP**  
   - Após a extração, todos os frames são compactados em um arquivo `.zip`.  
   - O ZIP é salvo localmente e, se configurado, enviado para **S3**.  

5. **Upload para S3 (opcional)**  
   - Caso a configuração `AWS:UploadEnabled` esteja habilitada, o pacote ZIP é enviado.  
   - A URL do arquivo no S3 é salva no registro do vídeo.  

6. **Conclusão**  
   - O status é atualizado para **Done**.  
   - O progresso vai para **100%**.  
   - Logs de sucesso são gravados.  

7. **Tratamento de Erros**  
   - Em caso de falha, o status é atualizado para **Error**.  
   - O progresso é zerado.  
   - Se o e-mail do usuário estiver disponível no token JWT, é enviado um aviso de falha.  

---

## 📡 Endpoints principais

| Método | Rota                          | Descrição                              | Autenticação |
|--------|-------------------------------|----------------------------------------|---------------|
| POST   | `/videos/convert`            | Enviar vídeo para conversão em frames  | ✅            |
| GET    | `/videos/status/{id}`        | Consultar status e **progresso (%)**   | ✅            |
| GET    | `/videos/user/{userId}`      | Listar vídeos de um usuário            | ✅            |
| GET    | `/videos/queue`              | Listar vídeos na fila de processamento | ✅            |
| GET    | `/videos/converted`          | Listar vídeos já convertidos           | ✅            |

---

## 🔒 Segurança

- Autenticação via **JWT** (integrado ao serviço de usuários)  
- Autorização para acesso apenas a vídeos do usuário autenticado  
- Logs de auditoria para acompanhamento de erros e atividades  

---

## 📧 Envio de E-mails

O sistema possui suporte para **envio de notificações por e-mail** em caso de falhas no processamento dos vídeos.  

A configuração deve ser feita no arquivo `appsettings.json`, conforme o exemplo abaixo:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "User": "yourapp@gmail.com",
  "Password": "your-app-password"
}
```

---

🔑 Detalhes:
User → E-mail responsável pelo envio (ex.: uma conta técnica da aplicação).

Password → Senha ou App Password (no caso do Gmail é necessário gerar uma senha de app).

SmtpServer → Servidor SMTP usado para envio (ex.: Gmail, Outlook, Amazon SES).

Port → Porta de envio (587 para TLS).

Caso um job falhe, o sistema tenta identificar o e-mail do usuário (a partir do token JWT) e envia uma mensagem de erro com detalhes do processamento.

---

## ❓ Por que Hangfire e não RabbitMQ/Kafka/etc.?

Optamos por utilizar o **Hangfire** como mecanismo de enfileiramento e execução assíncrona de jobs, em vez de soluções de mensageria tradicionais como **RabbitMQ, Kafka ou SQS**, pelos seguintes motivos:  

- O problema do projeto é **processamento de tarefas demoradas (CPU-bound)**, não **troca de mensagens entre múltiplos serviços**.  
- O **Hangfire** usa o próprio banco de dados relacional como fila persistente, o que simplifica a infraestrutura e reduz a complexidade.  
- Ele oferece **dashboard pronto** para monitorar jobs, falhas e progresso.  
- Suporta **retries automáticos** e histórico dos jobs sem configuração extra.  
- Integra-se facilmente ao **.NET**, com uma curva de aprendizado menor que a de mensagerias robustas como Kafka.  

Em resumo: enquanto RabbitMQ/Kafka seriam indicados em cenários de **mensageria distribuída em larga escala**, o **Hangfire** atende perfeitamente à necessidade do projeto, oferecendo **execução assíncrona confiável, com persistência, reprocessamento e monitoramento integrado**, sem exigir uma infraestrutura adicional.  

---

## 👨‍💻 Autores

Projeto desenvolvido pelo grupo SOAT 67 para o Hackaton - Pós-Graduação em Arquitetura de Software (FIAP).
