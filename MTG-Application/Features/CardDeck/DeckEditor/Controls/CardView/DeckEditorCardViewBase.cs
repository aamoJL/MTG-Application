using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.IOService;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTGApplication.Features.CardDeck;
[ObservableObject]
public partial class DeckEditorCardViewBase : UserControl
{
  public MTGCard Model
  {
    get => (MTGCard)GetValue(ModelProperty);
    set => SetValue(ModelProperty, value);
  }

  public ICommand OnDeleteCommand
  {
    get => (ICommand)GetValue(OnDeleteCommandProperty);
    set => SetValue(OnDeleteCommandProperty, value);
  }

  [ObservableProperty] private string selectedFaceUri = "";

  /// <summary>
  /// Changes selected face image if possible
  /// </summary>
  [RelayCommand]
  public void SwitchFaceImage()
  {
    if (Model?.Info.BackFace is null) return;

    SelectedFaceUri = Model.Info.FrontFace.ImageUri == SelectedFaceUri
      ? Model.Info.BackFace?.ImageUri : Model.Info.FrontFace.ImageUri;
  }

  /// <summary>
  /// Opens card's API Website in web browser
  /// </summary>
  [RelayCommand] public async Task OpenAPIWebsite() => await new OpenUri().Execute(Model.Info.APIWebsiteUri);

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand] public async Task OpenCardmarketWebsite() => await new OpenUri().Execute(Model.Info.CardMarketUri);

  public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(nameof(Model), typeof(MTGCard), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(MTGCard), OnModelPropertyChangedCallback));

  public static readonly DependencyProperty OnDeleteCommandProperty =
      DependencyProperty.Register(nameof(OnDeleteCommand), typeof(ICommand), typeof(DeckEditorCardViewBase),
        new PropertyMetadata(default(ICommand)));

  protected static void OnModelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is DeckEditorCardViewBase view && e.NewValue is MTGCard newModel)
    {
      if (e.Property.Equals(ModelProperty))
      {
        view.SelectedFaceUri = newModel.Info.FrontFace.ImageUri;
      }
    }
  }
}

