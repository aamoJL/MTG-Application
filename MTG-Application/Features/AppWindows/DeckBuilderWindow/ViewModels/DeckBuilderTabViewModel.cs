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
  public SaveStatus SaveStatus => EditorViewModel?.DeckViewModel.SaveStatus ?? new();

  private DeckEditorPageViewModel? EditorViewModel
  {
    get;
    set
    {
      field?.PropertyChanged -= EditorViewModel_PropertyChanged;
      field = value;
      field?.PropertyChanged += EditorViewModel_PropertyChanged;

      OnPropertyChanged(nameof(SaveStatus));
    }
  }

  public Action<DeckBuilderTabViewModel>? OnRequestSelection { private get; set; }
  public Action<DeckBuilderTabViewModel>? OnClose { private get; set; }

  [RelayCommand]
  private async Task RequestClose(SaveStatus.ConfirmArgs args)
  {
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
  private void ChangeViewModel(DeckEditorPageViewModel? editor) => EditorViewModel = editor;

  private void SaveStatus_PropertyChanged(object? sender, PropertyChangedEventArgs e) => throw new NotImplementedException();

  private void EditorViewModel_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckEditorPageViewModel.DeckName): OnPropertyChanged(nameof(HeaderText)); break;
      case nameof(DeckEditorPageViewModel.DeckViewModel): OnPropertyChanged(nameof(SaveStatus)); break;
    }
  }
}