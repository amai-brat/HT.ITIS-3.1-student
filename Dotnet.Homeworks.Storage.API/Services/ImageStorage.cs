using System.Reactive.Linq;
using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Storage.API.Constants;
using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class ImageStorage : IStorage<Image>
{
    private const string PendingBucket = Buckets.Pending;
    private readonly string _bucket;
    private readonly IMinioClient _minioClient;

    public ImageStorage(string bucket, IMinioClient minioClient)
    {
        _bucket = bucket;
        _minioClient = minioClient;
    }

    public async Task<Result> PutItemAsync(Image item, CancellationToken cancellationToken = default)
    {
        try
        {
            item.Metadata.Add(MetadataKeys.Destination, _bucket);
            
            await MakeBucketIfNotExistsAsync(PendingBucket, cancellationToken);
            
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(PendingBucket)
                .WithObject(item.FileName)
                .WithStreamData(item.Content)
                .WithObjectSize(item.Content.Length)
                .WithHeaders(item.Metadata)
                .WithContentType(item.ContentType), cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }

    public async Task<Image?> GetItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new MemoryStream(); 
            var objectStat = await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(itemName)
                .WithCallbackStream(async (stream, ct) =>
                {
                    await stream.CopyToAsync(content, ct);
                }), cancellationToken);

            return new Image(content, objectStat.ObjectName, objectStat.ContentType, objectStat.MetaData);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Result> RemoveItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(itemName), cancellationToken);
            
            return new Result(true);
        }
        catch (Exception)
        {
            return new Result(true);
        }
    }

    public async Task<IEnumerable<string>> EnumerateItemNamesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _minioClient.ListObjectsAsync(new ListObjectsArgs()
                .WithBucket(_bucket), cancellationToken)
            .Select(x => x.Key)
            .ToList();
        
        return items;
    }

    public async Task<Result> CopyItemToBucketAsync(string itemName, string destinationBucketName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _minioClient.CopyObjectAsync(new CopyObjectArgs()
                    .WithCopyObjectSource(new CopySourceObjectArgs()
                        .WithBucket(_bucket)
                        .WithObject(itemName))
                    .WithBucket(destinationBucketName), cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }

    private async Task MakeBucketIfNotExistsAsync(string bucket, CancellationToken cancellationToken = default)
    {
        var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs()
            .WithBucket(bucket), cancellationToken);
        
        if (exists) return;

        await _minioClient.MakeBucketAsync(new MakeBucketArgs()
            .WithBucket(bucket), cancellationToken);
    }
}