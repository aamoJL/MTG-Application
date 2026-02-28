using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace MTGApplication.General.Views.DragAndDrop;

public class DragAndDropHelpers
{
  /// <summary>
  /// Returns drag UI from the given <paramref name="uiElement"/>
  /// </summary>
  public static async Task<SoftwareBitmap> GetDragUI(UIElement uiElement)
  {
    var renderTargetBitmap = new RenderTargetBitmap();
    await renderTargetBitmap.RenderAsync(uiElement);

    return SoftwareBitmap.CreateCopyFromBuffer(
      await renderTargetBitmap.GetPixelsAsync(),
      BitmapPixelFormat.Bgra8,
      renderTargetBitmap.PixelWidth,
      renderTargetBitmap.PixelHeight,
      BitmapAlphaMode.Premultiplied);
  }
}