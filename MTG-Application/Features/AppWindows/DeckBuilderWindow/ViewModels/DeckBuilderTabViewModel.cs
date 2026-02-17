using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.ViewModels.EditorPage;
using MTGApplication.General.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;

public partial class DeckBuilderTabViewModel : ViewModelBase
{
  private static readonly string DefaultDeckSelectionTabHeaderText = "New tab";
  private static readonly string DefaultNewDeckTabHeaderText = "New deck";

  public string HeaderText
  {
    get
    {
      if (EditorViewModel == null) return DefaultDeckSelectionTabHeaderText;
      if (EditorViewModel.DeckName == string.Empty) return DefaultNewDeckTabHeaderText;
      else return EditorViewModel.DeckName;
    }
  }
  [ObservableProperty] public partial SaveStatus SaveStatus { get; private set; } = new();

  private DeckEditorPageViewModel? EditorViewModel
  {
    get;
    set
    {
      field?.PropertyChanged -= EditorViewModel_PropertyChanged;
      field = value;
      field?.PropertyChanged += EditorViewModel_PropertyChanged;
    }
  }

  public Action<DeckBuilderTabViewModel>? OnRequestSelection { private get; set; }
  public Action<DeckBuilderTabViewModel>? OnClose { private get; set; }

  [RelayCommand]
  private async Task RequestClose(SaveStatus.ConfirmArgs args)
  {
    // TODO: change to ISavable
    if (EditorViewModel is DeckEditorPageViewModel editorVM && editorVM.DeckViewModel.SaveStatus.HasUnsavedChanges)
    {
      OnRequestSelection?.Invoke(this);
      await editorVM.DeckViewModel.SaveUnsavedChangesCommand.ExecuteAsync(args);
    }
  }

  [RelayCommand]
  private async Task TryClose()
  {
    var saveArgs = new SaveStatus.ConfirmArgs();

    await RequestClose(saveArgs);

    if (saveArgs.Cancelled)
      return;

    EditorViewModel = null;
    OnClose?.Invoke(this);
  }

  [RelayCommand]
  private void ChangeViewModel(DeckEditorPageViewModel editor)
  {
    EditorViewModel = editor;
    SaveStatus = editor?.DeckViewModel.SaveStatus ?? new();
  }

  private void EditorViewModel_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(DeckEditorPageViewModel.DeckName))
      OnPropertyChanged(nameof(HeaderText));
  }
}