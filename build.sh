#!/bin/sh
set -e

(cd JellyJav; dotnet build)
mkdir -p ./docker-config/plugins
docker-compose kill
docker-compose up -d