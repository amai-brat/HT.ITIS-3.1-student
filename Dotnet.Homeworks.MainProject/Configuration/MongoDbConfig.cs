namespace Dotnet.Homeworks.MainProject.Configuration;

public class MongoDbConfig
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrdersCollectionName { get; set; } = null!;
}