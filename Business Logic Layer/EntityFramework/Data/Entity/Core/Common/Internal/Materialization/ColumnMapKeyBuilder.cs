// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.ColumnMapKeyBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class ColumnMapKeyBuilder : ColumnMapVisitor<int>
  {
    private readonly StringBuilder _builder = new StringBuilder();
    private readonly SpanIndex _spanIndex;

    private ColumnMapKeyBuilder(SpanIndex spanIndex)
    {
      this._spanIndex = spanIndex;
    }

    internal static string GetColumnMapKey(ColumnMap columnMap, SpanIndex spanIndex)
    {
      ColumnMapKeyBuilder columnMapKeyBuilder = new ColumnMapKeyBuilder(spanIndex);
      columnMap.Accept<int>((ColumnMapVisitor<int>) columnMapKeyBuilder, 0);
      return columnMapKeyBuilder._builder.ToString();
    }

    internal void Append(string value)
    {
      this._builder.Append(value);
    }

    internal void Append(string prefix, Type type)
    {
      this.Append(prefix, type.AssemblyQualifiedName);
    }

    internal void Append(string prefix, TypeUsage type)
    {
      if (type == null)
        return;
      InitializerMetadata initializerMetadata;
      if (InitializerMetadata.TryGetInitializerMetadata(type, out initializerMetadata))
        initializerMetadata.AppendColumnMapKey(this);
      this.Append(prefix, type.EdmType);
    }

    internal void Append(string prefix, EdmType type)
    {
      if (type == null)
        return;
      this.Append(prefix, type.NamespaceName);
      this.Append(".", type.Name);
      if (type.BuiltInTypeKind != BuiltInTypeKind.RowType || this._spanIndex == null)
        return;
      this.Append("<<");
      Dictionary<int, AssociationEndMember> spanMap = this._spanIndex.GetSpanMap((RowType) type);
      if (spanMap != null)
      {
        string str = string.Empty;
        foreach (KeyValuePair<int, AssociationEndMember> keyValuePair in spanMap)
        {
          this.Append(str);
          this.AppendValue("C", (object) keyValuePair.Key);
          this.Append(":", (EdmType) keyValuePair.Value.DeclaringType);
          this.Append(".", keyValuePair.Value.Name);
          str = ",";
        }
      }
      this.Append(">>");
    }

    private void Append(string prefix, string value)
    {
      this.Append(prefix);
      this.Append("'");
      this.Append(value);
      this.Append("'");
    }

    private void Append(string prefix, ColumnMap columnMap)
    {
      this.Append(prefix);
      this.Append("[");
      columnMap?.Accept<int>((ColumnMapVisitor<int>) this, 0);
      this.Append("]");
    }

    private void Append(string prefix, IEnumerable<ColumnMap> elements)
    {
      this.Append(prefix);
      this.Append("{");
      if (elements != null)
      {
        string prefix1 = string.Empty;
        foreach (ColumnMap element in elements)
        {
          this.Append(prefix1, element);
          prefix1 = ",";
        }
      }
      this.Append("}");
    }

    private void Append(string prefix, EntityIdentity entityIdentity)
    {
      this.Append(prefix);
      this.Append("[");
      this.Append(",K", (IEnumerable<ColumnMap>) entityIdentity.Keys);
      SimpleEntityIdentity simpleEntityIdentity = entityIdentity as SimpleEntityIdentity;
      if (simpleEntityIdentity != null)
      {
        this.Append(",", simpleEntityIdentity.EntitySet);
      }
      else
      {
        DiscriminatedEntityIdentity discriminatedEntityIdentity = (DiscriminatedEntityIdentity) entityIdentity;
        this.Append("CM", (ColumnMap) discriminatedEntityIdentity.EntitySetColumnMap);
        foreach (EntitySet entitySet in discriminatedEntityIdentity.EntitySetMap)
          this.Append(",E", entitySet);
      }
      this.Append("]");
    }

    private void Append(string prefix, EntitySet entitySet)
    {
      if (entitySet == null)
        return;
      this.Append(prefix, entitySet.EntityContainer.Name);
      this.Append(".", entitySet.Name);
    }

    private void AppendValue(string prefix, object value)
    {
      this.Append(prefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", value));
    }

    internal override void Visit(ComplexTypeColumnMap columnMap, int dummy)
    {
      this.Append("C-", columnMap.Type);
      this.Append(",N", (ColumnMap) columnMap.NullSentinel);
      this.Append(",P", (IEnumerable<ColumnMap>) columnMap.Properties);
    }

    internal override void Visit(DiscriminatedCollectionColumnMap columnMap, int dummy)
    {
      this.Append("DC-D", (ColumnMap) columnMap.Discriminator);
      this.AppendValue(",DV", columnMap.DiscriminatorValue);
      this.Append(",FK", (IEnumerable<ColumnMap>) columnMap.ForeignKeys);
      this.Append(",K", (IEnumerable<ColumnMap>) columnMap.Keys);
      this.Append(",E", columnMap.Element);
    }

    internal override void Visit(EntityColumnMap columnMap, int dummy)
    {
      this.Append("E-", columnMap.Type);
      this.Append(",N", (ColumnMap) columnMap.NullSentinel);
      this.Append(",P", (IEnumerable<ColumnMap>) columnMap.Properties);
      this.Append(",I", columnMap.EntityIdentity);
    }

    internal override void Visit(SimplePolymorphicColumnMap columnMap, int dummy)
    {
      this.Append("SP-", columnMap.Type);
      this.Append(",D", (ColumnMap) columnMap.TypeDiscriminator);
      this.Append(",N", (ColumnMap) columnMap.NullSentinel);
      this.Append(",P", (IEnumerable<ColumnMap>) columnMap.Properties);
      foreach (KeyValuePair<object, TypedColumnMap> typeChoice in columnMap.TypeChoices)
      {
        this.AppendValue(",K", typeChoice.Key);
        this.Append(":", (ColumnMap) typeChoice.Value);
      }
    }

    internal override void Visit(RecordColumnMap columnMap, int dummy)
    {
      this.Append("R-", columnMap.Type);
      this.Append(",N", (ColumnMap) columnMap.NullSentinel);
      this.Append(",P", (IEnumerable<ColumnMap>) columnMap.Properties);
    }

    internal override void Visit(RefColumnMap columnMap, int dummy)
    {
      this.Append("Ref-", columnMap.EntityIdentity);
      EntityType referencedEntityType;
      TypeHelpers.TryGetRefEntityType(columnMap.Type, out referencedEntityType);
      this.Append(",T", (EdmType) referencedEntityType);
    }

    internal override void Visit(ScalarColumnMap columnMap, int dummy)
    {
      this.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "S({0}-{1}:{2})", (object) columnMap.CommandId, (object) columnMap.ColumnPos, (object) columnMap.Type.Identity));
    }

    internal override void Visit(SimpleCollectionColumnMap columnMap, int dummy)
    {
      this.Append("DC-FK", (IEnumerable<ColumnMap>) columnMap.ForeignKeys);
      this.Append(",K", (IEnumerable<ColumnMap>) columnMap.Keys);
      this.Append(",E", columnMap.Element);
    }

    internal override void Visit(VarRefColumnMap columnMap, int dummy)
    {
    }

    internal override void Visit(
      MultipleDiscriminatorPolymorphicColumnMap columnMap,
      int dummy)
    {
      this.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "MD-{0}", (object) Guid.NewGuid()));
    }
  }
}
