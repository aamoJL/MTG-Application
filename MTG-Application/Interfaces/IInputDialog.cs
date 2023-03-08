namespace MTGApplication.Interfaces
{
  /// <summary>
  /// Dialogs with a secondary input
  /// </summary>
  public interface IInputDialog<T>
  {
    public abstract T GetInputValue();
  }
}