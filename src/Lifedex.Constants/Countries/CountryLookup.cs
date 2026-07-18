namespace Lifedex.Constants.Countries;

public static class CountryLookup
{
    private static readonly Dictionary<string, CountryInfo> ByCode;
    private static readonly Dictionary<string, CountryInfo> ByName;

    static CountryLookup()
    {
        ByCode = new Dictionary<string, CountryInfo>(StringComparer.OrdinalIgnoreCase);
        ByName = new Dictionary<string, CountryInfo>(StringComparer.OrdinalIgnoreCase);

        // Populate dataset directly to avoid messy file I/O operations at runtime
        var rawData = GetRawData();
        foreach (var country in rawData)
        {
            ByCode[country.Code] = country;
            ByName[country.Name] = country;

            // Allows "United States of America" to also be found via "United States"
            var cleanName = CleanCountryName(country.Name);
            if (cleanName != country.Name)
            {
                ByName[cleanName] = country;
            }
        }
    }

    /// <summary>
    /// Tries to resolve a country entity using either its full Name or 2-letter ISO Code.
    /// </summary>
    public static bool TryParse(string? input, out CountryInfo country)
    {
        country = new CountryInfo
        {
            Code = string.Empty,
            Name = string.Empty
        };

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var trimmedInput = input.Trim();

        if (trimmedInput.Length == 2 && ByCode.TryGetValue(trimmedInput, out country!))
        {
            return true;
        }

        return ByName.TryGetValue(trimmedInput, out country!);
    }

    private static string CleanCountryName(string name)
    {
        // Strips off formal formatting blocks like " (the)", " (Kingdom of the)", etc.
        var index = name.IndexOf(" (", StringComparison.Ordinal);
        if (index > 0)
            return name[..index];

        index = name.IndexOf(", ", StringComparison.Ordinal);
        return index > 0 ? name[..index] : name;
    }

    private static List<CountryInfo> GetRawData() =>
    [
        new() { Name = "Afghanistan", Code = "AF" },
        new() { Name = "Albania", Code = "AL" },
        new() { Name = "Algeria", Code = "DZ" },
        new() { Name = "American Samoa", Code = "AS" },
        new() { Name = "Andorra", Code = "AD" },
        new() { Name = "Angola", Code = "AO" },
        new() { Name = "Anguilla", Code = "AI" },
        new() { Name = "Antarctica", Code = "AQ" },
        new() { Name = "Antigua and Barbuda", Code = "AG" },
        new() { Name = "Argentina", Code = "AR" },
        new() { Name = "Armenia", Code = "AM" },
        new() { Name = "Aruba", Code = "AW" },
        new() { Name = "Australia", Code = "AU" },
        new() { Name = "Austria", Code = "AT" },
        new() { Name = "Azerbaijan", Code = "AZ" },
        new() { Name = "Bahamas (The)", Code = "BS" },
        new() { Name = "Bahrain", Code = "BH" },
        new() { Name = "Bangladesh", Code = "BD" },
        new() { Name = "Barbados", Code = "BB" },
        new() { Name = "Belarus", Code = "BY" },
        new() { Name = "Belgium", Code = "BE" },
        new() { Name = "Belize", Code = "BZ" },
        new() { Name = "Benin", Code = "BJ" },
        new() { Name = "Bermuda", Code = "BM" },
        new() { Name = "Bhutan", Code = "BT" },
        new() { Name = "Bolivia (Plurinational State of)", Code = "BO" },
        new() { Name = "Bonaire, Sint Eustatius and Saba", Code = "BQ" },
        new() { Name = "Bosnia and Herzegovina", Code = "BA" },
        new() { Name = "Botswana", Code = "BW" },
        new() { Name = "Bouvet Island", Code = "BV" },
        new() { Name = "Brazil", Code = "BR" },
        new() { Name = "British Indian Ocean Territory (the)", Code = "IO" },
        new() { Name = "Brunei Darussalam", Code = "BN" },
        new() { Name = "Bulgaria", Code = "BG" },
        new() { Name = "Burkina Faso", Code = "BF" },
        new() { Name = "Burundi", Code = "BI" },
        new() { Name = "Cabo Verde", Code = "CV" },
        new() { Name = "Cambodia", Code = "KH" },
        new() { Name = "Cameroon", Code = "CM" },
        new() { Name = "Canada", Code = "CA" },
        new() { Name = "Cayman Islands (the)", Code = "KY" },
        new() { Name = "Central African Republic (the)", Code = "CF" },
        new() { Name = "Chad", Code = "TD" },
        new() { Name = "Chile", Code = "CL" },
        new() { Name = "China", Code = "CN" },
        new() { Name = "Christmas Island", Code = "CX" },
        new() { Name = "Cocos (Keeling) Islands (the)", Code = "CC" },
        new() { Name = "Colombia", Code = "CO" },
        new() { Name = "Comoros (the)", Code = "KM" },
        new() { Name = "Congo (the Democratic Republic of the)", Code = "CD" },
        new() { Name = "Congo (the)", Code = "CG" },
        new() { Name = "Cook Islands (the)", Code = "CK" },
        new() { Name = "Costa Rica", Code = "CR" },
        new() { Name = "Croatia", Code = "HR" },
        new() { Name = "Cuba", Code = "CU" },
        new() { Name = "Curaçao", Code = "CW" },
        new() { Name = "Cyprus", Code = "CY" },
        new() { Name = "Czechia", Code = "CZ" },
        new() { Name = "Côte d'Ivoire", Code = "CI" },
        new() { Name = "Denmark", Code = "DK" },
        new() { Name = "Djibouti", Code = "DJ" },
        new() { Name = "Dominica", Code = "DM" },
        new() { Name = "Dominican Republic (the)", Code = "DO" },
        new() { Name = "Ecuador", Code = "EC" },
        new() { Name = "Egypt", Code = "EG" },
        new() { Name = "El Salvador", Code = "SV" },
        new() { Name = "Equatorial Guinea", Code = "GQ" },
        new() { Name = "Eritrea", Code = "ER" },
        new() { Name = "Estonia", Code = "EE" },
        new() { Name = "Eswatini", Code = "SZ" },
        new() { Name = "Ethiopia", Code = "ET" },
        new() { Name = "Falkland Islands (the) [Malvinas]", Code = "FK" },
        new() { Name = "Faroe Islands (the)", Code = "FO" },
        new() { Name = "Fiji", Code = "FJ" },
        new() { Name = "Finland", Code = "FI" },
        new() { Name = "France", Code = "FR" },
        new() { Name = "French Guiana", Code = "GF" },
        new() { Name = "French Polynesia", Code = "PF" },
        new() { Name = "French Southern Territories (the)", Code = "TF" },
        new() { Name = "Gabon", Code = "GA" },
        new() { Name = "Gambia (the)", Code = "GM" },
        new() { Name = "Georgia", Code = "GE" },
        new() { Name = "Germany", Code = "DE" },
        new() { Name = "Ghana", Code = "GH" },
        new() { Name = "Gibraltar", Code = "GI" },
        new() { Name = "Greece", Code = "GR" },
        new() { Name = "Greenland", Code = "GL" },
        new() { Name = "Grenada", Code = "GD" },
        new() { Name = "Guadeloupe", Code = "GP" },
        new() { Name = "Guam", Code = "GU" },
        new() { Name = "Guatemala", Code = "GT" },
        new() { Name = "Guernsey", Code = "GG" },
        new() { Name = "Guinea", Code = "GN" },
        new() { Name = "Guinea-Bissau", Code = "GW" },
        new() { Name = "Guyana", Code = "GY" },
        new() { Name = "Haiti", Code = "HT" },
        new() { Name = "Heard Island and McDonald Islands", Code = "HM" },
        new() { Name = "Holy See (the)", Code = "VA" },
        new() { Name = "Honduras", Code = "HN" },
        new() { Name = "Hong Kong", Code = "HK" },
        new() { Name = "Hungary", Code = "HU" },
        new() { Name = "Iceland", Code = "IS" },
        new() { Name = "India", Code = "IN" },
        new() { Name = "Indonesia", Code = "ID" },
        new() { Name = "Iran (Islamic Republic of)", Code = "IR" },
        new() { Name = "Iraq", Code = "IQ" },
        new() { Name = "Ireland", Code = "IE" },
        new() { Name = "Isle of Man", Code = "IM" },
        new() { Name = "Israel", Code = "IL" },
        new() { Name = "Italy", Code = "IT" },
        new() { Name = "Jamaica", Code = "JM" },
        new() { Name = "Japan", Code = "JP" },
        new() { Name = "Jersey", Code = "JE" },
        new() { Name = "Jordan", Code = "JO" },
        new() { Name = "Kazakhstan", Code = "KZ" },
        new() { Name = "Kenya", Code = "KE" },
        new() { Name = "Kiribati", Code = "KI" },
        new() { Name = "Korea (the Democratic People's Republic of)", Code = "KP" },
        new() { Name = "Korea (the Republic of)", Code = "KR" },
        new() { Name = "Kuwait", Code = "KW" },
        new() { Name = "Kyrgyzstan", Code = "KG" },
        new() { Name = "Lao People's Democratic Republic (the)", Code = "LA" },
        new() { Name = "Latvia", Code = "LV" },
        new() { Name = "Lebanon", Code = "LB" },
        new() { Name = "Lesotho", Code = "LS" },
        new() { Name = "Liberia", Code = "LR" },
        new() { Name = "Libya", Code = "LY" },
        new() { Name = "Liechtenstein", Code = "LI" }, new() { Name = "Lithuania", Code = "LT" },
        new() { Name = "Luxembourg", Code = "LU" }, new() { Name = "Macao", Code = "MO" },
        new() { Name = "Madagascar", Code = "MG" }, new() { Name = "Malawi", Code = "MW" },
        new() { Name = "Malaysia", Code = "MY" }, new() { Name = "Maldives", Code = "MV" },
        new() { Name = "Mali", Code = "ML" }, new() { Name = "Malta", Code = "MT" },
        new() { Name = "Marshall Islands (the)", Code = "MH" }, new() { Name = "Martinique", Code = "MQ" },
        new() { Name = "Mauritania", Code = "MR" }, new() { Name = "Mauritius", Code = "MU" },
        new() { Name = "Mayotte", Code = "YT" }, new() { Name = "Mexico", Code = "MX" },
        new() { Name = "Micronesia (Federated States of)", Code = "FM" },
        new() { Name = "Moldova (the Republic of)", Code = "MD" }, new() { Name = "Monaco", Code = "MC" },
        new() { Name = "Mongolia", Code = "MN" }, new() { Name = "Montenegro", Code = "ME" },
        new() { Name = "Montserrat", Code = "MS" }, new() { Name = "Morocco", Code = "MA" },
        new() { Name = "Mozambique", Code = "MZ" }, new() { Name = "Myanmar", Code = "MM" },
        new() { Name = "Namibia", Code = "NA" }, new() { Name = "Nauru", Code = "NR" },
        new() { Name = "Nepal", Code = "NP" }, new() { Name = "Netherlands (Kingdom of the)", Code = "NL" },
        new() { Name = "New Caledonia", Code = "NC" }, new() { Name = "New Zealand", Code = "NZ" },
        new() { Name = "Nicaragua", Code = "NI" }, new() { Name = "Niger (the)", Code = "NE" },
        new() { Name = "Nigeria", Code = "NG" }, new() { Name = "Niue", Code = "NU" },
        new() { Name = "Norfolk Island", Code = "NF" }, new() { Name = "North Macedonia", Code = "MK" },
        new() { Name = "Northern Mariana Islands (the)", Code = "MP" }, new() { Name = "Norway", Code = "NO" },
        new() { Name = "Oman", Code = "OM" }, new() { Code = "PK", Name = "Pakistan" },
        new() { Code = "PW", Name = "Palau" }, new() { Code = "PS", Name = "Palestine, State of" },
        new() { Code = "PA", Name = "Panama" }, new() { Code = "PG", Name = "Papua New Guinea" },
        new() { Code = "PY", Name = "Paraguay" }, new() { Code = "PE", Name = "Peru" },
        new() { Code = "PH", Name = "Philippines (the)" }, new() { Code = "PN", Name = "Pedcairn" },
        new() { Code = "PL", Name = "Poland" }, new() { Code = "PT", Name = "Portugal" },
        new() { Code = "PR", Name = "Puerto Rico" }, new() { Code = "QA", Name = "Qatar" },
        new() { Code = "RO", Name = "Romania" }, new() { Code = "RU", Name = "Russian Federation (the)" },
        new() { Code = "RW", Name = "Rwanda" }, new() { Code = "RE", Name = "Réunion" },
        new() { Code = "BL", Name = "Saint Barthélemy" },
        new() { Code = "SH", Name = "Saint Helena, Ascension and Tristan da Cunha" },
        new() { Code = "KN", Name = "Saint Kitts and Nevis" }, new() { Code = "LC", Name = "Saint Lucia" },
        new() { Code = "MF", Name = "Saint Martin (French part)" },
        new() { Code = "PM", Name = "Saint Pierre and Miquelon" },
        new() { Code = "VC", Name = "Saint Vincent and the Grenadines" }, new() { Code = "WS", Name = "Samoa" },
        new() { Code = "SM", Name = "San Marino" }, new() { Code = "ST", Name = "Sao Tome and Principe" },
        new() { Code = "SA", Name = "Saudi Arabia" }, new() { Code = "SN", Name = "Senegal" },
        new() { Code = "RS", Name = "Serbia" }, new() { Code = "SC", Name = "Seychelles" },
        new() { Code = "SL", Name = "Sierra Leone" }, new() { Code = "SG", Name = "Singapore" },
        new() { Code = "SX", Name = "Sint Maarten (Dutch part)" }, new() { Code = "SK", Name = "Slovakia" },
        new() { Code = "SI", Name = "Slovenia" }, new() { Code = "SB", Name = "Solomon Islands" },
        new() { Code = "SO", Name = "Somalia" }, new() { Code = "ZA", Name = "South Africa" },
        new() { Code = "GS", Name = "South Georgia and the South Sandwich Islands" },
        new() { Code = "SS", Name = "South Sudan" }, new() { Code = "ES", Name = "Spain" },
        new() { Code = "LK", Name = "Sri Lanka" }, new() { Code = "SD", Name = "Sudan (the)" },
        new() { Code = "SR", Name = "Suriname" }, new() { Code = "SJ", Name = "Svalbard and Jan Mayen" },
        new() { Code = "SE", Name = "Sweden" }, new() { Code = "CH", Name = "Switzerland" },
        new() { Code = "SY", Name = "Syrian Arab Republic (the)" },
        new() { Code = "TW", Name = "Taiwan (Province of China)" }, new() { Code = "TJ", Name = "Tajikistan" },
        new() { Code = "TZ", Name = "Tanzania, the United Republic of" }, new() { Code = "TH", Name = "Thailand" },
        new() { Code = "TL", Name = "Timor-Leste" }, new() { Code = "TG", Name = "Togo" },
        new() { Code = "TK", Name = "Tokelau" }, new() { Code = "TO", Name = "Tonga" },
        new() { Code = "TT", Name = "Trinidad and Tobago" }, new() { Code = "TN", Name = "Tunisia" },
        new() { Code = "TM", Name = "Turkmenistan" }, new() { Code = "TC", Name = "Turks and Caicos Islands (the)" },
        new() { Code = "TV", Name = "Tuvalu" }, new() { Code = "TR", Name = "Türkiye" },
        new() { Code = "UG", Name = "Uganda" }, new() { Code = "UA", Name = "Ukraine" },
        new() { Code = "AE", Name = "United Arab Emirates (the)" },
        new() { Code = "GB", Name = "United Kingdom" },
        new() { Code = "UM", Name = "United States Minor Outlying Islands (the)" },
        new() { Code = "US", Name = "United States of America (the)" }, new() { Code = "UY", Name = "Uruguay" },
        new() { Code = "UZ", Name = "Uzbekistan" }, new() { Code = "VU", Name = "Vanuatu" },
        new() { Code = "VE", Name = "Venezuela (Bolivarian Republic of)" }, new() { Code = "VN", Name = "Viet Nam" },
        new() { Code = "VG", Name = "Virgin Islands (British)" }, new() { Code = "VI", Name = "Virgin Islands (U.S.)" },
        new() { Code = "WF", Name = "Wallis and Futuna" }, new() { Code = "EH", Name = "Western Sahara*" },
        new() { Code = "YE", Name = "Yemen" }, new() { Code = "ZM", Name = "Zambia" },
        new() { Code = "ZW", Name = "Zimbabwe" }, new() { Code = "AX", Name = "Åland Islands" }
    ];
}

public record CountryInfo
{
    public required string Code { get; set; }
    public required string Name { get; set; }
}