// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.Internal.Error
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Linq.Expressions.Internal
{
  internal static class Error
  {
    internal static Exception UnhandledExpressionType(ExpressionType expressionType)
    {
      return (Exception) new NotSupportedException(Strings.ELinq_UnhandledExpressionType((object) expressionType));
    }

    internal static Exception UnhandledBindingType(MemberBindingType memberBindingType)
    {
      return (Exception) new NotSupportedException(Strings.ELinq_UnhandledBindingType((object) memberBindingType));
    }
  }
}
