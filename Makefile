SOLUTION := UrlShortener.sln
API_PROJECT := UrlShortener.Api/UrlShortener.Api.csproj
INFRA_PROJECT := UrlShortener.Infrastructure/UrlShortener.Infrastructure.csproj
CONFIGURATION ?= Release

.DEFAULT_GOAL := help

.PHONY: help restore build test run postgres docker docker-build stop down logs ps \
	migration migrate migration-remove migration-list migration-script clean reset-db

help:
	@echo "Comandos disponíveis:"
	@echo "  make run                    Roda a API localmente"
	@echo "  make postgres               Sobe apenas o PostgreSQL"
	@echo "  make docker                 Sobe API e PostgreSQL no Docker"
	@echo "  make docker-build           Reconstroi e sobe tudo no Docker"
	@echo "  make stop                   Para os containers"
	@echo "  make down                   Remove os containers"
	@echo "  make logs                   Acompanha os logs"
	@echo "  make ps                     Mostra os containers"
	@echo "  make build                  Compila a solução"
	@echo "  make test                   Executa os testes"
	@echo "  make migration name=Nome    Cria uma migration"
	@echo "  make migrate                Aplica as migrations no banco"
	@echo "  make migration-remove       Remove a última migration"
	@echo "  make migration-list         Lista as migrations"
	@echo "  make migration-script       Gera um script SQL idempotente"
	@echo "  make reset-db               APAGA o volume e recria o banco"

restore:
	dotnet restore $(SOLUTION)

build: restore
	dotnet build $(SOLUTION) --configuration $(CONFIGURATION) --no-restore

test: restore
	dotnet test $(SOLUTION) --configuration $(CONFIGURATION) --no-restore

run:
	dotnet run --project $(API_PROJECT) --launch-profile http

postgres:
	docker compose up -d postgres

docker:
	docker compose up -d

docker-build:
	docker compose up -d --build

stop:
	docker compose stop

down:
	docker compose down

logs:
	docker compose logs -f

ps:
	docker compose ps

migration:
	@test -n "$(name)" || (echo "Informe o nome: make migration name=NomeDaMigration" && exit 1)
	dotnet ef migrations add $(name) --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

migrate:
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

migration-remove:
	dotnet ef migrations remove --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

migration-list:
	dotnet ef migrations list --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

migration-script:
	dotnet ef migrations script --idempotent --output migrations.sql --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

clean:
	dotnet clean $(SOLUTION)

reset-db:
	docker compose down --volumes
	docker compose up -d postgres

