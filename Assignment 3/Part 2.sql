
INSERT INTO Products (ProductID, ProductName, Category, Price, StockQuantity)
VALUES
(1, 'MacBook Pro', 'Laptop', 1200.00, 10),
(2, 'iPhone 17', 'Phone', 999.00, 20),
(3, 'Dell XPS', 'Laptop', 1100.00, 8),
(4, 'Samsung S25', 'Phone', 850.00, 15),
(5, 'Sony Headphones', 'Audio', 200.00, 50);


INSERT INTO Customers (CustomerID, FirstName, LastName, Email, RegistrationDate)
VALUES
(1, 'John', 'Doe', 'john@mail.com', '2026-01-10'),
(2, 'Jane', 'Smith', 'jane@mail.com', '2026-02-15'),
(3, 'Alice', 'Brown', 'alice@mail.com', '2026-03-20'),
(4, 'Bob', 'White', 'bob@mail.com', '2026-04-05');


INSERT INTO Orders (OrderID, CustomerID, ProductID, Quantity, OrderDate)
VALUES
(1, 1, 1, 1, '2026-05-01'),
(2, 2, 2, 2, '2026-05-02'),
(3, 3, 5, 1, '2026-05-03'),
(4, 4, 3, 1, '2026-05-04'),
(5, 1, 4, 1, '2026-05-05');





