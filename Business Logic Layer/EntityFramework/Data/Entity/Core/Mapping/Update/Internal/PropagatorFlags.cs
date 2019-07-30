// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.PropagatorFlags
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  [Flags]
  internal enum PropagatorFlags : byte
  {
    NoFlags = 0,
    Preserve = 1,
    ConcurrencyValue = 2,
    Unknown = 8,
    Key = 16, // 0x10
    ForeignKey = 32, // 0x20
  }
}
