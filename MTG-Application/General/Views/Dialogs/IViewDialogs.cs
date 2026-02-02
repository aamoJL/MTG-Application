using Microsoft.UI.Xaml;

namespace MTGApplication.General.Views.Dialogs;

// TODO: remove?
public interface IViewDialogs<T>
{
  public static abstract void RegisterConfirmDialogs(T confirmers, XamlRoot root);
}