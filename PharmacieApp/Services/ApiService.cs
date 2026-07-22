using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PharmacieApp.Helpers;
using PharmacieApp.Models;

namespace PharmacieApp.Services;

/// <summary>Erreur renvoyée par l'API, avec son code HTTP.</summary>
public class ApiException : Exception
{
    public HttpStatusCode CodeHttp { get; }
    public bool EstNonAutorise => CodeHttp == HttpStatusCode.Unauthorized;

    public ApiException(string message, HttpStatusCode code) : base(message)
        => CodeHttp = code;
}

/// <summary>
/// Point d'accès unique à l'API PHP. Aucun ViewModel ne doit
/// instancier de HttpClient lui-même.
/// </summary>
public static class ApiService
{
    private const string BaseUrl = "http://localhost/apiPharma/";

    // static readonly : un seul HttpClient pour toute l'application.
    // En créer un par appel épuise les sockets disponibles (le système
    // les garde en TIME_WAIT plusieurs minutes après fermeture).
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri(BaseUrl),
        Timeout = TimeSpan.FromSeconds(15)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new MySqlDateTimeConverter() }
    };

    /// <summary>
    /// Déclenché quand l'API rejette le token (expiré ou invalide).
    /// L'application doit alors renvoyer l'utilisateur vers le login.
    /// </summary>
    public static event Action? SessionExpiree;

    // --- Méthodes publiques -----------------------------------------

    public static Task<T> GetAsync<T>(string route)
        => EnvoyerAsync<T>(HttpMethod.Get, route);

    public static Task<T> PostAsync<T>(string route, object? corps = null)
        => EnvoyerAsync<T>(HttpMethod.Post, route, corps);

    public static Task<T> PutAsync<T>(string route, object? corps = null)
        => EnvoyerAsync<T>(HttpMethod.Put, route, corps);

    public static Task<T> DeleteAsync<T>(string route)
        => EnvoyerAsync<T>(HttpMethod.Delete, route);

    // --- Cœur de la mécanique ---------------------------------------

    private static async Task<T> EnvoyerAsync<T>(HttpMethod methode,
                                                 string route,
                                                 object? corps = null)
    {
        using var requete = new HttpRequestMessage(methode, route);

        // Le token est ajouté automatiquement : aucun ViewModel
        // n'a à s'en préoccuper.
        if (AuthService.EstConnecte)
        {
            requete.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", AuthService.Token);
        }

        if (corps is not null)
        {
            var json = JsonSerializer.Serialize(corps, JsonOptions);
            requete.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage reponse;

        try
        {
            reponse = await Client.SendAsync(requete);
        }
        catch (TaskCanceledException)
        {
            Logger.Erreur($"Timeout sur {methode} {route}");
            throw new ApiException(
                "Le serveur ne répond pas. Vérifiez qu'Apache est démarré.",
                HttpStatusCode.RequestTimeout);
        }
        catch (HttpRequestException ex)
        {
            Logger.Erreur($"API injoignable sur {methode} {route}", ex);
            throw new ApiException(
                "Impossible de joindre l'API. Vérifiez que XAMPP est lancé.",
                HttpStatusCode.ServiceUnavailable);
        }

        var contenu = await reponse.Content.ReadAsStringAsync();

        ApiResponse<T>? resultat;

        try
        {
            resultat = JsonSerializer.Deserialize<ApiResponse<T>>(contenu, JsonOptions);
        }
        catch (JsonException ex)
        {
            // L'API a renvoyé du HTML (erreur PHP non interceptée)
            Logger.Erreur($"Réponse illisible sur {methode} {route}", ex);
            throw new ApiException(
                "Réponse illisible du serveur : " + Tronquer(contenu),
                reponse.StatusCode);
        }

        if (resultat is null || !resultat.Success)
        {
            var erreur = new ApiException(
                resultat?.Message ?? "Erreur inconnue.",
                reponse.StatusCode);

            Logger.Erreur($"Erreur API {(int)reponse.StatusCode} sur {methode} {route} : {erreur.Message}");

            if (erreur.EstNonAutorise && AuthService.EstConnecte && route != "login")
            {
                AuthService.Deconnecter();
                SessionExpiree?.Invoke();
            }

            throw erreur;
        }

        return resultat.Data!;
    }

    private static string Tronquer(string texte)
        => texte.Length <= 200 ? texte : texte[..200] + "...";
}