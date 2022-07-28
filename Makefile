.PHONY: build-test
build-test:
	docker-compose -f DocumentsApi.Tests/compose.yml build
	
.PHONY: serve-test
serve-test:
	make build-test && make start-test

.PHONY: start-test
start-test:
	docker compose -f DocumentsApi.Tests/compose.yml stop
	docker compose -f DocumentsApi.Tests/compose.yml run --rm test
	docker compose -f DocumentsApi.Tests/compose.yml stop

.PHONY: build-local
build-local:
	docker-compose -f DocumentsApi/compose.yml build
	
.PHONY: serve-local
serve-local:
	make build-local && make start-local

.PHONY: start-local
start-local:
	docker compose -f DocumentsApi/compose.yml stop
	docker compose -f DocumentsApi/compose.yml up -d
	
.PHONY: stop-local
stop-local:
	docker compose -f DocumentsApi/compose.yml stop


.PHONY: shell
shell:
	docker-compose run documents-api bash

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

.PHONY: restart-db
restart-db:
	docker stop $$(docker ps -q --filter ancestor=test-database -a)
	-docker rm $$(docker ps -q --filter ancestor=test-database -a)
	docker rmi test-database
	docker-compose up -d test-database
