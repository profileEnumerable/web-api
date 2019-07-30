// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.MetadataMappingHasherVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  internal class MetadataMappingHasherVisitor : BaseMetadataMappingVisitor
  {
    private Dictionary<object, int> m_itemsAlreadySeen = new Dictionary<object, int>();
    private CompressingHashBuilder m_hashSourceBuilder;
    private int m_instanceNumber;
    private EdmItemCollection m_EdmItemCollection;
    private double m_MappingVersion;

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    private MetadataMappingHasherVisitor(double mappingVersion, bool sortSequence)
      : base(sortSequence)
    {
      this.m_MappingVersion = mappingVersion;
      this.m_hashSourceBuilder = new CompressingHashBuilder(MetadataHelper.CreateMetadataHashAlgorithm(this.m_MappingVersion));
    }

    protected override void Visit(EntityContainerMapping entityContainerMapping)
    {
      this.m_MappingVersion = entityContainerMapping.StorageMappingItemCollection.MappingVersion;
      this.m_EdmItemCollection = entityContainerMapping.StorageMappingItemCollection.EdmItemCollection;
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) entityContainerMapping, out instanceIndex))
        return;
      if (this.m_itemsAlreadySeen.Count > 1)
      {
        this.Clean();
        this.Visit(entityContainerMapping);
      }
      else
      {
        this.AddObjectStartDumpToHashBuilder((object) entityContainerMapping, instanceIndex);
        this.AddObjectContentToHashBuilder((object) entityContainerMapping.Identity);
        this.AddV2ObjectContentToHashBuilder((object) entityContainerMapping.GenerateUpdateViews, this.m_MappingVersion);
        base.Visit(entityContainerMapping);
        this.AddObjectEndDumpToHashBuilder();
      }
    }

    protected override void Visit(EntityContainer entityContainer)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) entityContainer, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) entityContainer, instanceIndex);
      this.AddObjectContentToHashBuilder((object) entityContainer.Identity);
      base.Visit(entityContainer);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(EntitySetBaseMapping setMapping)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) setMapping, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) setMapping, instanceIndex);
      base.Visit(setMapping);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(TypeMapping typeMapping)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) typeMapping, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) typeMapping, instanceIndex);
      base.Visit(typeMapping);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(MappingFragment mappingFragment)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) mappingFragment, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) mappingFragment, instanceIndex);
      this.AddV2ObjectContentToHashBuilder((object) mappingFragment.IsSQueryDistinct, this.m_MappingVersion);
      base.Visit(mappingFragment);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(PropertyMapping propertyMapping)
    {
      base.Visit(propertyMapping);
    }

    protected override void Visit(ComplexPropertyMapping complexPropertyMapping)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) complexPropertyMapping, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) complexPropertyMapping, instanceIndex);
      base.Visit(complexPropertyMapping);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(ComplexTypeMapping complexTypeMapping)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) complexTypeMapping, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) complexTypeMapping, instanceIndex);
      base.Visit(complexTypeMapping);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(ConditionPropertyMapping conditionPropertyMapping)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) conditionPropertyMapping, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) conditionPropertyMapping, instanceIndex);
      this.AddObjectContentToHashBuilder((object) conditionPropertyMapping.IsNull);
      this.AddObjectContentToHashBuilder(conditionPropertyMapping.Value);
      base.Visit(conditionPropertyMapping);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(ScalarPropertyMapping scalarPropertyMapping)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) scalarPropertyMapping, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) scalarPropertyMapping, instanceIndex);
      base.Visit(scalarPropertyMapping);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(EntitySetBase entitySetBase)
    {
      base.Visit(entitySetBase);
    }

    protected override void Visit(EntitySet entitySet)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) entitySet, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) entitySet, instanceIndex);
      this.AddObjectContentToHashBuilder((object) entitySet.Name);
      this.AddObjectContentToHashBuilder((object) entitySet.Schema);
      this.AddObjectContentToHashBuilder((object) entitySet.Table);
      base.Visit(entitySet);
      foreach (EdmType edmType in this.GetSequence<EdmType>(MetadataHelper.GetTypeAndSubtypesOf((EdmType) entitySet.ElementType, (ItemCollection) this.m_EdmItemCollection, false).Where<EdmType>((Func<EdmType, bool>) (type => type != entitySet.ElementType)), (Func<EdmType, string>) (it => it.Identity)))
        this.Visit(edmType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(AssociationSet associationSet)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) associationSet, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) associationSet, instanceIndex);
      this.AddObjectContentToHashBuilder((object) associationSet.Identity);
      this.AddObjectContentToHashBuilder((object) associationSet.Schema);
      this.AddObjectContentToHashBuilder((object) associationSet.Table);
      base.Visit(associationSet);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(EntityType entityType)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) entityType, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) entityType, instanceIndex);
      this.AddObjectContentToHashBuilder((object) entityType.Abstract);
      this.AddObjectContentToHashBuilder((object) entityType.Identity);
      base.Visit(entityType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(AssociationSetEnd associationSetEnd)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) associationSetEnd, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) associationSetEnd, instanceIndex);
      this.AddObjectContentToHashBuilder((object) associationSetEnd.Identity);
      base.Visit(associationSetEnd);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(AssociationType associationType)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) associationType, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) associationType, instanceIndex);
      this.AddObjectContentToHashBuilder((object) associationType.Abstract);
      this.AddObjectContentToHashBuilder((object) associationType.Identity);
      base.Visit(associationType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(EdmProperty edmProperty)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) edmProperty, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) edmProperty, instanceIndex);
      this.AddObjectContentToHashBuilder(edmProperty.DefaultValue);
      this.AddObjectContentToHashBuilder((object) edmProperty.Identity);
      this.AddObjectContentToHashBuilder((object) edmProperty.IsStoreGeneratedComputed);
      this.AddObjectContentToHashBuilder((object) edmProperty.IsStoreGeneratedIdentity);
      this.AddObjectContentToHashBuilder((object) edmProperty.Nullable);
      base.Visit(edmProperty);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(NavigationProperty navigationProperty)
    {
    }

    protected override void Visit(EdmMember edmMember)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) edmMember, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) edmMember, instanceIndex);
      this.AddObjectContentToHashBuilder((object) edmMember.Identity);
      this.AddObjectContentToHashBuilder((object) edmMember.IsStoreGeneratedComputed);
      this.AddObjectContentToHashBuilder((object) edmMember.IsStoreGeneratedIdentity);
      base.Visit(edmMember);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(AssociationEndMember associationEndMember)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) associationEndMember, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) associationEndMember, instanceIndex);
      this.AddObjectContentToHashBuilder((object) associationEndMember.DeleteBehavior);
      this.AddObjectContentToHashBuilder((object) associationEndMember.Identity);
      this.AddObjectContentToHashBuilder((object) associationEndMember.IsStoreGeneratedComputed);
      this.AddObjectContentToHashBuilder((object) associationEndMember.IsStoreGeneratedIdentity);
      this.AddObjectContentToHashBuilder((object) associationEndMember.RelationshipMultiplicity);
      base.Visit(associationEndMember);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(ReferentialConstraint referentialConstraint)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) referentialConstraint, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) referentialConstraint, instanceIndex);
      this.AddObjectContentToHashBuilder((object) referentialConstraint.Identity);
      base.Visit(referentialConstraint);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(RelationshipEndMember relationshipEndMember)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) relationshipEndMember, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) relationshipEndMember, instanceIndex);
      this.AddObjectContentToHashBuilder((object) relationshipEndMember.DeleteBehavior);
      this.AddObjectContentToHashBuilder((object) relationshipEndMember.Identity);
      this.AddObjectContentToHashBuilder((object) relationshipEndMember.IsStoreGeneratedComputed);
      this.AddObjectContentToHashBuilder((object) relationshipEndMember.IsStoreGeneratedIdentity);
      this.AddObjectContentToHashBuilder((object) relationshipEndMember.RelationshipMultiplicity);
      base.Visit(relationshipEndMember);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(TypeUsage typeUsage)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) typeUsage, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) typeUsage, instanceIndex);
      base.Visit(typeUsage);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(RelationshipType relationshipType)
    {
      base.Visit(relationshipType);
    }

    protected override void Visit(EdmType edmType)
    {
      base.Visit(edmType);
    }

    protected override void Visit(EnumType enumType)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) enumType, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) enumType, instanceIndex);
      this.AddObjectContentToHashBuilder((object) enumType.Identity);
      this.Visit(enumType.UnderlyingType);
      base.Visit(enumType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(EnumMember enumMember)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) enumMember, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) enumMember, instanceIndex);
      this.AddObjectContentToHashBuilder((object) enumMember.Name);
      this.AddObjectContentToHashBuilder(enumMember.Value);
      base.Visit(enumMember);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(CollectionType collectionType)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) collectionType, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) collectionType, instanceIndex);
      this.AddObjectContentToHashBuilder((object) collectionType.Identity);
      base.Visit(collectionType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(RefType refType)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) refType, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) refType, instanceIndex);
      this.AddObjectContentToHashBuilder((object) refType.Identity);
      base.Visit(refType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(EntityTypeBase entityTypeBase)
    {
      base.Visit(entityTypeBase);
    }

    protected override void Visit(Facet facet)
    {
      int instanceIndex;
      if (facet.Name != "Nullable" || !this.AddObjectToSeenListAndHashBuilder((object) facet, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) facet, instanceIndex);
      this.AddObjectContentToHashBuilder((object) facet.Identity);
      this.AddObjectContentToHashBuilder(facet.Value);
      base.Visit(facet);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(EdmFunction edmFunction)
    {
    }

    protected override void Visit(ComplexType complexType)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) complexType, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) complexType, instanceIndex);
      this.AddObjectContentToHashBuilder((object) complexType.Abstract);
      this.AddObjectContentToHashBuilder((object) complexType.Identity);
      base.Visit(complexType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(PrimitiveType primitiveType)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) primitiveType, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) primitiveType, instanceIndex);
      this.AddObjectContentToHashBuilder((object) primitiveType.Name);
      this.AddObjectContentToHashBuilder((object) primitiveType.NamespaceName);
      base.Visit(primitiveType);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(FunctionParameter functionParameter)
    {
      int instanceIndex;
      if (!this.AddObjectToSeenListAndHashBuilder((object) functionParameter, out instanceIndex))
        return;
      this.AddObjectStartDumpToHashBuilder((object) functionParameter, instanceIndex);
      this.AddObjectContentToHashBuilder((object) functionParameter.Identity);
      this.AddObjectContentToHashBuilder((object) functionParameter.Mode);
      base.Visit(functionParameter);
      this.AddObjectEndDumpToHashBuilder();
    }

    protected override void Visit(DbProviderManifest providerManifest)
    {
    }

    internal string HashValue
    {
      get
      {
        return this.m_hashSourceBuilder.ComputeHash();
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    private void Clean()
    {
      this.m_hashSourceBuilder = new CompressingHashBuilder(MetadataHelper.CreateMetadataHashAlgorithm(this.m_MappingVersion));
      this.m_instanceNumber = 0;
      this.m_itemsAlreadySeen = new Dictionary<object, int>();
    }

    private bool TryAddSeenItem(object o, out int indexSeen)
    {
      if (this.m_itemsAlreadySeen.TryGetValue(o, out indexSeen))
        return false;
      this.m_itemsAlreadySeen.Add(o, this.m_instanceNumber);
      indexSeen = this.m_instanceNumber;
      ++this.m_instanceNumber;
      return true;
    }

    private bool AddObjectToSeenListAndHashBuilder(object o, out int instanceIndex)
    {
      if (o == null)
      {
        instanceIndex = -1;
        return false;
      }
      if (this.TryAddSeenItem(o, out instanceIndex))
        return true;
      this.AddObjectStartDumpToHashBuilder(o, instanceIndex);
      this.AddSeenObjectToHashBuilder(instanceIndex);
      this.AddObjectEndDumpToHashBuilder();
      return false;
    }

    private void AddSeenObjectToHashBuilder(int instanceIndex)
    {
      this.m_hashSourceBuilder.AppendLine("Instance Reference: " + (object) instanceIndex);
    }

    private void AddObjectStartDumpToHashBuilder(object o, int objectIndex)
    {
      this.m_hashSourceBuilder.AppendObjectStartDump(o, objectIndex);
    }

    private void AddObjectEndDumpToHashBuilder()
    {
      this.m_hashSourceBuilder.AppendObjectEndDump();
    }

    private void AddObjectContentToHashBuilder(object content)
    {
      if (content != null)
      {
        IFormattable formattable = content as IFormattable;
        if (formattable != null)
          this.m_hashSourceBuilder.AppendLine(formattable.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture));
        else
          this.m_hashSourceBuilder.AppendLine(content.ToString());
      }
      else
        this.m_hashSourceBuilder.AppendLine("NULL");
    }

    private void AddV2ObjectContentToHashBuilder(object content, double version)
    {
      if (version < 2.0)
        return;
      this.AddObjectContentToHashBuilder(content);
    }

    internal static string GetMappingClosureHash(
      double mappingVersion,
      EntityContainerMapping entityContainerMapping,
      bool sortSequence = true)
    {
      MetadataMappingHasherVisitor mappingHasherVisitor = new MetadataMappingHasherVisitor(mappingVersion, sortSequence);
      mappingHasherVisitor.Visit(entityContainerMapping);
      return mappingHasherVisitor.HashValue;
    }
  }
}
