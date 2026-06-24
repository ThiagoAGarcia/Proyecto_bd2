# Proyecto_bd2

## Instrucciones
# Creación del archivo de entorno
Crear un archivo `.env` dentro de la carpeta `Backend`. Cambiar los datos por los de tu entorno local.
- Contenido del archivo `.env`:

```
env
ConnectionStrings__DefaultConnection=Server=host.docker.internal;Port=3306;Database=proyectoBD2;User=root;Password=root;

Gmail__Email=mundialucu2026@gmail.com
Gmail__Password=kebzimmwvnvkiqmm
```
---
# Instalar Entity Framework CLI
```
dotnet tool install --global dotnet-ef
```
---
# Instalar paquetes utilizados
```
dotnet add package MySqlConnector
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
dotnet add package MailKit
```
---
# Correr el proyecto con Docker
Tener abierta la aplicación de Docker Desktop.
En dos terminales simultáneamente ejecutar el backend y el frontend del proyecto.
Para ejcutarlos moverse a las carpetas respectivas de cada capa.

- Backend: 
```
cd Backend
docker compose up --watch
```

- Frontend:
```
cd Frontend
docker compose up dev --build
```

- Para borrar un contenedor, ***hacer solo si es necesario***
```
docker compose down
```
---
# Ejecutar la aplicación de React Native
Para poder acceder a la interfaz del funcionario se debe de contar con la aplicación Expo Go instalada en un celular.
Todo esto se ejecuta en una tercera terminal.

- Primero, moverse al directorio "Funcionario" e instalar los paquetes utilizados:
```
cd Funcionario
npx expo install @react-native-async-storage/async-storage
npx expo install expo-camera
npm install react-native-modal
npm install react-qr-code 
```
---
- Para ejecutar efectivamente la aplicación:
```
npx expo start
```
Leer el código QR expuesto en la terminal, el cual nos va a redirigir a la aplicación Expo Go anteriormente instalada.

---
- Si llega a fallar:
1. Entrar a una terminal y ejecutar el comando `ipconfig`.
2. Copiar nuestra dirección IPv4.
3. Dentro de la carpeta `/Funcionario/api`, colocar nuestra dirección IPv4 en las cadenas `response` que se encuentran en ambos endpoints: `qrChecks.ts` y `login.ts`. De modo que las cadenas lean: `http://[NUESTRA DIRECCIÓN IPV4]:5001/qr/token?token=${...}&mailPerfil=${...)}`
