// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.FacetValueContainer`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal struct FacetValueContainer<T>
  {
    private T _value;
    private bool _hasValue;
    private bool _isUnbounded;

    internal T Value
    {
      set
      {
        this._isUnbounded = false;
        this._hasValue = true;
        this._value = value;
      }
    }

    private void SetUnbounded()
    {
      this._isUnbounded = true;
      this._hasValue = true;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "unbounded")]
    public static implicit operator FacetValueContainer<T>(
      EdmConstants.Unbounded unbounded)
    {
      FacetValueContainer<T> facetValueContainer = new FacetValueContainer<T>();
      facetValueContainer.SetUnbounded();
      return facetValueContainer;
    }

    public static implicit operator FacetValueContainer<T>(T value)
    {
      return new FacetValueContainer<T>() { Value = value };
    }

    internal object GetValueAsObject()
    {
      if (this._isUnbounded)
        return (object) EdmConstants.UnboundedValue;
      return (object) this._value;
    }

    internal bool HasValue
    {
      get
      {
        return this._hasValue;
      }
    }
  }
}
