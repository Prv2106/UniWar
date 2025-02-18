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

    user_owned_territories INT NOT NULL,
    user_owned_tanks INT NOT NULL,
    cpu_owned_territories INT NOT NULL,
    cpu_owned_tanks INT NOT NULL,
    user_owned_territories_list TEXT NOT NULL, -- Lista di territori posseduti dall'utente (serializzata, es. JSON)
    cpu_owned_territories_list TEXT NOT NULL,  -- Lista di territori posseduti dalla CPU (serializzata, es. JSON)
    user_win BOOLEAN NOT NULL,
    
    user_tanks_lost_per_turn FLOAT NOT NULL,
    cpu_tanks_lost_per_turn FLOAT NOT NULL,
    user_tanks_lost_attacking FLOAT NOT NULL,
    cpu_tanks_lost_attacking FLOAT NOT NULL,
    user_tanks_lost_defending FLOAT NOT NULL,
    cpu_tanks_lost_defending FLOAT NOT NULL,

    user_perfect_defenses INT NOT NULL,
    cpu_perfect_defenses INT NOT NULL,

    user_territories_lost_per_turn FLOAT NOT NULL,
    cpu_territories_lost_per_turn FLOAT NOT NULL,
    
    user_map_ownership_percentage FLOAT NOT NULL,
    cpu_map_ownership_percentage FLOAT NOT NULL,



    PRIMARY KEY (GameId, RoundId),  -- Chiave primaria composta
    FOREIGN KEY (GameId) REFERENCES Games(id) ON DELETE CASCADE
);


