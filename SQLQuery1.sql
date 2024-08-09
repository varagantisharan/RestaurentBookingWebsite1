use Restaurant

CREATE TABLE Customers (
    customer_id INT PRIMARY KEY IDENTITY(1,1),
    first_name VARCHAR(20) NOT NULL,
    last_name VARCHAR(20) NOT NULL,
    address VARCHAR(50),
    password VARCHAR(255) NOT NULL,
    phone_number VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL
);
CREATE TABLE Admins (
    admin_id INT PRIMARY KEY IDENTITY(1,1),
    first_name VARCHAR(20) NOT NULL,
    last_name VARCHAR(20) NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL
);
CREATE TABLE Bookings (
    booking_id INT PRIMARY KEY IDENTITY(1,1),
    customer_id INT NOT NULL,
    booking_date DATETIME NOT NULL,
	slot_Time int NOT NULL,
	Status varchar(10) DEFAULT'booked',
	creation_time DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (customer_id) References customers(customer_id),

);
CREATE TABLE CheckIns (
    checkin_id INT PRIMARY KEY IDENTITY(1,1),
    booking_id INT NOT NULL,
    Checkin_time DATETIME NOT NULL,
	check_out_time DATETIME,
	gross_amount DECIMAL(10,2)
    FOREIGN KEY (booking_id) REFERENCES bookings(booking_id)
);
select * from Customers
select * from Admins
select * from Bookings
select * from CheckIns

alter table Admins drop column user_id

ALTER TABLE Admins add Address varchar(50);
ALTER TABLE Admins add PhoneNumber varchar(10);

Alter table Admin_id

--Scaffold-DbContext "Server=SharanVm;Database=Restaurant;Trusted_Connection=True; TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir 'C:\Users\SRVARAGA\Documents\RestaurentBookingWebsite\DALayer\Model' 
Scaffold-DbContext "Server=myserver\mydb;Database=mydb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -f

Select choose(3,'hi','Helo','helohi')

select cast('10' as int) * 20 as Casting, convert(int,'10')*20

select choose(3,'Hi','Helo','hihelo') as Result


alter table CheckIns drop column Customer_Id

Alter table CheckIns
DROP CONSTRAINT FK__CheckIns__Custom__6EF57B66;