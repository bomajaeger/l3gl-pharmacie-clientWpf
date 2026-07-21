using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PharmacieApp.Commands;
using PharmacieApp.Models;
using PharmacieApp.Services;

namespace PharmacieApp.ViewModels;

public class MedicamentViewModel : BaseViewModel
{
    public ObservableCollection<Medicament> Medicaments { get; } = new();

    /// <summary>Alimente la ComboBox du formulaire.</summary>
    public ObservableCollection<Fournisseur> Fournisseurs { get; } = new();

    private Medicament? _selection;
    public Medicament? Selection
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
    public string TitreFormulaire => EnModification ? "Modifier le médicament" : "Nouveau médicament";

    // ===== Recherche =====

    private string _recherche = string.Empty;
    public string Recherche
    {
        get => _recherche;
        set => SetProperty(ref _recherche, value);
    }

    // ===== Champs du formulaire =====
    // Les valeurs numériques sont en string : une TextBox liée à un int
    // se bloque dès que le champ est vide ou en cours de saisie.

    private string _nom = string.Empty;
    public string Nom
    {
        get => _nom;
        set => SetProperty(ref _nom, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string _prix = string.Empty;
    public string Prix
    {
        get => _prix;
        set => SetProperty(ref _prix, value);
    }

    private string _quantiteStock = "0";
    public string QuantiteStock
    {
        get => _quantiteStock;
        set => SetProperty(ref _quantiteStock, value);
    }

    private string _seuilAlerte = "10";
    public string SeuilAlerte
    {
        get => _seuilAlerte;
        set => SetProperty(ref _seuilAlerte, value);
    }

    private DateTime? _datePeremption = DateTime.Today.AddYears(1);
    public DateTime? DatePeremption
    {
        get => _datePeremption;
        set => SetProperty(ref _datePeremption, value);
    }

    private Fournisseur? _fournisseurSelectionne;
    public Fournisseur? FournisseurSelectionne
    {
        get => _fournisseurSelectionne;
        set => SetProperty(ref _fournisseurSelectionne, value);
    }

    // ===== Commandes =====

    public ICommand ChargerCommand { get; }
    public ICommand RechercherCommand { get; }
    public ICommand AjouterCommand { get; }
    public ICommand ModifierCommand { get; }
    public ICommand SupprimerCommand { get; }
    public ICommand EffacerCommand { get; }

    public MedicamentViewModel()
    {
        ChargerCommand = new AsyncRelayCommand(_ => ChargerToutAsync());
        RechercherCommand = new AsyncRelayCommand(_ => RechercherAsync());

        AjouterCommand = new AsyncRelayCommand(
            _ => AjouterAsync(),
            _ => !EnModification && FormulaireValide());

        ModifierCommand = new AsyncRelayCommand(
            _ => ModifierAsync(),
            _ => EnModification && FormulaireValide());

        SupprimerCommand = new AsyncRelayCommand(
            _ => SupprimerAsync(),
            _ => EnModification);

        EffacerCommand = new RelayCommand(_ => Effacer());
    }

    // ===== Chargement =====

    private async Task ChargerToutAsync()
    {
        MessageErreur = null;
        EstOccupe = true;

        try
        {
            // Les deux appels partent en parallèle plutôt qu'en séquence
            var tacheMedicaments = ApiService.GetAsync<Medicament[]>("medicaments");
            var tacheFournisseurs = ApiService.GetAsync<Fournisseur[]>("fournisseurs");

            await Task.WhenAll(tacheMedicaments, tacheFournisseurs);

            Medicaments.Clear();
            foreach (var m in tacheMedicaments.Result)
                Medicaments.Add(m);

            Fournisseurs.Clear();
            foreach (var f in tacheFournisseurs.Result)
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

    private async Task RechercherAsync()
    {
        MessageErreur = null;
        EstOccupe = true;

        try
        {
            // Uri.EscapeDataString protège les caractères spéciaux
            // (espaces, accents, &) dans la query string.
            var route = $"medicaments/search?q={Uri.EscapeDataString(Recherche.Trim())}";
            var liste = await ApiService.GetAsync<Medicament[]>(route);

            Medicaments.Clear();
            foreach (var m in liste)
                Medicaments.Add(m);
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

    // ===== CRUD =====

    private async Task AjouterAsync()
    {
        MessageErreur = null;
        EstOccupe = true;

        try
        {
            await ApiService.PostAsync<Medicament>("medicaments", CorpsFormulaire());
            await ChargerToutAsync();
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
            await ApiService.PutAsync<Medicament>($"medicaments/{id}", CorpsFormulaire());
            await ChargerToutAsync();
            Selection = Medicaments.FirstOrDefault(m => m.Id == id);
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
            $"Supprimer le médicament « {Selection.Nom} » ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirmation != MessageBoxResult.Yes) return;

        MessageErreur = null;
        EstOccupe = true;

        try
        {
            await ApiService.DeleteAsync<object>($"medicaments/{Selection.Id}");
            await ChargerToutAsync();
            Effacer();
        }
        catch (ApiException ex)
        {
            // L'API renvoie un 409 si des ventes sont liées :
            // le message est déjà explicite, on l'affiche tel quel.
            MessageErreur = ex.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    // ===== Formulaire =====

    private bool FormulaireValide()
        => !string.IsNullOrWhiteSpace(Nom)
           && int.TryParse(Prix, out var p) && p >= 0
           && int.TryParse(QuantiteStock, out var q) && q >= 0
           && int.TryParse(SeuilAlerte, out var s) && s >= 0
           && DatePeremption.HasValue;

    private void Effacer()
    {
        Selection = null;
        Nom = string.Empty;
        Description = string.Empty;
        Prix = string.Empty;
        QuantiteStock = "0";
        SeuilAlerte = "10";
        DatePeremption = DateTime.Today.AddYears(1);
        FournisseurSelectionne = null;
        MessageErreur = null;
    }

    private void RemplirFormulaire(Medicament m)
    {
        Nom = m.Nom;
        Description = m.Description ?? string.Empty;
        Prix = m.Prix.ToString();
        QuantiteStock = m.QuantiteStock.ToString();
        SeuilAlerte = m.SeuilAlerte.ToString();
        DatePeremption = m.DatePeremption;
        FournisseurSelectionne = Fournisseurs.FirstOrDefault(f => f.Id == m.IdFournisseur);
    }

    private object CorpsFormulaire() => new
    {
        nom = Nom.Trim(),
        description = Description.Trim(),
        prix = int.Parse(Prix),
        quantite_stock = int.Parse(QuantiteStock),
        seuil_alerte = int.Parse(SeuilAlerte),
        // Format attendu par l'API : AAAA-MM-JJ
        date_peremption = DatePeremption!.Value.ToString("yyyy-MM-dd"),
        id_fournisseur = FournisseurSelectionne?.Id
    };
}