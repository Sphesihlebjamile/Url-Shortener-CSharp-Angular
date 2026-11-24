
CREATE PROCEDURE [dbo].[GetShortUrlCodeByLongUrl]
	@LongUrl NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	-- Returns the ShortUrlCode for the provided long url
	SELECT
		TOP(1) *
	FROM [dbo].[Urls]
	WHERE [LongUrl] = @LongUrl
END