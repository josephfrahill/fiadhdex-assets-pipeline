using FiadhDex.Models.AnimalData;
using FiadhDex.Models.Enums;
using System.ComponentModel;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FiadhDex.Models;

public static class JsonConfigSettings
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        /*
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            //new JsonDescriptionEnumConverterFactory(),
            //new JsonDescriptionEnumConverter<Habitat>(),
            //new JsonDescriptionEnumConverter<GeographicRegion>()
        }
        */
    };
}

public class JsonDescriptionEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Target only enum types
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Instantiates JsonDescriptionEnumConverter<T> dynamically for the specific enum
        var converterType = typeof(JsonDescriptionEnumConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}


public class JsonDescriptionEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Console.WriteLine("Test");
        var jsonValue = reader.GetString();
        if (string.IsNullOrWhiteSpace(jsonValue))
        {
            return default;
        }

        // Search enum fields for a matching Description attribute
        foreach (var field in typeof(T).GetFields())
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();

            // Check if description matches JSON value (case-insensitive fallback)
            if (attribute != null && string.Equals(attribute.Description, jsonValue, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null)!;
            }

            // Fallback: Check if the raw enum name matches the JSON value
            if (string.Equals(field.Name, jsonValue, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null)!;
            }
        }

        throw new JsonException($"Unable to convert \"{jsonValue}\" to enum {typeof(T).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Console.WriteLine("Test write");

        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        // Write the description if it exists; otherwise, default to the enum name
        var displayString = attribute?.Description ?? value.ToString();

        // Match camelCase naming policy if no description exists and it's configured
        if (attribute == null && options.PropertyNamingPolicy == JsonNamingPolicy.CamelCase)
        {
            displayString = JsonNamingPolicy.CamelCase.ConvertName(displayString);
        }

        writer.WriteStringValue(displayString);
    }
}
