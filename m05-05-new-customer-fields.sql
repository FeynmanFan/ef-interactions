-- Add Balance and RowVersion to Customers table
ALTER TABLE Customers
ADD 
    Balance     DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    RowVersion  ROWVERSION    NOT NULL;     -- Note: NO DEFAULT