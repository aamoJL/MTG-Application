using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardSearch.ViewModels.SearchCard;
using MTGApplication.General.Models;
using System.ComponentModel;
using System.Windows.Input;

namespace MTGApplication.Features.CardSearch.Views.Controls.CardView;

public partial class CardSearchCardViewBase : UserControl, INotifyPropertyChanged
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(CardSearchMTGCardViewModel), typeof(CardSearchCardViewBase), new PropertyMetadata(null, OnModelPropertyChangedCallback));

  public static readonly DependencyProperty ShowPrintsCommandProperty =
      DependencyProperty.Register(nameof(ShowPrintsCommand), typeof(ICommand), typeof(CardSearchCardViewBase), new PropertyMetadata(null));

  public static readonly DependencyProperty OpenAPIWebsiteCommandProperty =
      DependencyProperty.Register(nameof(OpenAPIWebsiteCommand), typeof(ICommand), typeof(CardSearchCardViewBase), new PropertyMetadata(null));

  public static readonly DependencyProperty OpenCardmarketWebsiteCommandProperty =
      DependencyProperty.Register(nameof(OpenCardmarketWebsiteCommand), typeof(ICommand), typeof(CardSearchCardViewBase), new PropertyMetadata(null));

  public CardSearchMTGCardViewModel Model
  {
    get => (CardSearchMTGCardViewModel)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }
  public ICommand ShowPrintsCommand
  {
    get => (ICommand)GetValue(ShowPrintsCommandProperty);
    set => SetValue(ShowPrintsCommandProperty, value);
  }
  public ICommand OpenAPIWebsiteCommand
  {
    get => (ICommand)GetValue(OpenAPIWebsiteCommandProperty);
    set => SetValue(OpenAPIWebsiteCommandProperty, value);
  }
  public ICommand OpenCardmarketWebsiteCommand
  {
    get => (ICommand)GetValue(OpenCardmarketWebsiteCommandProperty);
    set => SetValue(OpenCardmarketWebsiteCommandProperty, value);
  }

  public string SelectedFaceUri
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(SelectedFaceUri)));
      }
    }
  } = "";

  public event PropertyChangedEventHandler? PropertyChanged;

  [RelayCommand(CanExecute = nameof(CanExecuteSwitchFaceImage))]
  private void SwitchFaceImage()
  {
    if (!CanExecuteSwitchFaceImage()) return;

    SelectedFaceUri = (Model?.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri
      : Model?.Info.FrontFace.ImageUri)
      ?? string.Empty;
  }

  private void AppSettings_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    // Requested theme needs to be changed to the selected theme here, because flyouts will not change theme if
    // the requested theme is Default.
    // And ActualThemeChanged event invokes only once when changing the theme
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  private bool CanExecuteSwitchFaceImage() => !string.IsNullOrEmpty(Model?.Info.BackFace?.ImageUri);

  private static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is not CardSearchCardViewBase view) return;
    if (!e.Property.Equals(ModelProperty)) return;

    if (e.OldValue is CardSearchMTGCardViewModel oldValue)
      view.Model_Changing(oldValue);

    if (e.NewValue is CardSearchMTGCardViewModel newValue)
      view.Model_Changed(newValue);
  }

  private void Model_Changing(CardSearchMTGCardViewModel oldValue)
    => oldValue.PropertyChanged -= Model_PropertyChanged;

  private void Model_Changed(CardSearchMTGCardViewModel newValue)
  {
    SelectedFaceUri = newValue?.Info.FrontFace.ImageUri ?? string.Empty;

    if (newValue != null)
      newValue.PropertyChanged += Model_PropertyChanged;

    SwitchFaceImageCommand.NotifyCanExecuteChanged();
  }

  private void Model_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(MTGCard.Info))
      SelectedFaceUri = Model?.Info.FrontFace.ImageUri ?? string.Empty;
  }
}