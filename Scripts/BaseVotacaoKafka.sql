CREATE DATABASE BaseVotacaoKafka
GO

USE BaseVotacaoKafka
GO

CREATE TABLE dbo.VotoTecnologia(
    Id INT IDENTITY(1,1) NOT NULL,
    IdVoto VARCHAR(50) NOT NULL,
    Horario DATETIME NOT NULL,
    Tecnologia VARCHAR(50) NOT NULL,
    Topico VARCHAR(120) NOT NULL,
    Particao INT NOT NULL,
    Producer VARCHAR(120) NOT NULL,
    Consumer VARCHAR(120) NOT NULL,
    ConsumerGroup VARCHAR(120) NOT NULL,    
    CONSTRAINT PK_VotoTecnologia PRIMARY KEY (Id)
)
GO