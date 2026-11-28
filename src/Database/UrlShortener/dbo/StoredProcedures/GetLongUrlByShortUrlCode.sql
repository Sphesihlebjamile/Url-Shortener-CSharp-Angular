CREATE PROCEDURE [dbo].[GetLongUrlByShortUrlCode]
	@ShortUrlCode NVARCHAR(7),
	@Alias VARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	-- Returns a LongUrl for the provided short url

	IF EXISTS
	(
		SELECT 1
		FROM [dbo].[Urls]
		WHERE [ShortUrlCode] = @ShortUrlCode
			AND [IsActive] = 1
	)
	BEGIN
		SELECT
			TOP(1) *
		FROM [dbo].[Urls]
		WHERE [ShortUrlCode] = @ShortUrlCode
			AND [IsActive] = 1;
		
		RETURN;
	END

	SELECT
		TOP(1) *
	FROM [dbo].[Urls]
	WHERE [Alias] = @Alias AND [IsActive] = 1;
END