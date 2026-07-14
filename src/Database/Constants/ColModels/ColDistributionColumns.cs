namespace Database.Constants.ColModels;

public static class ColDistributionColumns
{
    public const string TaxonId = "col:taxonID";
    public const string SourceId = "col:sourceID";
    public const string AreaId = "col:areaID";
    public const string Area = "col:area";
    public const string Gazetteer = "col:gazetteer";

    // these would be useful to capture but always seem to be null when gazetteer is text for animalia
    public const string EstablishmentMeans = "col:establishmentMeans";
    public const string DegreeOfEstablishment = "col:degreeOfEstablishment";
    public const string Pathway = "col:pathway";
    public const string ThreatStatus = "col:threatStatus";
    public const string Year = "col:year";
    public const string Season = "col:season";
    public const string LifeStage = "col:lifeStage";
    public const string ReferenceId = "col:referenceID";
    public const string Remarks = "col:remarks";
    public const string Modified = "col:modified";
    public const string ModifiedBy = "col:modifiedBy";
    public const string Merged = "clb:merged";
}