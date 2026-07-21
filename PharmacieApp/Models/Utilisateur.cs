using System;
using System.Text.Json.Serialization;

namespace PharmacieApp.Models;

public class Utilisateur
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom_utilisateur")]
    public string NomUtilisateur { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}

public class LoginResult
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expiration")]
    public DateTime Expiration { get; set; }

    [JsonPropertyName("utilisateur")]
    public Utilisateur Utilisateur { get; set; } = new();
}