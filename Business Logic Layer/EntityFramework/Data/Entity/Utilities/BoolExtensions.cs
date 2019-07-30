// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.BoolExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Utilities
{
  internal static class BoolExtensions
  {
    internal static bool? Not(this bool? operand)
    {
      if (!operand.HasValue)
        return new bool?();
      return new bool?(!operand.Value);
    }

    internal static bool? And(this bool? left, bool? right)
    {
      return !left.HasValue || !right.HasValue ? (left.HasValue || right.HasValue ? (!left.HasValue ? (right.Value ? new bool?() : new bool?(false)) : (left.Value ? new bool?() : new bool?(false))) : new bool?()) : new bool?(left.Value && right.Value);
    }

    internal static bool? Or(this bool? left, bool? right)
    {
      return !left.HasValue || !right.HasValue ? (left.HasValue || right.HasValue ? (!left.HasValue ? (right.Value ? new bool?(true) : new bool?()) : (left.Value ? new bool?(true) : new bool?())) : new bool?()) : new bool?(left.Value || right.Value);
    }
  }
}
