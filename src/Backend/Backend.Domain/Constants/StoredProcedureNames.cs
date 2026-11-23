namespace Backend.Domain.Constants;

public static class StoredProcedureNames
{
    public const string GETLATESTURLSID = "[dbo].[GetLatestUrlsId]";
    public const string GETSHORTURLCODEBYLONGURL = "[dbo].[GetShortUrlCodeByLongUrl]";
    public const string INSERTNEWURLINTOURLS = "[dbo].[InsertNewUrlIntoUrls]";
    public const string GETLONGURLBYSHORTURLCODE = "[dbo].[GetLongUrlByShortUrlCode]";
    public const string ENABLESHORTURL = "[dbo].[EnableShortUrl]";
    public const string DISABLESHORTURL = "[dbo].[DisableShortUrl]";
}
