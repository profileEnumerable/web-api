// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ColumnMapCopier
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class ColumnMapCopier : ColumnMapVisitorWithResults<ColumnMap, VarMap>
  {
    private static readonly ColumnMapCopier _instance = new ColumnMapCopier();

    private ColumnMapCopier()
    {
    }

    internal static ColumnMap Copy(ColumnMap columnMap, VarMap replacementVarMap)
    {
      return columnMap.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) ColumnMapCopier._instance, replacementVarMap);
    }

    private static Var GetReplacementVar(Var originalVar, VarMap replacementVarMap)
    {
      Var key = originalVar;
      while (replacementVarMap.TryGetValue(key, out originalVar) && originalVar != key)
        key = originalVar;
      return key;
    }

    internal TListType[] VisitList<TListType>(TListType[] tList, VarMap replacementVarMap) where TListType : ColumnMap
    {
      TListType[] listTypeArray = new TListType[tList.Length];
      for (int index = 0; index < tList.Length; ++index)
        listTypeArray[index] = (TListType) tList[index].Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      return listTypeArray;
    }

    protected override EntityIdentity VisitEntityIdentity(
      DiscriminatedEntityIdentity entityIdentity,
      VarMap replacementVarMap)
    {
      SimpleColumnMap entitySetColumn = (SimpleColumnMap) entityIdentity.EntitySetColumnMap.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      SimpleColumnMap[] keyColumns = this.VisitList<SimpleColumnMap>(entityIdentity.Keys, replacementVarMap);
      return (EntityIdentity) new DiscriminatedEntityIdentity(entitySetColumn, entityIdentity.EntitySetMap, keyColumns);
    }

    protected override EntityIdentity VisitEntityIdentity(
      SimpleEntityIdentity entityIdentity,
      VarMap replacementVarMap)
    {
      SimpleColumnMap[] keyColumns = this.VisitList<SimpleColumnMap>(entityIdentity.Keys, replacementVarMap);
      return (EntityIdentity) new SimpleEntityIdentity(entityIdentity.EntitySet, keyColumns);
    }

    internal override ColumnMap Visit(
      ComplexTypeColumnMap columnMap,
      VarMap replacementVarMap)
    {
      SimpleColumnMap nullSentinel = columnMap.NullSentinel;
      if (nullSentinel != null)
        nullSentinel = (SimpleColumnMap) nullSentinel.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      ColumnMap[] properties = this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap);
      return (ColumnMap) new ComplexTypeColumnMap(columnMap.Type, columnMap.Name, properties, nullSentinel);
    }

    internal override ColumnMap Visit(
      DiscriminatedCollectionColumnMap columnMap,
      VarMap replacementVarMap)
    {
      ColumnMap elementMap = columnMap.Element.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      SimpleColumnMap discriminator = (SimpleColumnMap) columnMap.Discriminator.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      SimpleColumnMap[] keys = this.VisitList<SimpleColumnMap>(columnMap.Keys, replacementVarMap);
      SimpleColumnMap[] foreignKeys = this.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, replacementVarMap);
      return (ColumnMap) new DiscriminatedCollectionColumnMap(columnMap.Type, columnMap.Name, elementMap, keys, foreignKeys, discriminator, columnMap.DiscriminatorValue);
    }

    internal override ColumnMap Visit(EntityColumnMap columnMap, VarMap replacementVarMap)
    {
      EntityIdentity entityIdentity = this.VisitEntityIdentity(columnMap.EntityIdentity, replacementVarMap);
      ColumnMap[] properties = this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap);
      return (ColumnMap) new EntityColumnMap(columnMap.Type, columnMap.Name, properties, entityIdentity);
    }

    internal override ColumnMap Visit(
      SimplePolymorphicColumnMap columnMap,
      VarMap replacementVarMap)
    {
      SimpleColumnMap typeDiscriminator = (SimpleColumnMap) columnMap.TypeDiscriminator.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      Dictionary<object, TypedColumnMap> typeChoices = new Dictionary<object, TypedColumnMap>(columnMap.TypeChoices.Comparer);
      foreach (KeyValuePair<object, TypedColumnMap> typeChoice in columnMap.TypeChoices)
      {
        TypedColumnMap typedColumnMap = (TypedColumnMap) typeChoice.Value.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
        typeChoices[typeChoice.Key] = typedColumnMap;
      }
      ColumnMap[] baseTypeColumns = this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap);
      return (ColumnMap) new SimplePolymorphicColumnMap(columnMap.Type, columnMap.Name, baseTypeColumns, typeDiscriminator, typeChoices);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ColumnMapCopier")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "MultipleDiscriminatorPolymorphicColumnMap")]
    internal override ColumnMap Visit(
      MultipleDiscriminatorPolymorphicColumnMap columnMap,
      VarMap replacementVarMap)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "unexpected MultipleDiscriminatorPolymorphicColumnMap in ColumnMapCopier");
      return (ColumnMap) null;
    }

    internal override ColumnMap Visit(RecordColumnMap columnMap, VarMap replacementVarMap)
    {
      SimpleColumnMap nullSentinel = columnMap.NullSentinel;
      if (nullSentinel != null)
        nullSentinel = (SimpleColumnMap) nullSentinel.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      ColumnMap[] properties = this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap);
      return (ColumnMap) new RecordColumnMap(columnMap.Type, columnMap.Name, properties, nullSentinel);
    }

    internal override ColumnMap Visit(RefColumnMap columnMap, VarMap replacementVarMap)
    {
      EntityIdentity entityIdentity = this.VisitEntityIdentity(columnMap.EntityIdentity, replacementVarMap);
      return (ColumnMap) new RefColumnMap(columnMap.Type, columnMap.Name, entityIdentity);
    }

    internal override ColumnMap Visit(ScalarColumnMap columnMap, VarMap replacementVarMap)
    {
      return (ColumnMap) new ScalarColumnMap(columnMap.Type, columnMap.Name, columnMap.CommandId, columnMap.ColumnPos);
    }

    internal override ColumnMap Visit(
      SimpleCollectionColumnMap columnMap,
      VarMap replacementVarMap)
    {
      ColumnMap elementMap = columnMap.Element.Accept<ColumnMap, VarMap>((ColumnMapVisitorWithResults<ColumnMap, VarMap>) this, replacementVarMap);
      SimpleColumnMap[] keys = this.VisitList<SimpleColumnMap>(columnMap.Keys, replacementVarMap);
      SimpleColumnMap[] foreignKeys = this.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, replacementVarMap);
      return (ColumnMap) new SimpleCollectionColumnMap(columnMap.Type, columnMap.Name, elementMap, keys, foreignKeys);
    }

    internal override ColumnMap Visit(VarRefColumnMap columnMap, VarMap replacementVarMap)
    {
      Var replacementVar = ColumnMapCopier.GetReplacementVar(columnMap.Var, replacementVarMap);
      return (ColumnMap) new VarRefColumnMap(columnMap.Type, columnMap.Name, replacementVar);
    }
  }
}
