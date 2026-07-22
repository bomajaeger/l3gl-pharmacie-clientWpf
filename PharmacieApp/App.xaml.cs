using System.Windows;
using System.Windows.Threading;
using PharmacieApp.Helpers;

namespace PharmacieApp;

public partial class App : Application
{
    public App()
    {
        // Attrape toute exception non gérée dans le thread UI,
        // la journalise, et empêche le crash brutal de l'application.
        DispatcherUnhandledException += SurExceptionNonGeree;
    }

    private void SurExceptionNonGeree(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Logger.Erreur("Exception non gérée", e.Exception);

        MessageBox.Show(
            "Une erreur inattendue est survenue. L'application va continuer, " +
            "mais l'incident a été enregistré.",
            "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

        // true = l'erreur est considérée traitée, l'application ne se ferme pas
        e.Handled = true;
    }
}