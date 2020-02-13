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
COPY --from=reminders-build-image /app/out/mvc .

RUN chmod +x ./wait-for.sh