using System;
using System.Text.Json.Serialization;

namespace PharmacieApp.Models;

public class Vente
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("id_medicament")]
    public int IdMedicament { get; set; }

    [JsonPropertyName("medicament_nom")]
    public string MedicamentNom { get; set; } = string.Empty;

    [JsonPropertyName("quantite_vendue")]
    public int QuantiteVendue { get; set; }

    [JsonPropertyName("prix_unitaire")]
    public int PrixUnitaire { get; set; }

    [JsonPropertyName("prix_total")]
    public int PrixTotal { get; set; }

    [JsonPropertyName("date_vente")]
    public DateTime DateVente { get; set; }
}