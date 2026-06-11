SELECT COUNT(*) As Rows, 'Products' as 'Table' FROM Products
UNION
SELECT COUNT(*), 'Categories' FROM ProductCategories
UNION
SELECT COUNT(*), 'Orders' FROM Orders
UNION
SELECT COUNT(*), 'Items'  FROM OrderItems
