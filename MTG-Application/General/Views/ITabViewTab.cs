using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MTGApplication.General.Views;
/// <summary>
/// Interface for <see cref="TabView"/> tabs
/// </summary>
public interface ITabViewTab : INotifyPropertyChanged
{
  public string Header { get; }

  public Task<bool> TabCloseRequested();
}
