CREATE PROCEDURE [dbo].[BulkSearchShortUrlCodeByLongUrls]
	@LongUrlsStringArray NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(ISJSON(@LongUrlsStringArray, ARRAY), 0) <> 1
	BEGIN
		RETURN;
	END

	;WITH InputUrls AS
	(
		SELECT
			CONVERT(NVARCHAR(MAX), [value]) AS LongUrl
		FROM OPENJSON(@LongUrlsStringArray)
		WHERE [type] = 1 -- JSON string
	)
	SELECT
		inputUrls.LongUrl,
		shortUrl.ShortUrlCode
	FROM InputUrls AS inputUrls
	OUTER APPLY
	(
		SELECT TOP (1)
			urls.ShortUrlCode
		FROM [dbo].[Urls] AS urls
		WHERE urls.LongUrl = inputUrls.LongUrl
		ORDER BY urls.CreatedDate DESC, urls.Id DESC
	) AS shortUrl;
END