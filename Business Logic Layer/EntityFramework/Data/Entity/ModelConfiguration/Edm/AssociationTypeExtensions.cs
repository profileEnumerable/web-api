// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.AssociationTypeExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class AssociationTypeExtensions
  {
    private const string IsIndependentAnnotation = "IsIndependent";
    private const string IsPrincipalConfiguredAnnotation = "IsPrincipalConfigured";

    public static void MarkIndependent(this AssociationType associationType)
    {
      associationType.GetMetadataProperties().SetAnnotation("IsIndependent", (object) true);
    }

    public static bool IsIndependent(this AssociationType associationType)
    {
      object annotation = associationType.Annotations.GetAnnotation(nameof (IsIndependent));
      if (annotation != null)
        return (bool) annotation;
      return false;
    }

    public static void MarkPrincipalConfigured(this AssociationType associationType)
    {
      associationType.GetMetadataProperties().SetAnnotation("IsPrincipalConfigured", (object) true);
    }

    public static bool IsPrincipalConfigured(this AssociationType associationType)
    {
      object annotation = associationType.Annotations.GetAnnotation(nameof (IsPrincipalConfigured));
      if (annotation != null)
        return (bool) annotation;
      return false;
    }

    public static AssociationEndMember GetOtherEnd(
      this AssociationType associationType,
      AssociationEndMember associationEnd)
    {
      if (associationEnd != associationType.SourceEnd)
        return associationType.SourceEnd;
      return associationType.TargetEnd;
    }

    public static object GetConfiguration(this AssociationType associationType)
    {
      return associationType.Annotations.GetConfiguration();
    }

    public static void SetConfiguration(this AssociationType associationType, object configuration)
    {
      associationType.GetMetadataProperties().SetConfiguration(configuration);
    }

    public static bool IsRequiredToMany(this AssociationType associationType)
    {
      if (associationType.SourceEnd.IsRequired())
        return associationType.TargetEnd.IsMany();
      return false;
    }

    public static bool IsRequiredToRequired(this AssociationType associationType)
    {
      if (associationType.SourceEnd.IsRequired())
        return associationType.TargetEnd.IsRequired();
      return false;
    }

    public static bool IsManyToRequired(this AssociationType associationType)
    {
      if (associationType.SourceEnd.IsMany())
        return associationType.TargetEnd.IsRequired();
      return false;
    }

    public static bool IsManyToMany(this AssociationType associationType)
    {
      if (associationType.SourceEnd.IsMany())
        return associationType.TargetEnd.IsMany();
      return false;
    }

    public static bool IsOneToOne(this AssociationType associationType)
    {
      if (!associationType.SourceEnd.IsMany())
        return !associationType.TargetEnd.IsMany();
      return false;
    }

    public static bool IsSelfReferencing(this AssociationType associationType)
    {
      return associationType.SourceEnd.GetEntityType().GetRootType() == associationType.TargetEnd.GetEntityType().GetRootType();
    }

    public static bool IsRequiredToNonRequired(this AssociationType associationType)
    {
      if (associationType.SourceEnd.IsRequired() && !associationType.TargetEnd.IsRequired())
        return true;
      if (associationType.TargetEnd.IsRequired())
        return !associationType.SourceEnd.IsRequired();
      return false;
    }

    public static bool TryGuessPrincipalAndDependentEnds(
      this AssociationType associationType,
      out AssociationEndMember principalEnd,
      out AssociationEndMember dependentEnd)
    {
      principalEnd = dependentEnd = (AssociationEndMember) null;
      AssociationEndMember sourceEnd = associationType.SourceEnd;
      AssociationEndMember targetEnd = associationType.TargetEnd;
      if (sourceEnd.RelationshipMultiplicity != targetEnd.RelationshipMultiplicity)
      {
        principalEnd = sourceEnd.IsRequired() || sourceEnd.IsOptional() && targetEnd.IsMany() ? sourceEnd : targetEnd;
        dependentEnd = principalEnd == sourceEnd ? targetEnd : sourceEnd;
      }
      return principalEnd != null;
    }
  }
}
