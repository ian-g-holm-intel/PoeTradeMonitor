using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PoeTradeMonitor.GUI.ViewModels;

namespace PoeTradeMonitor.GUI.Views;

/// <summary>
/// Interaction logic for AddSearchDialogWindow.xaml
/// </summary>
public partial class NewSearchDialogWindow : Window
{
    public NewSearchDialogWindow(NewSearchDialogWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void NumericalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox == null) return;

        // Allow decimal point only if there isn't one already
        if (e.Text == ".")
        {
            e.Handled = textBox.Text.Contains(".");
            return;
        }

        // Only allow digits
        e.Handled = !char.IsDigit(e.Text[0]);
    }

    private void NumericalTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // This handles pasted text
        TextBox textBox = sender as TextBox;
        if (textBox == null) return;

        string text = textBox.Text;
        int cursorPosition = textBox.CaretIndex;

        // If text is already valid, do nothing
        if (string.IsNullOrEmpty(text) || IsValidDecimalString(text))
            return;

        // Clean up the text to be a valid decimal string
        string cleanText = CleanDecimalString(text);

        if (cleanText != text)
        {
            textBox.Text = cleanText;
            // Place cursor at the appropriate position
            textBox.CaretIndex = Math.Min(cursorPosition, cleanText.Length);
        }
    }

    private bool IsValidDecimalString(string text)
    {
        // A valid decimal string has at most one decimal point
        // and all other characters are digits
        int decimalCount = 0;
        foreach (char c in text)
        {
            if (c == '.')
            {
                decimalCount++;
                if (decimalCount > 1)
                    return false;
            }
            else if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    private string CleanDecimalString(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        // Keep only digits and at most one decimal point
        string result = "";
        bool hasDecimal = false;

        foreach (char c in text)
        {
            if (char.IsDigit(c))
            {
                result += c;
            }
            else if (c == '.' && !hasDecimal)
            {
                result += c;
                hasDecimal = true;
            }
        }

        return result;
    }
}