using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.General.ViewModels;

public abstract class ViewModelReversibleAction<TViewModel, TParam> : ReversibleAction<TParam>
{
  public ViewModelReversibleAction(TViewModel viewmodel)
  {
    Viewmodel = viewmodel;

    Action = ActionMethod;
    ReverseAction = ReverseActionMethod;
  }

  public TViewModel Viewmodel { get; }

  protected abstract void ActionMethod(TParam param);
  protected abstract void ReverseActionMethod(TParam param);
}

