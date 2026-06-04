DROP DATABASE IF EXISTS proyectoBD2;

CREATE DATABASE proyectoBD2;

USE proyectoBD2;

CREATE TABLE Perfil(
    mail VARCHAR(200) PRIMARY KEY,
    paisDocumento VARCHAR(32) NOT NULL,
    tipoDocumento VARCHAR(32) NOT NULL,
    numeroDocumento INT NOT NULL UNIQUE,
    direccionLocalidad VARCHAR(32) NOT NULL,
    direccionNumero INT NOT NULL,
    direccionCodigoPostal INT NOT NULL
);

CREATE TABLE Telefono (
    mailPerfil VARCHAR(200) PRIMARY KEY,
    telefono INT NOT NULL,
    FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail)
);

CREATE TABLE Login (
    mailPerfil VARCHAR(200) PRIMARY KEY,
    password VARCHAR(256) NOT NULL,
    FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail)
);

CREATE TABLE Pais(
    nombre VARCHAR(20) PRIMARY KEY,
    continente VARCHAR(30) NOT NULL
);

CREATE TABLE Jurisdiccion(
    nombre VARCHAR(20) PRIMARY KEY,
    continente VARCHAR(30) NOT NULL
);


CREATE TABLE Estadio (
    identificador INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(32) NOT NULL UNIQUE, #NO SE SI VA EL UNIQUE CREO QUE SI
    nombreJurisdiccion VARCHAR(20) NOT NULL,
    direccionLocalidad VARCHAR(200) NOT NULL,
    direccionCalle VARCHAR(20) NOT NULL,
    direccionNumero INT NOT NULL,
    direccionCodigoPostal INT NOT NULL,
    FOREIGN KEY (nombreJurisdiccion) REFERENCES Jurisdiccion(nombre)
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
    paisLocal VARCHAR(32) NOT NULL,
    paisVisitante VARCHAR(32) NOT NULL,
    identificadorEstadio INT NOT NULL,
    fechaHora DATETIME NOT NULL,
    FOREIGN KEY (paisLocal) REFERENCES Pais(nombre),
    FOREIGN KEY (paisVisitante) REFERENCES Pais(nombre),
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
    FOREIGN KEY (mailPerfil) REFERENCES Perfil(mail)
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
    fechaRegistro DATE NOT NULL,
    estadoVerificado ENUM('verificado', 'No verificado') NOT NULL,
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
    nombre VARCHAR(200)
);

CREATE TABLE Pertenece(
    nombrePais VARCHAR(30),
    identificadorGrupo INT,
    FOREIGN KEY (nombrePais) REFERENCES Pais(nombre),
    FOREIGN KEY (identificadorGrupo) REFERENCES Grupo(identificador),
    PRIMARY KEY (nombrePais, identificadorGrupo)
);

CREATE TABLE Etapa(
    identificador INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50) NOT NULL,
    identificadorGrupo INT NOT NULL,
    FOREIGN KEY (identificadorGrupo) REFERENCES Grupo(identificador)
);


