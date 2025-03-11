using Parquet;
using Parquet.Data;
using Parquet.Schema;

public interface IStoreEventHubService
{
    // Task<string?> ListEvents(string productNumber);
    Task ProcessEvent(EventPost eventPost);
}

public class StoreEventHubService : IStoreEventHubService
{
    // public async Task<string?> ListEvents(string productNumber)
    // {
    //     if (productNumber is null)
    //     {
    //         throw new ArgumentNullException(nameof(productNumber));
    //     }

    //     return await Task.FromResult(null);
    // }

    public async Task ProcessEvent(EventPost post)
    {
        // create file schema
        var schema = new ParquetSchema(
            new DataField<string>(nameof(EventPost.ProductNumber)),
            new DataField<int>(nameof(EventPost.CustomerNumber)));

        //create data columns with schema metadata and the data you need
        var productNumberColumn = new DataColumn(
            schema.DataFields[0],
            new string[] { post.ProductNumber });

        var customerNumberColumn = new DataColumn(
            schema.DataFields[1],
            new int[] { post.CustomerNumber });

        const string Path1 = "test.parquet";
        if (!File.Exists(Path1))
        {
            using Stream newFileStream = File.Create(Path1);
            using ParquetWriter writer = await ParquetWriter.CreateAsync(schema, newFileStream);
            using ParquetRowGroupWriter groupWriter = writer.CreateRowGroup();
            await groupWriter.WriteColumnAsync(productNumberColumn);
            await groupWriter.WriteColumnAsync(customerNumberColumn);
            return;
        }

        using Stream fileStream = System.IO.File.Open(Path1, FileMode.Open);
        {
            using ParquetWriter parquetWriter = await ParquetWriter.CreateAsync(schema, fileStream, append: true);
            parquetWriter.CompressionMethod = CompressionMethod.Gzip;
            parquetWriter.CompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            // create a new row group in the file
            using ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup();
            await groupWriter.WriteColumnAsync(productNumberColumn);
            await groupWriter.WriteColumnAsync(customerNumberColumn);
        }
    }
}

public class EventPost
{
    public required string ProductNumber { get; set; }
    public int CustomerNumber { get; set; }
};