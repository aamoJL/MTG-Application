using MTGApplication.General.Services.IOService;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplicationTests.Services;

public class TestIO
{
  public class TestClipboard : ClipboardService, IDisposable
  {
    public object? Content { get; set; }

    public override void CopyToClipboard(string text)
    {
      Content = text;
      base.CopyToClipboard(text);
    }

    protected override void SetClipboardContent(DataPackage dataPackage) { }

    public void Dispose()
    {
      Content = null;
      GC.SuppressFinalize(this);
    }
  }
}
