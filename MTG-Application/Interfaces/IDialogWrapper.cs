using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
  /// <summary>
  /// Interface that wraps <see cref="ContentDialog"/> inside of it.
  /// Can be used to unit test dialogs without calling UI elements
  /// </summary>
  public interface OLDIDialogWrapper
  {
    public ContentDialog Dialog { get; set; }

    /// <summary>
    /// Shows the dialog
    /// </summary>
    public Task<ContentDialogResult> ShowAsync();
  }
}
