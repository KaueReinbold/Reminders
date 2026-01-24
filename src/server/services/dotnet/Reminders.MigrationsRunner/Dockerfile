ARG DOTNET_ASPNET_IMAGE=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${DOTNET_SDK_IMAGE} AS build
WORKDIR /src

# Copy everything from source
COPY src/ .

# Restore dependencies
WORKDIR /src/server/services/dotnet/Reminders.MigrationsRunner
RUN dotnet restore "Reminders.MigrationsRunner.csproj"

# Build
ARG DOTNET_CONFIGURATION=Release
RUN dotnet build "Reminders.MigrationsRunner.csproj" -c ${DOTNET_CONFIGURATION} -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Reminders.MigrationsRunner.csproj" -c ${DOTNET_CONFIGURATION} -o /app/publish

# Final runtime image
FROM ${DOTNET_ASPNET_IMAGE} AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8081
ENTRYPOINT ["dotnet", "Reminders.MigrationsRunner.dll"]
