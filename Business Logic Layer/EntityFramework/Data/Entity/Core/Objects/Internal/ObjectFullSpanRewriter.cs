// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.ObjectFullSpanRewriter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class ObjectFullSpanRewriter : ObjectSpanRewriter
  {
    private readonly Stack<ObjectFullSpanRewriter.SpanPathInfo> _currentSpanPath = new Stack<ObjectFullSpanRewriter.SpanPathInfo>();

    internal ObjectFullSpanRewriter(
      DbCommandTree tree,
      DbExpression toRewrite,
      Span span,
      AliasGenerator aliasGenerator)
      : base(tree, toRewrite, aliasGenerator)
    {
      EntityType entityType = (EntityType) null;
      if (!ObjectFullSpanRewriter.TryGetEntityType(this.Query.ResultType, out entityType))
        throw new InvalidOperationException(Strings.ObjectQuery_Span_IncludeRequiresEntityOrEntityCollection);
      ObjectFullSpanRewriter.SpanPathInfo parentInfo = new ObjectFullSpanRewriter.SpanPathInfo(entityType);
      foreach (Span.SpanPath span1 in span.SpanList)
        this.AddSpanPath(parentInfo, span1.Navigations);
      this._currentSpanPath.Push(parentInfo);
    }

    private void AddSpanPath(
      ObjectFullSpanRewriter.SpanPathInfo parentInfo,
      List<string> navPropNames)
    {
      this.ConvertSpanPath(parentInfo, navPropNames, 0);
    }

    private void ConvertSpanPath(
      ObjectFullSpanRewriter.SpanPathInfo parentInfo,
      List<string> navPropNames,
      int pos)
    {
      NavigationProperty index = (NavigationProperty) null;
      if (!parentInfo.DeclaringType.NavigationProperties.TryGetValue(navPropNames[pos], true, out index))
        throw new InvalidOperationException(Strings.ObjectQuery_Span_NoNavProp((object) parentInfo.DeclaringType.FullName, (object) navPropNames[pos]));
      if (parentInfo.Children == null)
        parentInfo.Children = new Dictionary<NavigationProperty, ObjectFullSpanRewriter.SpanPathInfo>();
      ObjectFullSpanRewriter.SpanPathInfo parentInfo1 = (ObjectFullSpanRewriter.SpanPathInfo) null;
      if (!parentInfo.Children.TryGetValue(index, out parentInfo1))
      {
        parentInfo1 = new ObjectFullSpanRewriter.SpanPathInfo(ObjectFullSpanRewriter.EntityTypeFromResultType(index));
        parentInfo.Children[index] = parentInfo1;
      }
      if (pos >= navPropNames.Count - 1)
        return;
      this.ConvertSpanPath(parentInfo1, navPropNames, pos + 1);
    }

    private static EntityType EntityTypeFromResultType(NavigationProperty navProp)
    {
      EntityType entityType = (EntityType) null;
      ObjectFullSpanRewriter.TryGetEntityType(navProp.TypeUsage, out entityType);
      return entityType;
    }

    private static bool TryGetEntityType(TypeUsage resultType, out EntityType entityType)
    {
      if (BuiltInTypeKind.EntityType == resultType.EdmType.BuiltInTypeKind)
      {
        entityType = (EntityType) resultType.EdmType;
        return true;
      }
      if (BuiltInTypeKind.CollectionType == resultType.EdmType.BuiltInTypeKind)
      {
        EdmType edmType = ((CollectionType) resultType.EdmType).TypeUsage.EdmType;
        if (BuiltInTypeKind.EntityType == edmType.BuiltInTypeKind)
        {
          entityType = (EntityType) edmType;
          return true;
        }
      }
      entityType = (EntityType) null;
      return false;
    }

    private AssociationEndMember GetNavigationPropertyTargetEnd(
      NavigationProperty property)
    {
      return this.Metadata.GetItem<AssociationType>(property.RelationshipType.FullName, DataSpace.CSpace).AssociationEndMembers[property.ToEndMember.Name];
    }

    internal override ObjectSpanRewriter.SpanTrackingInfo CreateEntitySpanTrackingInfo(
      DbExpression expression,
      EntityType entityType)
    {
      ObjectSpanRewriter.SpanTrackingInfo spanTrackingInfo = new ObjectSpanRewriter.SpanTrackingInfo();
      ObjectFullSpanRewriter.SpanPathInfo spanPathInfo = this._currentSpanPath.Peek();
      if (spanPathInfo.Children != null)
      {
        int index = 1;
        foreach (KeyValuePair<NavigationProperty, ObjectFullSpanRewriter.SpanPathInfo> child in spanPathInfo.Children)
        {
          if (spanTrackingInfo.ColumnDefinitions == null)
            spanTrackingInfo = this.InitializeTrackingInfo(this.RelationshipSpan);
          DbExpression expression1 = (DbExpression) expression.Property(child.Key);
          this._currentSpanPath.Push(child.Value);
          DbExpression dbExpression = this.Rewrite(expression1);
          this._currentSpanPath.Pop();
          spanTrackingInfo.ColumnDefinitions.Add(new KeyValuePair<string, DbExpression>(spanTrackingInfo.ColumnNames.Next(), dbExpression));
          AssociationEndMember propertyTargetEnd = this.GetNavigationPropertyTargetEnd(child.Key);
          spanTrackingInfo.SpannedColumns[index] = propertyTargetEnd;
          if (this.RelationshipSpan)
            spanTrackingInfo.FullSpannedEnds[propertyTargetEnd] = true;
          ++index;
        }
      }
      return spanTrackingInfo;
    }

    private class SpanPathInfo
    {
      internal readonly EntityType DeclaringType;
      internal Dictionary<NavigationProperty, ObjectFullSpanRewriter.SpanPathInfo> Children;

      internal SpanPathInfo(EntityType declaringType)
      {
        this.DeclaringType = declaringType;
      }
    }
  }
}
