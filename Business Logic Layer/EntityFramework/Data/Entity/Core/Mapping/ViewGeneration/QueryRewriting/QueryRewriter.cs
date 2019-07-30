// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.QueryRewriter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Validation;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class QueryRewriter
  {
    private static readonly Tile<FragmentQuery> _trueViewSurrogate = (Tile<FragmentQuery>) QueryRewriter.CreateTile(FragmentQuery.Create(BoolExpression.True));
    private readonly List<FragmentQuery> _fragmentQueries = new List<FragmentQuery>();
    private readonly List<Tile<FragmentQuery>> _views = new List<Tile<FragmentQuery>>();
    private readonly HashSet<FragmentQuery> _usedViews = new HashSet<FragmentQuery>();
    private List<LeftCellWrapper> _usedCells = new List<LeftCellWrapper>();
    private Dictionary<MemberPath, CaseStatement> _caseStatements = new Dictionary<MemberPath, CaseStatement>();
    private readonly ErrorLog _errorLog = new ErrorLog();
    private readonly MemberPath _extentPath;
    private readonly MemberDomainMap _domainMap;
    private readonly ConfigViewGenerator _config;
    private readonly CqlIdentifiers _identifiers;
    private readonly ViewgenContext _context;
    private readonly RewritingProcessor<Tile<FragmentQuery>> _qp;
    private readonly List<MemberPath> _keyAttributes;
    private readonly FragmentQuery _domainQuery;
    private readonly EdmType _generatedType;
    private BoolExpression _topLevelWhereClause;
    private CellTreeNode _basicView;
    private readonly ViewGenMode _typesGenerationMode;

    internal QueryRewriter(
      EdmType generatedType,
      ViewgenContext context,
      ViewGenMode typesGenerationMode)
    {
      this._typesGenerationMode = typesGenerationMode;
      this._context = context;
      this._generatedType = generatedType;
      this._domainMap = context.MemberMaps.LeftDomainMap;
      this._config = context.Config;
      this._identifiers = context.CqlIdentifiers;
      this._qp = new RewritingProcessor<Tile<FragmentQuery>>((TileProcessor<Tile<FragmentQuery>>) new DefaultTileProcessor<FragmentQuery>((TileQueryProcessor<FragmentQuery>) context.LeftFragmentQP));
      this._extentPath = new MemberPath(context.Extent);
      this._keyAttributes = new List<MemberPath>(MemberPath.GetKeyMembers(context.Extent, this._domainMap));
      foreach (LeftCellWrapper leftCellWrapper in this._context.AllWrappersForExtent)
      {
        FragmentQuery fragmentQuery = leftCellWrapper.FragmentQuery;
        Tile<FragmentQuery> tile = (Tile<FragmentQuery>) QueryRewriter.CreateTile(fragmentQuery);
        this._fragmentQueries.Add(fragmentQuery);
        this._views.Add(tile);
      }
      this.AdjustMemberDomainsForUpdateViews();
      this._domainQuery = this.GetDomainQuery(this.FragmentQueries, generatedType);
      this._usedViews = new HashSet<FragmentQuery>();
    }

    internal void GenerateViewComponents()
    {
      this.EnsureExtentIsFullyMapped(this._usedViews);
      this.GenerateCaseStatements(this._domainMap.ConditionMembers(this._extentPath.Extent), this._usedViews);
      this.AddTrivialCaseStatementsForConditionMembers();
      if (this._usedViews.Count == 0 || this._errorLog.Count > 0)
        ExceptionHelpers.ThrowMappingException(this._errorLog, this._config);
      this._topLevelWhereClause = this.GetTopLevelWhereClause(this._usedViews);
      int viewTarget = (int) this._context.ViewTarget;
      this._usedCells = this.RemapFromVariables();
      this._basicView = new BasicViewGenerator(this._context.MemberMaps.ProjectedSlotMap, this._usedCells, this._domainQuery, this._context, this._domainMap, this._errorLog, this._config).CreateViewExpression();
      if (this._context.LeftFragmentQP.IsContainedIn(this._basicView.LeftFragmentQuery, this._domainQuery))
        this._topLevelWhereClause = BoolExpression.True;
      if (this._errorLog.Count <= 0)
        return;
      ExceptionHelpers.ThrowMappingException(this._errorLog, this._config);
    }

    internal ViewgenContext ViewgenContext
    {
      get
      {
        return this._context;
      }
    }

    internal Dictionary<MemberPath, CaseStatement> CaseStatements
    {
      get
      {
        return this._caseStatements;
      }
    }

    internal BoolExpression TopLevelWhereClause
    {
      get
      {
        return this._topLevelWhereClause;
      }
    }

    internal CellTreeNode BasicView
    {
      get
      {
        return this._basicView.MakeCopy();
      }
    }

    internal List<LeftCellWrapper> UsedCells
    {
      get
      {
        return this._usedCells;
      }
    }

    private IEnumerable<FragmentQuery> FragmentQueries
    {
      get
      {
        return (IEnumerable<FragmentQuery>) this._fragmentQueries;
      }
    }

    private IEnumerable<Constant> GetDomain(MemberPath currentPath)
    {
      if (this._context.ViewTarget != ViewTarget.QueryView || !MemberPath.EqualityComparer.Equals(currentPath, this._extentPath))
        return this._domainMap.GetDomain(currentPath);
      IEnumerable<EdmType> types;
      if (this._typesGenerationMode == ViewGenMode.OfTypeOnlyViews)
        types = (IEnumerable<EdmType>) new HashSet<EdmType>()
        {
          this._generatedType
        };
      else
        types = MetadataHelper.GetTypeAndSubtypesOf(this._generatedType, (ItemCollection) this._context.EdmItemCollection, false);
      return QueryRewriter.GetTypeConstants(types);
    }

    private void AdjustMemberDomainsForUpdateViews()
    {
      if (this._context.ViewTarget != ViewTarget.UpdateView)
        return;
      foreach (MemberPath memberPath in new List<MemberPath>(this._domainMap.ConditionMembers(this._extentPath.Extent)))
      {
        MemberPath currentPath = memberPath;
        Constant domainValue1 = this._domainMap.GetDomain(currentPath).FirstOrDefault<Constant>((Func<Constant, bool>) (domainValue => QueryRewriter.IsDefaultValue(domainValue, currentPath)));
        if (domainValue1 != null)
          this.RemoveUnusedValueFromStoreDomain(domainValue1, currentPath);
        Constant domainValue2 = this._domainMap.GetDomain(currentPath).FirstOrDefault<Constant>((Func<Constant, bool>) (domainValue => domainValue is NegatedConstant));
        if (domainValue2 != null)
          this.RemoveUnusedValueFromStoreDomain(domainValue2, currentPath);
      }
    }

    private void RemoveUnusedValueFromStoreDomain(Constant domainValue, MemberPath currentPath)
    {
      BoolExpression memberCondition = this.CreateMemberCondition(currentPath, domainValue);
      HashSet<FragmentQuery> outputUsedViews = new HashSet<FragmentQuery>();
      bool flag = false;
      Tile<FragmentQuery> rewriting;
      if (this.FindRewritingAndUsedViews((IEnumerable<MemberPath>) this._keyAttributes, memberCondition, outputUsedViews, out rewriting))
        flag = !QueryRewriter.TileToCellTree(rewriting, this._context).IsEmptyRightFragmentQuery;
      if (flag)
        return;
      Set<Constant> set = new Set<Constant>(this._domainMap.GetDomain(currentPath), Constant.EqualityComparer);
      set.Remove(domainValue);
      this._domainMap.UpdateConditionMemberDomain(currentPath, (IEnumerable<Constant>) set);
      foreach (FragmentQuery fragmentQuery in this._fragmentQueries)
        fragmentQuery.Condition.FixDomainMap(this._domainMap);
    }

    internal FragmentQuery GetDomainQuery(
      IEnumerable<FragmentQuery> fragmentQueries,
      EdmType generatedType)
    {
      if (this._context.ViewTarget != ViewTarget.QueryView)
        return FragmentQuery.Create((IEnumerable<MemberPath>) this._keyAttributes, BoolExpression.CreateOr(fragmentQueries.Select<FragmentQuery, BoolExpression>((Func<FragmentQuery, BoolExpression>) (fragmentQuery => fragmentQuery.Condition)).ToArray<BoolExpression>()));
      BoolExpression literal;
      if (generatedType == null)
      {
        literal = BoolExpression.True;
      }
      else
      {
        IEnumerable<EdmType> types;
        if (this._typesGenerationMode == ViewGenMode.OfTypeOnlyViews)
          types = (IEnumerable<EdmType>) new HashSet<EdmType>()
          {
            this._generatedType
          };
        else
          types = MetadataHelper.GetTypeAndSubtypesOf(generatedType, (ItemCollection) this._context.EdmItemCollection, false);
        literal = BoolExpression.CreateLiteral((BoolLiteral) new TypeRestriction(new MemberProjectedSlot(this._extentPath), new Domain(QueryRewriter.GetTypeConstants(types), this._domainMap.GetDomain(this._extentPath))), this._domainMap);
      }
      return FragmentQuery.Create((IEnumerable<MemberPath>) this._keyAttributes, literal);
    }

    private bool AddRewritingToCaseStatement(
      Tile<FragmentQuery> rewriting,
      CaseStatement caseStatement,
      MemberPath currentPath,
      Constant domainValue)
    {
      BoolExpression boolExpression = BoolExpression.True;
      bool flag = this._qp.IsContainedIn((Tile<FragmentQuery>) QueryRewriter.CreateTile(this._domainQuery), rewriting);
      if (this._qp.IsDisjointFrom((Tile<FragmentQuery>) QueryRewriter.CreateTile(this._domainQuery), rewriting))
        return false;
      int num = flag ? 1 : 0;
      ProjectedSlot projectedSlot = !domainValue.HasNotNull() ? (ProjectedSlot) new ConstantProjectedSlot(domainValue) : (ProjectedSlot) new MemberProjectedSlot(currentPath);
      BoolExpression condition = flag ? BoolExpression.True : QueryRewriter.TileToBoolExpr(rewriting);
      caseStatement.AddWhenThen(condition, projectedSlot);
      return flag;
    }

    private void EnsureConfigurationIsFullyMapped(
      MemberPath currentPath,
      BoolExpression currentWhereClause,
      HashSet<FragmentQuery> outputUsedViews,
      ErrorLog errorLog)
    {
      foreach (Constant domainValue in this.GetDomain(currentPath))
      {
        if (domainValue != Constant.Undefined)
        {
          BoolExpression memberCondition = this.CreateMemberCondition(currentPath, domainValue);
          BoolExpression and = BoolExpression.CreateAnd(currentWhereClause, memberCondition);
          Tile<FragmentQuery> rewriting;
          if (!this.FindRewritingAndUsedViews((IEnumerable<MemberPath>) this._keyAttributes, and, outputUsedViews, out rewriting))
          {
            if (!ErrorPatternMatcher.FindMappingErrors(this._context, this._domainMap, this._errorLog))
            {
              StringBuilder builder = new StringBuilder();
              string str = StringUtil.FormatInvariant("{0}", (object) this._extentPath);
              BoolExpression condition = rewriting.Query.Condition;
              condition.ExpensiveSimplify();
              if (condition.RepresentsAllTypeConditions)
              {
                string viewGenExtent = Strings.ViewGen_Extent;
                builder.AppendLine(Strings.ViewGen_Cannot_Recover_Types((object) viewGenExtent, (object) str));
              }
              else
              {
                string viewGenEntities = Strings.ViewGen_Entities;
                builder.AppendLine(Strings.ViewGen_Cannot_Disambiguate_MultiConstant((object) viewGenEntities, (object) str));
              }
              RewritingValidator.EntityConfigurationToUserString(condition, builder);
              ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.AmbiguousMultiConstants, builder.ToString(), (IEnumerable<LeftCellWrapper>) this._context.AllWrappersForExtent, string.Empty);
              errorLog.AddEntry(record);
            }
          }
          else
          {
            TypeConstant typeConstant = domainValue as TypeConstant;
            if (typeConstant != null)
            {
              EdmType edmType = typeConstant.EdmType;
              List<MemberPath> list = QueryRewriter.GetNonConditionalScalarMembers(edmType, currentPath, this._domainMap).Union<MemberPath>(QueryRewriter.GetNonConditionalComplexMembers(edmType, currentPath, this._domainMap)).ToList<MemberPath>();
              IEnumerable<MemberPath> notCoveredAttributes;
              if (list.Count > 0 && !this.FindRewritingAndUsedViews((IEnumerable<MemberPath>) list, and, outputUsedViews, out rewriting, out notCoveredAttributes))
              {
                List<MemberPath> memberPathList = new List<MemberPath>(list.Where<MemberPath>((Func<MemberPath, bool>) (a => !a.IsPartOfKey)));
                this.AddUnrecoverableAttributesError(notCoveredAttributes, memberCondition, errorLog);
              }
              else
              {
                foreach (MemberPath conditionalComplexMember in QueryRewriter.GetConditionalComplexMembers(edmType, currentPath, this._domainMap))
                  this.EnsureConfigurationIsFullyMapped(conditionalComplexMember, and, outputUsedViews, errorLog);
                foreach (MemberPath conditionalScalarMember in QueryRewriter.GetConditionalScalarMembers(edmType, currentPath, this._domainMap))
                  this.EnsureConfigurationIsFullyMapped(conditionalScalarMember, and, outputUsedViews, errorLog);
              }
            }
          }
        }
      }
    }

    private static List<string> GetTypeBasedMemberPathList(
      IEnumerable<MemberPath> nonConditionalScalarAttributes)
    {
      List<string> stringList = new List<string>();
      foreach (MemberPath conditionalScalarAttribute in nonConditionalScalarAttributes)
      {
        EdmMember leafEdmMember = conditionalScalarAttribute.LeafEdmMember;
        stringList.Add(leafEdmMember.DeclaringType.Name + "." + (object) leafEdmMember);
      }
      return stringList;
    }

    private void AddUnrecoverableAttributesError(
      IEnumerable<MemberPath> attributes,
      BoolExpression domainAddedWhereClause,
      ErrorLog errorLog)
    {
      StringBuilder builder = new StringBuilder();
      string str = StringUtil.FormatInvariant("{0}", (object) this._extentPath);
      string viewGenExtent = Strings.ViewGen_Extent;
      string commaSeparatedString = StringUtil.ToCommaSeparatedString((IEnumerable) QueryRewriter.GetTypeBasedMemberPathList(attributes));
      builder.AppendLine(Strings.ViewGen_Cannot_Recover_Attributes((object) commaSeparatedString, (object) viewGenExtent, (object) str));
      RewritingValidator.EntityConfigurationToUserString(domainAddedWhereClause, builder);
      ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.AttributesUnrecoverable, builder.ToString(), (IEnumerable<LeftCellWrapper>) this._context.AllWrappersForExtent, string.Empty);
      errorLog.AddEntry(record);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private void GenerateCaseStatements(
      IEnumerable<MemberPath> members,
      HashSet<FragmentQuery> outputUsedViews)
    {
      CellTreeNode rightDomainQuery = (CellTreeNode) new OpCellTreeNode(this._context, CellTreeOpType.Union, (CellTreeNode[]) this._context.AllWrappersForExtent.Where<LeftCellWrapper>((Func<LeftCellWrapper, bool>) (w => this._usedViews.Contains(w.FragmentQuery))).Select<LeftCellWrapper, LeafCellTreeNode>((Func<LeftCellWrapper, LeafCellTreeNode>) (wrapper => new LeafCellTreeNode(this._context, wrapper))).ToArray<LeafCellTreeNode>());
      foreach (MemberPath member in members)
      {
        List<Constant> list = this.GetDomain(member).ToList<Constant>();
        CaseStatement caseStatement = new CaseStatement(member);
        Tile<FragmentQuery> tile = (Tile<FragmentQuery>) null;
        bool flag = list.Count != 2 || !list.Contains<Constant>(Constant.Null, Constant.EqualityComparer) || !list.Contains<Constant>(Constant.NotNull, Constant.EqualityComparer);
        foreach (Constant domainValue in list)
        {
          if (domainValue == Constant.Undefined && this._context.ViewTarget == ViewTarget.QueryView)
          {
            caseStatement.AddWhenThen(BoolExpression.False, (ProjectedSlot) new ConstantProjectedSlot(Constant.Undefined));
          }
          else
          {
            FragmentQuery memberConditionQuery = this.CreateMemberConditionQuery(member, domainValue);
            Tile<FragmentQuery> rewriting;
            if (this.FindRewritingAndUsedViews((IEnumerable<MemberPath>) memberConditionQuery.Attributes, memberConditionQuery.Condition, outputUsedViews, out rewriting))
            {
              if (this._context.ViewTarget == ViewTarget.UpdateView)
                tile = tile != null ? this._qp.Union(tile, rewriting) : rewriting;
              if (flag)
              {
                if (this.AddRewritingToCaseStatement(rewriting, caseStatement, member, domainValue))
                  break;
              }
            }
            else if (!QueryRewriter.IsDefaultValue(domainValue, member) && !ErrorPatternMatcher.FindMappingErrors(this._context, this._domainMap, this._errorLog))
            {
              StringBuilder builder = new StringBuilder();
              string str1 = StringUtil.FormatInvariant("{0}", (object) this._extentPath);
              string str2 = this._context.ViewTarget == ViewTarget.QueryView ? Strings.ViewGen_Entities : Strings.ViewGen_Tuples;
              if (this._context.ViewTarget == ViewTarget.QueryView)
                builder.AppendLine(Strings.Viewgen_CannotGenerateQueryViewUnderNoValidation((object) str1));
              else
                builder.AppendLine(Strings.ViewGen_Cannot_Disambiguate_MultiConstant((object) str2, (object) str1));
              RewritingValidator.EntityConfigurationToUserString(memberConditionQuery.Condition, builder, this._context.ViewTarget == ViewTarget.UpdateView);
              this._errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.AmbiguousMultiConstants, builder.ToString(), (IEnumerable<LeftCellWrapper>) this._context.AllWrappersForExtent, string.Empty));
            }
          }
        }
        if (this._errorLog.Count == 0)
        {
          if (this._context.ViewTarget == ViewTarget.UpdateView && flag)
            this.AddElseDefaultToCaseStatement(member, caseStatement, list, rightDomainQuery, tile);
          if (caseStatement.Clauses.Count > 0)
            this._caseStatements[member] = caseStatement;
        }
      }
    }

    private void AddElseDefaultToCaseStatement(
      MemberPath currentPath,
      CaseStatement caseStatement,
      List<Constant> domain,
      CellTreeNode rightDomainQuery,
      Tile<FragmentQuery> unionCaseRewriting)
    {
      Constant defaultConstant;
      bool valueForMemberPath = Domain.TryGetDefaultValueForMemberPath(currentPath, out defaultConstant);
      if (valueForMemberPath && domain.Contains(defaultConstant))
        return;
      CellTreeNode cellTree = QueryRewriter.TileToCellTree(unionCaseRewriting, this._context);
      FragmentQuery query = this._context.RightFragmentQP.Difference(rightDomainQuery.RightFragmentQuery, cellTree.RightFragmentQuery);
      if (!this._context.RightFragmentQP.IsSatisfiable(query))
        return;
      if (valueForMemberPath)
      {
        caseStatement.AddWhenThen(BoolExpression.True, (ProjectedSlot) new ConstantProjectedSlot(defaultConstant));
      }
      else
      {
        query.Condition.ExpensiveSimplify();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Strings.ViewGen_No_Default_Value_For_Configuration((object) currentPath.PathToString(new bool?(false))));
        this._errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.NoDefaultValue, stringBuilder.ToString(), (IEnumerable<LeftCellWrapper>) this._context.AllWrappersForExtent, string.Empty));
      }
    }

    private BoolExpression GetTopLevelWhereClause(
      HashSet<FragmentQuery> outputUsedViews)
    {
      BoolExpression boolExpr = BoolExpression.True;
      Tile<FragmentQuery> rewriting;
      if (this._context.ViewTarget == ViewTarget.QueryView && !this._domainQuery.Condition.IsTrue && this.FindRewritingAndUsedViews((IEnumerable<MemberPath>) this._keyAttributes, this._domainQuery.Condition, outputUsedViews, out rewriting))
      {
        boolExpr = QueryRewriter.TileToBoolExpr(rewriting);
        boolExpr.ExpensiveSimplify();
      }
      return boolExpr;
    }

    internal void EnsureExtentIsFullyMapped(HashSet<FragmentQuery> outputUsedViews)
    {
      if (this._context.ViewTarget == ViewTarget.QueryView && this._config.IsValidationEnabled)
      {
        this.EnsureConfigurationIsFullyMapped(this._extentPath, BoolExpression.True, outputUsedViews, this._errorLog);
        if (this._errorLog.Count <= 0)
          return;
        ExceptionHelpers.ThrowMappingException(this._errorLog, this._config);
      }
      else
      {
        if (this._config.IsValidationEnabled)
        {
          foreach (MemberPath member in this._context.MemberMaps.ProjectedSlotMap.Members)
          {
            Constant defaultConstant;
            if (member.IsScalarType() && !member.IsPartOfKey && (!this._domainMap.IsConditionMember(member) && !Domain.TryGetDefaultValueForMemberPath(member, out defaultConstant)))
            {
              HashSet<MemberPath> memberPathSet = new HashSet<MemberPath>((IEnumerable<MemberPath>) this._keyAttributes);
              memberPathSet.Add(member);
              foreach (LeftCellWrapper leftCellWrapper in this._context.AllWrappersForExtent)
              {
                FragmentQuery fragmentQuery = leftCellWrapper.FragmentQuery;
                Tile<FragmentQuery> rewriting;
                IEnumerable<MemberPath> notCoveredAttributes;
                if (!this.RewriteQuery((Tile<FragmentQuery>) QueryRewriter.CreateTile(new FragmentQuery(fragmentQuery.Description, fragmentQuery.FromVariable, (IEnumerable<MemberPath>) memberPathSet, fragmentQuery.Condition)), (Tile<FragmentQuery>) QueryRewriter.CreateTile(FragmentQuery.Create((IEnumerable<MemberPath>) this._keyAttributes, BoolExpression.CreateNot(fragmentQuery.Condition))), out rewriting, out notCoveredAttributes, false))
                  Domain.GetDefaultValueForMemberPath(member, (IEnumerable<LeftCellWrapper>) new LeftCellWrapper[1]
                  {
                    leftCellWrapper
                  }, this._config);
              }
            }
          }
        }
        foreach (Tile<FragmentQuery> view in this._views)
        {
          Tile<FragmentQuery> toFill = view;
          Tile<FragmentQuery> tile = (Tile<FragmentQuery>) QueryRewriter.CreateTile(FragmentQuery.Create((IEnumerable<MemberPath>) this._keyAttributes, BoolExpression.CreateNot(toFill.Query.Condition)));
          Tile<FragmentQuery> rewriting;
          IEnumerable<MemberPath> notCoveredAttributes;
          if (!this.RewriteQuery(toFill, tile, out rewriting, out notCoveredAttributes, true))
          {
            LeftCellWrapper leftCellWrapper = this._context.AllWrappersForExtent.First<LeftCellWrapper>((Func<LeftCellWrapper, bool>) (lcr => lcr.FragmentQuery.Equals((object) toFill.Query)));
            this._errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.ImpopssibleCondition, Strings.Viewgen_QV_RewritingNotFound((object) leftCellWrapper.RightExtent.ToString()), leftCellWrapper.Cells, string.Empty));
          }
          else
            outputUsedViews.UnionWith(rewriting.GetNamedQueries());
        }
      }
    }

    private List<LeftCellWrapper> RemapFromVariables()
    {
      List<LeftCellWrapper> leftCellWrapperList = new List<LeftCellWrapper>();
      int index = 0;
      Dictionary<BoolLiteral, BoolLiteral> remap = new Dictionary<BoolLiteral, BoolLiteral>(BoolLiteral.EqualityIdentifierComparer);
      foreach (LeftCellWrapper leftCellWrapper in this._context.AllWrappersForExtent)
      {
        if (this._usedViews.Contains(leftCellWrapper.FragmentQuery))
        {
          leftCellWrapperList.Add(leftCellWrapper);
          int cellNumber = leftCellWrapper.OnlyInputCell.CellNumber;
          if (index != cellNumber)
            remap[(BoolLiteral) new CellIdBoolean(this._identifiers, cellNumber)] = (BoolLiteral) new CellIdBoolean(this._identifiers, index);
          ++index;
        }
      }
      if (remap.Count > 0)
      {
        this._topLevelWhereClause = this._topLevelWhereClause.RemapLiterals(remap);
        Dictionary<MemberPath, CaseStatement> dictionary = new Dictionary<MemberPath, CaseStatement>();
        foreach (KeyValuePair<MemberPath, CaseStatement> caseStatement1 in this._caseStatements)
        {
          CaseStatement caseStatement2 = new CaseStatement(caseStatement1.Key);
          foreach (CaseStatement.WhenThen clause in caseStatement1.Value.Clauses)
            caseStatement2.AddWhenThen(clause.Condition.RemapLiterals(remap), clause.Value);
          dictionary[caseStatement1.Key] = caseStatement2;
        }
        this._caseStatements = dictionary;
      }
      return leftCellWrapperList;
    }

    internal void AddTrivialCaseStatementsForConditionMembers()
    {
      for (int index = 0; index < this._context.MemberMaps.ProjectedSlotMap.Count; ++index)
      {
        MemberPath projectedSlot = this._context.MemberMaps.ProjectedSlotMap[index];
        if (!projectedSlot.IsScalarType() && !this._caseStatements.ContainsKey(projectedSlot))
        {
          Constant constant = (Constant) new TypeConstant(projectedSlot.EdmType);
          CaseStatement caseStatement = new CaseStatement(projectedSlot);
          caseStatement.AddWhenThen(BoolExpression.True, (ProjectedSlot) new ConstantProjectedSlot(constant));
          this._caseStatements[projectedSlot] = caseStatement;
        }
      }
    }

    private bool FindRewritingAndUsedViews(
      IEnumerable<MemberPath> attributes,
      BoolExpression whereClause,
      HashSet<FragmentQuery> outputUsedViews,
      out Tile<FragmentQuery> rewriting)
    {
      IEnumerable<MemberPath> notCoveredAttributes;
      return this.FindRewritingAndUsedViews(attributes, whereClause, outputUsedViews, out rewriting, out notCoveredAttributes);
    }

    private bool FindRewritingAndUsedViews(
      IEnumerable<MemberPath> attributes,
      BoolExpression whereClause,
      HashSet<FragmentQuery> outputUsedViews,
      out Tile<FragmentQuery> rewriting,
      out IEnumerable<MemberPath> notCoveredAttributes)
    {
      if (!this.FindRewriting(attributes, whereClause, out rewriting, out notCoveredAttributes))
        return false;
      outputUsedViews.UnionWith(rewriting.GetNamedQueries());
      return true;
    }

    private bool FindRewriting(
      IEnumerable<MemberPath> attributes,
      BoolExpression whereClause,
      out Tile<FragmentQuery> rewriting,
      out IEnumerable<MemberPath> notCoveredAttributes)
    {
      Tile<FragmentQuery> tile1 = (Tile<FragmentQuery>) QueryRewriter.CreateTile(FragmentQuery.Create(attributes, whereClause));
      Tile<FragmentQuery> tile2 = (Tile<FragmentQuery>) QueryRewriter.CreateTile(FragmentQuery.Create((IEnumerable<MemberPath>) this._keyAttributes, BoolExpression.CreateNot(whereClause)));
      bool isRelaxed = this._context.ViewTarget == ViewTarget.UpdateView;
      return this.RewriteQuery(tile1, tile2, out rewriting, out notCoveredAttributes, isRelaxed);
    }

    private bool RewriteQuery(
      Tile<FragmentQuery> toFill,
      Tile<FragmentQuery> toAvoid,
      out Tile<FragmentQuery> rewriting,
      out IEnumerable<MemberPath> notCoveredAttributes,
      bool isRelaxed)
    {
      notCoveredAttributes = (IEnumerable<MemberPath>) new List<MemberPath>();
      FragmentQuery query1 = toFill.Query;
      if (this._context.TryGetCachedRewriting(query1, out rewriting))
        return true;
      IEnumerable<Tile<FragmentQuery>> relevantViews = this.GetRelevantViews(query1);
      FragmentQuery query2 = query1;
      if (!this.RewriteQueryCached((Tile<FragmentQuery>) QueryRewriter.CreateTile(FragmentQuery.Create(query1.Condition)), toAvoid, relevantViews, out rewriting))
      {
        if (!isRelaxed)
          return false;
        query1 = FragmentQuery.Create((IEnumerable<MemberPath>) query1.Attributes, BoolExpression.CreateAndNot(query1.Condition, rewriting.Query.Condition));
        if (this._qp.IsEmpty((Tile<FragmentQuery>) QueryRewriter.CreateTile(query1)) || !this.RewriteQueryCached((Tile<FragmentQuery>) QueryRewriter.CreateTile(FragmentQuery.Create(query1.Condition)), toAvoid, relevantViews, out rewriting))
          return false;
      }
      if (query1.Attributes.Count == 0)
        return true;
      Dictionary<MemberPath, FragmentQuery> attributeConditions = new Dictionary<MemberPath, FragmentQuery>();
      foreach (MemberPath nonKey in QueryRewriter.NonKeys((IEnumerable<MemberPath>) query1.Attributes))
        attributeConditions[nonKey] = query1;
      if (attributeConditions.Count == 0 || this.CoverAttributes(ref rewriting, attributeConditions))
      {
        this.GetUsedViewsAndRemoveTrueSurrogate(ref rewriting);
        this._context.SetCachedRewriting(query2, rewriting);
        return true;
      }
      if (isRelaxed)
      {
        foreach (MemberPath nonKey in QueryRewriter.NonKeys((IEnumerable<MemberPath>) query1.Attributes))
        {
          FragmentQuery fragmentQuery;
          attributeConditions[nonKey] = !attributeConditions.TryGetValue(nonKey, out fragmentQuery) ? query1 : FragmentQuery.Create(BoolExpression.CreateAndNot(query1.Condition, fragmentQuery.Condition));
        }
        if (this.CoverAttributes(ref rewriting, attributeConditions))
        {
          this.GetUsedViewsAndRemoveTrueSurrogate(ref rewriting);
          this._context.SetCachedRewriting(query2, rewriting);
          return true;
        }
      }
      notCoveredAttributes = (IEnumerable<MemberPath>) attributeConditions.Keys;
      return false;
    }

    private bool RewriteQueryCached(
      Tile<FragmentQuery> toFill,
      Tile<FragmentQuery> toAvoid,
      IEnumerable<Tile<FragmentQuery>> views,
      out Tile<FragmentQuery> rewriting)
    {
      if (this._context.TryGetCachedRewriting(toFill.Query, out rewriting))
        return true;
      bool flag = this._qp.RewriteQuery(toFill, toAvoid, views, out rewriting);
      if (flag)
        this._context.SetCachedRewriting(toFill.Query, rewriting);
      return flag;
    }

    private bool CoverAttributes(
      ref Tile<FragmentQuery> rewriting,
      Dictionary<MemberPath, FragmentQuery> attributeConditions)
    {
      foreach (FragmentQuery view in new HashSet<FragmentQuery>(rewriting.GetNamedQueries()))
      {
        foreach (MemberPath nonKey in QueryRewriter.NonKeys((IEnumerable<MemberPath>) view.Attributes))
          this.CoverAttribute(nonKey, view, attributeConditions);
        if (attributeConditions.Count == 0)
          return true;
      }
      Tile<FragmentQuery> tile = (Tile<FragmentQuery>) null;
      foreach (FragmentQuery fragmentQuery in this._fragmentQueries)
      {
        foreach (MemberPath nonKey in QueryRewriter.NonKeys((IEnumerable<MemberPath>) fragmentQuery.Attributes))
        {
          if (this.CoverAttribute(nonKey, fragmentQuery, attributeConditions))
            tile = tile == null ? (Tile<FragmentQuery>) QueryRewriter.CreateTile(fragmentQuery) : this._qp.Union(tile, (Tile<FragmentQuery>) QueryRewriter.CreateTile(fragmentQuery));
        }
        if (attributeConditions.Count == 0)
          break;
      }
      if (attributeConditions.Count != 0)
        return false;
      rewriting = this._qp.Join(rewriting, tile);
      return true;
    }

    private bool CoverAttribute(
      MemberPath projectedAttribute,
      FragmentQuery view,
      Dictionary<MemberPath, FragmentQuery> attributeConditions)
    {
      FragmentQuery fragmentQuery;
      if (!attributeConditions.TryGetValue(projectedAttribute, out fragmentQuery))
        return false;
      FragmentQuery query = FragmentQuery.Create(BoolExpression.CreateAndNot(fragmentQuery.Condition, view.Condition));
      if (this._qp.IsEmpty((Tile<FragmentQuery>) QueryRewriter.CreateTile(query)))
        attributeConditions.Remove(projectedAttribute);
      else
        attributeConditions[projectedAttribute] = query;
      return true;
    }

    private IEnumerable<Tile<FragmentQuery>> GetRelevantViews(
      FragmentQuery query)
    {
      Set<MemberPath> variables = QueryRewriter.GetVariables(query);
      Tile<FragmentQuery> a1 = (Tile<FragmentQuery>) null;
      List<Tile<FragmentQuery>> tileList = new List<Tile<FragmentQuery>>();
      Tile<FragmentQuery> tile = (Tile<FragmentQuery>) null;
      foreach (Tile<FragmentQuery> view in this._views)
      {
        if (QueryRewriter.GetVariables(view.Query).Overlaps(variables))
        {
          a1 = a1 == null ? view : this._qp.Union(a1, view);
          tileList.Add(view);
        }
        else if (this.IsTrue(view.Query) && tile == null)
          tile = view;
      }
      if (a1 != null && this.IsTrue(a1.Query))
        return (IEnumerable<Tile<FragmentQuery>>) tileList;
      if (tile == null)
      {
        Tile<FragmentQuery> a2 = (Tile<FragmentQuery>) null;
        foreach (FragmentQuery fragmentQuery in this._fragmentQueries)
        {
          a2 = a2 == null ? (Tile<FragmentQuery>) QueryRewriter.CreateTile(fragmentQuery) : this._qp.Union(a2, (Tile<FragmentQuery>) QueryRewriter.CreateTile(fragmentQuery));
          if (this.IsTrue(a2.Query))
          {
            tile = QueryRewriter._trueViewSurrogate;
            break;
          }
        }
      }
      if (tile == null)
        return (IEnumerable<Tile<FragmentQuery>>) this._views;
      tileList.Add(tile);
      return (IEnumerable<Tile<FragmentQuery>>) tileList;
    }

    private HashSet<FragmentQuery> GetUsedViewsAndRemoveTrueSurrogate(
      ref Tile<FragmentQuery> rewriting)
    {
      HashSet<FragmentQuery> first = new HashSet<FragmentQuery>(rewriting.GetNamedQueries());
      if (!first.Contains(QueryRewriter._trueViewSurrogate.Query))
        return first;
      first.Remove(QueryRewriter._trueViewSurrogate.Query);
      Tile<FragmentQuery> tile = (Tile<FragmentQuery>) null;
      foreach (FragmentQuery query in first.Concat<FragmentQuery>((IEnumerable<FragmentQuery>) this._fragmentQueries))
      {
        tile = tile == null ? (Tile<FragmentQuery>) QueryRewriter.CreateTile(query) : this._qp.Union(tile, (Tile<FragmentQuery>) QueryRewriter.CreateTile(query));
        first.Add(query);
        if (this.IsTrue(tile.Query))
        {
          rewriting = rewriting.Replace(QueryRewriter._trueViewSurrogate, tile);
          return first;
        }
      }
      return first;
    }

    private BoolExpression CreateMemberCondition(
      MemberPath path,
      Constant domainValue)
    {
      return FragmentQuery.CreateMemberCondition(path, domainValue, this._domainMap);
    }

    private FragmentQuery CreateMemberConditionQuery(
      MemberPath currentPath,
      Constant domainValue)
    {
      return QueryRewriter.CreateMemberConditionQuery(currentPath, domainValue, (IEnumerable<MemberPath>) this._keyAttributes, this._domainMap);
    }

    internal static FragmentQuery CreateMemberConditionQuery(
      MemberPath currentPath,
      Constant domainValue,
      IEnumerable<MemberPath> keyAttributes,
      MemberDomainMap domainMap)
    {
      BoolExpression memberCondition = FragmentQuery.CreateMemberCondition(currentPath, domainValue, domainMap);
      IEnumerable<MemberPath> attrs = keyAttributes;
      if (domainValue is NegatedConstant)
        attrs = keyAttributes.Concat<MemberPath>((IEnumerable<MemberPath>) new MemberPath[1]
        {
          currentPath
        });
      return FragmentQuery.Create(attrs, memberCondition);
    }

    private static TileNamed<FragmentQuery> CreateTile(FragmentQuery query)
    {
      return new TileNamed<FragmentQuery>(query);
    }

    private static IEnumerable<Constant> GetTypeConstants(
      IEnumerable<EdmType> types)
    {
      foreach (EdmType type in types)
        yield return (Constant) new TypeConstant(type);
    }

    private static IEnumerable<MemberPath> GetNonConditionalScalarMembers(
      EdmType edmType,
      MemberPath currentPath,
      MemberDomainMap domainMap)
    {
      return currentPath.GetMembers(edmType, new bool?(true), new bool?(false), new bool?(), domainMap);
    }

    private static IEnumerable<MemberPath> GetConditionalComplexMembers(
      EdmType edmType,
      MemberPath currentPath,
      MemberDomainMap domainMap)
    {
      return currentPath.GetMembers(edmType, new bool?(false), new bool?(true), new bool?(), domainMap);
    }

    private static IEnumerable<MemberPath> GetNonConditionalComplexMembers(
      EdmType edmType,
      MemberPath currentPath,
      MemberDomainMap domainMap)
    {
      return currentPath.GetMembers(edmType, new bool?(false), new bool?(false), new bool?(), domainMap);
    }

    private static IEnumerable<MemberPath> GetConditionalScalarMembers(
      EdmType edmType,
      MemberPath currentPath,
      MemberDomainMap domainMap)
    {
      return currentPath.GetMembers(edmType, new bool?(true), new bool?(true), new bool?(), domainMap);
    }

    private static IEnumerable<MemberPath> NonKeys(
      IEnumerable<MemberPath> attributes)
    {
      return attributes.Where<MemberPath>((Func<MemberPath, bool>) (attr => !attr.IsPartOfKey));
    }

    internal static CellTreeNode TileToCellTree(
      Tile<FragmentQuery> tile,
      ViewgenContext context)
    {
      if (tile.OpKind == TileOpKind.Named)
      {
        FragmentQuery view = ((TileNamed<FragmentQuery>) tile).NamedQuery;
        LeftCellWrapper cellWrapper = context.AllWrappersForExtent.First<LeftCellWrapper>((Func<LeftCellWrapper, bool>) (w => w.FragmentQuery == view));
        return (CellTreeNode) new LeafCellTreeNode(context, cellWrapper);
      }
      CellTreeOpType opType;
      switch (tile.OpKind)
      {
        case TileOpKind.Union:
          opType = CellTreeOpType.Union;
          break;
        case TileOpKind.Join:
          opType = CellTreeOpType.IJ;
          break;
        case TileOpKind.AntiSemiJoin:
          opType = CellTreeOpType.LASJ;
          break;
        default:
          return (CellTreeNode) null;
      }
      return (CellTreeNode) new OpCellTreeNode(context, opType, new CellTreeNode[2]
      {
        QueryRewriter.TileToCellTree(tile.Arg1, context),
        QueryRewriter.TileToCellTree(tile.Arg2, context)
      });
    }

    private static BoolExpression TileToBoolExpr(Tile<FragmentQuery> tile)
    {
      switch (tile.OpKind)
      {
        case TileOpKind.Union:
          return BoolExpression.CreateOr(QueryRewriter.TileToBoolExpr(tile.Arg1), QueryRewriter.TileToBoolExpr(tile.Arg2));
        case TileOpKind.Join:
          return BoolExpression.CreateAnd(QueryRewriter.TileToBoolExpr(tile.Arg1), QueryRewriter.TileToBoolExpr(tile.Arg2));
        case TileOpKind.AntiSemiJoin:
          return BoolExpression.CreateAnd(QueryRewriter.TileToBoolExpr(tile.Arg1), BoolExpression.CreateNot(QueryRewriter.TileToBoolExpr(tile.Arg2)));
        case TileOpKind.Named:
          FragmentQuery namedQuery = ((TileNamed<FragmentQuery>) tile).NamedQuery;
          if (namedQuery.Condition.IsAlwaysTrue())
            return BoolExpression.True;
          return namedQuery.FromVariable;
        default:
          return (BoolExpression) null;
      }
    }

    private static bool IsDefaultValue(Constant domainValue, MemberPath path)
    {
      if (domainValue.IsNull() && path.IsNullable)
        return true;
      if (path.DefaultValue != null)
        return (domainValue as ScalarConstant).Value == path.DefaultValue;
      return false;
    }

    private static Set<MemberPath> GetVariables(FragmentQuery query)
    {
      return new Set<MemberPath>(query.Condition.VariableConstraints.Where<DomainConstraint<BoolLiteral, Constant>>((Func<DomainConstraint<BoolLiteral, Constant>, bool>) (domainConstraint =>
      {
        if (domainConstraint.Variable.Identifier is MemberRestriction)
          return !domainConstraint.Variable.Domain.All<Constant>((Func<Constant, bool>) (constant => domainConstraint.Range.Contains(constant)));
        return false;
      })).Select<DomainConstraint<BoolLiteral, Constant>, MemberPath>((Func<DomainConstraint<BoolLiteral, Constant>, MemberPath>) (domainConstraint => ((MemberRestriction) domainConstraint.Variable.Identifier).RestrictedMemberSlot.MemberPath)), MemberPath.EqualityComparer);
    }

    private bool IsTrue(FragmentQuery query)
    {
      return !this._context.LeftFragmentQP.IsSatisfiable(FragmentQuery.Create(BoolExpression.CreateNot(query.Condition)));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [Conditional("DEBUG")]
    private void PrintStatistics(RewritingProcessor<Tile<FragmentQuery>> qp)
    {
      int numSATChecks;
      int numIntersection;
      int numUnion;
      int numDifference;
      int numErrors;
      qp.GetStatistics(out numSATChecks, out numIntersection, out numUnion, out numDifference, out numErrors);
    }

    [Conditional("DEBUG")]
    internal void TraceVerbose(string msg, params object[] parameters)
    {
      if (!this._config.IsVerboseTracing)
        return;
      Helpers.FormatTraceLine(msg, parameters);
    }
  }
}
