using Dotnet.Homeworks.Storage.API.Constants;

namespace Dotnet.Homeworks.Storage.API.Services;

public class PendingObjectsProcessor : BackgroundService
{
    private const string PendingBucket = Buckets.Pending;
    private readonly IStorageFactory _storageFactory;
    public PendingObjectsProcessor(IStorageFactory storageFactory)
    {
        _storageFactory = storageFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pendingStorage = await _storageFactory.CreateImageStorageWithinBucketAsync(PendingBucket);
        
        while (stoppingToken.IsCancellationRequested == false)
        {
            var pendingItems = await pendingStorage.EnumerateItemNamesAsync(stoppingToken);
            
            foreach (var pendingItemName in pendingItems)
            {
                var item = await pendingStorage.GetItemAsync(pendingItemName, stoppingToken);

                if (item!.Metadata.TryGetValue(MetadataKeys.Destination, out var destBucket))
                {
                    await pendingStorage.CopyItemToBucketAsync(pendingItemName, destBucket, stoppingToken);
                }
                
                await pendingStorage.RemoveItemAsync(pendingItemName, stoppingToken);
            }
            
            await Task.Delay(PendingObjectProcessor.Period, stoppingToken);
        }
    }
}