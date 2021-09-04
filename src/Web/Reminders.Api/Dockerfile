FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Web/Reminders.Api/Reminders.Api.csproj", "src/Reminders.Api/"]
COPY ["src/Service/Reminders.Application/Reminders.Application.csproj", "src/Reminders.Application/"]
COPY ["src/Service/Reminders.CrossCutting/Reminders.CrossCutting.csproj", "src/Reminders.CrossCutting/"]
COPY ["src/Service/Reminders.Domain/Reminders.Domain.csproj", "src/Reminders.Domain/"]
COPY ["src/Data/Reminders.Data.EntityFramework/Reminders.Data.EntityFramework.csproj", "src/Reminders.Data.EntityFramework/"]
RUN dotnet restore "src/Reminders.Api/Reminders.Api.csproj"
COPY . .
WORKDIR "/src/src/Reminders.Api"
RUN dotnet build "Reminders.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reminders.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reminders.Api.dll"]
