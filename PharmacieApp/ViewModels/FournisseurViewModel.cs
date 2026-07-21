using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PharmacieApp.Commands;
using PharmacieApp.Models;
using PharmacieApp.Services;

namespace PharmacieApp.ViewModels;

public class FournisseurViewModel : BaseViewModel
{
    public ObservableCollection<Fournisseur> Fournisseurs { get; } = new();

    /// <summary>
    /// Ligne sélectionnée dans la grille. null = mode ajout.
    /// </summary>
    private Fournisseur? _selection;
    public Fournisseur? Selection
    {
        get => _selection;
        set
        {
            if (SetProperty(ref _selection, value) && value is not null)
                RemplirFormulaire(value);

            OnPropertyChanged(nameof(EnModification));
            OnPropertyChanged(nameof(TitreFormulaire));
        }
    }

    public bool EnModification => Selection is not null;
    public string TitreFormulaire => EnModification ? "Modifier le fournisseur" : "Nouveau fournisseur";

    // ===== Champs du formulaire =====

    private string _nom = string.Empty;
    public string Nom
    {
        get => _nom;
        set => SetProperty(ref _nom, value);
    }

    private string _contact = string.Empty;
    public string Contact
    {
        get => _contact;
        set => SetProperty(ref _contact, value);
    }

    private string _telephone = string.Empty;
    public string Telephone
    {
        get => _telephone;
        set => SetProperty(ref _telephone, value);
    }

    private string _adresse = string.Empty;
    public string Adresse
    {
        get => _adresse;
        set => SetProperty(ref _adresse, value);
    }

    // ===== Commandes =====

    public ICommand ChargerCommand { get; }
    public ICommand AjouterCommand { get; }
    public ICommand ModifierCommand { get; }
    public ICommand SupprimerCommand { get; }
    public ICommand EffacerCommand { get; }

    public FournisseurViewModel()
    {
        ChargerCommand = new AsyncRelayCommand(_ => ChargerAsync());

        AjouterCommand = new AsyncRelayCommand(
            _ => AjouterAsync(),
            _ => !EnModification && !string.IsNullOrWhiteSpace(Nom));

        ModifierCommand = new AsyncRelayCommand(
            _ => ModifierAsync(),
            _ => EnModification && !string.IsNullOrWhiteSpace(Nom));

        SupprimerCommand = new AsyncRelayCommand(
            _ => SupprimerAsync(),
            _ => EnModification);

        EffacerCommand = new RelayCommand(_ => Effacer());
    }

    // ===== Opérations =====

    private async Task ChargerAsync()
    {
        MessageErreur = null;
        EstOccupe = true;

        try
        {
            var liste = await ApiService.GetAsync<Fournisseur[]>("fournisseurs");

            Fournisseurs.Clear();
            foreach (var f in liste)
                Fournisseurs.Add(f);
        }
        catch (ApiException ex)
        {
            MessageErreur = ex.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    private async Task AjouterAsync()
    {
        MessageErreur = null;
        EstOccupe = true;

        try
        {
            await ApiService.PostAsync<Fournisseur>("fournisseurs", CorpsFormulaire());
            await ChargerAsync();
            Effacer();
        }
        catch (ApiException ex)
        {
            MessageErreur = ex.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    private async Task ModifierAsync()
    {
        if (Selection is null) return;

        MessageErreur = null;
        EstOccupe = true;
        var id = Selection.Id;

        try
        {
            await ApiService.PutAsync<Fournisseur>($"fournisseurs/{id}", CorpsFormulaire());
            await ChargerAsync();

            // On resélectionne la ligne modifiée pour que l'utilisateur
            // voie le résultat sans la rechercher.
            Selection = Fournisseurs.FirstOrDefault(f => f.Id == id);
        }
        catch (ApiException ex)
        {
            MessageErreur = ex.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    private async Task SupprimerAsync()
    {
        if (Selection is null) return;

        var confirmation = MessageBox.Show(
            $"Supprimer le fournisseur « {Selection.Nom} » ?\n\n" +
            "Ses médicaments seront conservés, mais sans fournisseur associé.",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirmation != MessageBoxResult.Yes) return;

        MessageErreur = null;
        EstOccupe = true;

        try
        {
            await ApiService.DeleteAsync<object>($"fournisseurs/{Selection.Id}");
            await ChargerAsync();
            Effacer();
        }
        catch (ApiException ex)
        {
            MessageErreur = ex.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    private void Effacer()
    {
        Selection = null;
        Nom = Contact = Telephone = Adresse = string.Empty;
        MessageErreur = null;
    }

    private void RemplirFormulaire(Fournisseur f)
    {
        Nom = f.Nom;
        Contact = f.Contact ?? string.Empty;
        Telephone = f.Telephone ?? string.Empty;
        Adresse = f.Adresse ?? string.Empty;
    }

    /// <summary>
    /// Objet anonyme sérialisé en JSON pour l'API.
    /// </summary>
    private object CorpsFormulaire() => new
    {
        nom = Nom.Trim(),
        contact = Contact.Trim(),
        telephone = Telephone.Trim(),
        adresse = Adresse.Trim()
    };
}