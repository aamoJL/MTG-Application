using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;
using MTGApplication.General.Models;
using MTGApplication.General.Views.Controls;
using MTGApplication.General.Views.DragAndDrop;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CommanderView;

public enum CommanderType { Commander, Partner };

public sealed partial class CommanderTextView : UserControl, INotifyPropertyChanged, CardDragArgs.IMoveOrigin
{
  public static readonly DependencyProperty HoverPreviewEnabledProperty =
      DependencyProperty.Register(nameof(HoverPreviewEnabled), typeof(bool), typeof(CommanderTextView), new PropertyMetadata(false));

  public static readonly DependencyProperty EdhrecButtonClickProperty =
      DependencyProperty.Register(nameof(EdhrecButtonClick), typeof(ICommand), typeof(CommanderTextView), new PropertyMetadata(default(ICommand)));

  public static readonly DependencyProperty OnDropCopyProperty =
      DependencyProperty.Register(nameof(OnDropCopy), typeof(IAsyncRelayCommand<DeckEditorMTGCard>), typeof(CommanderTextView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropImportProperty =
      DependencyProperty.Register(nameof(OnDropImport), typeof(IAsyncRelayCommand<string>), typeof(CommanderTextView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveToProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveTo), typeof(IAsyncRelayCommand<DeckEditorMTGCard>), typeof(CommanderTextView), new PropertyMetadata(default));

  public CommanderTextView()
  {
    InitializeComponent();

    Loaded += (_, _) =>
    {
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
      AppConfig.LocalSettings.PropertyChanged += AppSettings_PropertyChanged;
    };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= AppSettings_PropertyChanged; };

    DataContextChanged += View_DataContextChanged;

    DragStarting += OnDragStarting;
  }

  public bool HoverPreviewEnabled
  {
    get => (bool)GetValue(HoverPreviewEnabledProperty);
    set => SetValue(HoverPreviewEnabledProperty, value);
  }
  public CommanderType CommanderType { get; set; } = CommanderType.Commander;

  public ICommand EdhrecButtonClick
  {
    get => (ICommand)GetValue(EdhrecButtonClickProperty);
    set => SetValue(EdhrecButtonClickProperty, value);
  }
  public IAsyncRelayCommand<DeckEditorMTGCard> OnDropCopy
  {
    get => (IAsyncRelayCommand<DeckEditorMTGCard>)GetValue(OnDropCopyProperty);
    set => SetValue(OnDropCopyProperty, value);
  }
  public IAsyncRelayCommand<string> OnDropImport
  {
    get => (IAsyncRelayCommand<string>)GetValue(OnDropImportProperty);
    set => SetValue(OnDropImportProperty, value);
  }
  public IAsyncRelayCommand<DeckEditorMTGCard> OnDropBeginMoveTo
  {
    get => (IAsyncRelayCommand<DeckEditorMTGCard>)GetValue(OnDropBeginMoveToProperty);
    set => SetValue(OnDropBeginMoveToProperty, value);
  }
  public IRelayCommand<DeckEditorMTGCard>? OnDropBeginMoveFrom { get; set; }
  public IRelayCommand? OnDropExecuteMove { get; set; }

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

  private CommanderViewModelBase? Model => DataContext as CommanderViewModelBase;

  public event PropertyChangedEventHandler? PropertyChanged;

  private CommanderViewModelBase? _oldDataContext = null;

  [RelayCommand(CanExecute = nameof(CanExecuteSwitchFaceImage))]
  private void SwitchFaceImageClick()
  {
    if (!CanExecuteSwitchFaceImage()) return;

    SelectedFaceUri = (Model?.Info?.FrontFace.ImageUri == SelectedFaceUri
      ? Model?.Info?.BackFace?.ImageUri
      : Model?.Info?.FrontFace.ImageUri)
      ?? string.Empty;
  }

  private bool CanExecuteSwitchFaceImage() => !string.IsNullOrEmpty(Model?.Info?.BackFace?.ImageUri);

  private void AppSettings_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    // Requested theme needs to be changed to the selected theme here, because flyouts will not change theme if
    // the requested theme is Default.
    // And ActualThemeChanged event invokes only once when changing the theme
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  private void View_DataContextChanged(FrameworkElement _, DataContextChangedEventArgs e)
  {
    if (_oldDataContext != null)
      _oldDataContext.PropertyChanged -= DataContext_PropertyChanged;

    if (e.NewValue == null) return;
    if (e.NewValue is not CommanderViewModelBase vm)
      throw new InvalidOperationException($"DataContext needs to be {nameof(CommanderViewModelBase)}");

    _oldDataContext = vm;

    vm.PropertyChanged += DataContext_PropertyChanged;

    SelectedFaceUri = vm.Info?.FrontFace.ImageUri ?? string.Empty;

    PropertyChanged?.Invoke(this, new(nameof(Model)));
    SwitchFaceImageClickCommand.NotifyCanExecuteChanged();
  }

  private void DataContext_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Model.Info))
      SelectedFaceUri = Model?.Info?.FrontFace.ImageUri ?? string.Empty;
  }

  protected override void OnPointerMoved(PointerRoutedEventArgs e)
  {
    base.OnPointerMoved(e);

    if (!HoverPreviewEnabled) return;

    HoverPreviewUpdate(this, e);
  }

  protected override void OnPointerExited(PointerRoutedEventArgs e)
  {
    base.OnPointerExited(e);

    if (!HoverPreviewEnabled) return;

    CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });
  }

  private void HoverPreviewUpdate(FrameworkElement _, PointerRoutedEventArgs e)
  {
    var point = e.GetCurrentPoint(null).Position;

    CardPreview.Change(this, new(XamlRoot)
    {
      Uri = SelectedFaceUri,
      Coordinates = new((float)point.X, (float)point.Y)
    });
  }

  private void OnDragStarting(UIElement _, DragStartingEventArgs e)
  {
    if (Model == null || Model.GetModel() is not DeckEditorMTGCard dragItem) return;

    e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    e.Data.Properties.Add(nameof(CardDragArgs), new CardDragArgs(dragItem, origin: this));
  }

  protected override void OnDragEnter(DragEventArgs e)
  {
    e.Handled = true;

    SetDragEventArgs(e);
  }

  protected override void OnDragOver(DragEventArgs e)
  {
    e.Handled = true;

    if (e.AcceptedOperation == DataPackageOperation.None)
      return;

    SetDragEventArgs(e);
  }

  protected override async void OnDrop(DragEventArgs e)
  {
    var def = e.GetDeferral();

    e.Handled = true;

    e.DataView.Properties.TryGetValue(nameof(CardDragArgs), out var prop);
    var args = prop as CardDragArgs;

    if (e.AcceptedOperation == DataPackageOperation.Move)
    {
      // Move
      if (args?.Item is DeckEditorMTGCard editorCard)
      {
        var moveOrigin = args.Origin as CardDragArgs.IMoveOrigin;

        // Begin from
        if ((moveOrigin?.OnDropBeginMoveFrom?.CanExecute(editorCard) is true))
          moveOrigin.OnDropBeginMoveFrom.Execute(editorCard);

        // Begin to
        if (OnDropBeginMoveTo?.CanExecute(editorCard) is true)
          await OnDropBeginMoveTo.ExecuteAsync(editorCard);

        // Execute
        if (OnDropExecuteMove?.CanExecute(null) == true) OnDropExecuteMove.Execute(null);
        if (moveOrigin?.OnDropExecuteMove?.CanExecute(null) == true) moveOrigin.OnDropExecuteMove.Execute(null);
      }
    }
    else if ((e.AcceptedOperation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
    {
      if (args?.Item is MTGCard dropCard)
      {
        var editorCard = (dropCard as DeckEditorMTGCard) ?? new DeckEditorMTGCard(dropCard.Info);

        // Copy
        if (OnDropCopy?.CanExecute(editorCard) is true)
          await OnDropCopy.ExecuteAsync(editorCard);
      }
      else if (e.DataView.Contains(StandardDataFormats.Text) && await e.DataView.GetTextAsync() is string importText)
      {
        // Import
        if (OnDropImport?.CanExecute(importText) is true)
          await OnDropImport.ExecuteAsync(importText);
      }
    }

    def.Complete();
  }

  private void SetDragEventArgs(DragEventArgs e)
  {
    e.DragUIOverride.IsContentVisible = false;

    if (e.DataView.Properties.TryGetValue(nameof(CardDragArgs), out var prop) && prop is CardDragArgs args)
    {
      if (args.Origin.Equals(this)) e.AcceptedOperation = DataPackageOperation.None;
      else if (args.Item is MTGCard item)
      {
        if (!item.Info.TypeLine.Contains("Legendary")) e.AcceptedOperation = DataPackageOperation.None;
        else if (args.Item is DeckEditorMTGCard editorCard)
        {
          if ((e.Modifiers & CardDragArgs.MoveModifier) == CardDragArgs.MoveModifier) e.AcceptedOperation = DataPackageOperation.Move;
          else e.AcceptedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        }
        else e.AcceptedOperation = DataPackageOperation.Copy;
      }

      if (e.AcceptedOperation != DataPackageOperation.None)
      {
        if (e.AcceptedOperation == DataPackageOperation.Move) e.DragUIOverride.Caption = $"Move to {CommanderType}";
        else e.DragUIOverride.Caption = $"Change {CommanderType}";
      }
    }
    else if (e.DataView.Contains(StandardDataFormats.Text))
    {
      e.AcceptedOperation = DataPackageOperation.Copy;
      e.DragUIOverride.Caption = $"Import {CommanderType}";
    }
  }
}