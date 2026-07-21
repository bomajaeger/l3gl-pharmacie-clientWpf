using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PharmacieApp.ViewModels;

/// <summary>
/// Base commune à tous les ViewModels : notification des changements
/// de propriété vers les bindings XAML.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// [CallerMemberName] récupère automatiquement le nom de la
    /// propriété appelante : pas de chaîne de caractères à maintenir.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? nom = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));

    /// <summary>
    /// Affecte une valeur et notifie l'UI, uniquement si elle a changé.
    /// Évite les boucles de notification inutiles.
    /// </summary>
    protected bool SetProperty<T>(ref T champ, T valeur, [CallerMemberName] string? nom = null)
    {
        if (EqualityComparer<T>.Default.Equals(champ, valeur))
            return false;

        champ = valeur;
        OnPropertyChanged(nom);
        return true;
    }

    // --- État partagé par tous les écrans ---------------------------

    private bool _estOccupe;
    public bool EstOccupe
    {
        get => _estOccupe;
        set => SetProperty(ref _estOccupe, value);
    }

    private string? _messageErreur;
    public string? MessageErreur
    {
        get => _messageErreur;
        set => SetProperty(ref _messageErreur, value);
    }
}