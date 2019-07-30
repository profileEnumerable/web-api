// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.RelationshipFixer`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  [Serializable]
  internal class RelationshipFixer<TSourceEntity, TTargetEntity> : IRelationshipFixer
    where TSourceEntity : class
    where TTargetEntity : class
  {
    private readonly RelationshipMultiplicity _sourceRoleMultiplicity;
    private readonly RelationshipMultiplicity _targetRoleMultiplicity;

    internal RelationshipFixer(
      RelationshipMultiplicity sourceRoleMultiplicity,
      RelationshipMultiplicity targetRoleMultiplicity)
    {
      this._sourceRoleMultiplicity = sourceRoleMultiplicity;
      this._targetRoleMultiplicity = targetRoleMultiplicity;
    }

    RelatedEnd IRelationshipFixer.CreateSourceEnd(
      RelationshipNavigation navigation,
      RelationshipManager relationshipManager)
    {
      return relationshipManager.CreateRelatedEnd<TTargetEntity, TSourceEntity>(navigation, this._targetRoleMultiplicity, this._sourceRoleMultiplicity, (RelatedEnd) null);
    }
  }
}
