# GRMP — Gerenciador de Manutenção Predial

## 📌 Sobre o Projeto

O **GRMP (Gerenciador de Manutenção Predial)** é um sistema web desenvolvido para auxiliar no controle e gerenciamento de ordens de serviço de manutenção predial dentro do ambiente escolar do SENAI.

O sistema permite registrar solicitações, visualizar chamados em um mapa interativo da instituição, acompanhar status das ordens de serviço e gerenciar usuários.

---

# 🛠️ Funcionalidades

## 👤 Usuários

* Login e autenticação
* Controle de sessão
* Cadastro de usuários
* Alteração de usuários
* Diferentes níveis de acesso

## 📋 Ordens de Serviço

* Cadastro de OS
* Visualização de histórico
* Controle de status
* Prioridade da solicitação
* Associação com bloco e local
* Informações de solicitante e executor

## 🗺️ Mapa Interativo

* Visualização dos blocos da instituição
* Destaque visual para blocos com OS abertas
* Filtro por status
* Painel lateral com chamados
* Visualização de locais com ordens abertas

## 📄 Exportação

* Download das ordens de serviço em arquivo Word (.docx)

---

# 💻 Tecnologias Utilizadas

## Backend

* ASP.NET Core MVC
* C#
* SQL Server

## Frontend

* HTML5
* CSS3
* JavaScript
* Razor Pages

## Banco de Dados

* SQL Server

## Bibliotecas

* DocumentFormat.OpenXml

---

# 📁 Estrutura do Projeto

```bash
GRMP/
│
├── Controllers/
├── Models/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── img/
│   └── js/
│
├── Classes/
├── appsettings.json
└── Program.cs
```

---

# ⚙️ Como Executar o Projeto

## 1️⃣ Clonar o repositório

```bash
git clone https://github.com/Goose-CauaRodrigues/GRMP2.git
```

---

## 2️⃣ Abrir o projeto

Abra o projeto no:

* Visual Studio 2022

---

## 3️⃣ Configurar o banco de dados

Edite a string de conexão no arquivo:

```json
appsettings.json
```

Exemplo:

```json
"ConnectionStrings": {
  "StringConexaoSQLServer": "Server=SEU_SERVIDOR;Database=GRMP;Trusted_Connection=True;TrustServerCertificate=True"
}
```

---

## 4️⃣ Executar o projeto

No Visual Studio:

```bash
Ctrl + F5
```

Ou:

```bash
F5
```

---

# 🧩 Funcionalidades Futuras

* Dashboard com gráficos
* Upload de imagens nas OS
* Notificações automáticas
* Responsividade mobile
* Sistema de permissões mais avançado
* Exportação em PDF

---

# 👨‍💻 Desenvolvedores

Projeto desenvolvido para fins acadêmicos no SENAI.

* Nícolas Mendes
* Equipe GRMP

---

# 📄 Licença

Este projeto é destinado para fins educacionais.
