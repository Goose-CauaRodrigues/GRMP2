GRMP — Gerenciador de Requisições de Manutenção Predial

Sistema web desenvolvido para gerenciamento de Ordens de Serviço (OS) de manutenção predial dentro de uma instituição, utilizando mapa interativo dos blocos para visualização dos chamados.

📌 Sobre o Projeto

O GRMP foi criado para facilitar o controle de solicitações de manutenção predial, permitindo:

Cadastro de ordens de serviço
Visualização de chamados em mapa interativo
Controle de status das OS
Gerenciamento de usuários
Histórico completo das solicitações
Exportação de OS em documento Word
Dashboard e relatórios

O sistema foi desenvolvido utilizando ASP.NET Core MVC, com banco de dados em SQL Server.

🖼️ Funcionalidades
✅ Mapa Interativo
Visualização dos blocos da instituição
Destaque visual de blocos com OS abertas
Clique nos blocos para visualizar locais afetados
Painel lateral com OS abertas do local selecionado
✅ Ordens de Serviço
Cadastro de novas OS
Controle de:
Status
Prioridade
Categoria
Executor
Histórico completo de solicitações
✅ Gerenciamento de Usuários
Cadastro
Alteração
Controle de nível de acesso
✅ Exportação de Documentos
Download das informações da OS em .docx
✅ Dashboard
Relatórios visuais do sistema
🛠️ Tecnologias Utilizadas
Backend
ASP.NET Core MVC
C#
ADO.NET
SQL Server
Frontend
HTML5
CSS3
JavaScript
Bibliotecas
DocumentFormat.OpenXml
Bootstrap (se estiver usando)
Google Fonts
🗂️ Estrutura do Projeto
GRMP/
│
├── Controllers/
├── Models/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── img/
├── Banco/
└── README.md
⚙️ Configuração do Projeto
1️⃣ Clone o repositório
git clone https://github.com/Goose-CauaRodrigues/GRMP2.git
2️⃣ Configure a conexão com o banco

No arquivo:

appsettings.json

Configure:

"ConnectionStrings": {
  "StringConexaoSQLServer": "Server=SEU_SERVIDOR;Database=GRMP;Trusted_Connection=True;TrustServerCertificate=True;"
}
3️⃣ Execute o banco de dados

Execute os scripts SQL de criação das tabelas no SQL Server.

4️⃣ Execute o projeto

No Visual Studio:

Ctrl + F5

Ou pelo terminal:

dotnet run
👨‍💻 Desenvolvedores

Projeto desenvolvido por:

Nícolas Mendes
Cauã Rodrigues
📄 Licença

Projeto acadêmico desenvolvido para fins educacionais.
