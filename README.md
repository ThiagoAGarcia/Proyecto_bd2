# Proyecto_bd2

## Importante

Crear un archivo `.env` dentro de la carpeta `Backend`.

Cambiar los datos por los de tu entorno local.

Contenido del archivo `.env`:

```env
ConnectionStrings__DefaultConnection=Server=host.docker.internal;Port=3306;Database=proyectoBD2;User=root;Password=root;
Instalar Entity Framework CLI:

dotnet tool install --global dotnet-ef
Ejecutar con Docker
docker compose up --watch
Paquetes usados
dotnet add package MySqlConnector
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
Borrar imagen/contenedores
Solo si es necesario:

docker compose down
docker compose down --rmi local
```
