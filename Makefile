EF := dotnet ef
EF_PROJECTS := --project SocialMedia.Infrastructure --startup-project SocialMedia.Api

TEST_TYPES := unit integration functional architecture
TEST_PROJECTS_unit := $(wildcard tests/*.UnitTests)
TEST_PROJECTS_integration := $(wildcard tests/*.IntegrationTests)
TEST_PROJECTS_functional := $(wildcard tests/*.FunctionalTests)
TEST_PROJECTS_architecture := $(wildcard tests/*.ArchitectureTests)
TEST_ARGS := $(if $(FILTER),--filter "$(FILTER)")

.PHONY: tools build run watch format test migration migrate migrations-bundle db-reset

tools:
	dotnet tool restore

build:
	dotnet build SocialMedia.slnx

run:
	dotnet run --project SocialMedia.Api

watch:
	DOTNET_USE_POLLING_FILE_WATCHER=1 dotnet watch --project SocialMedia.Api run

format:
	dotnet format SocialMedia.slnx --exclude SocialMedia.Infrastructure/Migrations

test:
ifdef TYPE
ifeq ($(filter $(TYPE),$(TEST_TYPES)),)
	$(error TYPE must be one of: $(TEST_TYPES))
endif
	@for project in $(TEST_PROJECTS_$(TYPE)); do \
		dotnet test $$project $(TEST_ARGS) || exit 1; \
	done
else
	dotnet test SocialMedia.slnx $(TEST_ARGS)
endif

migration: tools
ifndef NAME
	$(error NAME is required, e.g. make migration NAME=AddUsersTable)
endif
	$(EF) migrations add $(NAME) $(EF_PROJECTS) --output-dir Migrations

migrate: tools
	$(EF) database update $(TARGET) $(EF_PROJECTS)

migrations-bundle: tools
	$(EF) migrations bundle $(EF_PROJECTS) --configuration Release --output artifacts/efbundle --force

db-reset: tools
	$(EF) database drop --force $(EF_PROJECTS)
	$(EF) database update $(EF_PROJECTS)
