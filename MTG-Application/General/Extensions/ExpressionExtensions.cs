using System;
using System.Linq.Expressions;

namespace MTGApplication.General.Extensions;

public static class ExpressionExtensions
{
  public static Expression<Func<T, object>>[] EmptyArray<T>() => [];
}
