// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.MemberInfoExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Entity.Utilities
{
  internal static class MemberInfoExtensions
  {
    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    public static object GetValue(this MemberInfo memberInfo)
    {
      PropertyInfo propertyInfo = memberInfo as PropertyInfo;
      if (!(propertyInfo != (PropertyInfo) null))
        return ((FieldInfo) memberInfo).GetValue((object) null);
      return propertyInfo.GetValue((object) null, (object[]) null);
    }
  }
}
