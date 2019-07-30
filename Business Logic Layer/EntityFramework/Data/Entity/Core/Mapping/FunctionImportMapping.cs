// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents a mapping from a model function import to a store composable or non-composable function.
  /// </summary>
  public abstract class FunctionImportMapping : MappingItem
  {
    private readonly EdmFunction _functionImport;
    private readonly EdmFunction _targetFunction;

    internal FunctionImportMapping(EdmFunction functionImport, EdmFunction targetFunction)
    {
      this._functionImport = functionImport;
      this._targetFunction = targetFunction;
    }

    /// <summary>Gets model function (or source of the mapping)</summary>
    public EdmFunction FunctionImport
    {
      get
      {
        return this._functionImport;
      }
    }

    /// <summary>Gets store function (or target of the mapping)</summary>
    public EdmFunction TargetFunction
    {
      get
      {
        return this._targetFunction;
      }
    }
  }
}
