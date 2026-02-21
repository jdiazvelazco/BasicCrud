CREATE DATABASE BdiExamen;
USE BdiExamen;

Create Table tblExamen
(
	idExamen int NOT NULL PRIMARY KEY,
	Nombre varchar(255) NULL,
	Descripcion varchar(255) NULL,
	CONSTRAINT PK_tblExamen PRIMARY KEY (IdExamen)
);

