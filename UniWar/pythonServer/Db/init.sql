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
    turn_completed INT NOT NULL DEFAULT 0, -- Completed turns within a round (1 or 2)
    user_owned_territories INT NOT NULL DEFAULT 0,
    user_owned_tanks INT NOT NULL DEFAULT 0,
    cpu_owned_territories INT NOT NULL DEFAULT 0,
    cpu_owned_tanks INT NOT NULL DEFAULT 0,
    user_owned_territories_list TEXT NOT NULL, -- User-owned territories serialized as JSON
    cpu_owned_territories_list TEXT NOT NULL,  -- CPU-owned territories serialized as JSON
    user_win INT NOT NULL DEFAULT -1, -- -1, 0, or 1
    
    user_tanks_lost INT NOT NULL DEFAULT 0,
    cpu_tanks_lost INT NOT NULL DEFAULT 0,
    user_tanks_lost_attacking INT NOT NULL DEFAULT 0,
    cpu_tanks_lost_attacking INT NOT NULL DEFAULT 0,
    user_tanks_lost_defending INT NOT NULL DEFAULT 0,
    cpu_tanks_lost_defending INT NOT NULL DEFAULT 0,

    user_perfect_defenses INT NOT NULL DEFAULT 0, -- Counters
    cpu_perfect_defenses INT NOT NULL DEFAULT 0, -- Counters

    user_territories_lost INT NOT NULL DEFAULT 0,
    cpu_territories_lost  INT NOT NULL DEFAULT 0,

    is_user_turn BOOLEAN NOT NULL DEFAULT FALSE,

    FOREIGN KEY (game_id) REFERENCES Games(id) ON DELETE CASCADE
);


