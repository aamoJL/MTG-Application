using Microsoft.UI.Xaml.Media;
using MTGApplication.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using static MTGApplication.Models.MTGCardModel;
using App1.API;

namespace MTGApplication.ViewModels
{
    public class MTGCardViewModel : ViewModelBase
  {
    public MTGCardViewModel(MTGCardModel model)
    {
      Model = model;
      FlipCardCommand = new RelayCommand(() =>
      {
        SelectedFaceIndex = SelectedFaceIndex == 0 ? 1 : 0;
      }, () =>
      {
        return Model.HasBackFace;
      });
      OpenAPIWebsiteCommand = new RelayCommand(async() => await ScryfallAPI.OpenAPICardWebsite(model));
      IncreaseCountCommand = new RelayCommand(() => Count++);
      DecreaseCountCommand = new RelayCommand(() => Count--);
    }

    private int selectedFaceIndex = 0;

    private MTGCardModel Model { get; }
    public ImageSource SelectedFace => SelectedFaceIndex == 0 ? Model.FrontFaceImg : Model.BackFaceImg;
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
    public int Count
    {
      get => Model.Count;
      set
      {
        Model.Count = value;
        OnPropertyChanged(nameof(Count));
      }
    }
    public CardInfo CardInfo => Model.Info;
    public bool HasBackFace => Model.HasBackFace;

    public ICommand FlipCardCommand { get; }
    public ICommand OpenAPIWebsiteCommand { get; }
    public ICommand DecreaseCountCommand { get; }
    public ICommand IncreaseCountCommand { get; }
    public ICommand RemoveRequestCommand { get; init; }

    public string ModelToJSON()
    {
      return Model.ToJSON();
    }
  }
}
