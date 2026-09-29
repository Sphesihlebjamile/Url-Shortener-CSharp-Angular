
CREATE TABLE [dbo].[Urls]
(
	Id int NOT NULL,
	LongUrl NVARCHAR(MAX) NOT NULL,
	ShortUrlCode NVARCHAR(7) COLLATE Latin1_General_BIN2 NOT NULL,	-- ensures that 'aB3xYz1' and 'ab3xyz1' are treated as unique values
	CONSTRAINT PK_Employees_EmployeeID PRIMARY KEY (Id),			-- set's the 'Id' as the primary key
	CONSTRAINT UQ_Urls_ShortUrlCode UNIQUE (ShortUrlCode)			-- ensures that 'ShortUrlCode' is unique
);
GO