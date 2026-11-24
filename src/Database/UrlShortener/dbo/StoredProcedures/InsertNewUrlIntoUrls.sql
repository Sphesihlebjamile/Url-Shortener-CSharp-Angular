CREATE PROCEDURE [dbo].[InsertNewUrlIntoUrls]
	@Id [BIGINT],
	@LongUrl [NVARCHAR](MAX),
	@ShortUrlCode [NVARCHAR](7),
	@ExpiryDate DATETIMEOFFSET(7)
AS
BEGIN
	SET XACT_ABORT ON;
	SET NOCOUNT ON;

	BEGIN TRANSACTION
	
	BEGIN TRY
		INSERT INTO [dbo].[Urls]
			([Id],
			[LongUrl],
			[ShortUrlCode],
			[IsActive],
			[ExpiryDate],
			[CreatedDate],
			[LastUpdated])
		VALUES
			(@Id,
			@LongUrl,
			@ShortUrlCode,
			1,
			@ExpiryDate,
			SYSDATETIMEOFFSET(),
			SYSDATETIMEOFFSET());
		COMMIT TRANSACTION;

		SELECT 1;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION
		END

		SELECT -1;
	END CATCH
END;
GO