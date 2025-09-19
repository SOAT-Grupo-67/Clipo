# Clipo Service - FIAP X (Hack SOAT10)

Este repositório implementa o **serviço principal de processamento de vídeos** do projeto **Sistema de Processamento de Vídeos - FIAP X**, desenvolvido como parte do desafio do curso de Arquitetura de Software da FIAP.  

Ele é responsável por **receber vídeos, processá-los em frames e disponibilizar os resultados** para os usuários autenticados, além de integrar-se com serviços de mensageria, armazenamento em nuvem e envio de notificações.  

---

## 📌 Funcionalidades

- Upload e processamento de vídeos em múltiplos formatos  
- Conversão de vídeos em **frames**  
- Listagem de vídeos processados por usuário  
- Consulta do **status de processamento** de um vídeo  
- Listagem de vídeos na fila de processamento  
- Envio de notificações (e-mail) em caso de falha ou conclusão  
- Armazenamento de arquivos em **S3** (ou serviço compatível)  

---

## 🏗️ Arquitetura

O serviço segue princípios de **arquitetura limpa (Clean Architecture)** e **Domain-Driven Design (DDD)**:  

- **Clipo.Api** → Camada de entrada (controllers e endpoints REST)  
- **Clipo.Application** → Casos de uso e serviços (processamento de vídeo, S3, envio de e-mails)  
- **Clipo.Domain** → Entidades e regras de negócio centrais  
- **Mensageria** → Integração com fila (ex.: RabbitMQ/Kafka) para processar vídeos assíncronamente  
- **Infra** → Configuração de banco, cache e storage  

---

## ⚙️ Tecnologias

- **.NET 6 / C#**  
- **ASP.NET Core Web API**  
- **Entity Framework Core**  
- **Docker + Docker Compose**  
- **Mensageria (RabbitMQ/Kafka)**  
- **S3 Storage (MinIO ou AWS S3)**  
- **xUnit / NUnit** (testes unitários)  

---

## 🚀 Como executar

### Pré-requisitos
- .NET 6 SDK  
- Docker e Docker Compose  

### Passos

```bash
# Clonar o repositório
git clone https://github.com/seu-grupo/clipo_service.git
cd clipo_service

# Subir containers (app + banco de dados + mensageria)
docker-compose up --build
```

O serviço estará disponível em:  
```
http://localhost:5000
```

---

## 📡 Endpoints principais

| Método | Rota                          | Descrição                              | Autenticação |
|--------|-------------------------------|----------------------------------------|---------------|
| POST   | `/videos/convert`            | Enviar vídeo para conversão em frames  | ✅            |
| GET    | `/videos/status/{id}`        | Consultar status do vídeo              | ✅            |
| GET    | `/videos/user/{userId}`      | Listar vídeos de um usuário            | ✅            |
| GET    | `/videos/queue`              | Listar vídeos na fila de processamento | ✅            |
| GET    | `/videos/converted`          | Listar vídeos já convertidos           | ✅            |

---

## 🔒 Segurança

- Autenticação via **JWT** (integrado ao serviço de usuários)  
- Autorização para acesso apenas a vídeos do usuário autenticado  

---

## 👨‍💻 Autores

Projeto desenvolvido pelo grupo SOAT 67 para o **Hackaton - Pós-Graduação em Arquitetura de Software (FIAP)**.  
