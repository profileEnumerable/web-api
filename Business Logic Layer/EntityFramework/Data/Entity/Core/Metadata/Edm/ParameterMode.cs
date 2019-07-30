// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ParameterMode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>The enumeration defining the mode of a parameter</summary>
  public enum ParameterMode
  {
    /// <summary>In parameter</summary>
    In,
    /// <summary>Out parameter</summary>
    Out,
    /// <summary>Both in and out parameter</summary>
    InOut,
    /// <summary>Return Parameter</summary>
    ReturnValue,
  }
}
