using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using MTGApplication.General;
using MTGApplication.Models;
using MTGApplication.Services;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using static MTGApplication.Enums;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels;

/// <summary>
/// View model for MTG cards
/// </summary>
public partial class MTGCardViewModel : ViewModelBase
{
  public MTGCardViewModel(MTGCard model)
  {
    Model = model;

    var inpc = Model as INotifyPropertyChanged;
    var weakPropertyChangedListener = new WeakEventListener<MTGCardViewModel, object, PropertyChangedEventArgs>(this)
    {
      OnEventAction = static (instance, source, eventArgs) => instance.Model_PropertyChanged(source, eventArgs),
      OnDetachAction = (weakEventListener) => inpc.PropertyChanged -= weakEventListener.OnEvent // Use Local References Only
    };
    inpc.PropertyChanged += weakPropertyChangedListener.OnEvent;
  }

  protected CardSide selectedFaceSide;

  #region Properties
  public MTGCard Model { get; }
  public string SelectedFaceUri => SelectedFaceSide == CardSide.Front ? Model.Info.FrontFace.ImageUri : Model.Info.BackFace?.ImageUri;
  public bool HasBackFaceImage => Model.Info.BackFace?.ImageUri != null;
  public string ModelAPIName => Model.APIName;
  public CardSide SelectedFaceSide
  {
    get => selectedFaceSide;
    set
    {
      selectedFaceSide = value;
      OnPropertyChanged(nameof(SelectedFaceSide));
      OnPropertyChanged(nameof(SelectedFaceUri));
    }
  }
  public ColorTypes ColorType => Model.Info.Colors.Length > 1 ? ColorTypes.M : Model.Info.Colors[0];
  public SpellType PrimaryType => Model.Info.SpellTypes[0];
  public RarityTypes Rarity => Model.Info.RarityType;
  public ColorTypes[] Colors => Model.Info.Colors;
  public string TypeLine => Model.Info.TypeLine;
  public string SetName => Model.Info.SetName;
  public float Price => Model.Info.Price;
  public string Name => Model.Info.Name;
  public int CMC => Model.Info.CMC;
  public int Count { get => Model.Count; set => Model.Count = value; }
  #endregion

  public ICommand DeleteCardCommand { get; set; }
  public ICommand ShowPrintsDialogCommand { get; set; }

  protected void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Model.Count):
        OnPropertyChanged(nameof(Count)); break;
      case nameof(Model.Info):
        OnPropertyChanged(nameof(SelectedFaceUri));
        OnPropertyChanged(nameof(Price));
        break;
    }
  }

  /// <summary>
  /// Changes selected face image if possible
  /// </summary>
  [RelayCommand(CanExecute = nameof(HasBackFaceImage))]
  public void FlipCard()
  {
    if (HasBackFaceImage)
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
  /// Returns the name of the viewmodel property that the given sort property uses to sort viewmodels
  /// </summary>
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