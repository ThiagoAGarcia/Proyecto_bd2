# Proyecto_bd2

**IMPORTANT**

> crear .env en la carpeta Backend
> cambiar datos por los suyos
> poner dentro : ConnectionStrings\_\_DefaultConnection=Server=host.docker.internal;Port=3306;Database=proyectoBD2;User=root;Password=root;
> dotnet tool install --global dotnet-ef

> runnear docker:
> docker compose up --watch

**NOTE**

> frameworks que usamos
> dotnet add package MySqlConnector
> dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
> dotnet add package BCrypt.Net-Next

**NOTE**

> borrar la imagen (solo si es necesario):
> docker compose down
> docker compose down --rmi local
