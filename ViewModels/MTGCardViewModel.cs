using MTGApplication.Models;
using System.Windows.Input;
using static MTGApplication.Models.MTGCardModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.ViewModels
{
  public partial class MTGCardViewModel : ViewModelBase
  {
    public MTGCardViewModel(MTGCardModel model)
    {
      Model = model;
      model.PropertyChanged += Model_PropertyChanged;

      Count = model.Count;
    }

    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Model.Count))
      {
        Count = Model.Count;
      }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DecreaseCountCommand))]
    private int count;

    private int selectedFaceIndex = 0;
    private bool controlsVisible;

    private MTGCardModel Model { get; }
    public string FrontFaceImg => App.CardAPI.GetFaceUri(Model.Info.Id, false);
    public string BackFaceImg => Model.Info.CardFaces.Length == 2 ? App.CardAPI.GetFaceUri(Model.Info.Id, true) : null;
    public string SetIcon => App.CardAPI.GetSetIconUri(Model.Info.SetCode);
    public string SelectedFaceUri => selectedFaceIndex == 0 ? FrontFaceImg : BackFaceImg;
    public int SelectedFaceIndex
    {
      get => selectedFaceIndex;
      set
      {
        if(value > 0 && !HasBackFace) { value = 0; }
        selectedFaceIndex = value;
        OnPropertyChanged(nameof(SelectedFaceIndex));
        OnPropertyChanged(nameof(SelectedFaceUri));
      }
    }
    public CardInfo CardInfo => Model.Info;
    public bool HasBackFace => Model.HasBackFace;
    public bool ControlsVisible
    {
      get => controlsVisible;
      set
      {
        controlsVisible = value;
        OnPropertyChanged(nameof(ControlsVisible));
      }
    }
    public float Price => Model.Info.Price;

    public ICommand RemoveRequestCommand { get; set; }
    [RelayCommand(CanExecute = nameof(HasBackFace))]
    public void FlipCard()
    {
      if (HasBackFace) SelectedFaceIndex = SelectedFaceIndex == 0 ? 1 : 0;
    }
    [RelayCommand]
    public async Task OpenAPIWebsite()
    {
      await App.CardAPI.OpenAPICardWebsite(Model);
    }
    [RelayCommand]
    public void IncreaseCount()
    {
      Model.Count++;
    }
    [RelayCommand(CanExecute = nameof(CanDecreaseCount))]
    public void DecreaseCount()
    {
      Model.Count--;
    }

    public string ModelToJSON()
    {
      return Model.ToJSON();
    }

    private bool CanDecreaseCount() => Count > 1;
  }
}
