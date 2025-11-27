BankMore — Sistema Bancário com Arquitetura de Microserviços

O BankMore é um sistema bancário construído com microserviços independentes, utilizando:

- .NET 8
- DDD (Domain-Driven Design)
- CQRS + MediatR
- Dapper
- SQLite
- JWT Authentication
- Testes Unitários e de Integração

O objetivo é oferecer um ambiente modular, simples e didático para operações bancárias como contas, movimentações e transferências, com comunicação interna entre serviços.

------------------------------------
MICROSERVIÇOS
------------------------------------

Contas
Gerencia:
- Abertura de contas
- Login e autenticação (JWT)
- Consulta de contas
- Inativação de conta
- Armazenamento e validação de senhas

Movimentações
Responsável por:
- Débito
- Crédito
- Registro de movimentos financeiros
- Consulta de saldo consolidado

Transferências
Executa:
- Transferências entre contas
- Idempotência
- Integração com Movimentações
- Registro e atualização do status

Shared
Biblioteca para:
- DTOs de contrato
- Enums
- Códigos de erro
- Requests e Responses padronizados

------------------------------------
ARQUITETURA INTERNA
------------------------------------

Cada microserviço segue:

.Api
.Application
.Domain
.Infra
.Tests

------------------------------------
SEGURANÇA
------------------------------------

- JWT
- Tokens internos para integração
- Separação entre tokens públicos e internos

------------------------------------
TESTES
------------------------------------

- Testes de integração com WebApplicationFactory
- Testes unitários com Moq
- SQLite em memória
- Testes de idempotência

------------------------------------
OBJETIVO
------------------------------------

Servir como base:
- microserviços
- DDD
- CQRS
- comunicação interna
- modularidade e escalabilidade
