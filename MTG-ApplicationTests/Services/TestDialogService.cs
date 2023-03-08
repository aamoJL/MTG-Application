using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Interfaces;
using static MTGApplication.Views.Dialogs;

namespace MTGApplicationTests.Services
{
  public class TestDialogService
  {
    public class TestDialogWrapper : IDialogWrapper
    {
      // The Dialog should not be used in testing
      public ContentDialog Dialog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public ContentDialogResult Result { get; set; }

      public TestDialogWrapper(ContentDialogResult result)
      {
        Result = result;
      }

      /// <summary>
      /// Returns the given result
      /// </summary>
      public Task<ContentDialogResult> ShowAsync()
      {
        return Task.Run(() => Result);
      }
    }

    public class TestMessageDialog : MessageDialog
    {
      public ContentDialogResult Result { get; set; }
      public TestMessageDialog(ContentDialogResult result, string title = "") : base(title)
      {
        Result = result;
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new TestDialogWrapper(Result);
      }
    }

    public class TestCheckBoxDialog : CheckBoxDialog
    {
      public ContentDialogResult Result { get; set; }
      public bool IsChecked { get; set; }

      public TestCheckBoxDialog(ContentDialogResult result, bool isChecked, string title = "") : base(title)
      {
        Result = result;
        IsChecked = isChecked;
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new TestDialogWrapper(Result);
      }
      public override bool? GetInputValue() => IsChecked;
    }

    public class TestComboBoxDialog : ComboBoxDialog
    {
      public ContentDialogResult Result { get; set; }
      public string? Selection { get; set; }

      public TestComboBoxDialog(ContentDialogResult result, string? selection, string title = "") : base(title)
      {
        Result = result;
        Selection = selection;
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new TestDialogWrapper(Result);
      }
      public override string? GetInputValue() => Selection;
    }

    public class TestTextBoxDialog : TextBoxDialog
    {
      public ContentDialogResult Result { get; set; }
      public string? Text { get; set; }
      
      public TestTextBoxDialog(ContentDialogResult result, string? text, string title = "") : base(title)
      {
        Result = result;
        Text = text;
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new TestDialogWrapper(Result);
      }
      public override string? GetInputValue() => Text;
    }

    public class TestConfirmationDialog : ConfirmationDialog
    {
      public ContentDialogResult Result { get; set; }
      
      public TestConfirmationDialog(ContentDialogResult result, string title = "") : base(title)
      {
        Result = result;
      }
      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new TestDialogWrapper(Result);
      }
    }

    public class TestTextAreaDialog : TextAreaDialog
    {
      public ContentDialogResult Result { get; set; }
      public string? Text { get; set; }
      
      public TestTextAreaDialog(ContentDialogResult result, string? text, string title = "") : base(title)
      {
        Result = result;
        Text = text;
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new TestDialogWrapper(Result);
      }
      public override string? GetInputValue() => Text;
    }

    public class TestGridViewDialog : GridViewDialog
    {
      public ContentDialogResult Result { get; set; }
      public object? ReturnObject { get; set; }

      public TestGridViewDialog(ContentDialogResult result, object? returnObject, string title = "") : base(title)
      {
        Result = result;
        ReturnObject = returnObject;
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new TestDialogWrapper(Result);
      }
      public override object? GetInputValue() => ReturnObject;
    }
  }
}
