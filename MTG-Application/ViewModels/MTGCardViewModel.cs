using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Models;
using MTGApplication.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using static MTGApplication.Enums;
using static MTGApplication.Models.MTGCard;

namespace MTGApplication.ViewModels;

public partial class MTGCardViewModel : ViewModelBase
{
  public MTGCardViewModel(MTGCard model)
  {
    Model = model;
    model.PropertyChanged += Model_PropertyChanged;
    PropertyChanged += MTGCardViewModel_PropertyChanged;
  }

  protected void MTGCardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(SelectedFaceSide))
    { OnPropertyChanged(nameof(SelectedFaceUri)); }
  }
  protected void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Model.Count))
    {
      DecreaseCountCommand.NotifyCanExecuteChanged();
      OnPropertyChanged(nameof(Count));
    }
    else if (e.PropertyName == nameof(Model.Info))
    {
      OnPropertyChanged(nameof(SelectedFaceUri));
      OnPropertyChanged(nameof(Price));
    }
  }

  public MTGCard Model { get; }

  [ObservableProperty]
  protected CardSide selectedFaceSide;

  public string SelectedFaceUri => SelectedFaceSide == CardSide.Front ? Model.Info.FrontFace.ImageUri : Model.Info.BackFace?.ImageUri;
  public bool HasBackFace => Model.Info.BackFace?.ImageUri != null;
  public string ModelAPIName => Model.GetAPIName();

  #region Model Properties
  public ColorTypes ColorType => Model.Info.Colors.Length > 1 ? ColorTypes.M : Model.Info.Colors[0];
  public SpellType PrimaryType => Model.Info.SpellTypes[0];
  public RarityTypes Rarity => Model.Info.RarityType;
  public ColorTypes[] Colors => Model.Info.Colors;
  public string TypeLine => Model.Info.TypeLine;
  public string SetName => Model.Info.SetName;
  public float Price => Model.Info.Price;
  public string Name => Model.Info.Name;
  public int CMC => Model.Info.CMC;
  public int Count => Model.Count;
  #endregion

  public ICommand DeleteCardCommand { get; set; }
  public ICommand ShowPrintsDialogCommand { get; set; }

  /// <summary>
  /// Changes selected face image if possible
  /// </summary>
  [RelayCommand(CanExecute = nameof(HasBackFace))]
  public void FlipCard()
  {
    if (HasBackFace)
      SelectedFaceSide = SelectedFaceSide == CardSide.Front ? CardSide.Back : CardSide.Front;
  }

  /// <summary>
  /// Opens card's API Website in web browser
  /// </summary>
  [RelayCommand]
  public async Task OpenAPIWebsite() => await IOService.OpenUri(Model.Info.APIWebsiteUri);

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand]
  public async Task OpenCardmarketWebsite() => await IOService.OpenUri(Model.Info.CardMarketUri);

  /// <summary>
  /// Increases count by one
  /// </summary>
  [RelayCommand]
  public void IncreaseCount() => Model.Count++;

  /// <summary>
  /// Decreases count by one if possible
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanExecuteDecreaseCountCommand))]
  public void DecreaseCount()
  {
    if (Model.Count > 0)
    { Model.Count--; }
  }

  protected bool CanExecuteDecreaseCountCommand() => Model.Count > 1;

  public static string GetPropertyName(MTGSortProperty prop)
  {
    return prop switch
    {
      MTGSortProperty.CMC => nameof(CMC),
      MTGSortProperty.Name => nameof(Name),
      MTGSortProperty.Rarity => nameof(Rarity),
      MTGSortProperty.Color => nameof(ColorType),
      MTGSortProperty.Set => nameof(SetName),
      MTGSortProperty.Count => nameof(Count),
      MTGSortProperty.Price => nameof(Price),
      MTGSortProperty.Type => nameof(PrimaryType),
      _ => string.Empty,
    };
  }
}

public partial class MTGCardCollectionCardViewModel : MTGCardViewModel
{
  public MTGCardCollectionCardViewModel(MTGCard model) : base(model) { }

  [ObservableProperty]
  private bool isOwned;
}
