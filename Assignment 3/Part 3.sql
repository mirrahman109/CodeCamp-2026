

UPDATE Products
    SET Price = price + 50
WHERE ProductName = 'iphone 17'


UPDATE Products
    SET StockQuantity = StockQuantity - 5
WHERE ProductName = 'Sony Headphones'


DELETE FROM Orders
WHERE OrderID = 3