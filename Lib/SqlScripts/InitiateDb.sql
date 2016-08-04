CREATE TABLE Bookings (
	Id int not null,
	Time datetime2,
	UserId int,
	Deleted bit,
	Note nvarchar(250)
)

go

CREATE TABLE Resident(
	Id int not null,
	Name nvarchar(50),
	Password nvarchar(50),
	Guid uniqueidentifier,
	IsAdmin bit
)