
CREATE PROCEDURE [dbo].[GetLatestUrlsId]
AS
BEGIN
	SET NOCOUNT ON; -- Prevents sending the row-count message to the client for better performance

	-- Returns the latest available Id, else 0
	SELECT
		TOP(1) [Id]
	FROM [dbo].[Urls]
	ORDER BY [Id] DESC;
END
