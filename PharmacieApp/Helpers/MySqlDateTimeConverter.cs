using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PharmacieApp.Helpers;

/// <summary>
/// MySQL renvoie ses DATETIME au format "2026-07-20 10:19:27" (espace
/// entre date et heure). System.Text.Json attend de l'ISO 8601, qui
/// exige un "T" : sans ce convertisseur, toute désérialisation échoue.
/// </summary>
public class MySqlDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] Formats =
    {
        "yyyy-MM-dd HH:mm:ss",   // DATETIME
        "yyyy-MM-dd",            // DATE (date_peremption)
        "yyyy-MM-ddTHH:mm:ss"    // ISO, au cas où
    };

    public override DateTime Read(ref Utf8JsonReader reader,
                                  Type typeToConvert,
                                  JsonSerializerOptions options)
    {
        var valeur = reader.GetString();

        if (string.IsNullOrWhiteSpace(valeur))
            return default;

        if (DateTime.TryParseExact(valeur, Formats,
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out var date))
            return date;

        // Dernier recours : le parseur générique
        return DateTime.Parse(valeur, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer,
                               DateTime value,
                               JsonSerializerOptions options)
    {
        // On réécrit dans le format que l'API attend
        writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}