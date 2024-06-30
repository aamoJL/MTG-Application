using Microsoft.UI.Xaml;

namespace MTGApplication.General.Views.Dialogs;

public interface IViewDialogs<T>
{
  public static abstract void RegisterConfirmDialogs(T confirmers, XamlRoot root);
}