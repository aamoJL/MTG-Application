using MTGApplication.Models;
using static MTGApplication.Models.MTGCard;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

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

    public ICommand DeleteCardCommand { get; set; }

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
      if(Model.Count > 0) { Model.Count--; }
    }

    private bool CanExecuteDecreaseCountCommand() => Model.Count > 1;
  }
}
