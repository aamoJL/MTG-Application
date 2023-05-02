using Microsoft.UI.Xaml.Controls;
using static MTGApplication.Services.DialogService;

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

  public class TestDialogWrapper : DialogWrapper
  {
    public ContentDialogResult Result { get; set; }

    public TestDialogWrapper(ContentDialogResult result) => Result = result;

    public override async Task<ContentDialogResult> ShowAsync(Dialog dialog, bool force = false) => await Task.Run(() => Result);
  }
}
