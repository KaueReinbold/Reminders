ARG DOTNET_ASPNET_IMAGE
ARG DOTNET_SDK_IMAGE

FROM ${DOTNET_ASPNET_IMAGE} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM ${DOTNET_SDK_IMAGE} AS build
WORKDIR /src

ARG NUGET_SOURCE
ARG NUGET_USER_NAME
ARG NUGET_PASSWORD

# Application
COPY ["src/server/api/dotnet/Reminders.Api/Reminders.Api.csproj", "src/server/api/dotnet/Reminders.Api/"]

COPY ["src/server/application/dotnet/Reminders.Application/Reminders.Application.csproj", "src/server/application/dotnet/Reminders.Application/"]
COPY ["src/server/infrastructure/dotnet/Reminders.Infrastructure.CrossCutting/Reminders.Infrastructure.CrossCutting.csproj", "src/server/infrastructure/dotnet/Reminders.Infrastructure.CrossCutting/"]
COPY ["src/server/domain/dotnet/Reminders.Domain/Reminders.Domain.csproj", "src/server/domain/dotnet/Reminders.Domain/"]

COPY ["src/server/data/dotnet/Reminders.Infrastructure.Data.EntityFramework/Reminders.Infrastructure.Data.EntityFramework.csproj", "src/server/data/dotnet/Reminders.Infrastructure.Data.EntityFramework/"]
COPY ["src/server/data/dotnet/Reminders.Infrastructure.Data.EntityFramework.SqlServer/Reminders.Infrastructure.Data.EntityFramework.SqlServer.csproj", "src/server/data/dotnet/Reminders.Infrastructure.Data.EntityFramework.SqlServer/"]
COPY ["src/server/data/dotnet/Reminders.Infrastructure.Data.EntityFramework.Postgres/Reminders.Infrastructure.Data.EntityFramework.Postgres.csproj", "src/server/data/dotnet/Reminders.Infrastructure.Data.EntityFramework.Postgres/"]

# Check if all required environment variables are set
RUN if [ -n "${NUGET_SOURCE}" ] && [ -n "${NUGET_USER_NAME}" ] && [ -n "${NUGET_PASSWORD}" ]; then \
    dotnet nuget add source ${NUGET_SOURCE} -n NugetFeed -u ${NUGET_USER_NAME} -p ${NUGET_PASSWORD} --store-password-in-clear-text; \
else \
    echo "One or more required environment variables are not set. Skipping nuget source addition."; \
fi

RUN dotnet restore "src/server/api/dotnet/Reminders.Api/Reminders.Api.csproj"

# Build
COPY ./src .
WORKDIR "/src/server/api/dotnet/Reminders.Api"

ARG DOTNET_CONFIGURATION=Release

RUN dotnet build "Reminders.Api.csproj" -c ${DOTNET_CONFIGURATION} -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Reminders.Api.csproj" -c ${DOTNET_CONFIGURATION} -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reminders.Api.dll"]