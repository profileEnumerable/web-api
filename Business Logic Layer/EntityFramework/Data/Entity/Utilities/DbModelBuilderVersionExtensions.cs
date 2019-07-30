// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbModelBuilderVersionExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Utilities
{
  internal static class DbModelBuilderVersionExtensions
  {
    public static double GetEdmVersion(this DbModelBuilderVersion modelBuilderVersion)
    {
      switch (modelBuilderVersion)
      {
        case DbModelBuilderVersion.Latest:
        case DbModelBuilderVersion.V5_0:
        case DbModelBuilderVersion.V6_0:
          return 3.0;
        case DbModelBuilderVersion.V4_1:
        case DbModelBuilderVersion.V5_0_Net4:
          return 2.0;
        default:
          throw new ArgumentOutOfRangeException(nameof (modelBuilderVersion));
      }
    }

    public static bool IsEF6OrHigher(this DbModelBuilderVersion modelBuilderVersion)
    {
      if (modelBuilderVersion < DbModelBuilderVersion.V6_0)
        return modelBuilderVersion == DbModelBuilderVersion.Latest;
      return true;
    }
  }
}
