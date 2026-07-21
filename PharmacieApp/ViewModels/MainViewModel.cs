using System;
using System.Windows.Input;
using PharmacieApp.Commands;
using PharmacieApp.Services;

namespace PharmacieApp.ViewModels;

/// <summary>
/// Pilote la fenêtre principale : barre de statut et actions du menu.
/// Les trois onglets ont chacun leur propre ViewModel.
/// </summary>
public class MainViewModel : BaseViewModel
{
    public string NomUtilisateur =>
        AuthService.UtilisateurConnecte?.NomUtilisateur ?? "inconnu";

    private string _statut = "Prêt";
    public string Statut
    {
        get => _statut;
        set => SetProperty(ref _statut, value);
    }

    /// <summary>Demande à la vue de revenir à l'écran de connexion.</summary>
    public event Action? DeconnexionDemandee;

    public ICommand DeconnexionCommand { get; }
    public ICommand QuitterCommand { get; }

    public MainViewModel()
    {
        DeconnexionCommand = new AsyncRelayCommand(async _ =>
        {
            try
            {
                // On invalide le token côté serveur ; si l'appel échoue
                // (API éteinte), on déconnecte quand même localement.
                await ApiService.PostAsync<object>("logout");
            }
            catch (ApiException) { }

            AuthService.Deconnecter();
            DeconnexionDemandee?.Invoke();
        });

        QuitterCommand = new RelayCommand(_ =>
            System.Windows.Application.Current.Shutdown());
    }
}