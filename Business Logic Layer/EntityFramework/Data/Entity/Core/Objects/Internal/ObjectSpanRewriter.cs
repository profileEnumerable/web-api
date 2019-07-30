// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.ObjectSpanRewriter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Globalization;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class ObjectSpanRewriter
  {
    private readonly Stack<ObjectSpanRewriter.NavigationInfo> _navSources = new Stack<ObjectSpanRewriter.NavigationInfo>();
    private int _spanCount;
    private SpanIndex _spanIndex;
    private readonly DbExpression _toRewrite;
    private bool _relationshipSpan;
    private readonly DbCommandTree _tree;
    private readonly AliasGenerator _aliasGenerator;

    internal static bool EntityTypeEquals(EntityTypeBase entityType1, EntityTypeBase entityType2)
    {
      return object.ReferenceEquals((object) entityType1, (object) entityType2);
    }

    internal static bool TryRewrite(
      DbQueryCommandTree tree,
      Span span,
      MergeOption mergeOption,
      AliasGenerator aliasGenerator,
      out DbExpression newQuery,
      out SpanIndex spanInfo)
    {
      newQuery = (DbExpression) null;
      spanInfo = (SpanIndex) null;
      ObjectSpanRewriter objectSpanRewriter = (ObjectSpanRewriter) null;
      bool flag = Span.RequiresRelationshipSpan(mergeOption);
      if (span != null && span.SpanList.Count > 0)
        objectSpanRewriter = (ObjectSpanRewriter) new ObjectFullSpanRewriter((DbCommandTree) tree, tree.Query, span, aliasGenerator);
      else if (flag)
        objectSpanRewriter = new ObjectSpanRewriter((DbCommandTree) tree, tree.Query, aliasGenerator);
      if (objectSpanRewriter != null)
      {
        objectSpanRewriter.RelationshipSpan = flag;
        newQuery = objectSpanRewriter.RewriteQuery();
        if (newQuery != null)
          spanInfo = objectSpanRewriter.SpanIndex;
      }
      return spanInfo != null;
    }

    internal ObjectSpanRewriter(
      DbCommandTree tree,
      DbExpression toRewrite,
      AliasGenerator aliasGenerator)
    {
      this._toRewrite = toRewrite;
      this._tree = tree;
      this._aliasGenerator = aliasGenerator;
    }

    internal MetadataWorkspace Metadata
    {
      get
      {
        return this._tree.MetadataWorkspace;
      }
    }

    internal DbExpression Query
    {
      get
      {
        return this._toRewrite;
      }
    }

    internal bool RelationshipSpan
    {
      get
      {
        return this._relationshipSpan;
      }
      set
      {
        this._relationshipSpan = value;
      }
    }

    internal SpanIndex SpanIndex
    {
      get
      {
        return this._spanIndex;
      }
    }

    internal DbExpression RewriteQuery()
    {
      DbExpression dbExpression = this.Rewrite(this._toRewrite);
      if (object.ReferenceEquals((object) this._toRewrite, (object) dbExpression))
        return (DbExpression) null;
      return dbExpression;
    }

    internal ObjectSpanRewriter.SpanTrackingInfo InitializeTrackingInfo(
      bool createAssociationEndTrackingInfo)
    {
      ObjectSpanRewriter.SpanTrackingInfo spanTrackingInfo = new ObjectSpanRewriter.SpanTrackingInfo();
      spanTrackingInfo.ColumnDefinitions = new List<KeyValuePair<string, DbExpression>>();
      spanTrackingInfo.ColumnNames = new AliasGenerator(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Span{0}_Column", (object) this._spanCount));
      spanTrackingInfo.SpannedColumns = new Dictionary<int, AssociationEndMember>();
      if (createAssociationEndTrackingInfo)
        spanTrackingInfo.FullSpannedEnds = new Dictionary<AssociationEndMember, bool>();
      return spanTrackingInfo;
    }

    internal virtual ObjectSpanRewriter.SpanTrackingInfo CreateEntitySpanTrackingInfo(
      DbExpression expression,
      EntityType entityType)
    {
      return new ObjectSpanRewriter.SpanTrackingInfo();
    }

    protected DbExpression Rewrite(DbExpression expression)
    {
      switch (expression.ExpressionKind)
      {
        case DbExpressionKind.Element:
          return this.RewriteElementExpression((DbElementExpression) expression);
        case DbExpressionKind.Limit:
          return this.RewriteLimitExpression((DbLimitExpression) expression);
        default:
          switch (expression.ResultType.EdmType.BuiltInTypeKind)
          {
            case BuiltInTypeKind.CollectionType:
              return this.RewriteCollection(expression);
            case BuiltInTypeKind.EntityType:
              return this.RewriteEntity(expression, (EntityType) expression.ResultType.EdmType);
            case BuiltInTypeKind.RowType:
              return this.RewriteRow(expression, (RowType) expression.ResultType.EdmType);
            default:
              return expression;
          }
      }
    }

    private void AddSpannedRowType(RowType spannedType, TypeUsage originalType)
    {
      if (this._spanIndex == null)
        this._spanIndex = new SpanIndex();
      this._spanIndex.AddSpannedRowType(spannedType, originalType);
    }

    private void AddSpanMap(RowType rowType, Dictionary<int, AssociationEndMember> columnMap)
    {
      if (this._spanIndex == null)
        this._spanIndex = new SpanIndex();
      this._spanIndex.AddSpanMap(rowType, columnMap);
    }

    private DbExpression RewriteEntity(DbExpression expression, EntityType entityType)
    {
      if (DbExpressionKind.NewInstance == expression.ExpressionKind)
        return expression;
      ++this._spanCount;
      int spanCount = this._spanCount;
      ObjectSpanRewriter.SpanTrackingInfo spanTrackingInfo = this.CreateEntitySpanTrackingInfo(expression, entityType);
      List<KeyValuePair<AssociationEndMember, AssociationEndMember>> relationshipSpanEnds = this.GetRelationshipSpanEnds(entityType);
      if (relationshipSpanEnds != null)
      {
        if (spanTrackingInfo.ColumnDefinitions == null)
          spanTrackingInfo = this.InitializeTrackingInfo(false);
        int index = spanTrackingInfo.ColumnDefinitions.Count + 1;
        foreach (KeyValuePair<AssociationEndMember, AssociationEndMember> keyValuePair in relationshipSpanEnds)
        {
          if (spanTrackingInfo.FullSpannedEnds == null || !spanTrackingInfo.FullSpannedEnds.ContainsKey(keyValuePair.Value))
          {
            DbExpression source = (DbExpression) null;
            if (!this.TryGetNavigationSource(keyValuePair.Value, out source))
              source = (DbExpression) expression.GetEntityRef().NavigateAllowingAllRelationshipsInSameTypeHierarchy((RelationshipEndMember) keyValuePair.Key, (RelationshipEndMember) keyValuePair.Value);
            spanTrackingInfo.ColumnDefinitions.Add(new KeyValuePair<string, DbExpression>(spanTrackingInfo.ColumnNames.Next(), source));
            spanTrackingInfo.SpannedColumns[index] = keyValuePair.Value;
            ++index;
          }
        }
      }
      if (spanTrackingInfo.ColumnDefinitions == null)
      {
        --this._spanCount;
        return expression;
      }
      spanTrackingInfo.ColumnDefinitions.Insert(0, new KeyValuePair<string, DbExpression>(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Span{0}_SpanRoot", (object) spanCount), expression));
      DbExpression dbExpression = (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) spanTrackingInfo.ColumnDefinitions);
      this.AddSpanMap((RowType) dbExpression.ResultType.EdmType, spanTrackingInfo.SpannedColumns);
      return dbExpression;
    }

    private DbExpression RewriteElementExpression(DbElementExpression expression)
    {
      DbExpression dbExpression = this.Rewrite(expression.Argument);
      if (!object.ReferenceEquals((object) expression.Argument, (object) dbExpression))
        expression = dbExpression.Element();
      return (DbExpression) expression;
    }

    private DbExpression RewriteLimitExpression(DbLimitExpression expression)
    {
      DbExpression dbExpression = this.Rewrite(expression.Argument);
      if (!object.ReferenceEquals((object) expression.Argument, (object) dbExpression))
        expression = dbExpression.Limit(expression.Limit);
      return (DbExpression) expression;
    }

    private DbExpression RewriteRow(DbExpression expression, RowType rowType)
    {
      DbLambdaExpression lambdaExpression = expression as DbLambdaExpression;
      DbNewInstanceExpression instanceExpression = lambdaExpression == null ? expression as DbNewInstanceExpression : lambdaExpression.Lambda.Body as DbNewInstanceExpression;
      Dictionary<int, DbExpression> dictionary1 = (Dictionary<int, DbExpression>) null;
      Dictionary<int, DbExpression> dictionary2 = (Dictionary<int, DbExpression>) null;
      for (int index = 0; index < rowType.Properties.Count; ++index)
      {
        EdmProperty property = rowType.Properties[index];
        DbExpression expression1 = instanceExpression == null ? (DbExpression) expression.Property(property.Name) : instanceExpression.Arguments[index];
        DbExpression dbExpression = this.Rewrite(expression1);
        if (!object.ReferenceEquals((object) dbExpression, (object) expression1))
        {
          if (dictionary2 == null)
            dictionary2 = new Dictionary<int, DbExpression>();
          dictionary2[index] = dbExpression;
        }
        else
        {
          if (dictionary1 == null)
            dictionary1 = new Dictionary<int, DbExpression>();
          dictionary1[index] = expression1;
        }
      }
      if (dictionary2 == null)
        return expression;
      List<DbExpression> dbExpressionList = new List<DbExpression>(rowType.Properties.Count);
      List<EdmProperty> edmPropertyList = new List<EdmProperty>(rowType.Properties.Count);
      for (int key = 0; key < rowType.Properties.Count; ++key)
      {
        EdmProperty property = rowType.Properties[key];
        DbExpression dbExpression = (DbExpression) null;
        if (!dictionary2.TryGetValue(key, out dbExpression))
          dbExpression = dictionary1[key];
        dbExpressionList.Add(dbExpression);
        edmPropertyList.Add(new EdmProperty(property.Name, dbExpression.ResultType));
      }
      RowType spannedType = new RowType((IEnumerable<EdmProperty>) edmPropertyList, rowType.InitializerMetadata);
      TypeUsage typeUsage = TypeUsage.Create((EdmType) spannedType);
      DbExpression dbExpression1 = (DbExpression) typeUsage.New((IEnumerable<DbExpression>) dbExpressionList);
      if (instanceExpression == null)
        dbExpression1 = (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new List<DbExpression>((IEnumerable<DbExpression>) new DbExpression[1]
        {
          (DbExpression) expression.IsNull()
        }), (IEnumerable<DbExpression>) new List<DbExpression>((IEnumerable<DbExpression>) new DbExpression[1]
        {
          (DbExpression) typeUsage.Null()
        }), dbExpression1);
      this.AddSpannedRowType(spannedType, expression.ResultType);
      if (lambdaExpression != null && instanceExpression != null)
        dbExpression1 = (DbExpression) DbLambda.Create(dbExpression1, (IEnumerable<DbVariableReferenceExpression>) lambdaExpression.Lambda.Variables).Invoke((IEnumerable<DbExpression>) lambdaExpression.Arguments);
      return dbExpression1;
    }

    private DbExpression RewriteCollection(DbExpression expression)
    {
      DbExpression dbExpression = expression;
      DbProjectExpression projectExpression = (DbProjectExpression) null;
      if (DbExpressionKind.Project == expression.ExpressionKind)
      {
        projectExpression = (DbProjectExpression) expression;
        dbExpression = projectExpression.Input.Expression;
      }
      ObjectSpanRewriter.NavigationInfo navInfo = (ObjectSpanRewriter.NavigationInfo) null;
      if (this.RelationshipSpan)
        dbExpression = ObjectSpanRewriter.RelationshipNavigationVisitor.FindNavigationExpression(dbExpression, this._aliasGenerator, out navInfo);
      if (navInfo != null)
        this.EnterNavigationCollection(navInfo);
      else
        this.EnterCollection();
      DbExpression body = expression;
      if (projectExpression != null)
      {
        DbExpression projection = this.Rewrite(projectExpression.Projection);
        if (!object.ReferenceEquals((object) projectExpression.Projection, (object) projection))
          body = (DbExpression) dbExpression.BindAs(projectExpression.Input.VariableName).Project(projection);
      }
      else
      {
        DbExpressionBinding input = dbExpression.BindAs(this._aliasGenerator.Next());
        DbExpression variable = (DbExpression) input.Variable;
        DbExpression projection = this.Rewrite(variable);
        if (!object.ReferenceEquals((object) variable, (object) projection))
          body = (DbExpression) input.Project(projection);
      }
      this.ExitCollection();
      if (navInfo != null && navInfo.InUse)
        body = (DbExpression) DbExpressionBuilder.Lambda(body, (IEnumerable<DbVariableReferenceExpression>) new List<DbVariableReferenceExpression>(1)
        {
          navInfo.SourceVariable
        }).Invoke((IEnumerable<DbExpression>) new List<DbExpression>(1)
        {
          navInfo.Source
        });
      return body;
    }

    private void EnterCollection()
    {
      this._navSources.Push((ObjectSpanRewriter.NavigationInfo) null);
    }

    private void EnterNavigationCollection(ObjectSpanRewriter.NavigationInfo info)
    {
      this._navSources.Push(info);
    }

    private void ExitCollection()
    {
      this._navSources.Pop();
    }

    private bool TryGetNavigationSource(
      AssociationEndMember wasSourceNowTargetEnd,
      out DbExpression source)
    {
      source = (DbExpression) null;
      ObjectSpanRewriter.NavigationInfo navigationInfo = (ObjectSpanRewriter.NavigationInfo) null;
      if (this._navSources.Count > 0)
      {
        navigationInfo = this._navSources.Peek();
        if (navigationInfo != null && !object.ReferenceEquals((object) wasSourceNowTargetEnd, (object) navigationInfo.SourceEnd))
          navigationInfo = (ObjectSpanRewriter.NavigationInfo) null;
      }
      if (navigationInfo == null)
        return false;
      source = (DbExpression) navigationInfo.SourceVariable;
      navigationInfo.InUse = true;
      return true;
    }

    private List<KeyValuePair<AssociationEndMember, AssociationEndMember>> GetRelationshipSpanEnds(
      EntityType entityType)
    {
      List<KeyValuePair<AssociationEndMember, AssociationEndMember>> keyValuePairList = (List<KeyValuePair<AssociationEndMember, AssociationEndMember>>) null;
      if (this._relationshipSpan)
      {
        foreach (AssociationType associationType in this._tree.MetadataWorkspace.GetItems<AssociationType>(DataSpace.CSpace))
        {
          if (2 == associationType.AssociationEndMembers.Count)
          {
            AssociationEndMember associationEndMember1 = associationType.AssociationEndMembers[0];
            AssociationEndMember associationEndMember2 = associationType.AssociationEndMembers[1];
            if (ObjectSpanRewriter.IsValidRelationshipSpan(entityType, associationType, associationEndMember1, associationEndMember2))
            {
              if (keyValuePairList == null)
                keyValuePairList = new List<KeyValuePair<AssociationEndMember, AssociationEndMember>>();
              keyValuePairList.Add(new KeyValuePair<AssociationEndMember, AssociationEndMember>(associationEndMember1, associationEndMember2));
            }
            if (ObjectSpanRewriter.IsValidRelationshipSpan(entityType, associationType, associationEndMember2, associationEndMember1))
            {
              if (keyValuePairList == null)
                keyValuePairList = new List<KeyValuePair<AssociationEndMember, AssociationEndMember>>();
              keyValuePairList.Add(new KeyValuePair<AssociationEndMember, AssociationEndMember>(associationEndMember2, associationEndMember1));
            }
          }
        }
      }
      return keyValuePairList;
    }

    private static bool IsValidRelationshipSpan(
      EntityType compareType,
      AssociationType associationType,
      AssociationEndMember fromEnd,
      AssociationEndMember toEnd)
    {
      if (associationType.IsForeignKey || RelationshipMultiplicity.One != toEnd.RelationshipMultiplicity && toEnd.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne)
        return false;
      EntityType elementType = (EntityType) ((RefType) fromEnd.TypeUsage.EdmType).ElementType;
      if (!ObjectSpanRewriter.EntityTypeEquals((EntityTypeBase) compareType, (EntityTypeBase) elementType) && !TypeSemantics.IsSubTypeOf((EdmType) compareType, (EdmType) elementType))
        return TypeSemantics.IsSubTypeOf((EdmType) elementType, (EdmType) compareType);
      return true;
    }

    internal struct SpanTrackingInfo
    {
      public List<KeyValuePair<string, DbExpression>> ColumnDefinitions;
      public AliasGenerator ColumnNames;
      public Dictionary<int, AssociationEndMember> SpannedColumns;
      public Dictionary<AssociationEndMember, bool> FullSpannedEnds;
    }

    private class NavigationInfo
    {
      private readonly DbVariableReferenceExpression _sourceRef;
      private readonly AssociationEndMember _sourceEnd;
      private readonly DbExpression _source;
      public bool InUse;

      public NavigationInfo(
        DbRelationshipNavigationExpression originalNavigation,
        DbRelationshipNavigationExpression rewrittenNavigation)
      {
        this._sourceEnd = (AssociationEndMember) originalNavigation.NavigateFrom;
        this._sourceRef = (DbVariableReferenceExpression) rewrittenNavigation.NavigationSource;
        this._source = originalNavigation.NavigationSource;
      }

      public AssociationEndMember SourceEnd
      {
        get
        {
          return this._sourceEnd;
        }
      }

      public DbExpression Source
      {
        get
        {
          return this._source;
        }
      }

      public DbVariableReferenceExpression SourceVariable
      {
        get
        {
          return this._sourceRef;
        }
      }
    }

    private class RelationshipNavigationVisitor : DefaultExpressionVisitor
    {
      private readonly AliasGenerator _aliasGenerator;
      private DbRelationshipNavigationExpression _original;
      private DbRelationshipNavigationExpression _rewritten;

      internal static DbExpression FindNavigationExpression(
        DbExpression expression,
        AliasGenerator aliasGenerator,
        out ObjectSpanRewriter.NavigationInfo navInfo)
      {
        navInfo = (ObjectSpanRewriter.NavigationInfo) null;
        TypeUsage typeUsage = ((CollectionType) expression.ResultType.EdmType).TypeUsage;
        if (!TypeSemantics.IsEntityType(typeUsage) && !TypeSemantics.IsReferenceType(typeUsage))
          return expression;
        ObjectSpanRewriter.RelationshipNavigationVisitor navigationVisitor = new ObjectSpanRewriter.RelationshipNavigationVisitor(aliasGenerator);
        DbExpression dbExpression = navigationVisitor.Find(expression);
        if (object.ReferenceEquals((object) expression, (object) dbExpression))
          return expression;
        navInfo = new ObjectSpanRewriter.NavigationInfo(navigationVisitor._original, navigationVisitor._rewritten);
        return dbExpression;
      }

      private RelationshipNavigationVisitor(AliasGenerator aliasGenerator)
      {
        this._aliasGenerator = aliasGenerator;
      }

      private DbExpression Find(DbExpression expression)
      {
        return this.VisitExpression(expression);
      }

      protected override DbExpression VisitExpression(DbExpression expression)
      {
        switch (expression.ExpressionKind)
        {
          case DbExpressionKind.Distinct:
          case DbExpressionKind.Filter:
          case DbExpressionKind.Limit:
          case DbExpressionKind.OfType:
          case DbExpressionKind.Project:
          case DbExpressionKind.RelationshipNavigation:
          case DbExpressionKind.Skip:
          case DbExpressionKind.Sort:
            return base.VisitExpression(expression);
          default:
            return expression;
        }
      }

      public override DbExpression Visit(DbRelationshipNavigationExpression expression)
      {
        Check.NotNull<DbRelationshipNavigationExpression>(expression, nameof (expression));
        this._original = expression;
        string name = this._aliasGenerator.Next();
        this._rewritten = new DbVariableReferenceExpression(expression.NavigationSource.ResultType, name).Navigate(expression.NavigateFrom, expression.NavigateTo);
        return (DbExpression) this._rewritten;
      }

      public override DbExpression Visit(DbFilterExpression expression)
      {
        Check.NotNull<DbFilterExpression>(expression, nameof (expression));
        DbExpression input = this.Find(expression.Input.Expression);
        if (!object.ReferenceEquals((object) input, (object) expression.Input.Expression))
          return (DbExpression) input.BindAs(expression.Input.VariableName).Filter(expression.Predicate);
        return (DbExpression) expression;
      }

      public override DbExpression Visit(DbProjectExpression expression)
      {
        Check.NotNull<DbProjectExpression>(expression, nameof (expression));
        DbExpression projection = expression.Projection;
        if (DbExpressionKind.Deref == projection.ExpressionKind)
          projection = ((DbUnaryExpression) projection).Argument;
        if (DbExpressionKind.VariableReference == projection.ExpressionKind && ((DbVariableReferenceExpression) projection).VariableName.Equals(expression.Input.VariableName, StringComparison.Ordinal))
        {
          DbExpression input = this.Find(expression.Input.Expression);
          if (!object.ReferenceEquals((object) input, (object) expression.Input.Expression))
            return (DbExpression) input.BindAs(expression.Input.VariableName).Project(expression.Projection);
        }
        return (DbExpression) expression;
      }

      public override DbExpression Visit(DbSortExpression expression)
      {
        Check.NotNull<DbSortExpression>(expression, nameof (expression));
        DbExpression input = this.Find(expression.Input.Expression);
        if (!object.ReferenceEquals((object) input, (object) expression.Input.Expression))
          return (DbExpression) input.BindAs(expression.Input.VariableName).Sort((IEnumerable<DbSortClause>) expression.SortOrder);
        return (DbExpression) expression;
      }

      public override DbExpression Visit(DbSkipExpression expression)
      {
        Check.NotNull<DbSkipExpression>(expression, nameof (expression));
        DbExpression input = this.Find(expression.Input.Expression);
        if (!object.ReferenceEquals((object) input, (object) expression.Input.Expression))
          return (DbExpression) input.BindAs(expression.Input.VariableName).Skip((IEnumerable<DbSortClause>) expression.SortOrder, expression.Count);
        return (DbExpression) expression;
      }
    }
  }
}
