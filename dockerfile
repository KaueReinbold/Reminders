FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

# Set up the work directory 
WORKDIR /app

# Copy all items to the work directory
COPY . ./

# Publish Api project
RUN dotnet publish -c Release -o out/api src/Reminders.Api

# Publish Mvc project
RUN  dotnet publish -c Release -o out/mvc src/Reminders.Mvc

# Let container up running tail command
RUN chmod +x ./entrypoint.sh
CMD /bin/sh ./entrypoint.sh /app/src/Reminders.Mvc