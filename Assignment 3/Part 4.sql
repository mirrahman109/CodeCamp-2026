<<<<<<< HEAD


SELECT CONCAT(FirstName, ' ', LastName) AS FullName, Products.ProductName, Orders.OrderDate FROM Orders
JOIN Customers ON Orders.CustomerID = Customers.CustomerID
JOIN Products ON Orders.ProductID = Products.ProductID


SELECT SUM(Products.Price * Orders.Quantity) AS TotalRevenue 
FROM Orders
JOIN Products ON Orders.ProductID = Products.ProductID

SELECT CONCAT(FirstName, ' ',LastName) AS CustomerName, SUM(Orders.Quantity) AS TotalQuantity FROM Customers
JOIN Orders On Customers.CustomerID = Orders.CustomerID
GROUP BY Customers.FirstName, Customers.LastName
ORDER BY TotalQuantity DESC



=======


SELECT CONCAT(FirstName, ' ', LastName) AS FullName, Products.ProductName, Orders.OrderDate FROM Orders
JOIN Customers ON Orders.CustomerID = Customers.CustomerID
JOIN Products ON Orders.ProductID = Products.ProductID


SELECT SUM(Products.Price * Orders.Quantity) AS TotalRevenue 
FROM Orders
JOIN Products ON Orders.ProductID = Products.ProductID

SELECT CONCAT(FirstName, ' ',LastName) AS CustomerName, SUM(Orders.Quantity) AS TotalQuantity FROM Customers
JOIN Orders On Customers.CustomerID = Orders.CustomerID
GROUP BY Customers.FirstName, Customers.LastName
ORDER BY TotalQuantity DESC



>>>>>>> e9f941987b0236229d01c8042a2218ffdc17b170
