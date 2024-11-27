using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace MTGApplication.General.Views.Extensions;

public static class DependencyObjectExtensions
{
  /// <summary>
  /// Returns <see cref="DependencyObject"/> from the <paramref name="root"/> that has the given <paramref name="name"/>.
  /// Searches every child element and child of the children elements
  /// </summary>
  public static DependencyObject? FindChildByName(this DependencyObject root, string name)
  {
    var childCount = VisualTreeHelper.GetChildrenCount(root);

    for (var i = 0; i < childCount; i++)
    {
      if (VisualTreeHelper.GetChild(root, i) is DependencyObject child)
      {
        if (child.GetValue(FrameworkElement.NameProperty) is string value && value == name)
          return child;
        else
        {
          if (child.FindChildByName(name) is DependencyObject recursiveResult && recursiveResult.GetValue(FrameworkElement.NameProperty) is string recValue && recValue == name)
            return recursiveResult;
        }
      }
    }

    return null;
  }

  /// <summary>
  /// Returns parent object of the given type from the given child element if the parent exists
  /// </summary>
  public static T? FindParentByType<T>(this DependencyObject child) where T : DependencyObject
  {
    var parent = VisualTreeHelper.GetParent(child);

    return parent is not null and not T and FrameworkElement element
      ? element.FindParentByType<T>()
      : (T?)parent;
  }
}