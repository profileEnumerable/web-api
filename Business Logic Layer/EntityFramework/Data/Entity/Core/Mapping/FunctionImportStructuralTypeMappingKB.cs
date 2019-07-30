// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportStructuralTypeMappingKB
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  internal sealed class FunctionImportStructuralTypeMappingKB
  {
    private readonly ItemCollection m_itemCollection;
    private readonly KeyToListMap<EntityType, LineInfo> m_entityTypeLineInfos;
    private readonly KeyToListMap<EntityType, LineInfo> m_isTypeOfLineInfos;
    internal readonly ReadOnlyCollection<EntityType> MappedEntityTypes;
    internal readonly ReadOnlyCollection<string> DiscriminatorColumns;
    internal readonly ReadOnlyCollection<FunctionImportNormalizedEntityTypeMapping> NormalizedEntityTypeMappings;
    internal readonly Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping> ReturnTypeColumnsRenameMapping;

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal FunctionImportStructuralTypeMappingKB(
      IEnumerable<FunctionImportStructuralTypeMapping> structuralTypeMappings,
      ItemCollection itemCollection)
    {
      this.m_itemCollection = itemCollection;
      if (structuralTypeMappings.Count<FunctionImportStructuralTypeMapping>() == 0)
      {
        this.ReturnTypeColumnsRenameMapping = new Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping>();
        this.NormalizedEntityTypeMappings = new ReadOnlyCollection<FunctionImportNormalizedEntityTypeMapping>((IList<FunctionImportNormalizedEntityTypeMapping>) new List<FunctionImportNormalizedEntityTypeMapping>());
        this.DiscriminatorColumns = new ReadOnlyCollection<string>((IList<string>) new List<string>());
        this.MappedEntityTypes = new ReadOnlyCollection<EntityType>((IList<EntityType>) new List<EntityType>());
      }
      else
      {
        IEnumerable<FunctionImportEntityTypeMapping> source1 = structuralTypeMappings.OfType<FunctionImportEntityTypeMapping>();
        if (source1 != null && source1.FirstOrDefault<FunctionImportEntityTypeMapping>() != null)
        {
          Dictionary<EntityType, Collection<FunctionImportReturnTypePropertyMapping>> isOfTypeEntityTypeColumnsRenameMapping = new Dictionary<EntityType, Collection<FunctionImportReturnTypePropertyMapping>>();
          Dictionary<EntityType, Collection<FunctionImportReturnTypePropertyMapping>> entityTypeColumnsRenameMapping = new Dictionary<EntityType, Collection<FunctionImportReturnTypePropertyMapping>>();
          List<FunctionImportNormalizedEntityTypeMapping> entityTypeMappingList = new List<FunctionImportNormalizedEntityTypeMapping>();
          this.MappedEntityTypes = new ReadOnlyCollection<EntityType>((IList<EntityType>) source1.SelectMany<FunctionImportEntityTypeMapping, EntityType>((Func<FunctionImportEntityTypeMapping, IEnumerable<EntityType>>) (mapping => mapping.GetMappedEntityTypes(this.m_itemCollection))).Distinct<EntityType>().ToList<EntityType>());
          this.DiscriminatorColumns = new ReadOnlyCollection<string>((IList<string>) source1.SelectMany<FunctionImportEntityTypeMapping, string>((Func<FunctionImportEntityTypeMapping, IEnumerable<string>>) (mapping => mapping.GetDiscriminatorColumns())).Distinct<string>().ToList<string>());
          this.m_entityTypeLineInfos = new KeyToListMap<EntityType, LineInfo>((IEqualityComparer<EntityType>) EqualityComparer<EntityType>.Default);
          this.m_isTypeOfLineInfos = new KeyToListMap<EntityType, LineInfo>((IEqualityComparer<EntityType>) EqualityComparer<EntityType>.Default);
          foreach (FunctionImportEntityTypeMapping entityTypeMapping in source1)
          {
            foreach (EntityType entityType in entityTypeMapping.EntityTypes)
              this.m_entityTypeLineInfos.Add(entityType, entityTypeMapping.LineInfo);
            foreach (EntityType ofTypeEntityType in entityTypeMapping.IsOfTypeEntityTypes)
              this.m_isTypeOfLineInfos.Add(ofTypeEntityType, entityTypeMapping.LineInfo);
            Dictionary<string, FunctionImportEntityTypeMappingCondition> dictionary = entityTypeMapping.Conditions.ToDictionary<FunctionImportEntityTypeMappingCondition, string, FunctionImportEntityTypeMappingCondition>((Func<FunctionImportEntityTypeMappingCondition, string>) (condition => condition.ColumnName), (Func<FunctionImportEntityTypeMappingCondition, FunctionImportEntityTypeMappingCondition>) (condition => condition));
            List<FunctionImportEntityTypeMappingCondition> columnConditions = new List<FunctionImportEntityTypeMappingCondition>(this.DiscriminatorColumns.Count);
            for (int index = 0; index < this.DiscriminatorColumns.Count; ++index)
            {
              string discriminatorColumn = this.DiscriminatorColumns[index];
              FunctionImportEntityTypeMappingCondition mappingCondition;
              if (dictionary.TryGetValue(discriminatorColumn, out mappingCondition))
                columnConditions.Add(mappingCondition);
              else
                columnConditions.Add((FunctionImportEntityTypeMappingCondition) null);
            }
            bool[] values = new bool[this.MappedEntityTypes.Count];
            Set<EntityType> set = new Set<EntityType>(entityTypeMapping.GetMappedEntityTypes(this.m_itemCollection));
            for (int index = 0; index < this.MappedEntityTypes.Count; ++index)
              values[index] = set.Contains(this.MappedEntityTypes[index]);
            entityTypeMappingList.Add(new FunctionImportNormalizedEntityTypeMapping(this, columnConditions, new BitArray(values)));
            foreach (EntityType ofTypeEntityType in entityTypeMapping.IsOfTypeEntityTypes)
            {
              if (!isOfTypeEntityTypeColumnsRenameMapping.Keys.Contains<EntityType>(ofTypeEntityType))
                isOfTypeEntityTypeColumnsRenameMapping.Add(ofTypeEntityType, new Collection<FunctionImportReturnTypePropertyMapping>());
              foreach (FunctionImportReturnTypePropertyMapping columnsRename in entityTypeMapping.ColumnsRenameList)
                isOfTypeEntityTypeColumnsRenameMapping[ofTypeEntityType].Add(columnsRename);
            }
            foreach (EntityType entityType in entityTypeMapping.EntityTypes)
            {
              if (!entityTypeColumnsRenameMapping.Keys.Contains<EntityType>(entityType))
                entityTypeColumnsRenameMapping.Add(entityType, new Collection<FunctionImportReturnTypePropertyMapping>());
              foreach (FunctionImportReturnTypePropertyMapping columnsRename in entityTypeMapping.ColumnsRenameList)
                entityTypeColumnsRenameMapping[entityType].Add(columnsRename);
            }
          }
          this.ReturnTypeColumnsRenameMapping = new FunctionImportReturnTypeEntityTypeColumnsRenameBuilder(isOfTypeEntityTypeColumnsRenameMapping, entityTypeColumnsRenameMapping).ColumnRenameMapping;
          this.NormalizedEntityTypeMappings = new ReadOnlyCollection<FunctionImportNormalizedEntityTypeMapping>((IList<FunctionImportNormalizedEntityTypeMapping>) entityTypeMappingList);
        }
        else
        {
          IEnumerable<FunctionImportComplexTypeMapping> source2 = structuralTypeMappings.Cast<FunctionImportComplexTypeMapping>();
          this.ReturnTypeColumnsRenameMapping = new Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping>();
          foreach (FunctionImportReturnTypePropertyMapping columnsRename in source2.First<FunctionImportComplexTypeMapping>().ColumnsRenameList)
          {
            FunctionImportReturnTypeStructuralTypeColumnRenameMapping columnRenameMapping = new FunctionImportReturnTypeStructuralTypeColumnRenameMapping(columnsRename.CMember);
            columnRenameMapping.AddRename(new FunctionImportReturnTypeStructuralTypeColumn(columnsRename.SColumn, (StructuralType) source2.First<FunctionImportComplexTypeMapping>().ReturnType, false, columnsRename.LineInfo));
            this.ReturnTypeColumnsRenameMapping.Add(columnsRename.CMember, columnRenameMapping);
          }
          this.NormalizedEntityTypeMappings = new ReadOnlyCollection<FunctionImportNormalizedEntityTypeMapping>((IList<FunctionImportNormalizedEntityTypeMapping>) new List<FunctionImportNormalizedEntityTypeMapping>());
          this.DiscriminatorColumns = new ReadOnlyCollection<string>((IList<string>) new List<string>());
          this.MappedEntityTypes = new ReadOnlyCollection<EntityType>((IList<EntityType>) new List<EntityType>());
        }
      }
    }

    internal bool ValidateTypeConditions(
      bool validateAmbiguity,
      IList<EdmSchemaError> errors,
      string sourceLocation)
    {
      KeyToListMap<EntityType, LineInfo> unreachableEntityTypes;
      KeyToListMap<EntityType, LineInfo> unreachableIsTypeOfs;
      this.GetUnreachableTypes(validateAmbiguity, out unreachableEntityTypes, out unreachableIsTypeOfs);
      bool flag = true;
      foreach (KeyValuePair<EntityType, List<LineInfo>> keyValuePair in unreachableEntityTypes.KeyValuePairs)
      {
        LineInfo lineInfo = keyValuePair.Value.First<LineInfo>();
        string commaSeparatedString = StringUtil.ToCommaSeparatedString((IEnumerable) keyValuePair.Value.Select<LineInfo, int>((Func<LineInfo, int>) (li => li.LineNumber)));
        EdmSchemaError edmSchemaError = new EdmSchemaError(Strings.Mapping_FunctionImport_UnreachableType((object) keyValuePair.Key.FullName, (object) commaSeparatedString), 2076, EdmSchemaErrorSeverity.Error, sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition);
        errors.Add(edmSchemaError);
        flag = false;
      }
      foreach (KeyValuePair<EntityType, List<LineInfo>> keyValuePair in unreachableIsTypeOfs.KeyValuePairs)
      {
        LineInfo lineInfo = keyValuePair.Value.First<LineInfo>();
        string commaSeparatedString = StringUtil.ToCommaSeparatedString((IEnumerable) keyValuePair.Value.Select<LineInfo, int>((Func<LineInfo, int>) (li => li.LineNumber)));
        EdmSchemaError edmSchemaError = new EdmSchemaError(Strings.Mapping_FunctionImport_UnreachableIsTypeOf((object) ("IsTypeOf(" + keyValuePair.Key.FullName + ")"), (object) commaSeparatedString), 2076, EdmSchemaErrorSeverity.Error, sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition);
        errors.Add(edmSchemaError);
        flag = false;
      }
      return flag;
    }

    private void GetUnreachableTypes(
      bool validateAmbiguity,
      out KeyToListMap<EntityType, LineInfo> unreachableEntityTypes,
      out KeyToListMap<EntityType, LineInfo> unreachableIsTypeOfs)
    {
      DomainVariable<string, ValueCondition>[] variables = this.ConstructDomainVariables();
      DomainConstraintConversionContext<string, ValueCondition> converter = new DomainConstraintConversionContext<string, ValueCondition>();
      Vertex[] vertices = this.ConvertMappingConditionsToVertices((ConversionContext<DomainConstraint<string, ValueCondition>>) converter, variables);
      this.CollectUnreachableTypes(validateAmbiguity ? this.FindUnambiguouslyReachableTypes(converter, vertices) : this.FindReachableTypes(converter, vertices), out unreachableEntityTypes, out unreachableIsTypeOfs);
    }

    private DomainVariable<string, ValueCondition>[] ConstructDomainVariables()
    {
      Set<ValueCondition>[] setArray = new Set<ValueCondition>[this.DiscriminatorColumns.Count];
      for (int index = 0; index < setArray.Length; ++index)
      {
        setArray[index] = new Set<ValueCondition>();
        setArray[index].Add(ValueCondition.IsOther);
        setArray[index].Add(ValueCondition.IsNull);
      }
      foreach (FunctionImportNormalizedEntityTypeMapping entityTypeMapping in this.NormalizedEntityTypeMappings)
      {
        for (int index = 0; index < this.DiscriminatorColumns.Count; ++index)
        {
          FunctionImportEntityTypeMappingCondition columnCondition = entityTypeMapping.ColumnConditions[index];
          if (columnCondition != null && !columnCondition.ConditionValue.IsNotNullCondition)
            setArray[index].Add(columnCondition.ConditionValue);
        }
      }
      DomainVariable<string, ValueCondition>[] domainVariableArray = new DomainVariable<string, ValueCondition>[setArray.Length];
      for (int index = 0; index < domainVariableArray.Length; ++index)
        domainVariableArray[index] = new DomainVariable<string, ValueCondition>(this.DiscriminatorColumns[index], setArray[index].MakeReadOnly());
      return domainVariableArray;
    }

    private Vertex[] ConvertMappingConditionsToVertices(
      ConversionContext<DomainConstraint<string, ValueCondition>> converter,
      DomainVariable<string, ValueCondition>[] variables)
    {
      Vertex[] vertexArray = new Vertex[this.NormalizedEntityTypeMappings.Count];
      for (int index1 = 0; index1 < vertexArray.Length; ++index1)
      {
        FunctionImportNormalizedEntityTypeMapping entityTypeMapping = this.NormalizedEntityTypeMappings[index1];
        Vertex left = Vertex.One;
        for (int index2 = 0; index2 < this.DiscriminatorColumns.Count; ++index2)
        {
          FunctionImportEntityTypeMappingCondition columnCondition = entityTypeMapping.ColumnConditions[index2];
          if (columnCondition != null)
          {
            ValueCondition conditionValue = columnCondition.ConditionValue;
            if (conditionValue.IsNotNullCondition)
            {
              TermExpr<DomainConstraint<string, ValueCondition>> term = new TermExpr<DomainConstraint<string, ValueCondition>>(new DomainConstraint<string, ValueCondition>(variables[index2], ValueCondition.IsNull));
              Vertex vertex = converter.TranslateTermToVertex(term);
              left = converter.Solver.And(left, converter.Solver.Not(vertex));
            }
            else
            {
              TermExpr<DomainConstraint<string, ValueCondition>> term = new TermExpr<DomainConstraint<string, ValueCondition>>(new DomainConstraint<string, ValueCondition>(variables[index2], conditionValue));
              left = converter.Solver.And(left, converter.TranslateTermToVertex(term));
            }
          }
        }
        vertexArray[index1] = left;
      }
      return vertexArray;
    }

    private Set<EntityType> FindReachableTypes(
      DomainConstraintConversionContext<string, ValueCondition> converter,
      Vertex[] mappingConditions)
    {
      Vertex[] vertexArray = new Vertex[this.MappedEntityTypes.Count];
      for (int index1 = 0; index1 < vertexArray.Length; ++index1)
      {
        Vertex left = Vertex.One;
        for (int index2 = 0; index2 < this.NormalizedEntityTypeMappings.Count; ++index2)
          left = !this.NormalizedEntityTypeMappings[index2].ImpliedEntityTypes[index1] ? converter.Solver.And(left, converter.Solver.Not(mappingConditions[index2])) : converter.Solver.And(left, mappingConditions[index2]);
        vertexArray[index1] = left;
      }
      Set<EntityType> set = new Set<EntityType>();
      for (int i = 0; i < vertexArray.Length; ++i)
      {
        if (!converter.Solver.And(((IEnumerable<Vertex>) vertexArray).Select<Vertex, Vertex>((Func<Vertex, int, Vertex>) ((typeCondition, ordinal) =>
        {
          if (ordinal != i)
            return converter.Solver.Not(typeCondition);
          return typeCondition;
        }))).IsZero())
          set.Add(this.MappedEntityTypes[i]);
      }
      return set;
    }

    private Set<EntityType> FindUnambiguouslyReachableTypes(
      DomainConstraintConversionContext<string, ValueCondition> converter,
      Vertex[] mappingConditions)
    {
      Vertex[] vertexArray = new Vertex[this.MappedEntityTypes.Count];
      for (int index1 = 0; index1 < vertexArray.Length; ++index1)
      {
        Vertex left = Vertex.One;
        for (int index2 = 0; index2 < this.NormalizedEntityTypeMappings.Count; ++index2)
        {
          if (this.NormalizedEntityTypeMappings[index2].ImpliedEntityTypes[index1])
            left = converter.Solver.And(left, mappingConditions[index2]);
        }
        vertexArray[index1] = left;
      }
      BitArray bitArray = new BitArray(vertexArray.Length, true);
      for (int index1 = 0; index1 < vertexArray.Length; ++index1)
      {
        if (vertexArray[index1].IsZero())
        {
          bitArray[index1] = false;
        }
        else
        {
          for (int index2 = index1 + 1; index2 < vertexArray.Length; ++index2)
          {
            if (!converter.Solver.And(vertexArray[index1], vertexArray[index2]).IsZero())
            {
              bitArray[index1] = false;
              bitArray[index2] = false;
            }
          }
        }
      }
      Set<EntityType> set = new Set<EntityType>();
      for (int index = 0; index < vertexArray.Length; ++index)
      {
        if (bitArray[index])
          set.Add(this.MappedEntityTypes[index]);
      }
      return set;
    }

    private void CollectUnreachableTypes(
      Set<EntityType> reachableTypes,
      out KeyToListMap<EntityType, LineInfo> entityTypes,
      out KeyToListMap<EntityType, LineInfo> isTypeOfEntityTypes)
    {
      entityTypes = new KeyToListMap<EntityType, LineInfo>((IEqualityComparer<EntityType>) EqualityComparer<EntityType>.Default);
      isTypeOfEntityTypes = new KeyToListMap<EntityType, LineInfo>((IEqualityComparer<EntityType>) EqualityComparer<EntityType>.Default);
      if (reachableTypes.Count == this.MappedEntityTypes.Count)
        return;
      foreach (EntityType key in this.m_isTypeOfLineInfos.Keys)
      {
        if (!MetadataHelper.GetTypeAndSubtypesOf((EdmType) key, this.m_itemCollection, false).Cast<EntityType>().Intersect<EntityType>((IEnumerable<EntityType>) reachableTypes).Any<EntityType>())
          isTypeOfEntityTypes.AddRange(key, this.m_isTypeOfLineInfos.EnumerateValues(key));
      }
      foreach (EntityType key in this.m_entityTypeLineInfos.Keys)
      {
        if (!reachableTypes.Contains(key))
          entityTypes.AddRange(key, this.m_entityTypeLineInfos.EnumerateValues(key));
      }
    }
  }
}
