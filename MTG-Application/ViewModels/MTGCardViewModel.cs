using MTGApplication.Models;
using static MTGApplication.Models.MTGCard;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using MTGApplication.Services;
using MTGApplication.Interfaces;
using static MTGApplication.Enums;

namespace MTGApplication.ViewModels
{
  public partial class MTGCardViewModel : ViewModelBase
  {
    public MTGCardViewModel(MTGCard model)
    {
      Model = model;
      model.PropertyChanged += Model_PropertyChanged;
      PropertyChanged += MTGCardViewModel_PropertyChanged;
    }

    private void MTGCardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(SelectedFace)) { OnPropertyChanged(nameof(SelectedFaceUri)); }
    }
    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Model.Count)) 
      { 
        DecreaseCountCommand.NotifyCanExecuteChanged(); 
        OnPropertyChanged(nameof(Count));
      }
      else if(e.PropertyName == nameof(Model.Info)) 
      { 
        OnPropertyChanged(nameof(SelectedFaceUri));
        OnPropertyChanged(nameof(Price));
      }
    }

    public MTGCard Model { get; }

    [ObservableProperty]
    private CardSide selectedFace;

    public string SelectedFaceUri
    {
      get
      {
        return SelectedFace == CardSide.Front ? Model.Info.FrontFace.ImageUri : Model.Info.BackFace?.ImageUri;
      }
    }
    public bool HasBackFace => Model.Info.BackFace?.ImageUri != null;
    public string ModelAPIName => ICardAPI<MTGCard>.GetAPIName(Model);

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
    public ICommand ChangePrintDialogCommand { get; set; }

    /// <summary>
    /// Changes selected face image if possible
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasBackFace))]
    public void FlipCard()
    {
      if (HasBackFace) SelectedFace = SelectedFace == CardSide.Front ? CardSide.Back : CardSide.Front;
    }

    /// <summary>
    /// Opens card's API Website in web browser
    /// </summary>
    [RelayCommand]
    public async Task OpenAPIWebsite()
    {
      await IO.OpenUri(Model.Info.APIWebsiteUri);
    }

    /// <summary>
    /// Opens card's Cardmarket page in web browser
    /// </summary>    
    [RelayCommand]
    public async Task OpenCardmarketWebsite()
    {
      await IO.OpenUri(Model.Info.CardMarketUri);
    }

    /// <summary>
    /// Increases count by one
    /// </summary>
    [RelayCommand]
    public void IncreaseCount()
    {
      Model.Count++;
    }

    /// <summary>
    /// Decreases count by one if possible
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteDecreaseCountCommand))]
    public void DecreaseCount()
    {
      if (Model.Count > 0) { Model.Count--; }
    }

    private bool CanExecuteDecreaseCountCommand() => Model.Count > 1;

    public static string GetPropertyName(SortMTGProperty prop)
    {
      return prop switch
      {
        SortMTGProperty.CMC => nameof(CMC),
        SortMTGProperty.Name => nameof(Name),
        SortMTGProperty.Rarity => nameof(Rarity),
        SortMTGProperty.Color => nameof(ColorType),
        SortMTGProperty.Set => nameof(SetName),
        SortMTGProperty.Count => nameof(Count),
        SortMTGProperty.Price => nameof(Price),
        SortMTGProperty.Type => nameof(PrimaryType),
        _ => string.Empty,
      };
    }
  }
}
