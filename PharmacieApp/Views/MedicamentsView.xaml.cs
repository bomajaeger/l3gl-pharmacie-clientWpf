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
        if (_vm.Medicaments.Count == 0)
            _vm.ChargerCommand.Execute(null);
    }
}