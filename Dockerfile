FROM mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04

ENV ACCEPT_EULA="Y" 
ENV SA_PASSWORD="MbvwrCa3x9ZpMCcT"

# Create a config directory
RUN mkdir -p /usr/config
WORKDIR /usr/config

# Bundle config source
COPY ./infrastructure/mssql /usr/config

# Grant permissions for to our scripts to be executable
USER root
RUN chmod +x /usr/config/mssql-entrypoint.sh
RUN chmod +x /usr/config/configure-database.sh

ENTRYPOINT ["./mssql-entrypoint.sh"]

CMD ["tail -f /dev/null"]
