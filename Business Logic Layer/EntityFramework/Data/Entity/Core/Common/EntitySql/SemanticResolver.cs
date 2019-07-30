// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.SemanticResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.EntitySql.AST;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class SemanticResolver
  {
    private readonly List<ScopeRegion> _scopeRegions = new List<ScopeRegion>();
    private readonly ParserOptions _parserOptions;
    private readonly Dictionary<string, DbParameterReferenceExpression> _parameters;
    private readonly Dictionary<string, DbVariableReferenceExpression> _variables;
    private readonly TypeResolver _typeResolver;
    private readonly ScopeManager _scopeManager;
    private bool _ignoreEntityContainerNameResolution;
    private GroupAggregateInfo _currentGroupAggregateInfo;
    private uint _namegenCounter;

    internal static SemanticResolver Create(
      Perspective perspective,
      ParserOptions parserOptions,
      IEnumerable<DbParameterReferenceExpression> parameters,
      IEnumerable<DbVariableReferenceExpression> variables)
    {
      return new SemanticResolver(parserOptions, SemanticResolver.ProcessParameters(parameters, parserOptions), SemanticResolver.ProcessVariables(variables, parserOptions), new TypeResolver(perspective, parserOptions));
    }

    internal SemanticResolver CloneForInlineFunctionConversion()
    {
      return new SemanticResolver(this._parserOptions, this._parameters, this._variables, this._typeResolver);
    }

    private SemanticResolver(
      ParserOptions parserOptions,
      Dictionary<string, DbParameterReferenceExpression> parameters,
      Dictionary<string, DbVariableReferenceExpression> variables,
      TypeResolver typeResolver)
    {
      this._parserOptions = parserOptions;
      this._parameters = parameters;
      this._variables = variables;
      this._typeResolver = typeResolver;
      this._scopeManager = new ScopeManager((IEqualityComparer<string>) this.NameComparer);
      this.EnterScopeRegion();
      foreach (DbVariableReferenceExpression varRef in this._variables.Values)
        this.CurrentScope.Add(varRef.VariableName, (ScopeEntry) new FreeVariableScopeEntry(varRef));
    }

    private static Dictionary<string, DbParameterReferenceExpression> ProcessParameters(
      IEnumerable<DbParameterReferenceExpression> paramDefs,
      ParserOptions parserOptions)
    {
      Dictionary<string, DbParameterReferenceExpression> dictionary = new Dictionary<string, DbParameterReferenceExpression>((IEqualityComparer<string>) parserOptions.NameComparer);
      if (paramDefs != null)
      {
        foreach (DbParameterReferenceExpression paramDef in paramDefs)
        {
          if (dictionary.ContainsKey(paramDef.ParameterName))
            throw new EntitySqlException(Strings.MultipleDefinitionsOfParameter((object) paramDef.ParameterName));
          dictionary.Add(paramDef.ParameterName, paramDef);
        }
      }
      return dictionary;
    }

    private static Dictionary<string, DbVariableReferenceExpression> ProcessVariables(
      IEnumerable<DbVariableReferenceExpression> varDefs,
      ParserOptions parserOptions)
    {
      Dictionary<string, DbVariableReferenceExpression> dictionary = new Dictionary<string, DbVariableReferenceExpression>((IEqualityComparer<string>) parserOptions.NameComparer);
      if (varDefs != null)
      {
        foreach (DbVariableReferenceExpression varDef in varDefs)
        {
          if (dictionary.ContainsKey(varDef.VariableName))
            throw new EntitySqlException(Strings.MultipleDefinitionsOfVariable((object) varDef.VariableName));
          dictionary.Add(varDef.VariableName, varDef);
        }
      }
      return dictionary;
    }

    internal Dictionary<string, DbParameterReferenceExpression> Parameters
    {
      get
      {
        return this._parameters;
      }
    }

    internal Dictionary<string, DbVariableReferenceExpression> Variables
    {
      get
      {
        return this._variables;
      }
    }

    internal TypeResolver TypeResolver
    {
      get
      {
        return this._typeResolver;
      }
    }

    internal ParserOptions ParserOptions
    {
      get
      {
        return this._parserOptions;
      }
    }

    internal StringComparer NameComparer
    {
      get
      {
        return this._parserOptions.NameComparer;
      }
    }

    internal IEnumerable<ScopeRegion> ScopeRegions
    {
      get
      {
        return (IEnumerable<ScopeRegion>) this._scopeRegions;
      }
    }

    internal ScopeRegion CurrentScopeRegion
    {
      get
      {
        return this._scopeRegions[this._scopeRegions.Count - 1];
      }
    }

    internal Scope CurrentScope
    {
      get
      {
        return this._scopeManager.CurrentScope;
      }
    }

    internal int CurrentScopeIndex
    {
      get
      {
        return this._scopeManager.CurrentScopeIndex;
      }
    }

    internal GroupAggregateInfo CurrentGroupAggregateInfo
    {
      get
      {
        return this._currentGroupAggregateInfo;
      }
    }

    private DbExpression GetExpressionFromScopeEntry(
      ScopeEntry scopeEntry,
      int scopeIndex,
      string varName,
      ErrorContext errCtx)
    {
      DbExpression dbExpression = scopeEntry.GetExpression(varName, errCtx);
      if (this._currentGroupAggregateInfo != null)
      {
        ScopeRegion definingScopeRegion = this.GetDefiningScopeRegion(scopeIndex);
        if (definingScopeRegion.ScopeRegionIndex <= this._currentGroupAggregateInfo.DefiningScopeRegion.ScopeRegionIndex)
        {
          this._currentGroupAggregateInfo.UpdateScopeIndex(scopeIndex, this);
          IGroupExpressionExtendedInfo expressionExtendedInfo = scopeEntry as IGroupExpressionExtendedInfo;
          if (expressionExtendedInfo != null)
          {
            GroupAggregateInfo groupAggregateInfo = this._currentGroupAggregateInfo;
            while (groupAggregateInfo != null && groupAggregateInfo.DefiningScopeRegion.ScopeRegionIndex >= definingScopeRegion.ScopeRegionIndex && groupAggregateInfo.DefiningScopeRegion.ScopeRegionIndex != definingScopeRegion.ScopeRegionIndex)
              groupAggregateInfo = groupAggregateInfo.ContainingAggregate;
            if (groupAggregateInfo == null || groupAggregateInfo.DefiningScopeRegion.ScopeRegionIndex < definingScopeRegion.ScopeRegionIndex)
              groupAggregateInfo = this._currentGroupAggregateInfo;
            switch (groupAggregateInfo.AggregateKind)
            {
              case GroupAggregateKind.Function:
                if (expressionExtendedInfo.GroupVarBasedExpression != null)
                {
                  dbExpression = expressionExtendedInfo.GroupVarBasedExpression;
                  break;
                }
                break;
              case GroupAggregateKind.Partition:
                if (expressionExtendedInfo.GroupAggBasedExpression != null)
                {
                  dbExpression = expressionExtendedInfo.GroupAggBasedExpression;
                  break;
                }
                break;
            }
          }
        }
      }
      return dbExpression;
    }

    internal IDisposable EnterIgnoreEntityContainerNameResolution()
    {
      this._ignoreEntityContainerNameResolution = true;
      return (IDisposable) new Disposer((Action) (() => this._ignoreEntityContainerNameResolution = false));
    }

    internal ExpressionResolution ResolveSimpleName(
      string name,
      bool leftHandSideOfMemberAccess,
      ErrorContext errCtx)
    {
      ScopeEntry scopeEntry;
      int scopeIndex;
      if (this.TryScopeLookup(name, out scopeEntry, out scopeIndex))
      {
        if (scopeEntry.EntryKind == ScopeEntryKind.SourceVar && ((SourceScopeEntry) scopeEntry).IsJoinClauseLeftExpr)
        {
          string joinLeftCorrelation = Strings.InvalidJoinLeftCorrelation;
          throw EntitySqlException.Create(errCtx, joinLeftCorrelation, (Exception) null);
        }
        this.SetScopeRegionCorrelationFlag(scopeIndex);
        return (ExpressionResolution) new ValueExpression(this.GetExpressionFromScopeEntry(scopeEntry, scopeIndex, name, errCtx));
      }
      EntityContainer defaultContainer = this.TypeResolver.Perspective.GetDefaultContainer();
      ExpressionResolution resolution;
      if (defaultContainer != null && this.TryResolveEntityContainerMemberAccess(defaultContainer, name, out resolution))
        return resolution;
      EntityContainer entityContainer;
      if (!this._ignoreEntityContainerNameResolution && this.TypeResolver.Perspective.TryGetEntityContainer(name, this._parserOptions.NameComparisonCaseInsensitive, out entityContainer))
        return (ExpressionResolution) new EntityContainerExpression(entityContainer);
      return (ExpressionResolution) this.TypeResolver.ResolveUnqualifiedName(name, leftHandSideOfMemberAccess, errCtx);
    }

    internal MetadataMember ResolveSimpleFunctionName(
      string name,
      ErrorContext errCtx)
    {
      MetadataMember metadataMember = this.TypeResolver.ResolveUnqualifiedName(name, false, errCtx);
      if (metadataMember.MetadataMemberClass == MetadataMemberClass.Namespace)
      {
        EntityContainer defaultContainer = this.TypeResolver.Perspective.GetDefaultContainer();
        ExpressionResolution resolution;
        if (defaultContainer != null && this.TryResolveEntityContainerMemberAccess(defaultContainer, name, out resolution) && resolution.ExpressionClass == ExpressionResolutionClass.MetadataMember)
          metadataMember = (MetadataMember) resolution;
      }
      return metadataMember;
    }

    private bool TryScopeLookup(string key, out ScopeEntry scopeEntry, out int scopeIndex)
    {
      scopeEntry = (ScopeEntry) null;
      scopeIndex = -1;
      for (int currentScopeIndex = this.CurrentScopeIndex; currentScopeIndex >= 0; --currentScopeIndex)
      {
        if (this._scopeManager.GetScopeByIndex(currentScopeIndex).TryLookup(key, out scopeEntry))
        {
          scopeIndex = currentScopeIndex;
          return true;
        }
      }
      return false;
    }

    internal MetadataMember ResolveMetadataMemberName(
      string[] name,
      ErrorContext errCtx)
    {
      return this.TypeResolver.ResolveMetadataMemberName(name, errCtx);
    }

    internal ValueExpression ResolvePropertyAccess(
      DbExpression valueExpr,
      string name,
      ErrorContext errCtx)
    {
      DbExpression propertyExpr;
      if (this.TryResolveAsPropertyAccess(valueExpr, name, out propertyExpr))
        return new ValueExpression(propertyExpr);
      if (this.TryResolveAsRefPropertyAccess(valueExpr, name, errCtx, out propertyExpr))
        return new ValueExpression(propertyExpr);
      if (TypeSemantics.IsCollectionType(valueExpr.ResultType))
      {
        string errorMessage = Strings.NotAMemberOfCollection((object) name, (object) valueExpr.ResultType.EdmType.FullName);
        throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
      }
      string errorMessage1 = Strings.NotAMemberOfType((object) name, (object) valueExpr.ResultType.EdmType.FullName);
      throw EntitySqlException.Create(errCtx, errorMessage1, (Exception) null);
    }

    private bool TryResolveAsPropertyAccess(
      DbExpression valueExpr,
      string name,
      out DbExpression propertyExpr)
    {
      propertyExpr = (DbExpression) null;
      EdmMember outMember;
      if (!Helper.IsStructuralType(valueExpr.ResultType.EdmType) || !this.TypeResolver.Perspective.TryGetMember((StructuralType) valueExpr.ResultType.EdmType, name, this._parserOptions.NameComparisonCaseInsensitive, out outMember))
        return false;
      propertyExpr = (DbExpression) DbExpressionBuilder.CreatePropertyExpressionFromMember(valueExpr, outMember);
      return true;
    }

    private bool TryResolveAsRefPropertyAccess(
      DbExpression valueExpr,
      string name,
      ErrorContext errCtx,
      out DbExpression propertyExpr)
    {
      propertyExpr = (DbExpression) null;
      if (!TypeSemantics.IsReferenceType(valueExpr.ResultType))
        return false;
      DbExpression valueExpr1 = (DbExpression) valueExpr.Deref();
      TypeUsage resultType = valueExpr1.ResultType;
      if (this.TryResolveAsPropertyAccess(valueExpr1, name, out propertyExpr))
        return true;
      string errorMessage = Strings.InvalidDeRefProperty((object) name, (object) resultType.EdmType.FullName, (object) valueExpr.ResultType.EdmType.FullName);
      throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
    }

    internal ExpressionResolution ResolveEntityContainerMemberAccess(
      EntityContainer entityContainer,
      string name,
      ErrorContext errCtx)
    {
      ExpressionResolution resolution;
      if (this.TryResolveEntityContainerMemberAccess(entityContainer, name, out resolution))
        return resolution;
      string entityContainer1 = Strings.MemberDoesNotBelongToEntityContainer((object) name, (object) entityContainer.Name);
      throw EntitySqlException.Create(errCtx, entityContainer1, (Exception) null);
    }

    private bool TryResolveEntityContainerMemberAccess(
      EntityContainer entityContainer,
      string name,
      out ExpressionResolution resolution)
    {
      EntitySetBase outSet;
      if (this.TypeResolver.Perspective.TryGetExtent(entityContainer, name, this._parserOptions.NameComparisonCaseInsensitive, out outSet))
      {
        resolution = (ExpressionResolution) new ValueExpression((DbExpression) outSet.Scan());
        return true;
      }
      EdmFunction functionImport;
      if (this.TypeResolver.Perspective.TryGetFunctionImport(entityContainer, name, this._parserOptions.NameComparisonCaseInsensitive, out functionImport))
      {
        resolution = (ExpressionResolution) new MetadataFunctionGroup(functionImport.FullName, (IList<EdmFunction>) new EdmFunction[1]
        {
          functionImport
        });
        return true;
      }
      resolution = (ExpressionResolution) null;
      return false;
    }

    internal MetadataMember ResolveMetadataMemberAccess(
      MetadataMember metadataMember,
      string name,
      ErrorContext errCtx)
    {
      return this.TypeResolver.ResolveMetadataMemberAccess(metadataMember, name, errCtx);
    }

    internal bool TryResolveInternalAggregateName(
      string name,
      ErrorContext errCtx,
      out DbExpression dbExpression)
    {
      ScopeEntry scopeEntry;
      int scopeIndex;
      if (this.TryScopeLookup(name, out scopeEntry, out scopeIndex))
      {
        this.SetScopeRegionCorrelationFlag(scopeIndex);
        dbExpression = scopeEntry.GetExpression(name, errCtx);
        return true;
      }
      dbExpression = (DbExpression) null;
      return false;
    }

    internal bool TryResolveDotExprAsGroupKeyAlternativeName(
      DotExpr dotExpr,
      out ValueExpression groupKeyResolution)
    {
      groupKeyResolution = (ValueExpression) null;
      string[] names;
      ScopeEntry scopeEntry;
      int scopeIndex;
      if (this.IsInAnyGroupScope() && dotExpr.IsMultipartIdentifier(out names) && this.TryScopeLookup(TypeResolver.GetFullName(names), out scopeEntry, out scopeIndex))
      {
        IGetAlternativeName getAlternativeName = scopeEntry as IGetAlternativeName;
        if (getAlternativeName != null && getAlternativeName.AlternativeName != null && ((IEnumerable<string>) names).SequenceEqual<string>((IEnumerable<string>) getAlternativeName.AlternativeName, (IEqualityComparer<string>) this.NameComparer))
        {
          this.SetScopeRegionCorrelationFlag(scopeIndex);
          groupKeyResolution = new ValueExpression(this.GetExpressionFromScopeEntry(scopeEntry, scopeIndex, TypeResolver.GetFullName(names), dotExpr.ErrCtx));
          return true;
        }
      }
      return false;
    }

    internal string GenerateInternalName(string hint)
    {
      return "_##" + hint + this._namegenCounter++.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    private string CreateNewAlias(DbExpression expr)
    {
      DbScanExpression dbScanExpression = expr as DbScanExpression;
      if (dbScanExpression != null)
        return dbScanExpression.Target.Name;
      DbPropertyExpression propertyExpression = expr as DbPropertyExpression;
      if (propertyExpression != null)
        return propertyExpression.Property.Name;
      DbVariableReferenceExpression referenceExpression = expr as DbVariableReferenceExpression;
      if (referenceExpression != null)
        return referenceExpression.VariableName;
      return this.GenerateInternalName(string.Empty);
    }

    internal string InferAliasName(AliasedExpr aliasedExpr, DbExpression convertedExpression)
    {
      if (aliasedExpr.Alias != null)
        return aliasedExpr.Alias.Name;
      Identifier expr1 = aliasedExpr.Expr as Identifier;
      if (expr1 != null)
        return expr1.Name;
      DotExpr expr2 = aliasedExpr.Expr as DotExpr;
      string[] names;
      if (expr2 != null && expr2.IsMultipartIdentifier(out names))
        return names[names.Length - 1];
      return this.CreateNewAlias(convertedExpression);
    }

    internal IDisposable EnterScopeRegion()
    {
      this._scopeManager.EnterScope();
      this._scopeRegions.Add(new ScopeRegion(this._scopeManager, this.CurrentScopeIndex, this._scopeRegions.Count));
      return (IDisposable) new Disposer((Action) (() =>
      {
        this.CurrentScopeRegion.GroupAggregateInfos.Each<GroupAggregateInfo>((Action<GroupAggregateInfo>) (groupAggregateInfo => groupAggregateInfo.DetachFromAstNode()));
        this.CurrentScopeRegion.RollbackAllScopes();
        this._scopeRegions.Remove(this.CurrentScopeRegion);
      }));
    }

    internal void RollbackToScope(int scopeIndex)
    {
      this._scopeManager.RollbackToScope(scopeIndex);
    }

    internal void EnterScope()
    {
      this._scopeManager.EnterScope();
    }

    internal void LeaveScope()
    {
      this._scopeManager.LeaveScope();
    }

    internal bool IsInAnyGroupScope()
    {
      for (int index = 0; index < this._scopeRegions.Count; ++index)
      {
        if (this._scopeRegions[index].IsAggregating)
          return true;
      }
      return false;
    }

    internal ScopeRegion GetDefiningScopeRegion(int scopeIndex)
    {
      for (int index = this._scopeRegions.Count - 1; index >= 0; --index)
      {
        if (this._scopeRegions[index].ContainsScope(scopeIndex))
          return this._scopeRegions[index];
      }
      return (ScopeRegion) null;
    }

    private void SetScopeRegionCorrelationFlag(int scopeIndex)
    {
      this.GetDefiningScopeRegion(scopeIndex).WasResolutionCorrelated = true;
    }

    internal IDisposable EnterFunctionAggregate(
      MethodExpr methodExpr,
      ErrorContext errCtx,
      out FunctionAggregateInfo aggregateInfo)
    {
      aggregateInfo = new FunctionAggregateInfo(methodExpr, errCtx, this._currentGroupAggregateInfo, this.CurrentScopeRegion);
      return this.EnterGroupAggregate((GroupAggregateInfo) aggregateInfo);
    }

    internal IDisposable EnterGroupPartition(
      GroupPartitionExpr groupPartitionExpr,
      ErrorContext errCtx,
      out GroupPartitionInfo aggregateInfo)
    {
      aggregateInfo = new GroupPartitionInfo(groupPartitionExpr, errCtx, this._currentGroupAggregateInfo, this.CurrentScopeRegion);
      return this.EnterGroupAggregate((GroupAggregateInfo) aggregateInfo);
    }

    internal IDisposable EnterGroupKeyDefinition(
      GroupAggregateKind aggregateKind,
      ErrorContext errCtx,
      out GroupKeyAggregateInfo aggregateInfo)
    {
      aggregateInfo = new GroupKeyAggregateInfo(aggregateKind, errCtx, this._currentGroupAggregateInfo, this.CurrentScopeRegion);
      return this.EnterGroupAggregate((GroupAggregateInfo) aggregateInfo);
    }

    private IDisposable EnterGroupAggregate(GroupAggregateInfo aggregateInfo)
    {
      this._currentGroupAggregateInfo = aggregateInfo;
      return (IDisposable) new Disposer((Action) (() =>
      {
        this._currentGroupAggregateInfo = aggregateInfo.ContainingAggregate;
        aggregateInfo.ValidateAndComputeEvaluatingScopeRegion(this);
      }));
    }

    internal static EdmFunction ResolveFunctionOverloads(
      IList<EdmFunction> functionsMetadata,
      IList<TypeUsage> argTypes,
      bool isGroupAggregateFunction,
      out bool isAmbiguous)
    {
      return FunctionOverloadResolver.ResolveFunctionOverloads(functionsMetadata, argTypes, new Func<TypeUsage, IEnumerable<TypeUsage>>(SemanticResolver.UntypedNullAwareFlattenArgumentType), new Func<TypeUsage, TypeUsage, IEnumerable<TypeUsage>>(SemanticResolver.UntypedNullAwareFlattenParameterType), new Func<TypeUsage, TypeUsage, bool>(SemanticResolver.UntypedNullAwareIsPromotableTo), new Func<TypeUsage, TypeUsage, bool>(SemanticResolver.UntypedNullAwareIsStructurallyEqual), isGroupAggregateFunction, out isAmbiguous);
    }

    internal static TFunctionMetadata ResolveFunctionOverloads<TFunctionMetadata, TFunctionParameterMetadata>(
      IList<TFunctionMetadata> functionsMetadata,
      IList<TypeUsage> argTypes,
      Func<TFunctionMetadata, IList<TFunctionParameterMetadata>> getSignatureParams,
      Func<TFunctionParameterMetadata, TypeUsage> getParameterTypeUsage,
      Func<TFunctionParameterMetadata, ParameterMode> getParameterMode,
      bool isGroupAggregateFunction,
      out bool isAmbiguous)
      where TFunctionMetadata : class
    {
      return FunctionOverloadResolver.ResolveFunctionOverloads<TFunctionMetadata, TFunctionParameterMetadata>(functionsMetadata, argTypes, getSignatureParams, getParameterTypeUsage, getParameterMode, new Func<TypeUsage, IEnumerable<TypeUsage>>(SemanticResolver.UntypedNullAwareFlattenArgumentType), new Func<TypeUsage, TypeUsage, IEnumerable<TypeUsage>>(SemanticResolver.UntypedNullAwareFlattenParameterType), new Func<TypeUsage, TypeUsage, bool>(SemanticResolver.UntypedNullAwareIsPromotableTo), new Func<TypeUsage, TypeUsage, bool>(SemanticResolver.UntypedNullAwareIsStructurallyEqual), isGroupAggregateFunction, out isAmbiguous);
    }

    private static IEnumerable<TypeUsage> UntypedNullAwareFlattenArgumentType(
      TypeUsage argType)
    {
      if (argType == null)
        return (IEnumerable<TypeUsage>) new TypeUsage[1];
      return TypeSemantics.FlattenType(argType);
    }

    private static IEnumerable<TypeUsage> UntypedNullAwareFlattenParameterType(
      TypeUsage paramType,
      TypeUsage argType)
    {
      if (argType != null)
        return TypeSemantics.FlattenType(paramType);
      return (IEnumerable<TypeUsage>) new TypeUsage[1]
      {
        paramType
      };
    }

    private static bool UntypedNullAwareIsPromotableTo(TypeUsage fromType, TypeUsage toType)
    {
      if (fromType == null)
        return !Helper.IsCollectionType((GlobalItem) toType.EdmType);
      return TypeSemantics.IsPromotableTo(fromType, toType);
    }

    private static bool UntypedNullAwareIsStructurallyEqual(TypeUsage fromType, TypeUsage toType)
    {
      if (fromType == null)
        return SemanticResolver.UntypedNullAwareIsPromotableTo(fromType, toType);
      return TypeSemantics.IsStructurallyEqual(fromType, toType);
    }
  }
}
