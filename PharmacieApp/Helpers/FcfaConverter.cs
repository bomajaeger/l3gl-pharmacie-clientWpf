using System;
using System.Globalization;
using System.Windows.Data;

namespace PharmacieApp.Helpers;

/// <summary>
/// Affiche un montant entier au format FCFA : 1500 -> "1 500 FCFA".
/// La culture est forcée pour que le séparateur de milliers reste
/// identique quelle que soit la configuration régionale du poste.
/// </summary>
public class FcfaConverter : IValueConverter
{
    private static readonly CultureInfo Culture = CreerCulture();

    private static CultureInfo CreerCulture()
    {
        var c = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        c.NumberFormat.NumberGroupSeparator = " ";   // espace insécable
        c.NumberFormat.NumberDecimalDigits = 0;
        return c;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int montant)
            return montant.ToString("N0", Culture) + " FCFA";

        return string.Empty;
    }

    // Affichage seul : le binding n'est jamais bidirectionnel.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}