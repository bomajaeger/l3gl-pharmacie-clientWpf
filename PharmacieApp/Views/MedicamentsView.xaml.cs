using System.Windows;
using System.Windows.Controls;
using PharmacieApp.ViewModels;

namespace PharmacieApp.Views;

public partial class MedicamentsView : UserControl
{
    private readonly MedicamentViewModel _vm = new();

    public MedicamentsView()
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