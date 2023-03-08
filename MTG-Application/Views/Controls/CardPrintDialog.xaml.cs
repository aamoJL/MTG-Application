using Microsoft.UI.Xaml.Controls;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System.Collections.Generic;

namespace MTGApplication.Views.Controls
{
  public sealed partial class CardPrintDialog : UserControl
  {
    public CardPrintDialog()
    {
      this.InitializeComponent();
      
      UpdateCardViewModels();
      DataContextChanged += CardPrintDialog_DataContextChanged;
    }

    private void CardPrintDialog_DataContextChanged(Microsoft.UI.Xaml.FrameworkElement sender, Microsoft.UI.Xaml.DataContextChangedEventArgs args)
    {
      UpdateCardViewModels();
    }

    public List<MTGCardViewModel> CardViewModels { get; set; }

    private void UpdateCardViewModels()
    {
      if (DataContext is not MTGCard card) { CardViewModels.Clear(); return; }

      
    }
  }
}
