using CommunityToolkit.WinUI.UI.Controls;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Pages
{
  /// <summary>
  /// Main Page
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      Notifications.OnCopied += Notifications_OnCopied;
      Notifications.OnError += Notifications_OnError;
    }

    #region //-----Notifications-----//
    private readonly int notificationDuration = 3000;

    private void Notifications_OnError(object sender, string error)
    {
      PopupAppNotification.Show(error, notificationDuration);
    }
    private void Notifications_OnCopied(object sender, string e)
    {
      PopupAppNotification.Show("Copied", notificationDuration);
    }
    #endregion
  }
}
