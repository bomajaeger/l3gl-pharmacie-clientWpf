using System.Windows;
using System.Windows.Controls;
using PharmacieApp.ViewModels;

namespace PharmacieApp.Views;

public partial class FournisseursView : UserControl
{
    private readonly FournisseurViewModel _vm = new();

    public FournisseursView()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    // Le chargement initial se fait à l'affichage, pas dans le constructeur :
    // un appel réseau dans un constructeur bloquerait la création de la vue.
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        _vm.ChargerCommand.Execute(null);
    }
}