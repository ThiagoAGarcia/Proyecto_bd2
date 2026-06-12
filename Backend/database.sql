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
    mailPerfil VARCHAR(200) PRIMARY KEY,
    telefono VARCHAR(16) NOT NULL,
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
    nombre VARCHAR(20) PRIMARY KEY
);

CREATE TABLE Estadio (
    identificador INT PRIMARY KEY AUTO_INCREMENT,
    imagen VARCHAR(256),
    nombre VARCHAR(32) NOT NULL,
    nombrePais ENUM('estados unidos', 'canada', 'mexico') NOT NULL,
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

CREATE TABLE Partido (
    identificador INT AUTO_INCREMENT PRIMARY KEY,
    fase VARCHAR(30) NOT NULL,
    EquipoLocal VARCHAR(32) NOT NULL,
    EquipoVisitante VARCHAR(32) NOT NULL,
    identificadorEstadio INT NOT NULL,
    fechaHora DATETIME NOT NULL,
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
    fecha DATE NOT NULL,
    FOREIGN KEY (mailFuncionario) REFERENCES Funcionario(mailPerfil),
    FOREIGN KEY (identificadorDispositivo) REFERENCES Dispositivo(identificador),
    PRIMARY KEY (mailFuncionario, identificadorDispositivo)
);

CREATE TABLE Administrador(
    mailPerfil VARCHAR(200) PRIMARY KEY,
    fechaAsignacionCargo DATE NOT NULL,
    nombrePais VARCHAR(20) NOT NULL,
    FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail),
    FOREIGN KEY (nombrePais) REFERENCES Pais(nombre)
);

CREATE TABLE EsAsignado(
    mailFuncionario VARCHAR(200),
    identificadorSector INT,
    identificadorEstadio INT,
    fecha DATE NOT NULL,
    FOREIGN KEY (mailFuncionario) REFERENCES Funcionario(mailPerfil),
    FOREIGN KEY (identificadorEstadio, identificadorSector) REFERENCES Sector(identificadorEstadio, identificador),
    PRIMARY KEY (mailFuncionario, identificadorSector, identificadorEstadio)
);

CREATE TABLE Usuario(
    mailPerfil VARCHAR(200) PRIMARY KEY ,
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
    fecha DATE NOT NULL,
    porcentakeComision INT NOT NULL,
    montoTotal INT NOT NULL,
    mailUsuarioComprado VARCHAR(200) NOT NULL,
    FOREIGN KEY (mailUsuarioComprado) REFERENCES Usuario(mailPerfil)
);

CREATE TABLE Entrada(
    identificador INT AUTO_INCREMENT PRIMARY KEY ,
    identificadorVenta INT,
    identificadorPartido INT,
    MailUsuarioTiene VARCHAR(200),
    estadoEntrada ENUM('Registrada', 'No registrada', 'Cancelada') NOT NULL,
    identificadorSector INT,
    identificadorEstadio INT,
    mailFuncionario VARCHAR(200),
    identificadorDispositivo INT,
    codigoQRAceptado VARCHAR(200),
    fechaHoraIngreso DATETIME NOT NULL,
    FOREIGN KEY (MailUsuarioTiene) REFERENCES Usuario(mailPerfil),
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
    FOREIGN KEY(identificadorEntrada) REFERENCES Entrada(identificador),
    FOREIGN KEY (mailUsuarioRealiza) REFERENCES Usuario(mailPerfil),
    FOREIGN KEY (mailUsuarioRecibe) REFERENCES Usuario(mailPerfil)
);

CREATE TABLE Grupo (
    identificador INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(200) UNIQUE
);

CREATE TABLE Pertenece(
    nombreEquipo VARCHAR(30),
    identificadorGrupo INT,
    FOREIGN KEY (nombreEquipo) REFERENCES Equipo(nombre),
    FOREIGN KEY (identificadorGrupo) REFERENCES Grupo(identificador),
    PRIMARY KEY (nombreEquipo, identificadorGrupo)
);

CREATE TABLE Etapa(
    identificador INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50) NOT NULL,
    identificadorGrupo INT NOT NULL,
    FOREIGN KEY (identificadorGrupo) REFERENCES Grupo(identificador)
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
('uruguay'),
('argentina'),
('brasil');

INSERT INTO Administrador VALUES
('admin@mundial.com','2026-01-01','uruguay');

INSERT INTO Funcionario VALUES
('func1@mundial.com',1001),
('func2@mundial.com',1002);


INSERT INTO Usuario VALUES
('user1@gmail.com','2026-06-01','verificado'),
('user2@gmail.com','2026-06-02','verificado'),
('user3@gmail.com','2026-06-03','noVerificado');

