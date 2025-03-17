-- Create Employee table
CREATE TABLE IF NOT EXISTS Employee (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    username TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL
);

-- Create Show table
CREATE TABLE IF NOT EXISTS Show (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    artist TEXT NOT NULL,
    date DATETIME NOT NULL,
    location TEXT NOT NULL,
    availableSeats INTEGER NOT NULL,
    soldSeats INTEGER NOT NULL
);

-- Create Client table
CREATE TABLE IF NOT EXISTS Client (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL
);

-- Create Ticket table
CREATE TABLE IF NOT EXISTS Ticket (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    showId INTEGER NOT NULL,
    clientId INTEGER NOT NULL,
    numberOfSeats INTEGER NOT NULL,
    price INTEGER NOT NULL,
    FOREIGN KEY (showId) REFERENCES Show(id),
    FOREIGN KEY (clientId) REFERENCES Client(id)
); 