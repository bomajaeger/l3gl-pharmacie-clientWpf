using System;
using System.Windows;
using PharmacieApp.Services;
using PharmacieApp.ViewModels;

namespace PharmacieApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var vm = new MainViewModel();
        DataContext = vm;

        vm.DeconnexionDemandee += RetournerAuLogin;

        // Réagit à l'expiration du token, quelle que soit la vue active
        ApiService.SessionExpiree += SurSessionExpiree;
        Closed += (_, _) => ApiService.SessionExpiree -= SurSessionExpiree;
    }

    private void SurSessionExpiree()
    {
        // L'événement peut survenir depuis un thread d'arrière-plan :
        // toute action sur l'interface doit repasser par le thread UI.
        Dispatcher.Invoke(() =>
        {
            MessageBox.Show(
                "Votre session a expiré. Veuillez vous reconnecter.",
                "Session expirée", MessageBoxButton.OK, MessageBoxImage.Information);

            RetournerAuLogin();
        });
    }

    private void RetournerAuLogin()
    {
        var login = new LoginWindow();
        Application.Current.MainWindow = login;
        login.Show();
        Close();
    }

    private void MenuAPropos_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Gestion de Pharmacie\n\n" +
            "Application WPF (MVVM) connectée à une API PHP et une base MySQL.\n" +
            "Projet d'examen — L3 Génie Logiciel.",
            "À propos", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}