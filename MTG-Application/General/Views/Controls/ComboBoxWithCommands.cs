using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace MTGApplication.General.Views.Controls;

public partial class ComboBoxWithCommands : ComboBox
{
  public static readonly DependencyProperty SelectionChangedCommandProperty =
      DependencyProperty.Register(nameof(SelectionChangedCommand), typeof(IAsyncRelayCommand), typeof(ComboBoxWithCommands), new PropertyMetadata(null));

  public ComboBoxWithCommands() : base()
    => SelectionChanged += ComboBox_SelectionChanged;

  public IAsyncRelayCommand? SelectionChangedCommand
  {
    get => (IAsyncRelayCommand)GetValue(SelectionChangedCommandProperty);
    set => SetValue(SelectionChangedCommandProperty, value);
  }

  private void ComboBox_SelectionChanged(object _, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.FirstOrDefault() is object item)
    {
      SelectionChangedCommand?.Cancel();
      SelectionChangedCommand?.Execute(item);
    }
  }
}
