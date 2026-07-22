using System;
using System.IO;

namespace PharmacieApp.Helpers;

/// <summary>
/// Journalise les erreurs du client dans un fichier local.
/// Emplacement : dossier de l'application, sous-dossier logs/.
/// L'écriture ne doit jamais faire planter l'application : toute
/// erreur de journalisation est silencieusement ignorée.
/// </summary>
public static class Logger
{
    private static readonly string Dossier =
        Path.Combine(AppContext.BaseDirectory, "logs");

    private static readonly object Verrou = new();

    public static void Erreur(string message, Exception? ex = null)
    {
        try
        {
            Directory.CreateDirectory(Dossier);

            var ligne = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            if (ex is not null)
                ligne += $" | {ex.GetType().Name}: {ex.Message}";

            // Verrou : deux appels simultanés n'écrivent pas en même temps
            lock (Verrou)
            {
                File.AppendAllText(
                    Path.Combine(Dossier, "erreurs.log"),
                    ligne + Environment.NewLine);
            }
        }
        catch
        {
            // Journaliser ne doit jamais provoquer d'erreur visible.
        }
    }
}