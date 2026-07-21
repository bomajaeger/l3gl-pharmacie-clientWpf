using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PharmacieApp.Commands;
using PharmacieApp.Models;
using PharmacieApp.Services;

namespace PharmacieApp.ViewModels;

public class VenteViewModel : BaseViewModel
{
    public ObservableCollection<Vente> Ventes { get; } = new();
    public ObservableCollection<Medicament> Medicaments { get; } = new();

    // ===== Formulaire de vente =====

    private Medicament? _medicamentSelectionne;
    public Medicament? MedicamentSelectionne
    {
        get => _medicamentSelectionne;
        set
        {
            if (SetProperty(ref _medicamentSelectionne, value))
                RafraichirCalculs();
        }
    }

    private string _quantite = "1";
    public string Quantite
    {
        get => _quantite;
        set
        {
            if (SetProperty(ref _quantite, value))
                RafraichirCalculs();
        }
    }

    // ===== Valeurs calculées, affichage seul =====

    /// <summary>Prix unitaire du médicament choisi, 0 si aucun.</summary>
    public int PrixUnitaire => MedicamentSelectionne?.Prix ?? 0;

    /// <summary>Stock disponible du médicament choisi.</summary>
    public int StockDisponible => MedicamentSelectionne?.QuantiteStock ?? 0;

    /// <summary>
    /// Total indicatif. Le montant qui fait foi est celui calculé
    /// par l'API : le client n'envoie jamais de prix.
    /// </summary>
    public int Total =>
        int.TryParse(Quantite, out var q) && q > 0 ? PrixUnitaire * q : 0;

    /// <summary>Prévient avant l'envoi que la quantité dépasse le stock.</summary>
    public bool StockInsuffisant =>
        MedicamentSelectionne is not null
        && int.TryParse(Quantite, out var q)
        && q > StockDisponible;

    private void RafraichirCalculs()
    {
        OnPropertyChanged(nameof(PrixUnitaire));
        OnPropertyChanged(nameof(StockDisponible));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(StockInsuffisant));
    }

    // ===== Statistiques =====

    private int _chiffreAffaires;
    public int ChiffreAffaires
    {
        get => _chiffreAffaires;
        set => SetProperty(ref _chiffreAffaires, value);
    }

    private int _nombreVentes;
    public int NombreVentes
    {
        get => _nombreVentes;
        set => SetProperty(ref _nombreVentes, value);
    }

    // ===== Commandes =====

    public ICommand ChargerCommand { get; }
    public ICommand EnregistrerCommand { get; }
    public ICommand EffacerCommand { get; }

    public VenteViewModel()
    {
        ChargerCommand = new AsyncRelayCommand(_ => ChargerToutAsync());

        EnregistrerCommand = new AsyncRelayCommand(
            _ => EnregistrerAsync(),
            _ => MedicamentSelectionne is not null
                 && int.TryParse(Quantite, out var q) && q > 0
                 && !StockInsuffisant);

        EffacerCommand = new RelayCommand(_ => Effacer());
    }

    private async Task ChargerToutAsync()
    {
        MessageErreur = null;
        EstOccupe = true;

        try
        {
            var tacheVentes = ApiService.GetAsync<Vente[]>("ventes");
            var tacheMedicaments = ApiService.GetAsync<Medicament[]>("medicaments");
            var tacheStats = ApiService.GetAsync<StatistiquesVentes>("ventes/stats");

            await Task.WhenAll(tacheVentes, tacheMedicaments, tacheStats);

            Ventes.Clear();
            foreach (var v in tacheVentes.Result)
                Ventes.Add(v);

            // On mémorise l'id sélectionné pour le restaurer après rechargement
            var idPrecedent = MedicamentSelectionne?.Id;

            Medicaments.Clear();
            foreach (var m in tacheMedicaments.Result)
                Medicaments.Add(m);

            if (idPrecedent is not null)
            {
                MedicamentSelectionne = null;
                foreach (var m in Medicaments)
                {
                    if (m.Id == idPrecedent) { MedicamentSelectionne = m; break; }
                }
            }

            ChiffreAffaires = tacheStats.Result.ChiffreAffaires;
            NombreVentes = tacheStats.Result.NombreVentes;
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

    private async Task EnregistrerAsync()
    {
        if (MedicamentSelectionne is null) return;

        MessageErreur = null;
        EstOccupe = true;

        try
        {
            // Seuls l'id et la quantité partent : le prix et le total
            // sont déterminés par le serveur.
            await ApiService.PostAsync<Vente>("ventes", new
            {
                id_medicament = MedicamentSelectionne.Id,
                quantite_vendue = int.Parse(Quantite)
            });

            await ChargerToutAsync();   // le stock a changé
            Quantite = "1";
        }
        catch (ApiException ex)
        {
            // Cas typique : 400 "Stock insuffisant pour ..."
            MessageErreur = ex.Message;
        }
        finally
        {
            EstOccupe = false;
        }
    }

    private void Effacer()
    {
        MedicamentSelectionne = null;
        Quantite = "1";
        MessageErreur = null;
    }
}