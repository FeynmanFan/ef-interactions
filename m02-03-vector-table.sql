CREATE TABLE Documents (
    Id              INT PRIMARY KEY IDENTITY,
    Title           NVARCHAR(400) NOT NULL,
    Content         NVARCHAR(MAX) NOT NULL,
    Category        NVARCHAR(100) NULL,
    LastUpdated     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Embedding       VECTOR(1536) NOT NULL          -- Important: VECTOR type
);