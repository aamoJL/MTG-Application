using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views;
using MTGApplication.Interfaces;

namespace MTGApplication.Views.Controls;

public sealed partial class DeckBuilderTabHeaderControl : UserControl
{
  public string DefaultHeaderText { get; } = "New Deck";
  public string Text
  {
    get => (string)GetValue(TextProperty);
    set
    {
      if (string.IsNullOrEmpty(value)) value = DefaultHeaderText;
      SetValue(TextProperty, value);
    }
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

  public DeckBuilderTabHeaderControl(ITabViewTab tab)
  {
    InitializeComponent();

    Text = tab.Header;
    tab.PropertyChanged += Tab_PropertyChanged;
  }

  private void Tab_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(ITabViewTab.Header):
        Text = (sender as ITabViewTab)?.Header ?? string.Empty; break;
      case nameof(ISavable.HasUnsavedChanges):
        HasUnsavedChanges = (sender as ISavable)?.HasUnsavedChanges ?? false; break;
      default: break;
    }
  }
}
