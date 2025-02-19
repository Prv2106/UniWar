-- init.sql

CREATE TABLE IF NOT EXISTS Users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    pwd VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS Games (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL,
    date DATETIME NOT NULL,
    state INT DEFAULT -1, 
    FOREIGN KEY (username) REFERENCES Users(username) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Data (
    game_id INT NOT NULL DEFAULT 1 PRIMARY KEY,
    round_id INT NOT NULL DEFAULT 1,
    turn_completed INT NOT NULL DEFAULT 0, -- Turni completati in un round (o 1 o 2)
    user_owned_territories INT NOT NULL DEFAULT 0,
    user_owned_tanks INT NOT NULL DEFAULT 0,
    cpu_owned_territories INT NOT NULL DEFAULT 0,
    cpu_owned_tanks INT NOT NULL DEFAULT 0,
    user_owned_territories_list TEXT NOT NULL, -- Lista di territori posseduti dall'utente (serializzata in JSON)
    cpu_owned_territories_list TEXT NOT NULL,  -- Lista di territori posseduti dalla CPU (serializzata in JSON)
    user_win INT NOT NULL DEFAULT -1, -- -1, 0 o 1
    
    user_tanks_lost INT NOT NULL DEFAULT 0,
    cpu_tanks_lost INT NOT NULL DEFAULT 0,
    user_tanks_lost_attacking INT NOT NULL DEFAULT 0,
    cpu_tanks_lost_attacking INT NOT NULL DEFAULT 0,
    user_tanks_lost_defending INT NOT NULL DEFAULT 0,
    cpu_tanks_lost_defending INT NOT NULL DEFAULT 0,

    user_perfect_defenses INT NOT NULL DEFAULT 0, -- contatori
    cpu_perfect_defenses INT NOT NULL DEFAULT 0, -- contatori

    user_territories_lost INT NOT NULL DEFAULT 0,
    cpu_territories_lost  INT NOT NULL DEFAULT 0,

    FOREIGN KEY (game_id) REFERENCES Games(id) ON DELETE CASCADE
);


