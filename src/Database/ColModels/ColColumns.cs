namespace Database.ColModels;

public static class ColColumns
{
    public const string Id = "col:ID";
    public const string ScientificName = "col:scientificName";
    public const string Status = "col:status";
    public const string Type = "col:class";
    public const string Rank = "col:rank";
    public const string Phylum = "col:phylum";
    public const string Order = "col:order";
    public const string Family = "col:family";
    public const string Genus = "col:genus";
    public const string Extinct = "col:extinct";

    public const string Kingdom = "col:kingdom";
    //public static ExtinctEnum Extinct = ExtinctEnum.Unknown;
}

public enum ExtinctEnum
{
    Unknown,
    Yes,
    No
}