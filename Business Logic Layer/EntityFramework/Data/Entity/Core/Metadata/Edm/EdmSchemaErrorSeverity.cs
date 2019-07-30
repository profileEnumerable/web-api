// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmSchemaErrorSeverity
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Defines the different severities of errors that can occur when validating an Entity Framework model.
  /// </summary>
  public enum EdmSchemaErrorSeverity
  {
    /// <summary>
    /// A warning that does not prevent the model from being used.
    /// </summary>
    Warning,
    /// <summary>An error that prevents the model from being used.</summary>
    Error,
  }
}
