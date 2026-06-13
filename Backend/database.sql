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
    nombrePais ENUM('estados unidos', 'canada', 'mexico') NOT NULL,
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

INSERT INTO Estadio (imagen, nombre, nombrePais, direccionLocalidad, direccionCalle, direccionNumero, direccionCodigoPostal) VALUES
(NULL, 'MetLife Stadium', 'estados unidos', 'New Jersey', 'MetLife Dr', 1, 7032),
(NULL, 'SoFi Stadium', 'estados unidos', 'Los Angeles', 'Stadium Dr', 1001, 90301),
(NULL, 'AT&T Stadium', 'estados unidos', 'Arlington', 'Legends Way', 1, 76011),
(NULL, 'BMO Field', 'canada', 'Toronto', 'Princes Blvd', 170, 10001),
(NULL, 'BC Place', 'canada', 'Vancouver', 'Pacific Blvd', 777, 20001),
(NULL, 'Estadio Azteca', 'mexico', 'Ciudad de Mexico', 'Calzada Tlalpan', 3465, 14370),
(NULL, 'Estadio Akron', 'mexico', 'Guadalajara', 'Circuito JVC', 2800, 45019),
(NULL, 'BBVA Stadium', 'mexico', 'Monterrey', 'Pablo Livas', 2011, 67140);

INSERT INTO Partido (fase, EquipoLocal, EquipoVisitante, identificadorEstadio, fechaHora) VALUES
('Fase de Grupos', 'argentina', 'canada', 1, '2026-06-11 20:00:00'),
('Fase de Grupos', 'mexico', 'uruguay', 2, '2026-06-12 18:00:00'),
('Fase de Grupos', 'brasil', 'estados unidos', 3, '2026-06-13 21:00:00'),
('Fase de Grupos', 'chile', 'paraguay', 4, '2026-06-14 17:00:00'),
('Fase de Grupos', 'colombia', 'peru', 5, '2026-06-15 19:00:00'),
('Fase de Grupos', 'españa', 'mexico', 6, '2026-06-16 20:30:00'),

-- Octavos
('Octavos de Final', 'argentina', 'mexico', 1, '2026-06-28 20:00:00'),
('Octavos de Final', 'brasil', 'uruguay', 2, '2026-06-29 21:00:00'),
('Octavos de Final', 'españa', 'canada', 3, '2026-06-30 18:00:00'),
('Octavos de Final', 'colombia', 'estados unidos', 4, '2026-07-01 19:00:00'),

-- Cuartos
('Cuartos de Final', 'argentina', 'brasil', 5, '2026-07-04 21:00:00'),
('Cuartos de Final', 'españa', 'colombia', 6, '2026-07-05 21:00:00'),

-- Semifinales
('Semifinal', 'argentina', 'españa', 1, '2026-07-10 20:00:00'),
('Semifinal', 'brasil', 'colombia', 2, '2026-07-11 20:00:00'),

-- Tercer puesto
('Tercer Puesto', 'españa', 'colombia', 3, '2026-07-14 18:00:00'),

-- Final
('Final', 'argentina', 'brasil', 1, '2026-07-19 21:00:00');