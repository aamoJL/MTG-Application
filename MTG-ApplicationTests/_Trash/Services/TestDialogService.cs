using Microsoft.UI.Xaml.Controls;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplicationTests.Services;

public static class TestDialogService
{
  public class TestDialogResult
  {
    public ContentDialogResult Result { get; set; } = ContentDialogResult.Primary;
  }

  public class TestDialogResult<T>
  {
    public ContentDialogResult Result { get; set; } = ContentDialogResult.Primary;
    public T? Values { get; set; }
  }

  public class TestDialogWrapper(ContentDialogResult result) : DialogWrapper(null)
  {
    public ContentDialogResult Result { get; set; } = result;

    public override async Task<ContentDialogResult> ShowAsync(Dialog dialog, bool force = false) => await Task.Run(() => Result);
  }
}
