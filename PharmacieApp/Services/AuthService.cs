using PharmacieApp.Models;

namespace PharmacieApp.Services;

/// <summary>
/// Conserve le token en mémoire pour la durée de la session.
/// Volontairement non persisté sur disque : à la fermeture de
/// l'application, la session disparaît.
/// </summary>
public static class AuthService
{
    public static string? Token { get; private set; }
    public static Utilisateur? UtilisateurConnecte { get; private set; }

    public static bool EstConnecte => !string.IsNullOrEmpty(Token);

    public static void Connecter(LoginResult resultat)
    {
        Token = resultat.Token;
        UtilisateurConnecte = resultat.Utilisateur;
    }

    public static void Deconnecter()
    {
        Token = null;
        UtilisateurConnecte = null;
    }
}