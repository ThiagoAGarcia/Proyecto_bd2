    DROP DATABASE IF EXISTS proyectoBD2;

    CREATE DATABASE proyectoBD2;

    USE proyectoBD2;

    CREATE TABLE Perfil(
        mail VARCHAR(200) PRIMARY KEY,
        paisDocumento ENUM(
          'uruguay',
          'argentina',
          'brasil',
          'chile',
          'paraguay',
          'peru',
          'colombia',
          'mexico',
          'españa',
          'estados_unidos',
          'canada'
        ) NOT NULL,
        tipoDocumento ENUM(
          'ci',
          'dni',
          'cpf',
          'rut',
          'cc',
          'curp',
          'ssn',
          'sin'
        ) NOT NULL,
        numeroDocumento VARCHAR(32) NOT NULL,
        direccionPais VARCHAR(100) NOT NULL,
        direccionLocalidad VARCHAR(32) NOT NULL,
        direccionCalle VARCHAR(100) NOT NULL,
        direccionNumero INT NOT NULL,
        direccionCodigoPostal INT NOT NULL,
        unique(numeroDocumento, tipoDocumento)
    );

    CREATE TABLE Telefono (
        mailPerfil VARCHAR(200) NOT NULL,
        telefono VARCHAR(16) NOT NULL,
        PRIMARY KEY (mailPerfil, telefono),
        FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail)
    );

    CREATE TABLE Login (
        mailPerfil VARCHAR(200) PRIMARY KEY,
        password VARCHAR(256) NOT NULL,
        FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail)
    );

    CREATE TABLE Equipo(
        nombre VARCHAR(50) PRIMARY KEY,
        bandera VARCHAR(256)
    );

    CREATE TABLE Estadio (
        identificador INT PRIMARY KEY AUTO_INCREMENT,
        imagen VARCHAR(256),
        nombre VARCHAR(32) NOT NULL,
        nombrePais ENUM('estados unidos', 'canada', 'mexico') NOT NULL,
        direccionLocalidad VARCHAR(200) NOT NULL,
        direccionCalle VARCHAR(20) NOT NULL,
        direccionNumero INT NOT NULL,
        direccionCodigoPostal INT NOT NULL
    );

    CREATE TABLE Sector (
        identificador INT NOT NULL,
        identificadorEstadio INT NOT NULL,
        nombre VARCHAR(10) NOT NULL,
        capMax INT NOT NULL,
        tarifaExtra INT NOT NULL,
        FOREIGN KEY (identificadorEstadio) REFERENCES Estadio(identificador),
        PRIMARY KEY (identificadorEstadio, identificador)
    );

    CREATE TABLE Partido (
        identificador INT AUTO_INCREMENT PRIMARY KEY,
        fase VARCHAR(30) NOT NULL,
        EquipoLocal VARCHAR(32) NOT NULL,
        EquipoVisitante VARCHAR(32) NOT NULL,
        identificadorEstadio INT NOT NULL,
        fechaHora DATETIME NOT NULL,
        precio INT NOT NULL,
        FOREIGN KEY (EquipoLocal) REFERENCES Equipo(nombre),
        FOREIGN KEY (EquipoVisitante) REFERENCES Equipo(nombre),
        FOREIGN KEY (identificadorEstadio) REFERENCES Estadio(identificador)
    );

    CREATE TABLE Habilita(
        identificadorPartido INT,
        identificadorSector INT,
        identificadorEstadio INT,
        FOREIGN KEY (identificadorEstadio, identificadorSector) REFERENCES Sector(identificadorEstadio, identificador),
        FOREIGN KEY (identificadorPartido) REFERENCES Partido(identificador),
        PRIMARY KEY (identificadorEstadio, identificadorPartido, identificadorSector)
    );

    CREATE TABLE Funcionario (
        mailPerfil VARCHAR(200) PRIMARY KEY,
        numeroLegajo INT UNIQUE NOT NULL,
        FOREIGN KEY (mailPerfil) REFERENCES Perfil (mail)
    );

    CREATE TABLE Dispositivo (
        identificador INT PRIMARY KEY AUTO_INCREMENT,
        mailFuncionario VARCHAR(200),
        fechaAsignacion DATE DEFAULT(CURRENT_DATE),
        FOREIGN KEY (mailFuncionario) REFERENCES Funcionario(mailPerfil)
    );

    CREATE TABLE Administrador(
        mailPerfil VARCHAR(200) PRIMARY KEY,
        fechaAsignacionCargo DATE NOT NULL DEFAULT(CURRENT_DATE),
        nombrePais ENUM('estados unidos', 'canada', 'mexico') NOT NULL,
        FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail)
    );

    CREATE TABLE EsAsignado(
        identificadorDispositivo INT,
        identificadorEstadio INT,
        identificadorPartido INT,
        identificadorSector INT,
        fecha DATE NOT NULL DEFAULT(CURRENT_DATE),
        FOREIGN KEY (identificadorDispositivo) REFERENCES Dispositivo(identificador),
        FOREIGN KEY (identificadorEstadio, identificadorPartido, identificadorSector) REFERENCES Habilita(identificadorEstadio,identificadorPartido,identificadorSector),
        PRIMARY KEY (identificadorDispositivo, identificadorEstadio, identificadorPartido, identificadorSector));

    CREATE TABLE Usuario(
        mailPerfil VARCHAR(200) PRIMARY KEY,
        fechaRegistro DATE NOT NULL DEFAULT(CURRENT_DATE),
        estadoVerificado ENUM('verificado', 'noVerificado') NOT NULL DEFAULT('noVerificado'),
        FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail)
    );

    CREATE TABLE Gestiona(
        identificadorPartido INT AUTO_INCREMENT,
        mailAdministrador VARCHAR(200),
        FOREIGN KEY (identificadorPartido) REFERENCES Partido(identificador),
        FOREIGN KEY (mailAdministrador) REFERENCES Administrador(mailPerfil),
        PRIMARY KEY (identificadorPartido, mailAdministrador)
    );

    CREATE TABLE Venta(
        identificador INT PRIMARY KEY AUTO_INCREMENT,
        fecha DATE NOT NULL DEFAULT(CURRENT_DATE),
        porcentajeComision INT NOT NULL,
        montoTotal INT NOT NULL,
        mailUsuarioComprado VARCHAR(200) NOT NULL,
        FOREIGN KEY (mailUsuarioComprado) REFERENCES Usuario(mailPerfil)
    );

    CREATE TABLE Entrada(
        identificador INT AUTO_INCREMENT PRIMARY KEY,
        identificadorVenta INT NOT NULL,
        identificadorPartido INT NOT NULL,
        mailUsuarioTiene VARCHAR(200) NOT NULL,
        estadoEntrada ENUM('Registrada', 'No registrada', 'Cancelada') DEFAULT('No registrada') NOT NULL,
        identificadorSector INT NOT NULL,
        identificadorEstadio INT NOT NULL,
        identificadorDispositivo INT DEFAULT NULL,
        codigoQRAceptado VARCHAR(200) DEFAULT NULL,
        fechaHoraIngreso DATETIME DEFAULT NULL,
        FOREIGN KEY (mailUsuarioTiene) REFERENCES Usuario(mailPerfil),
        FOREIGN KEY (identificadorEstadio, identificadorPartido, identificadorSector) REFERENCES Habilita(identificadorEstadio, identificadorPartido, identificadorSector),
        FOREIGN KEY (identificadorDispositivo) REFERENCES Dispositivo(identificador),
        FOREIGN KEY (identificadorVenta) REFERENCES Venta(identificador)
    );

    CREATE TABLE Transferencia(
        identificador INT PRIMARY KEY AUTO_INCREMENT,
        identificadorEntrada INT NOT NULL,
        mailUsuarioRealiza VARCHAR(200) NOT NULL,
        mailUsuarioRecibe VARCHAR(200) NOT NULL,
        fechaHora DATETIME NOT NULL DEFAULT(CURRENT_TIMESTAMP),
        FOREIGN KEY(identificadorEntrada) REFERENCES Entrada(identificador),
        FOREIGN KEY (mailUsuarioRealiza) REFERENCES Usuario(mailPerfil),
        FOREIGN KEY (mailUsuarioRecibe) REFERENCES Usuario(mailPerfil)
    );

    CREATE TABLE Grupo (
        nombreGrupo VARCHAR(25),
        nombreEtapa VARCHAR(25),
        PRIMARY KEY (nombreGrupo, nombreEtapa)
    );

    CREATE TABLE Pertenece(
        nombreEquipo VARCHAR(50),
        nombreGrupo VARCHAR(25),
        nombreEtapa VARCHAR(25),
        FOREIGN KEY (nombreEquipo) REFERENCES Equipo(nombre),
        FOREIGN KEY (nombreGrupo, nombreEtapa) REFERENCES Grupo(nombreGrupo, nombreEtapa),
        PRIMARY KEY (nombreEquipo, nombreGrupo, nombreEtapa)
    );

    CREATE TABLE VerificacionMail(
        mailPerfil VARCHAR(200) PRIMARY KEY,
        token VARCHAR(200) NOT NULL UNIQUE,
        fechaVencimiento DATETIME NOT NULL,
        usado BOOLEAN NOT NULL DEFAULT FALSE,
        FOREIGN KEY (mailPerfil) REFERENCES Usuario(mailPerfil)
    );

    CREATE TABLE qr(
        identificadorEntrada INT PRIMARY KEY,
        token VARCHAR(200) NOT NULL UNIQUE,
        fechaVencimiento DATETIME NOT NULL,
        identificadorDispositivo INT,
        FOREIGN KEY (identificadorEntrada) REFERENCES Entrada(identificador),
        FOREIGN KEY (identificadorDispositivo) REFERENCES Dispositivo(identificador)
    );

    INSERT INTO Perfil VALUES
    ('admin1@mundial.com','uruguay','ci','12345678','Uruguay','Montevideo','18 de Julio',1000,11000),
    ('admin2@mundial.com','uruguay','ci','55531973','Uruguay','Montevideo','18 de Julio',1000,11000),
    ('admin3@mundial.com','uruguay','ci','25081560','Uruguay','Montevideo','18 de Julio',1000,11000),
    ('func1@mundial.com','uruguay','ci','23456789','Uruguay','Montevideo','Rivera',2000,11000),
    ('func2@mundial.com','argentina','dni','30123456','Argentina','Buenos Aires','Corrientes',1500,1000),
    ('func3@mundial.com','argentina','dni','30123412','Argentina','Buenos Aires','Corrientes',1500,1000),
    ('user1@gmail.com','uruguay','ci','34567890','Uruguay','Canelones','Artigas',500,90000),
    ('user2@gmail.com','brasil','cpf','12345678901','Brasil','Porto Alegre','Central',100,9000),
    ('user3@gmail.com','argentina','dni','40123456','Argentina','Córdoba','San Martín',800,5000);

    INSERT INTO Login VALUES
    ('admin1@mundial.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('admin2@mundial.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('admin3@mundial.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('func1@mundial.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('func2@mundial.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('func3@mundial.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('user1@gmail.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('user2@gmail.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm'),
    ('user3@gmail.com','$2a$11$Ni4TFIkVeLnPOH/wgWL2fetrRfMt2GXEf4Qy0L/ppOiNSq7FsTiSm');

    INSERT INTO Administrador VALUES
    ('admin1@mundial.com','2026-01-01','mexico'),
    ('admin2@mundial.com','2026-01-01','estados unidos'),
    ('admin3@mundial.com','2026-01-01','canada');

    INSERT INTO Funcionario VALUES
    ('func1@mundial.com',1001),
    ('func2@mundial.com',1002),
    ('func3@mundial.com',1003);

    INSERT INTO Dispositivo (identificador, mailFuncionario) VALUES
    (1, 'func1@mundial.com'),
    (2, 'func2@mundial.com');

    INSERT INTO Usuario VALUES
    ('user1@gmail.com','2026-06-01','verificado'),
    ('user2@gmail.com','2026-06-02','verificado'),
    ('user3@gmail.com','2026-06-03','noVerificado');

    INSERT INTO Equipo VALUES
    ('mexico','https://upload.wikimedia.org/wikipedia/commons/f/fc/Flag_of_Mexico.svg'),
    ('sudafrica','https://upload.wikimedia.org/wikipedia/commons/a/af/Flag_of_South_Africa.svg'),
    ('corea del sur','https://upload.wikimedia.org/wikipedia/commons/0/09/Flag_of_South_Korea.svg'),
    ('republica checa','https://upload.wikimedia.org/wikipedia/commons/c/cb/Flag_of_the_Czech_Republic.svg'),
    ('canada','https://upload.wikimedia.org/wikipedia/commons/c/cf/Flag_of_Canada.svg'),
    ('bosnia y herzegovina','https://upload.wikimedia.org/wikipedia/commons/b/bf/Flag_of_Bosnia_and_Herzegovina.svg'),
    ('qatar','https://upload.wikimedia.org/wikipedia/commons/6/65/Flag_of_Qatar.svg'),
    ('suiza','https://upload.wikimedia.org/wikipedia/commons/f/f3/Flag_of_Switzerland.svg'),
    ('brasil','https://upload.wikimedia.org/wikipedia/commons/0/05/Flag_of_Brazil.svg'),
    ('marruecos','https://upload.wikimedia.org/wikipedia/commons/2/2c/Flag_of_Morocco.svg'),
    ('haiti','https://upload.wikimedia.org/wikipedia/commons/5/56/Flag_of_Haiti.svg'),
    ('escocia','https://upload.wikimedia.org/wikipedia/commons/1/10/Flag_of_Scotland.svg'),
    ('estados unidos','https://upload.wikimedia.org/wikipedia/en/a/a4/Flag_of_the_United_States.svg'),
    ('paraguay','https://upload.wikimedia.org/wikipedia/commons/2/27/Flag_of_Paraguay.svg'),
    ('australia','https://upload.wikimedia.org/wikipedia/commons/b/b9/Flag_of_Australia.svg'),
    ('turquia','https://upload.wikimedia.org/wikipedia/commons/b/b4/Flag_of_Turkey.svg'),
    ('alemania','https://upload.wikimedia.org/wikipedia/en/b/ba/Flag_of_Germany.svg'),
    ('curazao','https://upload.wikimedia.org/wikipedia/commons/b/b1/Flag_of_Cura%C3%A7ao.svg'),
    ('costa de marfil','https://upload.wikimedia.org/wikipedia/commons/f/fe/Flag_of_C%C3%B4te_d%27Ivoire.svg'),
    ('ecuador','https://upload.wikimedia.org/wikipedia/commons/e/e8/Flag_of_Ecuador.svg'),
    ('paises bajos','https://upload.wikimedia.org/wikipedia/commons/2/20/Flag_of_the_Netherlands.svg'),
    ('japon','https://upload.wikimedia.org/wikipedia/en/9/9e/Flag_of_Japan.svg'),
    ('suecia','https://upload.wikimedia.org/wikipedia/en/4/4c/Flag_of_Sweden.svg'),
    ('tunez','https://upload.wikimedia.org/wikipedia/commons/c/ce/Flag_of_Tunisia.svg'),
    ('belgica','https://upload.wikimedia.org/wikipedia/commons/6/65/Flag_of_Belgium.svg'),
    ('egipto','https://upload.wikimedia.org/wikipedia/commons/f/fe/Flag_of_Egypt.svg'),
    ('iran','https://upload.wikimedia.org/wikipedia/commons/c/ca/Flag_of_Iran.svg'),
    ('nueva zelanda','https://upload.wikimedia.org/wikipedia/commons/3/3e/Flag_of_New_Zealand.svg'),
    ('españa','https://upload.wikimedia.org/wikipedia/en/9/9a/Flag_of_Spain.svg'),
    ('cabo verde','https://upload.wikimedia.org/wikipedia/commons/3/38/Flag_of_Cape_Verde.svg'),
    ('arabia saudita','https://upload.wikimedia.org/wikipedia/commons/0/0d/Flag_of_Saudi_Arabia.svg'),
    ('uruguay','https://upload.wikimedia.org/wikipedia/commons/f/fe/Flag_of_Uruguay.svg'),
    ('francia','https://upload.wikimedia.org/wikipedia/en/c/c3/Flag_of_France.svg'),
    ('senegal','https://upload.wikimedia.org/wikipedia/commons/f/fd/Flag_of_Senegal.svg'),
    ('iraq','https://upload.wikimedia.org/wikipedia/commons/f/f6/Flag_of_Iraq.svg'),
    ('noruega','https://upload.wikimedia.org/wikipedia/commons/d/d9/Flag_of_Norway.svg'),
    ('argentina','https://upload.wikimedia.org/wikipedia/commons/1/1a/Flag_of_Argentina.svg'),
    ('algeria','https://upload.wikimedia.org/wikipedia/commons/7/77/Flag_of_Algeria.svg'),
    ('austria','https://upload.wikimedia.org/wikipedia/commons/4/41/Flag_of_Austria.svg'),
    ('jordania','https://upload.wikimedia.org/wikipedia/commons/c/c0/Flag_of_Jordan.svg'),
    ('portugal','https://upload.wikimedia.org/wikipedia/commons/5/5c/Flag_of_Portugal.svg'),
    ('republica democratica del congo','https://upload.wikimedia.org/wikipedia/commons/6/6f/Flag_of_the_Democratic_Republic_of_the_Congo.svg'),
    ('uzbekistan','https://upload.wikimedia.org/wikipedia/commons/8/84/Flag_of_Uzbekistan.svg'),
    ('colombia','https://upload.wikimedia.org/wikipedia/commons/2/21/Flag_of_Colombia.svg'),
    ('inglaterra','https://upload.wikimedia.org/wikipedia/en/b/be/Flag_of_England.svg'),
    ('croacia','https://upload.wikimedia.org/wikipedia/commons/1/1b/Flag_of_Croatia.svg'),
    ('ghana','https://upload.wikimedia.org/wikipedia/commons/1/19/Flag_of_Ghana.svg'),
    ('panama','https://upload.wikimedia.org/wikipedia/commons/a/ab/Flag_of_Panama.svg');

    INSERT INTO Grupo VALUES
    ('A','Grupos'),
    ('B','Grupos'),
    ('C','Grupos'),
    ('D','Grupos'),
    ('E','Grupos'),
    ('F','Grupos'),
    ('G','Grupos'),
    ('H','Grupos'),
    ('I','Grupos'),
    ('J','Grupos'),
    ('K','Grupos'),
    ('L','Grupos');

    INSERT INTO Pertenece VALUES
    ('mexico','A','Grupos'),
    ('sudafrica','A','Grupos'),
    ('corea del sur','A','Grupos'),
    ('republica checa','A','Grupos'),
    ('canada','B','Grupos'),
    ('bosnia y herzegovina','B','Grupos'),
    ('qatar','B','Grupos'),
    ('suiza','B','Grupos'),
    ('brasil','C','Grupos'),
    ('marruecos','C','Grupos'),
    ('haiti','C','Grupos'),
    ('escocia','C','Grupos'),
    ('estados unidos','D','Grupos'),
    ('paraguay','D','Grupos'),
    ('australia','D','Grupos'),
    ('turquia','D','Grupos'),
    ('alemania','E','Grupos'),
    ('curazao','E','Grupos'),
    ('costa de marfil','E','Grupos'),
    ('ecuador','E','Grupos'),
    ('paises bajos','F','Grupos'),
    ('japon','F','Grupos'),
    ('suecia','F','Grupos'),
    ('tunez','F','Grupos'),
    ('belgica','G','Grupos'),
    ('egipto','G','Grupos'),
    ('iran','G','Grupos'),
    ('nueva zelanda','G','Grupos'),
    ('españa','H','Grupos'),
    ('cabo verde','H','Grupos'),
    ('arabia saudita','H','Grupos'),
    ('uruguay','H','Grupos'),
    ('francia','I','Grupos'),
    ('senegal','I','Grupos'),
    ('iraq','I','Grupos'),
    ('noruega','I','Grupos'),
    ('argentina','J','Grupos'),
    ('algeria','J','Grupos'),
    ('austria','J','Grupos'),
    ('jordania','J','Grupos'),
    ('portugal','K','Grupos'),
    ('republica democratica del congo','K','Grupos'),
    ('uzbekistan','K','Grupos'),
    ('colombia','K','Grupos'),
    ('inglaterra','L','Grupos'),
    ('croacia','L','Grupos'),
    ('ghana','L','Grupos'),
    ('panama','L','Grupos');

    INSERT INTO Estadio (imagen,nombre,nombrePais,direccionLocalidad,direccionCalle,direccionNumero,direccionCodigoPostal) VALUES
    ('https://visitmexico.com/media/usercontent/68ed273a99de4-WhatsApp-Image-2025-10-10-at-11_gmxdot_22_gmxdot_26-AM_gmxdot_jpeg','Estadio Azteca','mexico','Ciudad de Mexico','Calzada de Tlalpan',3465,14370),
    ('https://upload.wikimedia.org/wikipedia/commons/thumb/f/f9/Estadio_BBVA.jpg/1920px-Estadio_BBVA.jpg','Estadio BBVA','mexico','Monterrey','Pablo Livas',2011,67140),
    ('https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/Estadio_Akron_02-07-2022_cabecera_sur_lado_derecho.jpg/1280px-Estadio_Akron_02-07-2022_cabecera_sur_lado_derecho.jpg','Estadio Akron','mexico','Circuito JVC',2800,45019,10000),
    ('https://upload.wikimedia.org/wikipedia/commons/thumb/b/b0/BC_Place_Stadium_-_panoramio.jpg/1280px-BC_Place_Stadium_-_panoramio.jpg','BC Place','canada','Vancouver','Pacific Blvd',777,20001),
    ('https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Bmo_Field_2016_East_Stand.jpg/1920px-Bmo_Field_2016_East_Stand.jpg','BMO Field','canada','Toronto','Princes Blvd',170,10001),
    ('https://upload.wikimedia.org/wikipedia/commons/thumb/1/10/Mercedes_Benz_Stadium_time_lapse_capture_2017-08-13.jpg/1920px-Mercedes_Benz_Stadium_time_lapse_capture_2017-08-13.jpg','Mercedes-Benz Stadium','estados unidos','Atlanta','Northside Dr NW',1,30313);

    INSERT INTO Partido (fase, EquipoLocal, EquipoVisitante, identificadorEstadio, precio, fechaHora) VALUES
    ('Grupos','mexico','sudafrica',1,1000,'2026-06-11 10:00:00'),
    ('Grupos','mexico','corea del sur',2,1000,'2026-06-11 13:00:00'),
    ('Grupos','mexico','republica checa',3,1000,'2026-06-11 16:00:00'),
    ('Grupos','sudafrica','corea del sur',4,1000,'2026-06-12 10:00:00'),
    ('Grupos','sudafrica','republica checa',5,1000,'2026-06-12 13:00:00'),
    ('Grupos','corea del sur','republica checa',6,1000,'2026-06-12 16:00:00'),
    ('Grupos','canada','bosnia y herzegovina',1,750,'2026-06-13 10:00:00'),
    ('Grupos','canada','qatar',2,750,'2026-06-13 13:00:00'),
    ('Grupos','canada','suiza',3,750,'2026-06-13 16:00:00'),
    ('Grupos','bosnia y herzegovina','qatar',4,750,'2026-06-14 10:00:00'),
    ('Grupos','bosnia y herzegovina','suiza',5,100,'2026-06-14 13:00:00'),
    ('Grupos','qatar','suiza',6,100,'2026-06-14 16:00:00'),
    ('Grupos','brasil','marruecos',1,100,'2026-06-15 10:00:00'),
    ('Grupos','brasil','haiti',2,100,'2026-06-15 13:00:00'),
    ('Grupos','brasil','escocia',3,100,'2026-06-15 16:00:00'),
    ('Grupos','marruecos','haiti',4,100,'2026-06-16 10:00:00'),
    ('Grupos','marruecos','escocia',5,100,'2026-06-16 13:00:00'),
    ('Grupos','haiti','escocia',6,100,'2026-06-16 16:00:00'),
    ('Grupos','estados unidos','paraguay',1,100,'2026-06-17 10:00:00'),
    ('Grupos','estados unidos','australia',2,100,'2026-06-17 13:00:00'),
    ('Grupos','estados unidos','turquia',3,100,'2026-06-17 16:00:00'),
    ('Grupos','paraguay','australia',4,100,'2026-06-18 10:00:00'),
    ('Grupos','paraguay','turquia',5,100,'2026-06-18 13:00:00'),
    ('Grupos','australia','turquia',6,100,'2026-06-18 16:00:00'),
    ('Grupos','alemania','curazao',1,100,'2026-06-19 10:00:00'),
    ('Grupos','alemania','costa de marfil',2,100,'2026-06-19 13:00:00'),
    ('Grupos','alemania','ecuador',3,100,'2026-06-19 16:00:00'),
    ('Grupos','curazao','costa de marfil',4,100,'2026-06-20 10:00:00'),
    ('Grupos','curazao','ecuador',5,100,'2026-06-20 13:00:00'),
    ('Grupos','costa de marfil','ecuador',6,100,'2026-06-20 16:00:00'),
    ('Grupos','paises bajos','japon',1,100,'2026-06-21 10:00:00'),
    ('Grupos','paises bajos','suecia',2,100,'2026-06-21 13:00:00'),
    ('Grupos','paises bajos','tunez',3,100,'2026-06-21 16:00:00'),
    ('Grupos','japon','suecia',4,100,'2026-06-22 10:00:00'),
    ('Grupos','japon','tunez',5,100,'2026-06-22 13:00:00'),
    ('Grupos','suecia','tunez',6,100,'2026-06-22 16:00:00'),
    ('Grupos','belgica','egipto',1,100,'2026-06-23 10:00:00'),
    ('Grupos','belgica','iran',2,100,'2026-06-23 13:00:00'),
    ('Grupos','belgica','nueva zelanda',3,100,'2026-06-23 16:00:00'),
    ('Grupos','egipto','iran',4,100,'2026-06-24 10:00:00'),
    ('Grupos','egipto','nueva zelanda',5,100,'2026-06-24 13:00:00'),
    ('Grupos','iran','nueva zelanda',6,100,'2026-06-24 16:00:00'),
    ('Grupos','españa','cabo verde',1,100,'2026-06-25 10:00:00'),
    ('Grupos','españa','arabia saudita',2,100,'2026-06-25 13:00:00'),
    ('Grupos','españa','uruguay',3,100,'2026-06-25 16:00:00'),
    ('Grupos','cabo verde','arabia saudita',4,100,'2026-06-26 10:00:00'),
    ('Grupos','cabo verde','uruguay',5,100,'2026-06-26 13:00:00'),
    ('Grupos','arabia saudita','uruguay',6,100,'2026-06-26 16:00:00'),
    ('Grupos','francia','senegal',1,100,'2026-06-27 10:00:00'),
    ('Grupos','francia','iraq',2,100,'2026-06-27 13:00:00'),
    ('Grupos','francia','noruega',3,100,'2026-06-27 16:00:00'),
    ('Grupos','senegal','iraq',4,100,'2026-06-28 10:00:00'),
    ('Grupos','senegal','noruega',5,100,'2026-06-28 13:00:00'),
    ('Grupos','iraq','noruega',6,100,'2026-06-28 16:00:00'),
    ('Grupos','argentina','algeria',1,100,'2026-06-29 10:00:00'),
    ('Grupos','argentina','austria',2,100,'2026-06-29 13:00:00'),
    ('Grupos','argentina','jordania',3,100,'2026-06-29 16:00:00'),
    ('Grupos','algeria','austria',4,100,'2026-06-30 10:00:00'),
    ('Grupos','algeria','jordania',5,100,'2026-06-30 13:00:00'),
    ('Grupos','austria','jordania',6,100,'2026-06-30 16:00:00'),
    ('Grupos','portugal','republica democratica del congo',1,100,'2026-07-01 10:00:00'),
    ('Grupos','portugal','uzbekistan',2,100,'2026-07-01 13:00:00'),
    ('Grupos','portugal','colombia',3,100,'2026-07-01 16:00:00'),
    ('Grupos','republica democratica del congo','uzbekistan',4,100,'2026-07-02 10:00:00'),
    ('Grupos','republica democratica del congo','colombia',5,100,'2026-07-02 13:00:00'),
    ('Grupos','uzbekistan','colombia',6,100,'2026-07-02 16:00:00'),
    ('Grupos','inglaterra','croacia',1,750,'2026-07-03 10:00:00'),
    ('Grupos','inglaterra','ghana',2,750,'2026-07-03 13:00:00'),
    ('Grupos','inglaterra','panama',3,1000,'2026-07-03 16:00:00'),
    ('Grupos','croacia','ghana',4,1000,'2026-07-04 10:00:00'),
    ('Grupos','croacia','panama',5,1000,'2026-07-04 13:00:00'),
    ('Grupos','ghana','panama',6,1000,'2026-07-04 16:00:00');

    INSERT INTO Sector (identificador, identificadorEstadio, nombre, capMax, tarifaExtra) VALUES
    (1, 1, 'Sector A', 20, 1000),
    (2, 1, 'Sector B', 30, 500),
    (3, 1, 'Sector C', 40, 250),
    (4, 1, 'Sector D', 50, 0),

    (1, 2, 'Sector A', 20, 1000),
    (2, 2, 'Sector B', 30, 500),
    (3, 2, 'Sector C', 40, 250),
    (4, 2, 'Sector D', 40, 0),

    (1, 3, 'Sector A', 20, 1000),
    (2, 3, 'Sector B', 30, 500),
    (3, 3, 'Sector C', 40, 250),
    (4, 3, 'Sector D', 50, 0),

    (1, 4, 'Sector A', 20, 1000),
    (2, 4, 'Sector B', 30, 500),
    (3, 4, 'Sector C', 40, 250),
    (4, 4, 'Sector D', 50, 0),

    (1, 5, 'Sector A', 20, 1000),
    (2, 5, 'Sector B', 30, 500),
    (3, 5, 'Sector C', 40, 250),
    (4, 5, 'Sector D', 50, 0),

    (1, 6, 'Sector A', 20, 1000),
    (2, 6, 'Sector B', 30, 500),
    (3, 6, 'Sector C', 40, 250),
    (4, 6, 'Sector D', 50, 0);

    INSERT INTO Habilita VALUES
    (1, 1, 1),
    (1, 2, 1),
    (5, 2, 2),
    (6, 3, 3),
    (2, 2, 2),
    (3,3, 3);

    INSERT INTO EsAsignado ( identificadorDispositivo, identificadorEstadio, identificadorPartido, identificadorSector) VALUES
    (1, 1, 1, 1),
    (2, 1, 1, 2),
    (1, 2, 5, 2),
    (2, 3, 6, 3);

    INSERT INTO Venta (porcentajeComision, montoTotal, mailUsuarioComprado) VALUES
    (5, 1100, 'user1@gmail.com'),
    (5, 1600, 'user1@gmail.com'),
    (5, 600,  'user2@gmail.com'),
    (5, 2100, 'user1@gmail.com'),
    (5, 1100, 'user3@gmail.com');

    INSERT INTO Entrada ( identificadorVenta, identificadorPartido, mailUsuarioTiene, estadoEntrada, identificadorSector, identificadorEstadio ) VALUES
    (1, 1, 'user1@gmail.com', 'Registrada', 1, 1),
    (2, 1, 'user1@gmail.com', 'No registrada', 2, 1),
    (2, 1, 'user1@gmail.com', 'No registrada', 2, 1),
    (3, 2, 'user2@gmail.com', 'Registrada', 2, 2),
    (4, 3, 'user1@gmail.com', 'Registrada', 3, 3),
    (4, 3, 'user1@gmail.com', 'No registrada', 3, 3),
    (4, 3, 'user1@gmail.com', 'No registrada', 3, 3),
    (5, 1, 'user3@gmail.com', 'Cancelada', 1, 1);
