create table Admin(
	id serial primary key,
	login varchar(255) not null,
	password varchar(255) not null
);

create table Client(
	id serial primary key,
	nom varchar(255),
	email varchar(255) not null
);

create table Proprietaire(
	id serial primary key,
	nom varchar(255),
	numeroTel varchar(15) not null
);

create table Region(
	id serial primary key,
	nom varchar(255) not null
);

create table Photo(
	id serial primary key,
	path text not null
);

create table TypeBien(
	id serial primary key,
	nom varchar(100) not null,
	commission double precision not null
);

CREATE TABLE Bien (
    id SERIAL PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    idtype INT NOT NULL,
    description TEXT NOT NULL,
    idregion INT NOT NULL,
    loyerMensuel DOUBLE PRECISION NOT NULL,
    idproprietaire INT NOT NULL,
    reference TEXT NOT NULL UNIQUE,
    FOREIGN KEY (idregion) REFERENCES Region(id),
    FOREIGN KEY (idtype) REFERENCES TypeBien(id),
    FOREIGN KEY (idproprietaire) REFERENCES Proprietaire(id)
);

create table PhotoBien(
	id serial primary key,
	idbien int not null,
	idphoto int not null,
	foreign key (idbien) references Bien(id),
	foreign key (idphoto) references Photo(id)
);

CREATE TABLE Location (
    id SERIAL PRIMARY KEY,
    referenceBien TEXT NOT NULL,
    idclient INT NOT NULL,
    idbien INT NOT NULL,
    duree INT NOT NULL,
    dateDebut timestamp NOT NULL,
    ispayed INT NOT NULL DEFAULT 0,
    FOREIGN KEY (idclient) REFERENCES Client(id),
    FOREIGN KEY (idbien) REFERENCES Bien(id),
    FOREIGN KEY (referenceBien) REFERENCES Bien(reference)
);

create table CsvCommission(
	id serial primary key,
	Type varchar(255) not null,
	Commission DOUBLE PRECISION NOT NULL
);

create table CsvBiens(
	id serial primary key,
	reference varchar(255) not null,
	nom varchar(255) not null,
	Description varchar(255) not null,
	Type varchar(255) not null,
	region varchar(255) not null,
	loyer_mensuel DOUBLE PRECISION NOT NULL,
	Proprietaire varchar(255) not null
);

create table CsvLocation(
	id serial primary key,
	reference varchar(255) not null,
	Date_debut timestamp not null,
	duree_mois int not null,
	client varchar(255) not null
);

insert into Admin(login,password) values ('admin@gmail.com','admin');

insert into Client(nom,email) values ('Client 1','client1@gmail.com');

insert into Proprietaire(nom,numeroTel) values ('Propriétaire 1','0340000000');

INSERT INTO Region (nom) VALUES
  ('Antananarivo'),
  ('Toamasina'),
  ('Fianarantsoa');

INSERT INTO TypeBien (nom, commission) VALUES
  ('Vila', 9),
  ('Chambre', 5),
  ('Hotel', 2);

INSERT INTO Bien (nom, idtype, description, idregion, loyerMensuel, idproprietaire, reference) VALUES
  ('Villa V1', 7, 'Modern apartment with stunning views', 7, 200000, 3, 'V1'),
  ('Villa V2', 7, 'Charming house with a backyard', 7, 170000, 3, 'V2'),
  ('Chambre C1', 8, 'Compact studio in a convenient location', 7, 35000, 3, 'C1'),
  ('Hotel H1', 9, 'Charming house with a backyard', 7, 80000, 3, 'H1'),
  ('Hotel H2', 9, 'Compact studio in a convenient location', 7, 73000, 3, 'H2');

INSERT INTO Location (idclient,idbien,duree,dateDebut,ispayed) VALUES
  (2,4,3,'2024-01-10',0),
  (2,5,1,'2024-02-01',0),
  (2,5,2,'2024-03-13',0),
  (2,6,5,'2024-02-14',0),
  (2,7,3,'2023-11-30',0);
