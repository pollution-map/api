namespace PollutionMapAPI.Data.Entities;

/// <summary>
/// https://www.c-sharpcorner.com/UploadFile/raj1979/authorization-using-web-api/
/// 
/// 1 issue the DatasetAcces tokens with dataset id user ID has access to
/// 2 Authenticate using custom DataSetAcces token as a user with claims to manage specific dataset
/// 
/// JWT of USER ID CLAIM + DATASET ID CLAIM
/// 
/// https://developer.twitter.com/en/docs/authentication/oauth-2-0/application-only
/// https://auth0.com/docs/get-started/authentication-and-authorization-flow/client-credentials-flow
/// https://auth0.com/docs/quickstart/backend/aspnet-core-webapi
/// authorize dataset:read
/// authorize dataset:write?
/// 
/// 
/// 3 in theroy user shoud have CUD acces to specific dataset
/// </summary>

public class Dataset : BaseEntity<Guid>
{
    public Guid MapId { get; set; }
    public virtual Map Map { get; set; }
    public virtual List<DatasetProperty> Properties { get; set; }
    public virtual List<DatasetItem> Items { get; set; }

    public Dataset()
    {
        Id = Guid.NewGuid();
    }
}