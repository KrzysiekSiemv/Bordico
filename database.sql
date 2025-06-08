DROP DATABASE IF EXISTS app_bordico;
CREATE DATABASE app_bordico;
USE app_bordico;

CREATE TABLE users(
    id_user INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    username VARCHAR(24) NOT NULL,
    email_address VARCHAR(128) NOT NULL,
    nickname VARCHAR(80),
    description TEXT,
    password TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT NOW(),
    updated_at DATETIME NOT NULL DEFAULT NOW()
);

CREATE TABLE friends(
    id_first_user INT NOT NULL,
    id_second_user INT NOT NULL,
    first_nickname TEXT,
    second_nickname TEXT,
    created_at DATETIME NOT NULL DEFAULT NOW(),
    FOREIGN KEY (id_first_user) REFERENCES users(id_user),
    FOREIGN KEY (id_second_user) REFERENCES users(id_user)
);

CREATE TABLE conversations(
    id_conversation INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
    id_first_user INT NOT NULL,
    id_second_user INT NOT NULL,
    FOREIGN KEY (id_first_user) REFERENCES users(id_user),
    FOREIGN KEY (id_second_user) REFERENCES users(id_user)
);

CREATE TABLE groups(
    id_group INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    group_name VARCHAR(60),
    id_owner INT,
    requires_confirmation BOOLEAN NOT NULL DEFAULT False,
    FOREIGN KEY (id_owner) REFERENCES users(id_user)
);

CREATE TABLE messages(
    id_message INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
    id_conversation INT,
    id_group INT,
    id_sent_by INT,
    content TEXT,
    sent_at DATETIME NOT NULL DEFAULT NOW(),
    edited BOOLEAN NOT NULL DEFAULT False,
    delivered BOOLEAN NOT NULL DEFAULT False,
    read BOOLEAN NOT NULL DEFAULT False,
    FOREIGN KEY (id_conversation) REFERENCES conversations(id_conversation),
    FOREIGN KEY (id_group) REFERENCES groups(id_group),
    FOREIGN KEY (id_sent_by) REFERENCES users(id_user)
);

CREATE TABLE read_status(
    id_message INT,
    id_user INT,
    read BOOLEAN NOT NULL DEFAULT False,
    FOREIGN KEY (id_message) REFERENCES messages(id_message),
    FOREIGN KEY (id_user) REFERENCES users(id_user)
);

