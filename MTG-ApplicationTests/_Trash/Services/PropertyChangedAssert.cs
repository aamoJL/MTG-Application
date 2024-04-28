using System.ComponentModel;

namespace MTGApplicationTests.Services;

/// <summary>
/// Catches property change event of the notifier and compares the changed property to the expected property
/// </summary>
public class PropertyChangedAssert : IDisposable
{
  public PropertyChangedAssert(INotifyPropertyChanged notifier, string expectedPropertyChanged)
  {
    Notifier = notifier;
    ExpectedPropertyChanged = expectedPropertyChanged;
    Notifier.PropertyChanged += Notifier_PropertyChanged;
  }

  public INotifyPropertyChanged Notifier { get; }
  public string ExpectedPropertyChanged { get; }
  public bool PropertyChanged { get; private set; } = false;

  private void Notifier_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == ExpectedPropertyChanged) { PropertyChanged = true; }
  }

  public void Reset() => PropertyChanged = false;

  public void Dispose()
  {
    Notifier.PropertyChanged -= Notifier_PropertyChanged;
    GC.SuppressFinalize(this);
  }
}