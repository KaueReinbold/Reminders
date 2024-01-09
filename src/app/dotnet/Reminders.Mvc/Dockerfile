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
COPY ["src/app/dotnet/Reminders.Mvc/Reminders.Mvc.csproj", "src/app/dotnet/Reminders.Mvc/"]

# Check if all required environment variables are set
RUN if [ -n "${NUGET_SOURCE}" ] && [ -n "${NUGET_USER_NAME}" ] && [ -n "${NUGET_PASSWORD}" ]; then \
    dotnet nuget add source ${NUGET_SOURCE} -n NugetFeed -u ${NUGET_USER_NAME} -p ${NUGET_PASSWORD} --store-password-in-clear-text; \
else \
    echo "One or more required environment variables are not set. Skipping nuget source addition."; \
fi

RUN dotnet restore "src/app/dotnet/Reminders.Mvc/Reminders.Mvc.csproj"

# Build
COPY ./src .
WORKDIR "/src/app/dotnet/Reminders.Mvc"

ARG DOTNET_CONFIGURATION=Release

RUN dotnet build "Reminders.Mvc.csproj" -c ${DOTNET_CONFIGURATION} -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Reminders.Mvc.csproj" -c ${DOTNET_CONFIGURATION} -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reminders.Mvc.dll"]
