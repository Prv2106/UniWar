-- init.sql

CREATE TABLE IF NOT EXISTS Users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(255) UNIQUE NOT NULL,
    pwd VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS Games (
    id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(255) NOT NULL,
    date DATETIME NOT NULL,
    state INT DEFAULT -1, 
    FOREIGN KEY (Username) REFERENCES Users(Username) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Statistics (
    GameId INT NOT NULL,
    RoundId INT NOT NULL,

    -- Aggiungere le altre statistiche


    PRIMARY KEY (GameId, RoundId),  -- Chiave primaria composta
    FOREIGN KEY (GameId) REFERENCES Games(id) ON DELETE CASCADE
);


