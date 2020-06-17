# SDK Build configuration
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS reminders-sdk

# Add dotnet tool for migration
RUN dotnet tool install --global dotnet-ef 
ENV PATH="$PATH:/root/.dotnet/tools"

# Set up the work directory 
WORKDIR /app

# Copy all items to the work directory
COPY ./src ./

# Generate scripts to create tables
RUN dotnet ef migrations script --idempotent --output /app/database/out/create-tables.sql --project Reminders.Infrastructure.Data.EntityFramework/

# ---------------------------------------------------------------------------------- #

# Use sqlserver images.
FROM mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04

# Set envrionment variables
# Accept MSSQL policies 
ENV ACCEPT_EULA="Y" 
# SA password
ENV SA_PASSWORD="MbvwrCa3x9ZpMCcT"

# Create a config directory
RUN mkdir -p /usr/config

# Set up the work directory 
WORKDIR /usr/config

# Bundle config source
COPY ./infrastructure/mssql /usr/config

# Copy create_tables script from .Net image
COPY --from=reminders-sdk /app/database/out/create-tables.sql .

# Grant permissions for to our scripts to be executable
USER root
RUN chmod +x /usr/config/mssql-entrypoint.sh
RUN chmod +x /usr/config/configure-database.sh

ENTRYPOINT ["./mssql-entrypoint.sh"]

CMD ["tail -f /dev/null"]
