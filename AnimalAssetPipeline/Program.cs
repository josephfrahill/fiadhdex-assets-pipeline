using AnimalAssetPipeline;

//Console.WriteLine("Hello, World!");

if (args.Length < 2)
{
    Console.WriteLine(
        "Invalid args");
    return;
}

if (args[0].Equals("1"))
{
    Console.WriteLine(
        "Usage: AnimalAssetPipeline <animals.json>");

    await SourceImageFetcher.RunAsync(args[1]);

    return;
}

return;


//await pipeline.RunAsync(args[0]);

