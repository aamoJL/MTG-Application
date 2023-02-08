using MTGApplication.Models;
using static MTGApplication.Models.MTGCard;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

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
      if(e.PropertyName == nameof(SelectedFace)) { OnPropertyChanged(nameof(SelectedFaceUri)); }
    }
    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Model.Count)) { DecreaseCountCommand.NotifyCanExecuteChanged(); }
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
    //private bool controlsVisible;
    //public bool ControlsVisible
    //{
    //  get => controlsVisible;
    //  set
    //  {
    //    controlsVisible = value;
    //    OnPropertyChanged(nameof(ControlsVisible));
    //  }
    //}

    //public ICommand RemoveRequestCommand { get; set; }
    [RelayCommand(CanExecute = nameof(HasBackFace))]
    public void FlipCard()
    {
      if (HasBackFace) SelectedFace = SelectedFace == CardSide.Front ? CardSide.Back : CardSide.Front;
    }
    [RelayCommand]
    public async Task OpenAPIWebsite()
    {
      await IO.OpenUri(Model.Info.APIWebsiteUri);
    }
    [RelayCommand]
    public async Task OpenCardmarketWebsite()
    {
      await IO.OpenUri(Model.Info.CardMarketUri);
    }
    [RelayCommand]
    public void IncreaseCount()
    {
      Model.Count++;
    }
    [RelayCommand(CanExecute = nameof(CanExecuteDecreaseCountCommand))]
    public void DecreaseCount()
    {
      Model.Count--;
    }
    [RelayCommand]
    public void DeleteCard()
    {
      Model.MTGCardDeckDeckCards?.DeckCards.Remove(Model);
      Model.MTGCardDeckWishlist?.Wishlist.Remove(Model);
      Model.MTGCardDeckMaybelist?.Maybelist.Remove(Model);
    }

    private bool CanExecuteDecreaseCountCommand() => Model.Count > 1;
  }
}
