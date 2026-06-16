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
    nombre VARCHAR(20) PRIMARY KEY,
    bandera VARCHAR(256)
);

CREATE TABLE Pais(
    nombre ENUM('estados unidos', 'canada', 'mexico') PRIMARY KEY
);

CREATE TABLE Estadio (
    identificador INT PRIMARY KEY AUTO_INCREMENT,
    imagen VARCHAR(256),
    nombre VARCHAR(32) NOT NULL,
    nombrePais ENUM('estados unidos', 'canada', 'mexico') NOT NULL, # cambio para que lo estadios solo puedan pertenecer a los paises anfitriones del mundial
    direccionLocalidad VARCHAR(200) NOT NULL,
    direccionCalle VARCHAR(20) NOT NULL,
    direccionNumero INT NOT NULL,
    direccionCodigoPostal INT NOT NULL,
    FOREIGN KEY (nombrePais) REFERENCES Pais(nombre)
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

CREATE TABLE Partido (  # que hacer con fase aca
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

CREATE TABLE Dispositivo(
    identificador INT PRIMARY KEY
);

CREATE TABLE Funcionario
(
    mailPerfil VARCHAR(200) PRIMARY KEY,
    numeroLegajo INT UNIQUE NOT NULL,
    FOREIGN KEY (mailPerfil) REFERENCES Perfil (mail)
);

CREATE TABLE DispositivoFuncionario(
    mailFuncionario VARCHAR(200),
    identificadorDispositivo INT,
    fecha DATE NOT NULL DEFAULT(CURRENT_DATE),
    FOREIGN KEY (mailFuncionario) REFERENCES Funcionario(mailPerfil),
    FOREIGN KEY (identificadorDispositivo) REFERENCES Dispositivo(identificador),
    PRIMARY KEY (mailFuncionario, identificadorDispositivo)
);

CREATE TABLE Administrador(
    mailPerfil VARCHAR(200) PRIMARY KEY,
    fechaAsignacionCargo DATE NOT NULL DEFAULT(CURRENT_DATE),
    nombrePais ENUM('estados unidos', 'canada', 'mexico') NOT NULL,
    FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail),
    FOREIGN KEY (nombrePais) REFERENCES Pais(nombre)
);

CREATE TABLE EsAsignado(
    mailFuncionario VARCHAR(200),
    identificadorSector INT,
    identificadorEstadio INT,
    fecha DATE NOT NULL DEFAULT(CURRENT_DATE),
    FOREIGN KEY (mailFuncionario) REFERENCES Funcionario(mailPerfil),
    FOREIGN KEY (identificadorEstadio, identificadorSector) REFERENCES Sector(identificadorEstadio, identificador),
    PRIMARY KEY (mailFuncionario, identificadorSector, identificadorEstadio)
);

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
    mailFuncionario VARCHAR(200) DEFAULT NULL,
    identificadorDispositivo INT DEFAULT NULL,
    codigoQRAceptado VARCHAR(200) DEFAULT NULL,
    fechaHoraIngreso DATETIME DEFAULT NULL,
    FOREIGN KEY (mailUsuarioTiene) REFERENCES Usuario(mailPerfil),
    FOREIGN KEY (identificadorPartido) REFERENCES Partido(identificador),
    FOREIGN KEY (identificadorEstadio, identificadorSector) REFERENCES Sector(identificadorEstadio, identificador),
    FOREIGN KEY (mailFuncionario) REFERENCES  Funcionario(mailPerfil),
    FOREIGN KEY (identificadorDispositivo) REFERENCES  Dispositivo(identificador)
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
    nombreEquipo VARCHAR(30),
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

SELECT * FROM login;

INSERT INTO Perfil VALUES
('admin@mundial.com','uruguay','ci','12345678','Uruguay','Montevideo','18 de Julio',1000,11000),
('func1@mundial.com','uruguay','ci','23456789','Uruguay','Montevideo','Rivera',2000,11000),
('func2@mundial.com','argentina','dni','30123456','Argentina','Buenos Aires','Corrientes',1500,1000),
('user1@gmail.com','uruguay','ci','34567890','Uruguay','Canelones','Artigas',500,90000),
('user2@gmail.com','brasil','cpf','12345678901','Brasil','Porto Alegre','Central',100,9000),
('user3@gmail.com','argentina','dni','40123456','Argentina','Córdoba','San Martín',800,5000);

INSERT INTO Login VALUES
('admin@mundial.com','$2a$11$Iwsbt6qrxj4auhu9ZyAWTO99qdq2jCNdeC1w.EjNOwv0MocNkJH06'),
('func1@mundial.com','$2a$11$Iwsbt6qrxj4auhu9ZyAWTO99qdq2jCNdeC1w.EjNOwv0MocNkJH06'),
('func2@mundial.com','$2a$11$Iwsbt6qrxj4auhu9ZyAWTO99qdq2jCNdeC1w.EjNOwv0MocNkJH06'),
('user1@gmail.com','$2a$11$Iwsbt6qrxj4auhu9ZyAWTO99qdq2jCNdeC1w.EjNOwv0MocNkJH06'),
('user2@gmail.com','$2a$11$Iwsbt6qrxj4auhu9ZyAWTO99qdq2jCNdeC1w.EjNOwv0MocNkJH06'),
('user3@gmail.com','$2a$11$Iwsbt6qrxj4auhu9ZyAWTO99qdq2jCNdeC1w.EjNOwv0MocNkJH06');

INSERT INTO Pais VALUES
('canada'),
('estados unidos'),
('mexico');

INSERT INTO Administrador VALUES
('admin@mundial.com','2026-01-01','canada');

INSERT INTO Funcionario VALUES
('func1@mundial.com',1001),
('func2@mundial.com',1002);

INSERT INTO Usuario VALUES
('user1@gmail.com','2026-06-01','verificado'),
('user2@gmail.com','2026-06-02','verificado'),
('user3@gmail.com','2026-06-03','noVerificado');

INSERT INTO Equipo VALUES
('argentina', 'https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/Flag_of_Argentina.svg/960px-Flag_of_Argentina.svg.png'),
('brasil', 'https://upload.wikimedia.org/wikipedia/commons/0/05/Flag_of_Brazil.svg'),
('uruguay', 'https://upload.wikimedia.org/wikipedia/commons/f/fe/Flag_of_Uruguay.svg'),
('chile', 'https://upload.wikimedia.org/wikipedia/commons/thumb/7/78/Flag_of_Chile.svg/960px-Flag_of_Chile.svg.png'),
('paraguay', 'https://upload.wikimedia.org/wikipedia/commons/thumb/2/27/Flag_of_Paraguay.svg/960px-Flag_of_Paraguay.svg.png'),
('peru', 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/cf/Flag_of_Peru.svg/960px-Flag_of_Peru.svg.png'),
('colombia', 'https://upload.wikimedia.org/wikipedia/commons/thumb/2/21/Flag_of_Colombia.svg/960px-Flag_of_Colombia.svg.png'),
('mexico', 'https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Flag_of_Mexico.svg/960px-Flag_of_Mexico.svg.png'),
('españa', 'https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Flag_of_Spain.svg/960px-Flag_of_Spain.svg.png'),
('estados unidos', 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/a4/Flag_of_the_United_States.svg/960px-Flag_of_the_United_States.svg.png'),
('canada', 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/cf/Flag_of_Canada.svg/960px-Flag_of_Canada.svg.png');

INSERT INTO Estadio (imagen,nombre,nombrePais,direccionLocalidad,direccionCalle,direccionNumero,direccionCodigoPostal) VALUES
('https://visitmexico.com/media/usercontent/68ed273a99de4-WhatsApp-Image-2025-10-10-at-11_gmxdot_22_gmxdot_26-AM_gmxdot_jpeg','Estadio Azteca','mexico','Ciudad de Mexico','Calzada de Tlalpan',3465,14370),
('https://upload.wikimedia.org/wikipedia/commons/thumb/f/f9/Estadio_BBVA.jpg/1920px-Estadio_BBVA.jpg','Estadio BBVA','mexico','Monterrey','Pablo Livas',2011,67140),
('https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/Estadio_Akron_02-07-2022_cabecera_sur_lado_derecho.jpg/1280px-Estadio_Akron_02-07-2022_cabecera_sur_lado_derecho.jpg','Estadio Akron','mexico','Circuito JVC',2800,45019,10000),
('https://upload.wikimedia.org/wikipedia/commons/thumb/b/b0/BC_Place_Stadium_-_panoramio.jpg/1280px-BC_Place_Stadium_-_panoramio.jpg','BC Place','canada','Vancouver','Pacific Blvd',777,20001),
('https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Bmo_Field_2016_East_Stand.jpg/1920px-Bmo_Field_2016_East_Stand.jpg','BMO Field','canada','Toronto','Princes Blvd',170,10001),
('https://upload.wikimedia.org/wikipedia/commons/thumb/1/10/Mercedes_Benz_Stadium_time_lapse_capture_2017-08-13.jpg/1920px-Mercedes_Benz_Stadium_time_lapse_capture_2017-08-13.jpg','Mercedes-Benz Stadium','estados unidos','Atlanta','Northside Dr NW',1,30313);

INSERT INTO Partido (fase, EquipoLocal, EquipoVisitante, identificadorEstadio, precio, fechaHora) VALUES
('Grupos', 'argentina', 'canada', 1, 100, '2026-06-11 20:00:00'),
('Grupos', 'mexico', 'uruguay', 2, 100, '2026-06-12 18:00:00'),
('Grupos', 'brasil', 'estados unidos', 3, 100, '2026-06-13 21:00:00'),
('Grupos', 'chile', 'paraguay', 4, 100, '2026-06-14 17:00:00'),
('Grupos', 'colombia', 'peru', 5, 100, '2026-06-15 19:00:00'),
('Grupos', 'españa', 'mexico', 6, 100, '2026-06-16 20:30:00');

INSERT INTO Sector (identificador, identificadorEstadio, nombre, capMax, tarifaExtra) VALUES
(1, 1, 'Sector A', 3, 1000),
(2, 1, 'Sector B', 2, 500),
(3, 1, 'Sector C', 1, 0),

(1, 2, 'Sector A', 3, 1000),
(2, 2, 'Sector B', 2, 500),
(3, 2, 'Sector C', 1, 0),

(1, 3, 'Sector A', 3, 1000),
(2, 3, 'Sector B', 2, 500),
(3, 3, 'Sector C', 1, 0),

(1, 4, 'Sector A', 3, 1000),
(2, 4, 'Sector B', 2, 500),
(3, 4, 'Sector C', 1, 0),

(1, 5, 'Sector A', 3, 1000),
(2, 5, 'Sector B', 2, 500),
(3, 5, 'Sector C', 1, 0),

(1, 6, 'Sector A', 3, 1000),
(2, 6, 'Sector B', 2, 500),
(3, 6, 'Sector C', 1, 0);

INSERT INTO Habilita VALUES
(1, 1, 1),
(1, 2, 1),
(2, 2, 2),
(3, 3, 3);
