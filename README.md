# Proyecto_bd2

## Importante

Crear un archivo `.env` dentro de la carpeta `Backend`.

Cambiar los datos por los de tu entorno local.

Contenido del archivo `.env`:

```env
ConnectionStrings__DefaultConnection=Server=host.docker.internal;Port=3306;Database=proyectoBD2;User=root;Password=root;

Gmail__Email=mundialucu2026@gmail.com
Gmail__Password=kebzimmwvnvkiqmm


Instalar Entity Framework CLI:

dotnet tool install --global dotnet-ef
Ejecutar con Docker
docker compose up --watch
Paquetes usados
dotnet add package MySqlConnector
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
dotnet add package MailKit

Borrar imagen/contenedores
Solo si es necesario:

docker compose down
docker compose down --rmi local

Para runnear el Frontend ir a /Frontend y ejecutar el comando
docker compose up dev --watch

Para runnear la pagina de expo del funcionario:

Librerias usadas:
npx expo install @react-native-async-storage/async-storage
npx expo install expo-camera
npm install react-native-modal
npm install react-qr-code 

npx expo start (para esto en sus dispositivos tienen que instalar la aplicacion de expo)
IMPORTANTE leer lo siguiente si no les funciona

Si la pantalla de funcionario no les funciona probar fijarse su direccion ipv4 tirando el comando ipconfig en su cmd y cambiando en la ruta /Funcionario/api en los dos endpoints poniendo su direccion ipv4
```
