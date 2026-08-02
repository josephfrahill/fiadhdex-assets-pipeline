using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace FiadhDex.Core.Concrete.Api;

public class DbCloudBackupService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public DbCloudBackupService(
        IAmazonS3 s3Client,
        IConfiguration configuration)
    {
        _s3Client = s3Client;

        _bucketName =
            configuration["R2_DATA_BUCKET_NAME"]
            ?? throw new InvalidOperationException(
                "R2 bucket name is missing.");
    }

    public async Task PushToCloudAsync(
        string databasePath, string dbFileName,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Starting upload: {databasePath}");

        var fileInfo = new FileInfo(databasePath);

        Console.WriteLine(
            $"File size: {fileInfo.Length / 1024d / 1024d:F2} MB");

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = $"db-backup/{dbFileName}",
            FilePath = databasePath,
            ContentType = "application/x-sqlite3",
            DisablePayloadSigning = true
        };

        Console.WriteLine("Calling R2...");

        var response = await _s3Client.PutObjectAsync(
            request,
            cancellationToken);

        Console.WriteLine(
            $"Upload complete: {response.HttpStatusCode}");
    }
}