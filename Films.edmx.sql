
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/30/2014 15:21:57
-- Generated from EDMX file: C:\Programming\VideoFileRenamer\Code\Desktop Client\VideoFileRenamer\Films.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Videos];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Genre_Film_Film]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Genre_Film] DROP CONSTRAINT [FK_Genre_Film_Film];
GO
IF OBJECT_ID(N'[dbo].[FK_Genre_Film_Genre]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Genre_Film] DROP CONSTRAINT [FK_Genre_Film_Genre];
GO
IF OBJECT_ID(N'[dbo].[FK_Film_Country_Film]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Film_Country] DROP CONSTRAINT [FK_Film_Country_Film];
GO
IF OBJECT_ID(N'[dbo].[FK_Film_Country_Country]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Film_Country] DROP CONSTRAINT [FK_Film_Country_Country];
GO
IF OBJECT_ID(N'[dbo].[FK_Film_File]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files] DROP CONSTRAINT [FK_Film_File];
GO
IF OBJECT_ID(N'[dbo].[FK_Films_Director]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Films] DROP CONSTRAINT [FK_Films_Director];
GO
IF OBJECT_ID(N'[dbo].[FK_FilmActor_Film]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FilmActor] DROP CONSTRAINT [FK_FilmActor_Film];
GO
IF OBJECT_ID(N'[dbo].[FK_FilmActor_Actor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FilmActor] DROP CONSTRAINT [FK_FilmActor_Actor];
GO
IF OBJECT_ID(N'[dbo].[FK_Films_Director1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Films] DROP CONSTRAINT [FK_Films_Director1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Films]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Films];
GO
IF OBJECT_ID(N'[dbo].[Genres]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Genres];
GO
IF OBJECT_ID(N'[dbo].[Countries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Countries];
GO
IF OBJECT_ID(N'[dbo].[Files]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Files];
GO
IF OBJECT_ID(N'[dbo].[Directors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Directors];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[Actors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Actors];
GO
IF OBJECT_ID(N'[dbo].[Genre_Film]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Genre_Film];
GO
IF OBJECT_ID(N'[dbo].[Film_Country]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Film_Country];
GO
IF OBJECT_ID(N'[dbo].[FilmActor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FilmActor];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Films'
CREATE TABLE [dbo].[Films] (
    [IdFilm] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [OriginalName] varchar(50)  NULL,
    [Year] int  NOT NULL,
    [Image] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Rate] int  NULL,
    [Link] nvarchar(max)  NOT NULL,
    [Director_id] int  NOT NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'Genres'
CREATE TABLE [dbo].[Genres] (
    [IdGenre] int IDENTITY(1,1) NOT NULL,
    [Genre1] varchar(50)  NOT NULL
);
GO

-- Creating table 'Countries'
CREATE TABLE [dbo].[Countries] (
    [IdCountry] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Files'
CREATE TABLE [dbo].[Files] (
    [IdFile] int IDENTITY(1,1) NOT NULL,
    [MD5] nvarchar(max)  NOT NULL,
    [FileName] nvarchar(500)  NOT NULL,
    [Size] bigint  NOT NULL,
    [Path] nvarchar(max)  NOT NULL,
    [Created] time  NOT NULL,
    [Modified] time  NOT NULL,
    [Film_IdFilm] int  NOT NULL
);
GO

-- Creating table 'Directors'
CREATE TABLE [dbo].[Directors] (
    [IdDirector] int IDENTITY(1,1) NOT NULL,
    [FistName] varchar(50)  NOT NULL,
    [SecondName] varchar(50)  NULL,
    [Link] nvarchar(max)  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'Actors'
CREATE TABLE [dbo].[Actors] (
    [IdActor] int IDENTITY(1,1) NOT NULL,
    [FistName] varchar(50)  NOT NULL,
    [SecondName] varchar(50)  NOT NULL
);
GO

-- Creating table 'Genre_Film'
CREATE TABLE [dbo].[Genre_Film] (
    [Films_IdFilm] int  NOT NULL,
    [Genres_IdGenre] int  NOT NULL
);
GO

-- Creating table 'Film_Country'
CREATE TABLE [dbo].[Film_Country] (
    [Film_IdFilm] int  NOT NULL,
    [Countries_IdCountry] int  NOT NULL
);
GO

-- Creating table 'FilmActor'
CREATE TABLE [dbo].[FilmActor] (
    [Film_IdFilm] int  NOT NULL,
    [Actors_IdActor] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [IdFilm] in table 'Films'
ALTER TABLE [dbo].[Films]
ADD CONSTRAINT [PK_Films]
    PRIMARY KEY CLUSTERED ([IdFilm] ASC);
GO

-- Creating primary key on [IdGenre] in table 'Genres'
ALTER TABLE [dbo].[Genres]
ADD CONSTRAINT [PK_Genres]
    PRIMARY KEY CLUSTERED ([IdGenre] ASC);
GO

-- Creating primary key on [IdCountry] in table 'Countries'
ALTER TABLE [dbo].[Countries]
ADD CONSTRAINT [PK_Countries]
    PRIMARY KEY CLUSTERED ([IdCountry] ASC);
GO

-- Creating primary key on [IdFile] in table 'Files'
ALTER TABLE [dbo].[Files]
ADD CONSTRAINT [PK_Files]
    PRIMARY KEY CLUSTERED ([IdFile] ASC);
GO

-- Creating primary key on [IdDirector] in table 'Directors'
ALTER TABLE [dbo].[Directors]
ADD CONSTRAINT [PK_Directors]
    PRIMARY KEY CLUSTERED ([IdDirector] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [IdActor] in table 'Actors'
ALTER TABLE [dbo].[Actors]
ADD CONSTRAINT [PK_Actors]
    PRIMARY KEY CLUSTERED ([IdActor] ASC);
GO

-- Creating primary key on [Films_IdFilm], [Genres_IdGenre] in table 'Genre_Film'
ALTER TABLE [dbo].[Genre_Film]
ADD CONSTRAINT [PK_Genre_Film]
    PRIMARY KEY CLUSTERED ([Films_IdFilm], [Genres_IdGenre] ASC);
GO

-- Creating primary key on [Film_IdFilm], [Countries_IdCountry] in table 'Film_Country'
ALTER TABLE [dbo].[Film_Country]
ADD CONSTRAINT [PK_Film_Country]
    PRIMARY KEY CLUSTERED ([Film_IdFilm], [Countries_IdCountry] ASC);
GO

-- Creating primary key on [Film_IdFilm], [Actors_IdActor] in table 'FilmActor'
ALTER TABLE [dbo].[FilmActor]
ADD CONSTRAINT [PK_FilmActor]
    PRIMARY KEY CLUSTERED ([Film_IdFilm], [Actors_IdActor] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Films_IdFilm] in table 'Genre_Film'
ALTER TABLE [dbo].[Genre_Film]
ADD CONSTRAINT [FK_Genre_Film_Film]
    FOREIGN KEY ([Films_IdFilm])
    REFERENCES [dbo].[Films]
        ([IdFilm])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Genres_IdGenre] in table 'Genre_Film'
ALTER TABLE [dbo].[Genre_Film]
ADD CONSTRAINT [FK_Genre_Film_Genre]
    FOREIGN KEY ([Genres_IdGenre])
    REFERENCES [dbo].[Genres]
        ([IdGenre])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Genre_Film_Genre'
CREATE INDEX [IX_FK_Genre_Film_Genre]
ON [dbo].[Genre_Film]
    ([Genres_IdGenre]);
GO

-- Creating foreign key on [Film_IdFilm] in table 'Film_Country'
ALTER TABLE [dbo].[Film_Country]
ADD CONSTRAINT [FK_Film_Country_Film]
    FOREIGN KEY ([Film_IdFilm])
    REFERENCES [dbo].[Films]
        ([IdFilm])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Countries_IdCountry] in table 'Film_Country'
ALTER TABLE [dbo].[Film_Country]
ADD CONSTRAINT [FK_Film_Country_Country]
    FOREIGN KEY ([Countries_IdCountry])
    REFERENCES [dbo].[Countries]
        ([IdCountry])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Film_Country_Country'
CREATE INDEX [IX_FK_Film_Country_Country]
ON [dbo].[Film_Country]
    ([Countries_IdCountry]);
GO

-- Creating foreign key on [Film_IdFilm] in table 'Files'
ALTER TABLE [dbo].[Files]
ADD CONSTRAINT [FK_Film_File]
    FOREIGN KEY ([Film_IdFilm])
    REFERENCES [dbo].[Films]
        ([IdFilm])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Film_File'
CREATE INDEX [IX_FK_Film_File]
ON [dbo].[Files]
    ([Film_IdFilm]);
GO

-- Creating foreign key on [Director_id] in table 'Films'
ALTER TABLE [dbo].[Films]
ADD CONSTRAINT [FK_Films_Director]
    FOREIGN KEY ([Director_id])
    REFERENCES [dbo].[Directors]
        ([IdDirector])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Films_Director'
CREATE INDEX [IX_FK_Films_Director]
ON [dbo].[Films]
    ([Director_id]);
GO

-- Creating foreign key on [Film_IdFilm] in table 'FilmActor'
ALTER TABLE [dbo].[FilmActor]
ADD CONSTRAINT [FK_FilmActor_Film]
    FOREIGN KEY ([Film_IdFilm])
    REFERENCES [dbo].[Films]
        ([IdFilm])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Actors_IdActor] in table 'FilmActor'
ALTER TABLE [dbo].[FilmActor]
ADD CONSTRAINT [FK_FilmActor_Actor]
    FOREIGN KEY ([Actors_IdActor])
    REFERENCES [dbo].[Actors]
        ([IdActor])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FilmActor_Actor'
CREATE INDEX [IX_FK_FilmActor_Actor]
ON [dbo].[FilmActor]
    ([Actors_IdActor]);
GO

-- Creating foreign key on [Director_id] in table 'Films'
ALTER TABLE [dbo].[Films]
ADD CONSTRAINT [FK_Films_Director1]
    FOREIGN KEY ([Director_id])
    REFERENCES [dbo].[Directors]
        ([IdDirector])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Films_Director1'
CREATE INDEX [IX_FK_Films_Director1]
ON [dbo].[Films]
    ([Director_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------