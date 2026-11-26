
CREATE TABLE [dbo].[Urls]
(
	[Id] [BIGINT] NOT NULL,
	[LongUrl] [NVARCHAR](MAX) NOT NULL,
	[ShortUrlCode] [NVARCHAR](7) COLLATE Latin1_General_BIN2 NOT NULL,		-- ensures that 'aB3xYz1' and 'ab3xyz1' are treated as unique values
	[Alias] [VARCHAR](100) COLLATE Latin1_General_BIN2,      				-- stores a custom alias set by a user
	[IsActive] BIT NOT NULL DEFAULT 1,										-- tracks whether or not a value is active
	[ExpiryDate] DATETIMEOFFSET(7)						,					-- tracks the expiry date of an entry, default is to never expire (NULL)
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),	-- tracks when the record was created
	[LastUpdated] DATETIMEOFFSET(7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),	-- tracks when the record was last updated
	CONSTRAINT PK_Employees_EmployeeID PRIMARY KEY (Id),					-- set's the 'Id' as the primary key
	CONSTRAINT UQ_Urls_ShortUrlCode UNIQUE (ShortUrlCode),  				-- ensures that 'ShortUrlCode' is unique
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_Urls_Alias
ON [dbo].[Urls] ([Alias])
WHERE [Alias] IS NOT NULL