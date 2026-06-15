using AnimalAssetsPipeline;

if (args.Length < 2)
{
    Console.WriteLine(
        "Invalid args");
    return;
}

switch (args[0])
{
    case "1":
        await SourceImageFetcher.RunAsync(args[1]);
        break;

    default:
        Console.WriteLine(
            $"Unknown mode: {args[0]}");
        break;
}