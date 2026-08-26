# ☕ Café Missionário — Sistema de Gestão de Pedidos e Estoque

O **Café Missionário** é uma aplicação multiplataforma desenvolvida para simplificar a operação de atendimento, controle de vendas e baixa automatizada de estoque com base em fichas técnicas ajustáveis.

---

## 🚀 Funcionalidades Principais

* **Gestão Dinâmica de Pedidos:** Seleção rápida de itens com controle individual de quantidades e cálculo de total em tempo real.
* **Baixa Proporcional de Estoque:** Desconto automático dos insumos do estoque base utilizando o cadastro de **Ficha Técnica** (ex: receitas compostas por frações de insumos).
* **Integração com WhatsApp:** Geração automática do resumo formatado do pedido para cópia e envio direto ao cliente.
* **Dashboard de Relatórios:** Visualização de faturamento total, quantidade de pedidos e divisão de vendas por forma de pagamento com filtro por data.
* **Interface Moderna:** Design responsivo e focado na usabilidade, otimizado para navegação em desktop.

---

## 🛠️ Tecnologias Utilizadas

* **Linguagem:** C# (.NET 8)
* **Framework UI:** .NET MAUI (Multi-platform App UI)
* **Arquitetura:** MVVM (Model-View-ViewModel) via `CommunityToolkit.Mvvm`
* **Banco de Dados:** SQLite
* **ORM:** Entity Framework Core

---

## 📁 Estrutura do Projeto

```text
CafeMissionario/
├── Data/            # Configuração do DbContext e migrações do banco
├── Models/          # Entidades (Produto, Pedido, FichaTecnica)
├── ViewModels/      # Regras de negócio e bindings das telas
├── Views/           # Interfaces de usuário em XAML
└── Resources/       # Ícones, fontes e estilos visuais