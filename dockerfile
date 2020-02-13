FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

# Add dotnet tool for migration
RUN dotnet tool install --global dotnet-ef --version 3.1.1 
ENV PATH="$PATH:/root/.dotnet/tools"

# Set up the work directory 
WORKDIR /app

# Copy all items to the work directory
COPY . ./

# Publish Api project
RUN dotnet publish -c Release -o out/api src/Reminders.Api

# Publish Mvc project
RUN dotnet publish -c Release -o out/mvc src/Reminders.Mvc

COPY ./wait-for.sh out/api
COPY ./wait-for.sh out/mvc

RUN chmod +x ./wait-for.sh