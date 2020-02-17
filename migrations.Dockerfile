FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

# Add dotnet tool for migration
RUN dotnet tool install --global dotnet-ef --version 3.1.1 
ENV PATH="$PATH:/root/.dotnet/tools"

# Set up the work directory 
WORKDIR /app

# Copy all items to the work directory
COPY . ./

RUN chmod +x ./infrastructure/wait-for.sh

RUN ls

# Update databse
CMD ./infrastructure/wait-for.sh reminders-database:1433 -t 4000 -- dotnet ef database update --project ./src/Reminders.Infrastructure.Data/