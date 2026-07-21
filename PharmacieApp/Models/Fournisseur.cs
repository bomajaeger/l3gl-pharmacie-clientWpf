using System.Text.Json.Serialization;

namespace PharmacieApp.Models;

public class Fournisseur
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = string.Empty;

    [JsonPropertyName("contact")]
    public string? Contact { get; set; }

    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    [JsonPropertyName("adresse")]
    public string? Adresse { get; set; }

    // Affichage dans les ComboBox
    public override string ToString() => Nom;
}