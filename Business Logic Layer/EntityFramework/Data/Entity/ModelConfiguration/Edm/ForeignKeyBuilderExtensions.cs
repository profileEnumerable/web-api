// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.ForeignKeyBuilderExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class ForeignKeyBuilderExtensions
  {
    private const string IsTypeConstraint = "IsTypeConstraint";
    private const string IsSplitConstraint = "IsSplitConstraint";
    private const string AssociationType = "AssociationType";
    private const string PreferredNameAnnotation = "PreferredName";

    public static string GetPreferredName(this ForeignKeyBuilder fk)
    {
      return (string) fk.Annotations.GetAnnotation("PreferredName");
    }

    public static void SetPreferredName(this ForeignKeyBuilder fk, string name)
    {
      fk.GetMetadataProperties().SetAnnotation("PreferredName", (object) name);
    }

    public static bool GetIsTypeConstraint(this ForeignKeyBuilder fk)
    {
      object annotation = fk.Annotations.GetAnnotation("IsTypeConstraint");
      if (annotation != null)
        return (bool) annotation;
      return false;
    }

    public static void SetIsTypeConstraint(this ForeignKeyBuilder fk)
    {
      fk.GetMetadataProperties().SetAnnotation("IsTypeConstraint", (object) true);
    }

    public static void SetIsSplitConstraint(this ForeignKeyBuilder fk)
    {
      fk.GetMetadataProperties().SetAnnotation("IsSplitConstraint", (object) true);
    }

    public static System.Data.Entity.Core.Metadata.Edm.AssociationType GetAssociationType(
      this ForeignKeyBuilder fk)
    {
      return fk.Annotations.GetAnnotation("AssociationType") as System.Data.Entity.Core.Metadata.Edm.AssociationType;
    }

    public static void SetAssociationType(
      this ForeignKeyBuilder fk,
      System.Data.Entity.Core.Metadata.Edm.AssociationType associationType)
    {
      fk.GetMetadataProperties().SetAnnotation("AssociationType", (object) associationType);
    }
  }
}
