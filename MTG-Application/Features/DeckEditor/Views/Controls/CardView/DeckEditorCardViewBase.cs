using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Models;
using System;
using System.ComponentModel;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardView;

public partial class DeckEditorCardViewBase : UserControl, INotifyPropertyChanged
{
  public static readonly DependencyProperty TagVisibleProperty =
      DependencyProperty.Register(nameof(TagVisible), typeof(bool), typeof(DeckEditorCardViewBase), new PropertyMetadata(true));

  public DeckEditorCardViewBase()
  {
    Loaded += (_, _) =>
    {
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
      AppConfig.LocalSettings.PropertyChanged += AppSettings_PropertyChanged;
    };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= AppSettings_PropertyChanged; };

    DataContextChanged += DeckEditorCardViewBase_DataContextChanged;
  }

  public bool? TagVisible
  {
    get => (bool)GetValue(TagVisibleProperty);
    set => SetValue(TagVisibleProperty, value);
  }
  protected DeckCardViewModel? Model => DataContext as DeckCardViewModel;

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

  protected DeckCardViewModel? _oldDataContext = null;

  [RelayCommand]
  protected virtual void DeleteClick()
  {
    if (Model?.DeleteCardCommand?.CanExecute(null) is true)
      Model.DeleteCardCommand.Execute(null);
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSwitchFaceImage))]
  private void SwitchFaceImageClick()
  {
    if (!CanExecuteSwitchFaceImage()) return;

    SelectedFaceUri = (Model?.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri
      : Model?.Info.FrontFace.ImageUri)
      ?? string.Empty;
  }

  [RelayCommand]
  protected virtual void ChangeTagClick(string? tag)
  {
    CardTag? cardTag = null;

    if (tag != null && Enum.TryParse(tag, out CardTag parsedTag))
      cardTag = parsedTag;

    if (Model?.CardTag == cardTag)
      return;

    if (Model?.ChangeTagCommand?.CanExecute(cardTag) is true)
      Model.ChangeTagCommand.Execute(cardTag);
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

  private void DeckEditorCardViewBase_DataContextChanged(FrameworkElement _, DataContextChangedEventArgs e)
  {
    if (_oldDataContext != null)
      _oldDataContext.PropertyChanged -= DataContext_PropertyChanged;

    if (e.NewValue == null) return;
    if (e.NewValue is not DeckCardViewModel vm)
      throw new InvalidOperationException($"DataContext needs to be {nameof(DeckCardViewModel)}");

    _oldDataContext = vm;

    vm.PropertyChanged += DataContext_PropertyChanged;

    SelectedFaceUri = vm.Info.FrontFace.ImageUri ?? string.Empty;

    PropertyChanged?.Invoke(this, new(nameof(Model)));
    SwitchFaceImageClickCommand.NotifyCanExecuteChanged();
  }

  private void DataContext_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Model.Info))
      SelectedFaceUri = Model?.Info.FrontFace.ImageUri ?? string.Empty;
  }

  protected virtual void NumberBox_ValueChanged(NumberBox _, NumberBoxValueChangedEventArgs e)
  {
    if (e.NewValue == Model?.Count)
      return;

    if (Model?.ChangeCountCommand?.CanExecute((int)e.NewValue) is true)
      Model.ChangeCountCommand.Execute((int)e.NewValue);
  }
}