using System.Windows;
using System.Windows.Controls;
using PharmacieApp.ViewModels;

namespace PharmacieApp.Views;

public partial class VentesView : UserControl
{
    private readonly VenteViewModel _vm = new();

    public VentesView()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Rechargement à chaque affichage de l'onglet : les données
        // peuvent avoir changé depuis une autre vue (une vente
        // décrémente le stock, par exemple).
        _vm.ChargerCommand.Execute(null);
    }
}