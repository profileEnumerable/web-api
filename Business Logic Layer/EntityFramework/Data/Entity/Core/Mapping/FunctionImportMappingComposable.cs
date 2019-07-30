// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportMappingComposable
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Core.Query.PlanCompiler;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents a mapping from a model function import to a store composable function.
  /// </summary>
  public sealed class FunctionImportMappingComposable : FunctionImportMapping
  {
    private readonly FunctionImportResultMapping _resultMapping;
    private readonly EntityContainerMapping _containerMapping;
    private readonly DbParameterReferenceExpression[] m_commandParameters;
    private readonly List<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>> m_structuralTypeMappings;
    private readonly EdmProperty[] m_targetFunctionKeys;
    private System.Data.Entity.Core.Query.InternalTrees.Node m_internalTreeNode;

    /// <summary>
    /// Initializes a new FunctionImportMappingComposable instance.
    /// </summary>
    /// <param name="functionImport">The model function import.</param>
    /// <param name="targetFunction">The store composable function.</param>
    /// <param name="resultMapping">The result mapping for the function import.</param>
    /// <param name="containerMapping">The parent container mapping.</param>
    public FunctionImportMappingComposable(
      EdmFunction functionImport,
      EdmFunction targetFunction,
      FunctionImportResultMapping resultMapping,
      EntityContainerMapping containerMapping)
      : base(Check.NotNull<EdmFunction>(functionImport, nameof (functionImport)), Check.NotNull<EdmFunction>(targetFunction, nameof (targetFunction)))
    {
      Check.NotNull<FunctionImportResultMapping>(resultMapping, nameof (resultMapping));
      Check.NotNull<EntityContainerMapping>(containerMapping, nameof (containerMapping));
      if (!functionImport.IsComposableAttribute)
        throw new ArgumentException(Strings.NonComposableFunctionCannotBeMappedAsComposable((object) nameof (functionImport)));
      if (!targetFunction.IsComposableAttribute)
        throw new ArgumentException(Strings.NonComposableFunctionCannotBeMappedAsComposable((object) nameof (targetFunction)));
      EdmType returnType;
      if (!MetadataHelper.TryGetFunctionImportReturnType<EdmType>(functionImport, 0, out returnType))
        throw new ArgumentException(Strings.InvalidReturnTypeForComposableFunction);
      EdmFunction edmFunction = containerMapping.StorageMappingItemCollection != null ? containerMapping.StorageMappingItemCollection.StoreItemCollection.ConvertToCTypeFunction(targetFunction) : StoreItemCollection.ConvertFunctionSignatureToCType(targetFunction);
      RowType tvfReturnType1 = TypeHelpers.GetTvfReturnType(edmFunction);
      RowType tvfReturnType2 = TypeHelpers.GetTvfReturnType(targetFunction);
      if (tvfReturnType1 == null)
        throw new ArgumentException(Strings.Mapping_FunctionImport_ResultMapping_InvalidSType((object) functionImport.Identity), nameof (functionImport));
      List<EdmSchemaError> parsingErrors = new List<EdmSchemaError>();
      FunctionImportMappingComposableHelper composableHelper = new FunctionImportMappingComposableHelper(containerMapping, string.Empty, parsingErrors);
      FunctionImportMappingComposable mapping;
      if (Helper.IsStructuralType(returnType))
        composableHelper.TryCreateFunctionImportMappingComposableWithStructuralResult(functionImport, edmFunction, resultMapping.SourceList, tvfReturnType1, tvfReturnType2, (IXmlLineInfo) LineInfo.Empty, out mapping);
      else
        composableHelper.TryCreateFunctionImportMappingComposableWithScalarResult(functionImport, edmFunction, targetFunction, returnType, tvfReturnType1, (IXmlLineInfo) LineInfo.Empty, out mapping);
      if (mapping == null)
        throw new InvalidOperationException(parsingErrors.Count > 0 ? parsingErrors[0].Message : string.Empty);
      this._containerMapping = mapping._containerMapping;
      this.m_commandParameters = mapping.m_commandParameters;
      this.m_structuralTypeMappings = mapping.m_structuralTypeMappings;
      this.m_targetFunctionKeys = mapping.m_targetFunctionKeys;
      this._resultMapping = resultMapping;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    internal FunctionImportMappingComposable(
      EdmFunction functionImport,
      EdmFunction targetFunction,
      List<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>> structuralTypeMappings)
      : base(functionImport, targetFunction)
    {
      if (!functionImport.IsComposableAttribute)
        throw new ArgumentException(Strings.NonComposableFunctionCannotBeMappedAsComposable((object) nameof (functionImport)));
      if (!targetFunction.IsComposableAttribute)
        throw new ArgumentException(Strings.NonComposableFunctionCannotBeMappedAsComposable((object) nameof (targetFunction)));
      EdmType returnType;
      if (!MetadataHelper.TryGetFunctionImportReturnType<EdmType>(functionImport, 0, out returnType))
        throw new ArgumentException(Strings.InvalidReturnTypeForComposableFunction);
      if (!TypeSemantics.IsScalarType(returnType) && (structuralTypeMappings == null || structuralTypeMappings.Count == 0))
        throw new ArgumentException(Strings.StructuralTypeMappingsMustNotBeNullForFunctionImportsReturingNonScalarValues);
      this.m_structuralTypeMappings = structuralTypeMappings;
    }

    internal FunctionImportMappingComposable(
      EdmFunction functionImport,
      EdmFunction targetFunction,
      List<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>> structuralTypeMappings,
      EdmProperty[] targetFunctionKeys,
      EntityContainerMapping containerMapping)
      : base(functionImport, targetFunction)
    {
      this._containerMapping = containerMapping;
      this.m_commandParameters = functionImport.Parameters.Select<FunctionParameter, DbParameterReferenceExpression>((Func<FunctionParameter, DbParameterReferenceExpression>) (p => TypeHelpers.GetPrimitiveTypeUsageForScalar(p.TypeUsage).Parameter(p.Name))).ToArray<DbParameterReferenceExpression>();
      this.m_structuralTypeMappings = structuralTypeMappings;
      this.m_targetFunctionKeys = targetFunctionKeys;
    }

    /// <summary>Gets the result mapping for the function import.</summary>
    public FunctionImportResultMapping ResultMapping
    {
      get
      {
        return this._resultMapping;
      }
    }

    internal override void SetReadOnly()
    {
      MappingItem.SetReadOnly((MappingItem) this._resultMapping);
      base.SetReadOnly();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal ReadOnlyCollection<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>> StructuralTypeMappings
    {
      get
      {
        if (this.m_structuralTypeMappings != null)
          return new ReadOnlyCollection<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>>((IList<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>>) this.m_structuralTypeMappings);
        return (ReadOnlyCollection<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>>) null;
      }
    }

    internal EdmProperty[] TvfKeys
    {
      get
      {
        return this.m_targetFunctionKeys;
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "projectOp")]
    internal System.Data.Entity.Core.Query.InternalTrees.Node GetInternalTree(
      Command targetIqtCommand,
      IList<System.Data.Entity.Core.Query.InternalTrees.Node> targetIqtArguments)
    {
      if (this.m_internalTreeNode == null)
      {
        DiscriminatorMap discriminatorMap;
        Command command = ITreeGenerator.Generate(this.GenerateFunctionView(out discriminatorMap), discriminatorMap);
        System.Data.Entity.Core.Query.InternalTrees.Node root = command.Root;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(root.Op.OpType == OpType.PhysicalProject, "Expected a physical projectOp at the root of the tree - found " + (object) root.Op.OpType);
        PhysicalProjectOp op = (PhysicalProjectOp) root.Op;
        System.Data.Entity.Core.Query.InternalTrees.Node child0 = root.Child0;
        command.DisableVarVecEnumCaching();
        System.Data.Entity.Core.Query.InternalTrees.Node relOpNode = child0;
        Var computedVar = op.Outputs[0];
        if (!Command.EqualTypes(op.ColumnMap.Type, this.FunctionImport.ReturnParameter.TypeUsage))
        {
          TypeUsage typeUsage = ((CollectionType) this.FunctionImport.ReturnParameter.TypeUsage.EdmType).TypeUsage;
          System.Data.Entity.Core.Query.InternalTrees.Node node1 = command.CreateNode((Op) command.CreateVarRefOp(computedVar));
          System.Data.Entity.Core.Query.InternalTrees.Node node2 = command.CreateNode((Op) command.CreateSoftCastOp(typeUsage), node1);
          System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = command.CreateVarDefListNode(node2, out computedVar);
          ProjectOp projectOp = command.CreateProjectOp(computedVar);
          relOpNode = command.CreateNode((Op) projectOp, relOpNode, varDefListNode);
        }
        this.m_internalTreeNode = command.BuildCollect(relOpNode, computedVar);
      }
      Dictionary<string, System.Data.Entity.Core.Query.InternalTrees.Node> viewArguments = new Dictionary<string, System.Data.Entity.Core.Query.InternalTrees.Node>(this.m_commandParameters.Length);
      for (int index = 0; index < this.m_commandParameters.Length; ++index)
      {
        DbParameterReferenceExpression commandParameter = this.m_commandParameters[index];
        System.Data.Entity.Core.Query.InternalTrees.Node node = targetIqtArguments[index];
        if (TypeSemantics.IsEnumerationType(node.Op.Type))
          node = targetIqtCommand.CreateNode((Op) targetIqtCommand.CreateSoftCastOp(TypeHelpers.CreateEnumUnderlyingTypeUsage(node.Op.Type)), node);
        viewArguments.Add(commandParameter.ParameterName, node);
      }
      return FunctionImportMappingComposable.FunctionViewOpCopier.Copy(targetIqtCommand, this.m_internalTreeNode, viewArguments);
    }

    internal DbQueryCommandTree GenerateFunctionView(
      out DiscriminatorMap discriminatorMap)
    {
      discriminatorMap = (DiscriminatorMap) null;
      DbExpression storeFunctionInvoke = (DbExpression) this.TargetFunction.Invoke(this.GetParametersForTargetFunctionCall());
      return DbQueryCommandTree.FromValidExpression(this._containerMapping.StorageMappingItemCollection.Workspace, DataSpace.SSpace, this.m_structuralTypeMappings == null ? this.GenerateScalarResultMappingView(storeFunctionInvoke) : this.GenerateStructuralTypeResultMappingView(storeFunctionInvoke, out discriminatorMap), true);
    }

    private IEnumerable<DbExpression> GetParametersForTargetFunctionCall()
    {
      foreach (FunctionParameter parameter in this.TargetFunction.Parameters)
      {
        FunctionParameter targetParameter = parameter;
        FunctionParameter functionImportParameter = this.FunctionImport.Parameters.Single<FunctionParameter>((Func<FunctionParameter, bool>) (p => p.Name == targetParameter.Name));
        yield return (DbExpression) this.m_commandParameters[this.FunctionImport.Parameters.IndexOf(functionImportParameter)];
      }
    }

    private DbExpression GenerateStructuralTypeResultMappingView(
      DbExpression storeFunctionInvoke,
      out DiscriminatorMap discriminatorMap)
    {
      discriminatorMap = (DiscriminatorMap) null;
      DbExpression dbExpression = storeFunctionInvoke;
      DbExpression queryView;
      if (this.m_structuralTypeMappings.Count == 1)
      {
        Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>> structuralTypeMapping = this.m_structuralTypeMappings[0];
        StructuralType structuralType = structuralTypeMapping.Item1;
        List<ConditionPropertyMapping> conditions = structuralTypeMapping.Item2;
        List<PropertyMapping> propertyMappings = structuralTypeMapping.Item3;
        if (conditions.Count > 0)
          dbExpression = (DbExpression) dbExpression.Where((Func<DbExpression, DbExpression>) (row => FunctionImportMappingComposable.GenerateStructuralTypeConditionsPredicate(conditions, row)));
        DbExpressionBinding input = dbExpression.BindAs("row");
        DbExpression structuralTypeMappingView = FunctionImportMappingComposable.GenerateStructuralTypeMappingView(structuralType, propertyMappings, (DbExpression) input.Variable);
        queryView = (DbExpression) input.Project(structuralTypeMappingView);
      }
      else
      {
        DbExpressionBinding binding = dbExpression.BindAs("row");
        List<DbExpression> list = this.m_structuralTypeMappings.Select<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>, DbExpression>((Func<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>, DbExpression>) (m => FunctionImportMappingComposable.GenerateStructuralTypeConditionsPredicate(m.Item2, (DbExpression) binding.Variable))).ToList<DbExpression>();
        binding = binding.Filter(Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) list.ToArray(), (Func<DbExpression, DbExpression, DbExpression>) ((prev, next) => (DbExpression) prev.Or(next)))).BindAs("row");
        List<DbExpression> source = new List<DbExpression>(this.m_structuralTypeMappings.Count);
        foreach (Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>> structuralTypeMapping in this.m_structuralTypeMappings)
        {
          StructuralType structuralType = structuralTypeMapping.Item1;
          List<PropertyMapping> propertyMappings = structuralTypeMapping.Item3;
          source.Add(FunctionImportMappingComposable.GenerateStructuralTypeMappingView(structuralType, propertyMappings, (DbExpression) binding.Variable));
        }
        DbExpression projection = (DbExpression) DbExpressionBuilder.Case(list.Take<DbExpression>(this.m_structuralTypeMappings.Count - 1), source.Take<DbExpression>(this.m_structuralTypeMappings.Count - 1), source[this.m_structuralTypeMappings.Count - 1]);
        queryView = (DbExpression) binding.Project(projection);
        DiscriminatorMap.TryCreateDiscriminatorMap(this.FunctionImport.EntitySet, queryView, out discriminatorMap);
      }
      return queryView;
    }

    private static DbExpression GenerateStructuralTypeMappingView(
      StructuralType structuralType,
      List<PropertyMapping> propertyMappings,
      DbExpression row)
    {
      List<DbExpression> dbExpressionList = new List<DbExpression>(TypeHelpers.GetAllStructuralMembers((EdmType) structuralType).Count);
      for (int index = 0; index < propertyMappings.Count; ++index)
      {
        PropertyMapping propertyMapping = propertyMappings[index];
        dbExpressionList.Add(FunctionImportMappingComposable.GeneratePropertyMappingView(propertyMapping, row));
      }
      return (DbExpression) TypeUsage.Create((EdmType) structuralType).New((IEnumerable<DbExpression>) dbExpressionList);
    }

    private static DbExpression GenerateStructuralTypeConditionsPredicate(
      List<ConditionPropertyMapping> conditions,
      DbExpression row)
    {
      return Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) conditions.Select<ConditionPropertyMapping, DbExpression>((Func<ConditionPropertyMapping, DbExpression>) (c => FunctionImportMappingComposable.GeneratePredicate(c, row))).ToArray<DbExpression>(), (Func<DbExpression, DbExpression, DbExpression>) ((prev, next) => (DbExpression) prev.And(next)));
    }

    private static DbExpression GeneratePredicate(
      ConditionPropertyMapping condition,
      DbExpression row)
    {
      DbExpression columnRef = FunctionImportMappingComposable.GenerateColumnRef(row, condition.Column);
      if (!condition.IsNull.HasValue)
        return (DbExpression) columnRef.Equal((DbExpression) columnRef.ResultType.Constant(condition.Value));
      if (!condition.IsNull.Value)
        return (DbExpression) columnRef.IsNull().Not();
      return (DbExpression) columnRef.IsNull();
    }

    private static DbExpression GeneratePropertyMappingView(
      PropertyMapping mapping,
      DbExpression row)
    {
      ScalarPropertyMapping scalarPropertyMapping = (ScalarPropertyMapping) mapping;
      return FunctionImportMappingComposable.GenerateScalarPropertyMappingView(scalarPropertyMapping.Property, scalarPropertyMapping.Column, row);
    }

    private static DbExpression GenerateScalarPropertyMappingView(
      EdmProperty edmProperty,
      EdmProperty columnProperty,
      DbExpression row)
    {
      DbExpression columnRef = FunctionImportMappingComposable.GenerateColumnRef(row, columnProperty);
      if (!TypeSemantics.IsEqual(columnRef.ResultType, edmProperty.TypeUsage))
        columnRef = (DbExpression) columnRef.CastTo(edmProperty.TypeUsage);
      return columnRef;
    }

    private static DbExpression GenerateColumnRef(DbExpression row, EdmProperty column)
    {
      RowType edmType = (RowType) row.ResultType.EdmType;
      return (DbExpression) row.Property(column.Name);
    }

    private DbExpression GenerateScalarResultMappingView(
      DbExpression storeFunctionInvoke)
    {
      DbExpression source = storeFunctionInvoke;
      CollectionType functionImportReturnType;
      MetadataHelper.TryGetFunctionImportReturnCollectionType(this.FunctionImport, 0, out functionImportReturnType);
      EdmProperty column = ((RowType) ((CollectionType) source.ResultType.EdmType).TypeUsage.EdmType).Properties[0];
      Func<DbExpression, DbExpression> scalarView = (Func<DbExpression, DbExpression>) (row =>
      {
        DbPropertyExpression propertyExpression = row.Property(column);
        if (TypeSemantics.IsEqual(functionImportReturnType.TypeUsage, column.TypeUsage))
          return (DbExpression) propertyExpression;
        return (DbExpression) propertyExpression.CastTo(functionImportReturnType.TypeUsage);
      });
      return (DbExpression) source.Select<DbExpression>((Func<DbExpression, DbExpression>) (row => scalarView(row)));
    }

    private sealed class FunctionViewOpCopier : OpCopier
    {
      private readonly Dictionary<string, System.Data.Entity.Core.Query.InternalTrees.Node> m_viewArguments;

      private FunctionViewOpCopier(Command cmd, Dictionary<string, System.Data.Entity.Core.Query.InternalTrees.Node> viewArguments)
        : base(cmd)
      {
        this.m_viewArguments = viewArguments;
      }

      internal static System.Data.Entity.Core.Query.InternalTrees.Node Copy(
        Command cmd,
        System.Data.Entity.Core.Query.InternalTrees.Node viewNode,
        Dictionary<string, System.Data.Entity.Core.Query.InternalTrees.Node> viewArguments)
      {
        return new FunctionImportMappingComposable.FunctionViewOpCopier(cmd, viewArguments).CopyNode(viewNode);
      }

      public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
        VarRefOp op,
        System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node n1;
        if (op.Var.VarType == VarType.Parameter && this.m_viewArguments.TryGetValue(((ParameterVar) op.Var).ParameterName, out n1))
          return OpCopier.Copy(this.m_destCmd, n1);
        return base.Visit(op, n);
      }
    }
  }
}
