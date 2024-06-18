using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ChangeList(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel, MTGCardCollectionList>(viewmodel)
  {
    protected override async Task Execute(MTGCardCollectionList list) 
      => await Viewmodel.ChangeCollectionList(list);
  }
}