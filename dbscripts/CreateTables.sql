CREATE TABLE [User](
	Id int Identity,
	Name nvarchar(50),
	Password nvarchar(50),
	Guid UniqueIdentifier,
	IsAdmin bit,
	Deleted bit default 0
);

GO

CREATE TABLE Booking(
	Id int Identity,
	Time datetime2,
	UserId int,
	Note varchar(255)
)