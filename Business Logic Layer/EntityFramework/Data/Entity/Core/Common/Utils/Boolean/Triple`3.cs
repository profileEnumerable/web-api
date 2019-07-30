// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.Triple`3
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal struct Triple<T1, T2, T3> : IEquatable<Triple<T1, T2, T3>>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
    where T3 : IEquatable<T3>
  {
    private readonly T1 _value1;
    private readonly T2 _value2;
    private readonly T3 _value3;

    internal Triple(T1 value1, T2 value2, T3 value3)
    {
      this._value1 = value1;
      this._value2 = value2;
      this._value3 = value3;
    }

    public bool Equals(Triple<T1, T2, T3> other)
    {
      if (this._value1.Equals(other._value1) && this._value2.Equals(other._value2))
        return this._value3.Equals(other._value3);
      return false;
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this._value1.GetHashCode() ^ this._value2.GetHashCode() ^ this._value3.GetHashCode();
    }
  }
}
