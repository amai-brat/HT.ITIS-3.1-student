using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.MainProject.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.MongoDb;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoClient(this IServiceCollection services,
        MongoDbConfig mongoConfiguration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var client = new MongoClient(mongoConfiguration.ConnectionString);
        var database = client.GetDatabase(mongoConfiguration.DatabaseName);
        services.AddSingleton<IMongoClient, MongoClient>(_ => client);
        services.AddSingleton<IMongoCollection<Order>, IMongoCollection<Order>>(_ => database 
            .GetCollection<Order>(mongoConfiguration.OrdersCollectionName));
        
        return services;
    }
}