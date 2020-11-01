# SDK Build configuration
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env

# Set up the work directory
WORKDIR /app

# Copy file to container
COPY . .

# Restore packages
RUN dotnet restore

# Publish code
RUN dotnet publish -c Release -o ./out/ 

# ============================================================================

# Building image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

# Section to enable globalization unavailable on alpine version
# https://www.abhith.net/blog/docker-sql-error-on-aspnet-core-alpine/
# Start
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
# End

# Set up the work directory
WORKDIR /app

# Copy publish result from build image
COPY --from=build-env /app/out/ .

# Setup entrypoint
ENTRYPOINT dotnet Reminders.Api.dll
