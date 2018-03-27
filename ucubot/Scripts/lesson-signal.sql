use ucubot;
CREATE TABLE lesson_signal (
    id INT NOT NULL AUTO_INCREMENT,
    timestamp1 DATETIME DEFAULT CURRENT_TIMESTAMP,
    SignalType INT,
    UserId VARCHAR(100) NOT NULL ,
    PRIMARY KEY (id)
);
