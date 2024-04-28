using MTGApplication.Services.IOService;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplicationTests.Services;

public class TestIO
{
  public class TestClipboard : IOService.ClipboardService, IDisposable
  {
    public object? Content { get; set; }

    public override void Copy(string text)
    {
      Content = text;
      base.Copy(text);
    }

    protected override void SetClipboardContent(DataPackage dataPackage) { }

    public void Dispose()
    {
      Content = null;
      GC.SuppressFinalize(this);
    }
  }
}
