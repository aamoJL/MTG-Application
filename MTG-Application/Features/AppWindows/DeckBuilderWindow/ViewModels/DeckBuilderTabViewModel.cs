using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Editor.UseCases;
using MTGApplication.Features.DeckEditor.ViewModels;
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
      if (EditorViewModel.Name == string.Empty) return DefaultNewDeckTabHeaderText;
      else return EditorViewModel.Name;
    }
  }
  [ObservableProperty] public partial SaveStatus SaveStatus { get; private set; } = new();

  private DeckEditorViewModel? EditorViewModel
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
    if (EditorViewModel is DeckEditorViewModel editorVM && editorVM.HasUnsavedChanges)
    {
      OnRequestSelection?.Invoke(this);
      await new ConfirmUnsavedChanges(editorVM).Execute(args);
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
  private void ChangeViewModel(DeckEditorViewModel editor)
  {
    EditorViewModel = editor;
    SaveStatus = editor?.SaveStatus ?? new();
  }

  private void EditorViewModel_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(DeckEditorViewModel.Name))
      OnPropertyChanged(nameof(HeaderText));
  }
}