using MTGApplication.General.Services.ReversibleCommandService;
using System;

namespace MTGApplication.General.ViewModels;

[Obsolete]
public abstract class ViewModelReversibleAction<TViewModel, TParam> : ReversibleAction<TParam>
{
  public ViewModelReversibleAction(TViewModel viewmodel)
  {
    Viewmodel = viewmodel;
  }

  public TViewModel Viewmodel { get; }
}

