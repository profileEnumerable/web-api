// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.RelationshipMultiplicityExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class RelationshipMultiplicityExtensions
  {
    public static bool IsMany(this RelationshipMultiplicity associationEndKind)
    {
      return associationEndKind == RelationshipMultiplicity.Many;
    }

    public static bool IsOptional(this RelationshipMultiplicity associationEndKind)
    {
      return associationEndKind == RelationshipMultiplicity.ZeroOrOne;
    }

    public static bool IsRequired(this RelationshipMultiplicity associationEndKind)
    {
      return associationEndKind == RelationshipMultiplicity.One;
    }
  }
}
