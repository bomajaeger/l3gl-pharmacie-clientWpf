using System.Windows;
using PharmacieApp.ViewModels;

namespace PharmacieApp.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();

        var vm = new LoginViewModel();
        DataContext = vm;

        // Wiring minimal : le ViewModel signale le succès,
        // la vue gère la navigation entre fenêtres.
        vm.ConnexionReussie += OuvrirFenetrePrincipale;

        ChampIdentifiant.Focus();
    }

    private void OuvrirFenetrePrincipale()
    {
        var principale = new MainWindow();
        Application.Current.MainWindow = principale;
        principale.Show();
        Close();
    }
}