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
        if (_vm.Ventes.Count == 0)
            _vm.ChargerCommand.Execute(null);
    }
}