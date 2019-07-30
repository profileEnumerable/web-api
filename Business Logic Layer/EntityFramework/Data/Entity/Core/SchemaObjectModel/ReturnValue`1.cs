// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.ReturnValue`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class ReturnValue<T>
  {
    private bool _succeeded;
    private T _value;

    internal bool Succeeded
    {
      get
      {
        return this._succeeded;
      }
    }

    internal T Value
    {
      get
      {
        return this._value;
      }
      set
      {
        this._value = value;
        this._succeeded = true;
      }
    }
  }
}
