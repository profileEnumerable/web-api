// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.EdmComplexPropertyAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>
  /// Attribute for complex properties
  /// Implied default AttributeUsage properties Inherited=True, AllowMultiple=False,
  /// The metadata system expects this and will only look at the first of each of these attributes, even if there are more.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class EdmComplexPropertyAttribute : EdmPropertyAttribute
  {
  }
}
