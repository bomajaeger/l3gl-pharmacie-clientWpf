using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using PharmacieApp.Commands;
using PharmacieApp.Models;
using PharmacieApp.Services;

namespace PharmacieApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _nomUtilisateur = string.Empty;
    public string NomUtilisateur
    {
        get => _nomUtilisateur;
        set => SetProperty(ref _nomUtilisateur, value);
    }

    /// <summary>
    /// Signale à la vue que la connexion a réussi.
    /// Le ViewModel ne connaît aucune fenêtre : c'est le code-behind
    /// qui décide quoi faire (ouvrir MainWindow, se fermer).
    /// </summary>
    public event Action? ConnexionReussie;

    public ICommand ConnexionCommand { get; }

    public LoginViewModel()
    {
        ConnexionCommand = new AsyncRelayCommand(ConnecterAsync);
    }

    private async Task ConnecterAsync(object? parametre)
    {
        MessageErreur = null;

        // Le mot de passe arrive via CommandParameter : WPF interdit
        // le binding sur PasswordBox.Password pour éviter qu'un mot de
        // passe traîne en clair dans une propriété liée.
        var motDePasse = (parametre as PasswordBox)?.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(NomUtilisateur) || motDePasse.Length == 0)
        {
            MessageErreur = "Veuillez saisir votre identifiant et votre mot de passe.";
            return;
        }

        EstOccupe = true;

        try
        {
            var resultat = await ApiService.PostAsync<LoginResult>("login", new
            {
                nom_utilisateur = NomUtilisateur.Trim(),
                mot_de_passe = motDePasse
            });

            AuthService.Connecter(resultat);
            ConnexionReussie?.Invoke();
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
}