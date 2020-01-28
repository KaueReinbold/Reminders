# Reminders

Application to register and list reminders.
Each reminder has a Title, Description, Date Limit and Status.

## Technical Objective

This application was developed with Asp.Net Core. 
The propos is show the knowledge using this techology and applying best code practices.

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
