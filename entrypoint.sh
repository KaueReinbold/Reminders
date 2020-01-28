#!/bin/bash

dotnet tool install --global dotnet-ef --version 3.1.1 

export PATH="$PATH:/root/.dotnet/tools"
set -e
run_cmd="tail -f /dev/null"

until dotnet ef database update --startup-project $1; do
>&2 echo "SQL Server is starting up"
sleep 1
done

>&2 echo "SQL Server is up - executing command"
exec $run_cmd