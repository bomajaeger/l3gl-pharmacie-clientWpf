using System;
using System.Text.Json.Serialization;

namespace PharmacieApp.Models;

public class Medicament
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Prix en FCFA : entier, jamais de décimales.
    /// </summary>
    [JsonPropertyName("prix")]
    public int Prix { get; set; }

    [JsonPropertyName("quantite_stock")]
    public int QuantiteStock { get; set; }

    [JsonPropertyName("seuil_alerte")]
    public int SeuilAlerte { get; set; }

    [JsonPropertyName("date_peremption")]
    public DateTime DatePeremption { get; set; }

    [JsonPropertyName("id_fournisseur")]
    public int? IdFournisseur { get; set; }

    [JsonPropertyName("fournisseur_nom")]
    public string? FournisseurNom { get; set; }

    // Flags calculés par l'API : jamais recalculés ici.
    [JsonPropertyName("en_alerte_stock")]
    public bool EnAlerteStock { get; set; }

    [JsonPropertyName("en_alerte_peremption")]
    public bool EnAlertePeremption { get; set; }

    [JsonPropertyName("est_perime")]
    public bool EstPerime { get; set; }

    public override string ToString() => Nom;
}