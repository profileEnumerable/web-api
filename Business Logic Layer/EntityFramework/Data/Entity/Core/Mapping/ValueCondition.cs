// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ValueCondition
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping
{
  internal class ValueCondition : IEquatable<ValueCondition>
  {
    internal static readonly ValueCondition IsNull = new ValueCondition("NULL", true);
    internal static readonly ValueCondition IsNotNull = new ValueCondition("NOT NULL", true);
    internal static readonly ValueCondition IsOther = new ValueCondition("OTHER", true);
    internal const string IsNullDescription = "NULL";
    internal const string IsNotNullDescription = "NOT NULL";
    internal const string IsOtherDescription = "OTHER";
    internal readonly string Description;
    internal readonly bool IsSentinel;

    private ValueCondition(string description, bool isSentinel)
    {
      this.Description = description;
      this.IsSentinel = isSentinel;
    }

    internal ValueCondition(string description)
      : this(description, false)
    {
    }

    internal bool IsNotNullCondition
    {
      get
      {
        return object.ReferenceEquals((object) this, (object) ValueCondition.IsNotNull);
      }
    }

    public bool Equals(ValueCondition other)
    {
      if (other.IsSentinel == this.IsSentinel)
        return other.Description == this.Description;
      return false;
    }

    public override int GetHashCode()
    {
      return this.Description.GetHashCode();
    }

    public override string ToString()
    {
      return this.Description;
    }
  }
}
