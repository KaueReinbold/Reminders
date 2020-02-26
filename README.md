# Reminders

Application to register and list reminders.
Each reminder has a Title, Description, Date Limit and Status.

## Technical Objective

This application was developed with Asp.Net Core. 
The propos is show the knowledge using this techology and applying best code practices.

![.NET Core](https://github.com/KaueReinbold/Reminders/workflows/.NET%20Core/badge.svg?branch=master)

![Docker Image CI](https://github.com/KaueReinbold/Reminders/workflows/Docker%20Image%20CI/badge.svg?branch=master)

[![Coverage Status](https://coveralls.io/repos/github/KaueReinbold/Reminders/badge.svg?branch=master&service=github)](https://coveralls.io/github/KaueReinbold/Reminders?branch=master)

## Tip
    
List images 
```cmd
docker images --format '{{.ID}} - {{.Size}} - {{.Repository}}:{{.Tag}}'
```

List containers
```cmd
docker ps -a --format '{{.ID}} - {{.Image}} - {{.Names}} - {{.Ports}}'
```

Check container ip
```cmd
docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' {container_name}
``` 
