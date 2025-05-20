             CREATE TABLE IF NOT EXISTS Actors
        (
            id INT AUTO_INCREMENT PRIMARY KEY,
            firstname NVARCHAR(64) NOT NULL,
            lastname NVARCHAR(64) NOT NULL,
            bio NVARCHAR(4096),
            rating FLOAT
        ) ;

            CREATE TABLE IF NOT EXISTS Movies
        (
            id INT AUTO_INCREMENT PRIMARY KEY,
            title NVARCHAR(256) NOT NULL,
            year int NOT NULL,
            description NVARCHAR(4096),
            rating FLOAT
        );

            CREATE TABLE IF NOT EXISTS ActorsMovies
       (
        id INT AUTO_INCREMENT PRIMARY KEY,
        actorId INT NOT NULL,
        movieId INT NOT NULL,
        rolename NVARCHAR(64),
        FOREIGN KEY(actorId) REFERENCES Actors(id) ON DELETE CASCADE,
        FOREIGN KEY(movieId) REFERENCES Movies(id) ON DELETE CASCADE
       );

       CREATE TABLE IF NOT EXISTS Users
       (
        id int AUTO_INCREMENT PRIMARY KEY,
        username NVARCHAR(64) NOT NULL UNIQUE,
        password NVARCHAR(64) NOT NULL,
        salt NVARCHAR(64) NOT NULL,
        role ENUM('Admin', 'User') NOT NULL
       );