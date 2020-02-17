FROM mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04

ENV ACCEPT_EULA="Y" 
ENV SA_PASSWORD="MbvwrCa3x9ZpMCcT"

# Create a config directory
RUN mkdir -p /usr/config
WORKDIR /usr/config

# Bundle config source
COPY . /usr/config

# Grant permissions for to our scripts to be executable
USER root
RUN chmod +x /usr/config/entrypoint.sh
RUN chmod +x /usr/config/configure.sh

ENTRYPOINT ["./entrypoint.sh"]

CMD ["tail -f /dev/null"]