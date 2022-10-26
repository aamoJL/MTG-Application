using Microsoft.UI.Xaml.Media;
using MTGApplication.Models;
using System.Windows.Input;
using static MTGApplication.Models.MTGCardModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace MTGApplication.ViewModels
{
  public partial class MTGCardViewModel : ViewModelBase
  {
    public MTGCardViewModel(MTGCardModel model)
    {
      Model = model;
      model.PropertyChanged += Model_PropertyChanged;
    }

    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Model.Count))
      {
        OnPropertyChanged(nameof(CanDecreaseCount));
        OnPropertyChanged(nameof(Count));
      }
    }

    private int selectedFaceIndex = 0;
    private bool controlsVisible;

    private MTGCardModel Model { get; }
    public ImageSource SelectedFace => SelectedFaceIndex == 0 ? Model.FrontFaceImg : Model.BackFaceImg;
    public ImageSource SetIcon => Model.SetIcon;
    public int SelectedFaceIndex
    {
      get => selectedFaceIndex;
      set
      {
        selectedFaceIndex = value;
        OnPropertyChanged(nameof(SelectedFaceIndex));
        OnPropertyChanged(nameof(SelectedFace));
      }
    }
    public int Count => Model.Count;
    public CardInfo CardInfo => Model.Info;
    public bool HasBackFace => Model.HasBackFace;
    public bool CanDecreaseCount
    {
      get => Count > 1;
    }
    public bool ControlsVisible
    {
      get => controlsVisible;
      set
      {
        controlsVisible = value;
        OnPropertyChanged(nameof(ControlsVisible));
      }
    }

    public ICommand RemoveRequestCommand { get; set; }

    [RelayCommand]
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
    [RelayCommand]
    public void DecreaseCount()
    {
      Model.Count--;
    }

    public string ModelToJSON()
    {
      return Model.ToJSON();
    }
  }
}
