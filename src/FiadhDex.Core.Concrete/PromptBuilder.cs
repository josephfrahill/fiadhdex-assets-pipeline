using FiadhDex.Core.Abstraction;

namespace FiadhDex.Core.Concrete;

public class PromptBuilder : IPromptBuilder
{
    public string Build(string scientificName, string? commonName)
    {
        //Native Regions
        //This describes the geographic area where the species naturally occurs:
        // "Southeast Asia",
        //  "Indonesia",
        //  "Malaysia",
        //  "Thailand",
        // "Australia",
        //  "New Guinea"

        /*
         * Native to Australia
            Australia
            New Guinea
            Indonesia
            Endemic to Australia
            Australia

            The second means the species naturally occurs nowhere else.

             dectability = How easy it is for a person actively searching in the appropriate habitat to physically find and observe this animal.

         */
        var test = "You are an expert zoologist." +
                   "\n\nReturn ONLY valid JSON." +
                   $"\n\nSpecies:\nScientific name: {scientificName}" +
                   $"\nCommon name: {commonName}" +
                   "\n\nReturn:";

        return test;
    }
}