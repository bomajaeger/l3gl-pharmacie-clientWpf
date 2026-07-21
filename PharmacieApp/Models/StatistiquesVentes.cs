using System.Text.Json.Serialization;

namespace PharmacieApp.Models;

/// <summary>
/// Réponse de GET /ventes/stats.
/// </summary>
public class StatistiquesVentes
{
    [JsonPropertyName("nombre_ventes")]
    public int NombreVentes { get; set; }

    /// <summary>
    /// Chiffre d'affaires en FCFA.
    /// </summary>
    [JsonPropertyName("chiffre_affaires")]
    public int ChiffreAffaires { get; set; }

    [JsonPropertyName("unites_vendues")]
    public int UnitesVendues { get; set; }
}