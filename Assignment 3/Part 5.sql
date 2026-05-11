<<<<<<< HEAD

CREATE VIEW HighValueProducts AS
SELECT ProductName, Price 
FROM Products
WHERE Price > 1000


CREATE PROCEDURE RegisterCustomer
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100)
AS
BEGIN
    INSERT INTO Customers (FirstName, LastName, Email, RegistrationDate)
    VALUES(@FirstName, @LastName, @Email, CURRENT_DATE)
=======

CREATE VIEW HighValueProducts AS
SELECT ProductName, Price 
FROM Products
WHERE Price > 1000


CREATE PROCEDURE RegisterCustomer
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100)
AS
BEGIN
    INSERT INTO Customers (FirstName, LastName, Email, RegistrationDate)
    VALUES(@FirstName, @LastName, @Email, CURRENT_DATE)
>>>>>>> e9f941987b0236229d01c8042a2218ffdc17b170
END;