CREATE Database TechZoneDB

USE TechZoneDB

CREATE TABLE Products(

	ProductID INT PRIMARY KEY,
	ProductName VARCHAR(50) NOT NULL,
	Category VARCHAR(50) NOT NULL,
	Price INT NOT NULL,
	StockQuantity INT NOT NULL

);


CREATE TABLE Customers(

	CustomerID INT PRIMARY KEY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Email VARCHAR(100) NOT NULL,
	RegistrationDate DATE NOT NULL

);


CREATE TABLE Orders(

	OrderID INT PRIMARY KEY,
	Quantity INT NOT NULL,
	OrderDate DATE NOT NULL,
	CustomerID INT NOT NULL,
	ProductID INT NOT NULL,
	FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
	FOREIGN KEY (ProductID) REFERENCES Products(ProductID)

);



