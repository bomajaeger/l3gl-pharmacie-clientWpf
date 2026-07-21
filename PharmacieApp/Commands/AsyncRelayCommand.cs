using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PharmacieApp.Commands;

/// <summary>
/// Version asynchrone : indispensable pour les appels réseau.
/// Un RelayCommand classique bloquerait le thread UI pendant toute
/// la durée de la requête, et la fenêtre paraîtrait figée.
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Predicate<object?>? _canExecute;
    private bool _enCours;

    public AsyncRelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    // Bloque les clics multiples pendant qu'une requête est en vol
    public bool CanExecute(object? parameter)
        => !_enCours && (_canExecute?.Invoke(parameter) ?? true);

    public async void Execute(object? parameter)
    {
        _enCours = true;
        CommandManager.InvalidateRequerySuggested();

        try
        {
            await _execute(parameter);
        }
        finally
        {
            _enCours = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}