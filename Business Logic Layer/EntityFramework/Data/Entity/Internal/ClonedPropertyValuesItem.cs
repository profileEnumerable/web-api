// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ClonedPropertyValuesItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Internal
{
  internal class ClonedPropertyValuesItem : IPropertyValuesItem
  {
    private readonly string _name;
    private readonly bool _isComplex;
    private readonly Type _type;

    public ClonedPropertyValuesItem(string name, object value, Type type, bool isComplex)
    {
      this._name = name;
      this._type = type;
      this._isComplex = isComplex;
      this.Value = value;
    }

    public object Value { get; set; }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public bool IsComplex
    {
      get
      {
        return this._isComplex;
      }
    }

    public Type Type
    {
      get
      {
        return this._type;
      }
    }
  }
}
