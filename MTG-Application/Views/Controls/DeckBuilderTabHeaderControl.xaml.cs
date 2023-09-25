using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Views.Controls;

public sealed partial class DeckBuilderTabHeaderControl : UserControl
{
  public DeckBuilderTabHeaderControl() => InitializeComponent();

  public string Text
  {
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public bool HasUnsavedChanges
  {
    get => (bool)GetValue(HasUnsavedChangesProperty);
    set => SetValue(HasUnsavedChangesProperty, value);
  }

  public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register(nameof(Text), typeof(string), typeof(DeckBuilderTabHeaderControl), new PropertyMetadata("New Deck"));

  public static readonly DependencyProperty HasUnsavedChangesProperty =
      DependencyProperty.Register(nameof(HasUnsavedChanges), typeof(bool), typeof(DeckBuilderTabHeaderControl), new PropertyMetadata(false));
}
