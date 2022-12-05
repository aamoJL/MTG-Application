using MTGApplication.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public abstract partial class SavableMTGCardCollectionViewModel : MTGCardCollectionViewModel
  {
    public SavableMTGCardCollectionViewModel(NamedMTGCardCollectionModel model) : base(model) 
    {
      Model = model;
    }

    protected override NamedMTGCardCollectionModel Model { get; }

    public string Name => Model.Name;
    public bool HasUnsavedChanges { get; set; }

    protected override void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.Model_PropertyChanged(sender, e);

      switch (e.PropertyName)
      {
        case nameof(Model.Name): OnPropertyChanged(nameof(Name)); break;
        case nameof(Model.TotalCount):
          HasUnsavedChanges = true; break;
        default:
          break;
      }
    }

    public void Reset()
    {
      Model.Reset();
      HasUnsavedChanges = false;
    }
    public async Task Save(string name)
    {
      IsBusy = true;
      
      if (await Task.Run(() => OnSave(name)))
      {
        // TODO: notification
        Model.Name = name;
        HasUnsavedChanges = false;
      }

      IsBusy = false;
    }
    public async Task LoadAsync(string name)
    {
      IsBusy = true;

      if (await Task.Run(() => OnLoadAsync(name)) is var loadedCards)
      {
        // TODO: notification
        Clear();
        foreach (var card in loadedCards) { Model.Add(card); }
        Model.Name = name;
        Model.SortCollection(SelectedSortDirection, SelectedSortProperty);
        HasUnsavedChanges = false;
      }

      IsBusy = false;
    }
    public async Task Delete()
    {
      IsBusy = true;

      if (await Task.Run(() => OnDeleteAsync(Name)))
      {
        // TODO: notification
        Reset();
      }

      IsBusy = false;
    }

    protected abstract Task<bool> OnSave(string name);
    protected abstract Task<MTGCardModel[]> OnLoadAsync(string name);
    protected abstract Task<bool> OnDeleteAsync(string name);
  }
}
