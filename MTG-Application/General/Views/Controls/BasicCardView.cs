using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.IOService;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTGApplication.General.Views;

[ObservableObject]
public partial class BasicCardView : UserControl
{
  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(MTGCard), typeof(BasicCardView),
        new PropertyMetadata(default(MTGCard), OnModelPropertyChangedCallback));

  public BasicCardView()
  {
    DragAndDrop = new(new MTGCardCopier())
    {
      OnCopy = (item) => OnDropCopy?.Execute(item),
      OnRemove = (item) => OnDropRemove?.Execute(item),
      OnExternalImport = (data) => OnDropImport?.Execute(data),
      OnBeginMoveTo = (item) => OnDropBeginMoveTo?.Execute(item),
      OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute(item),
      OnExecuteMove = (item) => OnDropExecuteMove?.Execute(item)
    };

    DragStarting += DragAndDrop.DragStarting;
    DropCompleted += DragAndDrop.DropCompleted;
    DragOver += DragAndDrop.DragOver;
    Drop += DragAndDrop.Drop;
  }

  public MTGCard Model
  {
    get => (MTGCard)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }
  public CommanderTextViewDragAndDrop DragAndDrop { get; }
  public string CardName => Model?.Info.Name ?? string.Empty;

  [ObservableProperty] protected string selectedFaceUri = "";

  public ICommand OnDropCopy { get; set; }
  public ICommand OnDropRemove { get; set; }
  public ICommand OnDropImport { get; set; }
  public ICommand OnDropBeginMoveFrom { get; set; }
  public ICommand OnDropBeginMoveTo { get; set; }
  public ICommand OnDropExecuteMove { get; set; }

  /// <summary>
  /// Changes selected face image if possible
  /// </summary>
  [RelayCommand(CanExecute = nameof(SwitchFaceImageCanExecute))]
  protected void SwitchFaceImage()
  {
    if (!SwitchFaceImageCanExecute()) return;

    SelectedFaceUri = Model?.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri : Model.Info.FrontFace.ImageUri;
  }

  /// <summary>
  /// Opens card's API Website in web browser
  /// </summary>
  [RelayCommand] protected async Task OpenAPIWebsite() => await NetworkService.OpenUri(Model?.Info.APIWebsiteUri);

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand] protected async Task OpenCardmarketWebsite() => await NetworkService.OpenUri(Model?.Info.CardMarketUri);

  protected bool SwitchFaceImageCanExecute() => !string.IsNullOrEmpty(Model?.Info.BackFace?.ImageUri);

  protected virtual void OnModelChanging(MTGCard oldValue) { }

  protected virtual void OnModelChanged(MTGCard newValue)
  {
    SelectedFaceUri = newValue?.Info.FrontFace.ImageUri ?? string.Empty;

    OnPropertyChanged(nameof(CardName));
  }

  private static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (e.Property.Equals(ModelProperty))
    {
      var view = (sender as BasicCardView);

      view.OnModelChanging((MTGCard)e.OldValue);
      view.OnModelChanged((MTGCard)e.NewValue);
    }
  }
}

