ARG DOTNET_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Application
COPY ["src/Web/Reminders.Api/Reminders.Api.csproj", "src/Web/Reminders.Api/"]

COPY ["src/Services/Reminders.Application/Reminders.Application.csproj", "src/Services/Reminders.Application/"]
COPY ["src/Services/Reminders.Infrastructure.CrossCutting/Reminders.Infrastructure.CrossCutting.csproj", "src/Services/Reminders.Infrastructure.CrossCutting/"]
COPY ["src/Services/Reminders.Domain/Reminders.Domain.csproj", "src/Services/Reminders.Domain/"]

COPY ["src/Data/Reminders.Infrastructure.Data.EntityFramework/Reminders.Infrastructure.Data.EntityFramework.csproj", "src/Data/Reminders.Infrastructure.Data.EntityFramework/"]

RUN dotnet restore "src/Web/Reminders.Api/Reminders.Api.csproj"

# Build
COPY . .
WORKDIR "/src/src/Web/Reminders.Api"
RUN dotnet build "Reminders.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Reminders.Api.csproj" -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reminders.Api.dll"]
