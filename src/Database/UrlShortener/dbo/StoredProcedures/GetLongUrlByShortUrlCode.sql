CREATE PROCEDURE [dbo].[GetLongUrlByShortUrlCode]
	@ShortUrl NVARCHAR(7),
	@Alias VARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	-- Returns a LongUrl for the provided short url
	SELECT
		TOP(1) *
	FROM [dbo].[Urls]
	WHERE ([ShortUrlCode] = @ShortUrl OR [Alias] = @Alias) 
		AND [IsActive] = 1;
END