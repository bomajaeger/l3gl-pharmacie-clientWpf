using System.Windows;
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