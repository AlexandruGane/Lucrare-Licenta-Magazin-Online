CREATE TABLE Parfumuri(
ID int not null primary key identity,
Title varchar(100) not null,
Cod_parfum varchar(20) not null,
Pret decimal (16,2) not null,
Descriere TEXT not null,
categorie varchar(100) not null,
imagine varchar(255) not null,
created_at Datetime not null  default current_timestamp
);

INSERT INTO Parfumuri(Title,Cod_parfum,Pret,Descriere,categorie,imagine) VALUES
('Bleu de chanel','001',105,'Pentru toți gentlemanii cu spiritul deschis','Apa de toaleta','res_02f8e872a9524db4987334c1ab2729c8');