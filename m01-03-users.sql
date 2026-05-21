CREATE TABLE Users (
    UserId              INT PRIMARY KEY IDENTITY(1,1),
    Username            NVARCHAR(100) NOT NULL UNIQUE,
    Email               NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash        NVARCHAR(255) NOT NULL,     -- Currently plaintext for demo
    FirstName           NVARCHAR(100) NULL,
    LastName            NVARCHAR(100) NULL,
    CreatedDate         DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastLoginDate       DATETIME2 NULL,
    IsActive            BIT NOT NULL DEFAULT 1
);

-- Insert some test data (plaintext passwords)
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName)
VALUES 
    ('john.doe', 'john@example.com', 'password123', 'John', 'Doe'),
    ('jane.smith', 'jane@example.com', 'MySecret456!', 'Jane', 'Smith'),
    ('bob.wilson', 'bob@example.com', 'LetMeIn789', 'Bob', 'Wilson');