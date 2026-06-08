# Proyecto_bd2

**IMPORTANT**

> create .env en la carpeta Backend
> cambiar datos por los suyos
> poner dentro : ConnectionStrings\_\_DefaultConnection=Server=host.docker.internal;Port=3306;Database=proyectoBD2;User=root;Password=root;
> dotnet tool install --global dotnet-ef

> runnear docker:
> docker compose up --watch

**NOTE**

> frameworks que usamos
> dotnet add package Microsoft.EntityFrameworkCore
> dotnet add package Microsoft.EntityFrameworkCore.SqlServer
> dotnet add package Microsoft.EntityFrameworkCore.Design
> dotnet add package Microsoft.EntityFrameworkCore.Tools
> dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

**NOTE**

> crear clases con las tablas de la BD:
> dotnet ef dbcontext scaffold "Server=localhost;Port=3306;Database=ProyectoBD2;User=root;Password=root;" Pomelo.EntityFrameworkCore.MySql -o Models -c AppDbContext --context-dir Data --force

**IMPORTANT**

> borrar la imagen:
> docker compose down
> docker compose down --rmi local
