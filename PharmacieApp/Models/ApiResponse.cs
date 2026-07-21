using System.Text.Json.Serialization;

namespace PharmacieApp.Models;

/// <summary>
/// Reflète le format imposé par Response.php : { success, data } ou
/// { success, message }.
/// </summary>
public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}