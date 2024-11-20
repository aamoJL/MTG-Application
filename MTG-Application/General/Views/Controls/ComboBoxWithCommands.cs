using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Windows.Input;

namespace MTGApplication.General.Views.Controls;
public partial class ComboBoxWithCommands : ComboBox
{
  public static readonly DependencyProperty SelectionChangedCommandProperty =
      DependencyProperty.Register(nameof(SelectionChangedCommand), typeof(ICommand), typeof(ComboBoxWithCommands), new PropertyMetadata(default(ICommand)));

  public ComboBoxWithCommands() : base()
    => SelectionChanged += ComboBox_SelectionChanged;

  public ICommand SelectionChangedCommand
  {
    get => (ICommand)GetValue(SelectionChangedCommandProperty);
    set => SetValue(SelectionChangedCommandProperty, value);
  }

  private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.FirstOrDefault() is object item)
      if (SelectionChangedCommand?.CanExecute(item) is true)
        SelectionChangedCommand.Execute(item);
  }
}
