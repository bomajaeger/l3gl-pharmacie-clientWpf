using System;
using System.Windows.Input;

namespace PharmacieApp.Commands;

/// <summary>
/// Implémentation générique d'ICommand : évite d'écrire une classe
/// par bouton. On lui passe l'action à exécuter et, éventuellement,
/// une condition d'activation.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    /// <summary>
    /// CommandManager réévalue automatiquement CanExecute à chaque
    /// interaction utilisateur : les boutons se grisent tout seuls.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}