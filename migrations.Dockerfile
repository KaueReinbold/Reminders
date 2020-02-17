# SDK Build configuration
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS reminders-sdk

# Add dotnet tool for migration
RUN dotnet tool install --global dotnet-ef --version 3.1.1 
ENV PATH="$PATH:/root/.dotnet/tools"

# Set up the work directory 
WORKDIR /app

# Copy all items to the work directory
COPY . ./

# Give script permissions
RUN chmod +x ./infrastructure/wait-for.sh

# Update databse
CMD /app/infrastructure/wait-for.sh reminders-database:1433 -t 4000 -- dotnet ef database update --project /app/src/Reminders.Infrastructure.Data
