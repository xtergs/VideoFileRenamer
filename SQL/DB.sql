USE Videos;

Drop table film.film;
Drop table film.director;
Drop table film.Genre;

Create table film.Director
(
	IdDirector Integer not null Identity,
	FistName varchar(50) not null,
	SecondName varchar(50) not null,
	Constraint PK_Director Primary key (IdDirector)
)

Create table film.Genre
(
	IdGenre integer not null identity,
	Genre varchar(50) not null,
	Constraint PK_Genre Primary key (IdGenre)
);



Create table film.Film
(
 IdFilm Integer not null Identity,
 Name varchar(50) not null,
 OriginalName varchar(50),
 Year integer not null,
 Director_id integer not null,
 Genre_id integer not null,
 Constraint PK_Films Primary key(idFilm),
 Constraint FK_Films_Director Foreign key (Director_id)
  references film.Director(idDirector)
);

Create Table film.Genry_Film
(
	IdGenre integer,
	IdFilm integer,
	constraint FK_Genre Foreign key(idGenre)
		references film.genre(idGenre),
	constraint FK_Film Foreign key (idfilm)
		references film.Film(Genre_id)
);