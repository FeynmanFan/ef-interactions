-- =============================================
-- CONFIGURABLE SCALE SEEDING SCRIPT
-- =============================================

DECLARE @ScaleFactor INT = 20;   -- <<< CHANGE THIS (try 10 first, then 20, then 50-100)

-- =============================================
-- WIPE EXISTING DATA
-- =============================================
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Products;
DELETE FROM ProductCategories;

DBCC CHECKIDENT ('ProductCategories', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('OrderItems', RESEED, 0);

PRINT 'Existing data cleared'

-- =============================================
-- SEED DATA (Scaled)
-- =============================================

-- 1. Product Categories
INSERT INTO ProductCategories (Name)
SELECT 'Category ' + CAST(n.number AS VARCHAR) + ' - ' + c.BaseName
FROM (VALUES 
    ('Electronics'),('Clothing'),('Books'),('Home & Kitchen'),
    ('Sports'),('Beauty'),('Toys'),('Automotive'),('Garden'),
    ('Office'),('Pet'),('Health'),('Tools'),('Jewelry'),
    ('Baby'),('Grocery'),('Music'),('Movies'),('Games'),('Industrial')
) c(BaseName)
CROSS JOIN (SELECT TOP (@ScaleFactor) number FROM master..spt_values WHERE type = 'P') n;

PRINT 'Product categories created'

-- 2. Products (~500 * ScaleFactor)
INSERT INTO Products (Name, CategoryId, Price)
SELECT 
    'Product ' + CAST(ROW_NUMBER() OVER(ORDER BY c.Id, n.number) AS VARCHAR(20)) 
    + ' - ' + c.Name,
    c.Id,
    ROUND(CAST(CHECKSUM(NEWID()) AS float) / 2147483647 * 1200 + 9.99, 2)
FROM ProductCategories c
CROSS JOIN (SELECT TOP (25 * @ScaleFactor) number FROM master..spt_values WHERE type = 'P') n;

PRINT 'Products created'

-- 3. Orders (~ hundreds * ScaleFactor)
INSERT INTO Orders (CustomerId, CreatedAt, Total)
SELECT 
    c.Id,
    DATEADD(day, - (ABS(CHECKSUM(NEWID())) % 730), GETDATE()),
    0.00
FROM Customers c
CROSS JOIN (SELECT TOP (40 * @ScaleFactor) number FROM master..spt_values WHERE type = 'P') n
WHERE c.Id % 3 = 0;

PRINT 'Orders created'

-- 4. OrderItems (This is the heavy part — dense graph)
INSERT INTO OrderItems (OrderId, ProductId, Quantity)
SELECT 
    o.Id,
    p.Id,
    (ABS(CHECKSUM(NEWID())) % 8) + 1
FROM Orders o
CROSS JOIN Products p
WHERE o.Id % 3 = 0;        -- Adjust this divisor to control density (lower = more items)

PRINT 'Order items created'

-- 5. Update Order Totals safely
UPDATE o
SET o.Total = ISNULL((
    SELECT SUM(oi.Quantity * p.Price)
    FROM OrderItems oi
    JOIN Products p ON p.Id = oi.ProductId
    WHERE oi.OrderId = o.Id
), 0.00)
FROM Orders o;

PRINT 'Order totals updated'

-- =============================================
-- FINAL STATS
-- =============================================
PRINT '=== SEEDING COMPLETE (ScaleFactor = ' + CAST(@ScaleFactor AS VARCHAR) + ') ===';
SELECT 
    (SELECT COUNT(*) FROM ProductCategories) AS Categories,
    (SELECT COUNT(*) FROM Products) AS Products,
    (SELECT COUNT(*) FROM Orders) AS Orders,
    (SELECT COUNT(*) FROM OrderItems) AS OrderItems;