// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.Propagator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class Propagator : UpdateExpressionVisitor<ChangeNode>
  {
    private static readonly string _visitorName = typeof (Propagator).FullName;
    private readonly UpdateTranslator m_updateTranslator;
    private readonly EntitySet m_table;

    private Propagator(UpdateTranslator parent, EntitySet table)
    {
      this.m_updateTranslator = parent;
      this.m_table = table;
    }

    internal UpdateTranslator UpdateTranslator
    {
      get
      {
        return this.m_updateTranslator;
      }
    }

    protected override string VisitorName
    {
      get
      {
        return Propagator._visitorName;
      }
    }

    internal static ChangeNode Propagate(
      UpdateTranslator parent,
      EntitySet table,
      DbQueryCommandTree umView)
    {
      DbExpressionVisitor<ChangeNode> visitor = (DbExpressionVisitor<ChangeNode>) new Propagator(parent, table);
      return umView.Query.Accept<ChangeNode>(visitor);
    }

    private static ChangeNode BuildChangeNode(DbExpression node)
    {
      return new ChangeNode(MetadataHelper.GetElementType(node.ResultType));
    }

    public override ChangeNode Visit(DbCrossJoinExpression node)
    {
      Check.NotNull<DbCrossJoinExpression>(node, nameof (node));
      throw new NotSupportedException(Strings.Update_UnsupportedJoinType((object) node.ExpressionKind));
    }

    public override ChangeNode Visit(DbJoinExpression node)
    {
      Check.NotNull<DbJoinExpression>(node, nameof (node));
      if (DbExpressionKind.InnerJoin != node.ExpressionKind && DbExpressionKind.LeftOuterJoin != node.ExpressionKind)
        throw new NotSupportedException(Strings.Update_UnsupportedJoinType((object) node.ExpressionKind));
      return new Propagator.JoinPropagator(this.Visit(node.Left.Expression), this.Visit(node.Right.Expression), node, this).Propagate();
    }

    public override ChangeNode Visit(DbUnionAllExpression node)
    {
      Check.NotNull<DbUnionAllExpression>(node, nameof (node));
      ChangeNode changeNode1 = Propagator.BuildChangeNode((DbExpression) node);
      ChangeNode changeNode2 = this.Visit(node.Left);
      ChangeNode changeNode3 = this.Visit(node.Right);
      changeNode1.Inserted.AddRange((IEnumerable<PropagatorResult>) changeNode2.Inserted);
      changeNode1.Inserted.AddRange((IEnumerable<PropagatorResult>) changeNode3.Inserted);
      changeNode1.Deleted.AddRange((IEnumerable<PropagatorResult>) changeNode2.Deleted);
      changeNode1.Deleted.AddRange((IEnumerable<PropagatorResult>) changeNode3.Deleted);
      changeNode1.Placeholder = changeNode2.Placeholder;
      return changeNode1;
    }

    public override ChangeNode Visit(DbProjectExpression node)
    {
      Check.NotNull<DbProjectExpression>(node, nameof (node));
      ChangeNode changeNode1 = Propagator.BuildChangeNode((DbExpression) node);
      ChangeNode changeNode2 = this.Visit(node.Input.Expression);
      foreach (PropagatorResult row in changeNode2.Inserted)
        changeNode1.Inserted.Add(Propagator.Project(node, row, changeNode1.ElementType));
      foreach (PropagatorResult row in changeNode2.Deleted)
        changeNode1.Deleted.Add(Propagator.Project(node, row, changeNode1.ElementType));
      changeNode1.Placeholder = Propagator.Project(node, changeNode2.Placeholder, changeNode1.ElementType);
      return changeNode1;
    }

    private static PropagatorResult Project(
      DbProjectExpression node,
      PropagatorResult row,
      TypeUsage resultType)
    {
      DbNewInstanceExpression projection = node.Projection as DbNewInstanceExpression;
      if (projection == null)
        throw new NotSupportedException(Strings.Update_UnsupportedProjection((object) node.Projection.ExpressionKind));
      PropagatorResult[] values = new PropagatorResult[projection.Arguments.Count];
      for (int index = 0; index < values.Length; ++index)
        values[index] = Propagator.Evaluator.Evaluate(projection.Arguments[index], row);
      return PropagatorResult.CreateStructuralValue(values, (StructuralType) resultType.EdmType, false);
    }

    public override ChangeNode Visit(DbFilterExpression node)
    {
      Check.NotNull<DbFilterExpression>(node, nameof (node));
      ChangeNode changeNode1 = Propagator.BuildChangeNode((DbExpression) node);
      ChangeNode changeNode2 = this.Visit(node.Input.Expression);
      changeNode1.Inserted.AddRange(Propagator.Evaluator.Filter(node.Predicate, (IEnumerable<PropagatorResult>) changeNode2.Inserted));
      changeNode1.Deleted.AddRange(Propagator.Evaluator.Filter(node.Predicate, (IEnumerable<PropagatorResult>) changeNode2.Deleted));
      changeNode1.Placeholder = changeNode2.Placeholder;
      return changeNode1;
    }

    public override ChangeNode Visit(DbScanExpression node)
    {
      Check.NotNull<DbScanExpression>(node, nameof (node));
      EntitySetBase target = node.Target;
      ChangeNode extentModifications = this.UpdateTranslator.GetExtentModifications(target);
      if (extentModifications.Placeholder == null)
        extentModifications.Placeholder = Propagator.ExtentPlaceholderCreator.CreatePlaceholder(target);
      return extentModifications;
    }

    private class Evaluator : UpdateExpressionVisitor<PropagatorResult>
    {
      private static readonly string _visitorName = typeof (Propagator.Evaluator).FullName;
      private readonly PropagatorResult m_row;

      private Evaluator(PropagatorResult row)
      {
        this.m_row = row;
      }

      protected override string VisitorName
      {
        get
        {
          return Propagator.Evaluator._visitorName;
        }
      }

      internal static IEnumerable<PropagatorResult> Filter(
        DbExpression predicate,
        IEnumerable<PropagatorResult> rows)
      {
        foreach (PropagatorResult row in rows)
        {
          if (Propagator.Evaluator.EvaluatePredicate(predicate, row))
            yield return row;
        }
      }

      internal static bool EvaluatePredicate(DbExpression predicate, PropagatorResult row)
      {
        Propagator.Evaluator evaluator = new Propagator.Evaluator(row);
        return Propagator.Evaluator.ConvertResultToBool(predicate.Accept<PropagatorResult>((DbExpressionVisitor<PropagatorResult>) evaluator)) ?? false;
      }

      internal static PropagatorResult Evaluate(
        DbExpression node,
        PropagatorResult row)
      {
        DbExpressionVisitor<PropagatorResult> visitor = (DbExpressionVisitor<PropagatorResult>) new Propagator.Evaluator(row);
        return node.Accept<PropagatorResult>(visitor);
      }

      private static bool? ConvertResultToBool(PropagatorResult result)
      {
        if (result.IsNull)
          return new bool?();
        return new bool?((bool) result.GetSimpleValue());
      }

      private static PropagatorResult ConvertBoolToResult(
        bool? booleanValue,
        params PropagatorResult[] inputs)
      {
        object obj = !booleanValue.HasValue ? (object) null : (object) booleanValue.Value;
        return PropagatorResult.CreateSimpleValue(Propagator.Evaluator.PropagateUnknownAndPreserveFlags((PropagatorResult) null, (IEnumerable<PropagatorResult>) inputs), obj);
      }

      public override PropagatorResult Visit(DbIsOfExpression predicate)
      {
        Check.NotNull<DbIsOfExpression>(predicate, nameof (predicate));
        if (DbExpressionKind.IsOfOnly != predicate.ExpressionKind)
          throw this.ConstructNotSupportedException((DbExpression) predicate);
        PropagatorResult propagatorResult = this.Visit(predicate.Argument);
        return Propagator.Evaluator.ConvertBoolToResult(new bool?(!propagatorResult.IsNull && propagatorResult.StructuralType.EdmEquals((MetadataItem) predicate.OfType.EdmType)), propagatorResult);
      }

      public override PropagatorResult Visit(DbComparisonExpression predicate)
      {
        Check.NotNull<DbComparisonExpression>(predicate, nameof (predicate));
        if (DbExpressionKind.Equals != predicate.ExpressionKind)
          throw this.ConstructNotSupportedException((DbExpression) predicate);
        PropagatorResult propagatorResult1 = this.Visit(predicate.Left);
        PropagatorResult propagatorResult2 = this.Visit(predicate.Right);
        bool? booleanValue;
        if (propagatorResult1.IsNull || propagatorResult2.IsNull)
        {
          booleanValue = new bool?();
        }
        else
        {
          object simpleValue1 = propagatorResult1.GetSimpleValue();
          object simpleValue2 = propagatorResult2.GetSimpleValue();
          booleanValue = new bool?(ByValueEqualityComparer.Default.Equals(simpleValue1, simpleValue2));
        }
        return Propagator.Evaluator.ConvertBoolToResult(booleanValue, propagatorResult1, propagatorResult2);
      }

      public override PropagatorResult Visit(DbAndExpression predicate)
      {
        Check.NotNull<DbAndExpression>(predicate, nameof (predicate));
        PropagatorResult result1 = this.Visit(predicate.Left);
        PropagatorResult result2 = this.Visit(predicate.Right);
        bool? left = Propagator.Evaluator.ConvertResultToBool(result1);
        bool? right = Propagator.Evaluator.ConvertResultToBool(result2);
        if (left.HasValue && !left.Value && Propagator.Evaluator.PreservedAndKnown(result1) || right.HasValue && !right.Value && Propagator.Evaluator.PreservedAndKnown(result2))
          return Propagator.Evaluator.CreatePerservedAndKnownResult((object) false);
        return Propagator.Evaluator.ConvertBoolToResult(left.And(right), result1, result2);
      }

      public override PropagatorResult Visit(DbOrExpression predicate)
      {
        Check.NotNull<DbOrExpression>(predicate, nameof (predicate));
        PropagatorResult result1 = this.Visit(predicate.Left);
        PropagatorResult result2 = this.Visit(predicate.Right);
        bool? left = Propagator.Evaluator.ConvertResultToBool(result1);
        bool? right = Propagator.Evaluator.ConvertResultToBool(result2);
        if (left.HasValue && left.Value && Propagator.Evaluator.PreservedAndKnown(result1) || right.HasValue && right.Value && Propagator.Evaluator.PreservedAndKnown(result2))
          return Propagator.Evaluator.CreatePerservedAndKnownResult((object) true);
        return Propagator.Evaluator.ConvertBoolToResult(left.Or(right), result1, result2);
      }

      private static PropagatorResult CreatePerservedAndKnownResult(object value)
      {
        return PropagatorResult.CreateSimpleValue(PropagatorFlags.Preserve, value);
      }

      private static bool PreservedAndKnown(PropagatorResult result)
      {
        return PropagatorFlags.Preserve == (result.PropagatorFlags & (PropagatorFlags.Preserve | PropagatorFlags.Unknown));
      }

      public override PropagatorResult Visit(DbNotExpression predicate)
      {
        Check.NotNull<DbNotExpression>(predicate, nameof (predicate));
        PropagatorResult result = this.Visit(predicate.Argument);
        return Propagator.Evaluator.ConvertBoolToResult(Propagator.Evaluator.ConvertResultToBool(result).Not(), result);
      }

      public override PropagatorResult Visit(DbCaseExpression node)
      {
        Check.NotNull<DbCaseExpression>(node, nameof (node));
        int index = -1;
        int num = 0;
        List<PropagatorResult> propagatorResultList = new List<PropagatorResult>();
        foreach (DbExpression expression in (IEnumerable<DbExpression>) node.When)
        {
          PropagatorResult result = this.Visit(expression);
          propagatorResultList.Add(result);
          bool? nullable = Propagator.Evaluator.ConvertResultToBool(result);
          if (nullable.HasValue && nullable.GetValueOrDefault())
          {
            index = num;
            break;
          }
          ++num;
        }
        PropagatorResult result1 = -1 != index ? this.Visit(node.Then[index]) : this.Visit(node.Else);
        propagatorResultList.Add(result1);
        PropagatorFlags flags = Propagator.Evaluator.PropagateUnknownAndPreserveFlags(result1, (IEnumerable<PropagatorResult>) propagatorResultList);
        return result1.ReplicateResultWithNewFlags(flags);
      }

      public override PropagatorResult Visit(DbVariableReferenceExpression node)
      {
        Check.NotNull<DbVariableReferenceExpression>(node, nameof (node));
        return this.m_row;
      }

      public override PropagatorResult Visit(DbPropertyExpression node)
      {
        Check.NotNull<DbPropertyExpression>(node, nameof (node));
        PropagatorResult propagatorResult = this.Visit(node.Instance);
        return !propagatorResult.IsNull ? propagatorResult.GetMemberValue(node.Property) : PropagatorResult.CreateSimpleValue(propagatorResult.PropagatorFlags, (object) null);
      }

      public override PropagatorResult Visit(DbConstantExpression node)
      {
        Check.NotNull<DbConstantExpression>(node, nameof (node));
        return PropagatorResult.CreateSimpleValue(PropagatorFlags.Preserve, node.Value);
      }

      public override PropagatorResult Visit(DbRefKeyExpression node)
      {
        Check.NotNull<DbRefKeyExpression>(node, nameof (node));
        return this.Visit(node.Argument);
      }

      public override PropagatorResult Visit(DbNullExpression node)
      {
        Check.NotNull<DbNullExpression>(node, nameof (node));
        return PropagatorResult.CreateSimpleValue(PropagatorFlags.Preserve, (object) null);
      }

      public override PropagatorResult Visit(DbTreatExpression node)
      {
        Check.NotNull<DbTreatExpression>(node, nameof (node));
        PropagatorResult propagatorResult = this.Visit(node.Argument);
        if (MetadataHelper.IsSuperTypeOf(node.ResultType.EdmType, (EdmType) propagatorResult.StructuralType))
          return propagatorResult;
        return PropagatorResult.CreateSimpleValue(propagatorResult.PropagatorFlags, (object) null);
      }

      public override PropagatorResult Visit(DbCastExpression node)
      {
        Check.NotNull<DbCastExpression>(node, nameof (node));
        PropagatorResult propagatorResult = this.Visit(node.Argument);
        TypeUsage resultType = node.ResultType;
        if (!propagatorResult.IsSimple || BuiltInTypeKind.PrimitiveType != resultType.EdmType.BuiltInTypeKind)
          throw new NotSupportedException(Strings.Update_UnsupportedCastArgument((object) resultType.EdmType.Name));
        object obj;
        if (propagatorResult.IsNull)
        {
          obj = (object) null;
        }
        else
        {
          try
          {
            obj = Propagator.Evaluator.Cast(propagatorResult.GetSimpleValue(), ((PrimitiveType) resultType.EdmType).ClrEquivalentType);
          }
          catch
          {
            throw;
          }
        }
        return propagatorResult.ReplicateResultWithNewValue(obj);
      }

      private static object Cast(object value, Type clrPrimitiveType)
      {
        IFormatProvider invariantCulture = (IFormatProvider) CultureInfo.InvariantCulture;
        if (value == null || value == DBNull.Value || value.GetType() == clrPrimitiveType)
          return value;
        if (value is DateTime && clrPrimitiveType == typeof (DateTimeOffset))
          return (object) new DateTimeOffset(((DateTime) value).Ticks, TimeSpan.Zero);
        return Convert.ChangeType(value, clrPrimitiveType, invariantCulture);
      }

      public override PropagatorResult Visit(DbIsNullExpression node)
      {
        Check.NotNull<DbIsNullExpression>(node, nameof (node));
        PropagatorResult propagatorResult = this.Visit(node.Argument);
        return Propagator.Evaluator.ConvertBoolToResult(new bool?(propagatorResult.IsNull), propagatorResult);
      }

      private static PropagatorFlags PropagateUnknownAndPreserveFlags(
        PropagatorResult result,
        IEnumerable<PropagatorResult> inputs)
      {
        bool flag1 = false;
        bool flag2 = true;
        bool flag3 = true;
        foreach (PropagatorResult input in inputs)
        {
          flag3 = false;
          PropagatorFlags propagatorFlags = input.PropagatorFlags;
          if ((PropagatorFlags.Unknown & propagatorFlags) != PropagatorFlags.NoFlags)
            flag1 = true;
          if ((PropagatorFlags.Preserve & propagatorFlags) == PropagatorFlags.NoFlags)
            flag2 = false;
        }
        if (flag3)
          flag2 = false;
        if (result != null)
        {
          PropagatorFlags propagatorFlags = result.PropagatorFlags;
          if (flag1)
            propagatorFlags |= PropagatorFlags.Unknown;
          if (!flag2)
            propagatorFlags &= ~PropagatorFlags.Preserve;
          return propagatorFlags;
        }
        PropagatorFlags propagatorFlags1 = PropagatorFlags.NoFlags;
        if (flag1)
          propagatorFlags1 |= PropagatorFlags.Unknown;
        if (flag2)
          propagatorFlags1 |= PropagatorFlags.Preserve;
        return propagatorFlags1;
      }
    }

    internal class ExtentPlaceholderCreator
    {
      private static readonly Dictionary<PrimitiveTypeKind, object> _typeDefaultMap = Propagator.ExtentPlaceholderCreator.InitializeTypeDefaultMap();
      private static readonly Lazy<Dictionary<PrimitiveTypeKind, object>> _spatialTypeDefaultMap = new Lazy<Dictionary<PrimitiveTypeKind, object>>(new Func<Dictionary<PrimitiveTypeKind, object>>(Propagator.ExtentPlaceholderCreator.InitializeSpatialTypeDefaultMap));

      private static Dictionary<PrimitiveTypeKind, object> InitializeTypeDefaultMap()
      {
        return new Dictionary<PrimitiveTypeKind, object>((IEqualityComparer<PrimitiveTypeKind>) EqualityComparer<PrimitiveTypeKind>.Default)
        {
          [PrimitiveTypeKind.Binary] = (object) new byte[0],
          [PrimitiveTypeKind.Boolean] = (object) false,
          [PrimitiveTypeKind.Byte] = (object) (byte) 0,
          [PrimitiveTypeKind.DateTime] = (object) new DateTime(),
          [PrimitiveTypeKind.Time] = (object) new TimeSpan(),
          [PrimitiveTypeKind.DateTimeOffset] = (object) new DateTimeOffset(),
          [PrimitiveTypeKind.Decimal] = (object) new Decimal(0),
          [PrimitiveTypeKind.Double] = (object) 0.0,
          [PrimitiveTypeKind.Guid] = (object) new Guid(),
          [PrimitiveTypeKind.Int16] = (object) (short) 0,
          [PrimitiveTypeKind.Int32] = (object) 0,
          [PrimitiveTypeKind.Int64] = (object) 0L,
          [PrimitiveTypeKind.Single] = (object) 0.0f,
          [PrimitiveTypeKind.SByte] = (object) (sbyte) 0,
          [PrimitiveTypeKind.String] = (object) string.Empty
        };
      }

      private static Dictionary<PrimitiveTypeKind, object> InitializeSpatialTypeDefaultMap()
      {
        return new Dictionary<PrimitiveTypeKind, object>((IEqualityComparer<PrimitiveTypeKind>) EqualityComparer<PrimitiveTypeKind>.Default)
        {
          [PrimitiveTypeKind.Geometry] = (object) DbGeometry.FromText("POINT EMPTY"),
          [PrimitiveTypeKind.GeometryPoint] = (object) DbGeometry.FromText("POINT EMPTY"),
          [PrimitiveTypeKind.GeometryLineString] = (object) DbGeometry.FromText("LINESTRING EMPTY"),
          [PrimitiveTypeKind.GeometryPolygon] = (object) DbGeometry.FromText("POLYGON EMPTY"),
          [PrimitiveTypeKind.GeometryMultiPoint] = (object) DbGeometry.FromText("MULTIPOINT EMPTY"),
          [PrimitiveTypeKind.GeometryMultiLineString] = (object) DbGeometry.FromText("MULTILINESTRING EMPTY"),
          [PrimitiveTypeKind.GeometryMultiPolygon] = (object) DbGeometry.FromText("MULTIPOLYGON EMPTY"),
          [PrimitiveTypeKind.GeometryCollection] = (object) DbGeometry.FromText("GEOMETRYCOLLECTION EMPTY"),
          [PrimitiveTypeKind.Geography] = (object) DbGeography.FromText("POINT EMPTY"),
          [PrimitiveTypeKind.GeographyPoint] = (object) DbGeography.FromText("POINT EMPTY"),
          [PrimitiveTypeKind.GeographyLineString] = (object) DbGeography.FromText("LINESTRING EMPTY"),
          [PrimitiveTypeKind.GeographyPolygon] = (object) DbGeography.FromText("POLYGON EMPTY"),
          [PrimitiveTypeKind.GeographyMultiPoint] = (object) DbGeography.FromText("MULTIPOINT EMPTY"),
          [PrimitiveTypeKind.GeographyMultiLineString] = (object) DbGeography.FromText("MULTILINESTRING EMPTY"),
          [PrimitiveTypeKind.GeographyMultiPolygon] = (object) DbGeography.FromText("MULTIPOLYGON EMPTY"),
          [PrimitiveTypeKind.GeographyCollection] = (object) DbGeography.FromText("GEOMETRYCOLLECTION EMPTY")
        };
      }

      private static bool TryGetDefaultValue(PrimitiveType primitiveType, out object defaultValue)
      {
        PrimitiveTypeKind primitiveTypeKind = primitiveType.PrimitiveTypeKind;
        if (!Helper.IsSpatialType(primitiveType))
          return Propagator.ExtentPlaceholderCreator._typeDefaultMap.TryGetValue(primitiveTypeKind, out defaultValue);
        return Propagator.ExtentPlaceholderCreator._spatialTypeDefaultMap.Value.TryGetValue(primitiveTypeKind, out defaultValue);
      }

      internal static PropagatorResult CreatePlaceholder(EntitySetBase extent)
      {
        Propagator.ExtentPlaceholderCreator placeholderCreator = new Propagator.ExtentPlaceholderCreator();
        AssociationSet associationSet = extent as AssociationSet;
        if (associationSet != null)
          return placeholderCreator.CreateAssociationSetPlaceholder(associationSet);
        EntitySet entitySet = extent as EntitySet;
        if (entitySet != null)
          return placeholderCreator.CreateEntitySetPlaceholder(entitySet);
        throw new NotSupportedException(Strings.Update_UnsupportedExtentType((object) extent.Name, (object) extent.GetType().Name));
      }

      private PropagatorResult CreateEntitySetPlaceholder(EntitySet entitySet)
      {
        ReadOnlyMetadataCollection<EdmProperty> properties = entitySet.ElementType.Properties;
        PropagatorResult[] values = new PropagatorResult[properties.Count];
        for (int index = 0; index < properties.Count; ++index)
        {
          PropagatorResult memberPlaceholder = this.CreateMemberPlaceholder((EdmMember) properties[index]);
          values[index] = memberPlaceholder;
        }
        return PropagatorResult.CreateStructuralValue(values, (StructuralType) entitySet.ElementType, false);
      }

      private PropagatorResult CreateAssociationSetPlaceholder(
        AssociationSet associationSet)
      {
        ReadOnlyMetadataCollection<AssociationEndMember> associationEndMembers = associationSet.ElementType.AssociationEndMembers;
        PropagatorResult[] values1 = new PropagatorResult[associationEndMembers.Count];
        for (int index1 = 0; index1 < associationEndMembers.Count; ++index1)
        {
          EntityType elementType = (EntityType) ((RefType) associationEndMembers[index1].TypeUsage.EdmType).ElementType;
          PropagatorResult[] values2 = new PropagatorResult[elementType.KeyMembers.Count];
          for (int index2 = 0; index2 < elementType.KeyMembers.Count; ++index2)
          {
            PropagatorResult memberPlaceholder = this.CreateMemberPlaceholder(elementType.KeyMembers[index2]);
            values2[index2] = memberPlaceholder;
          }
          RowType keyRowType = elementType.GetKeyRowType();
          PropagatorResult structuralValue = PropagatorResult.CreateStructuralValue(values2, (StructuralType) keyRowType, false);
          values1[index1] = structuralValue;
        }
        return PropagatorResult.CreateStructuralValue(values1, (StructuralType) associationSet.ElementType, false);
      }

      private PropagatorResult CreateMemberPlaceholder(EdmMember member)
      {
        return this.Visit(member);
      }

      internal PropagatorResult Visit(EdmMember node)
      {
        TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(node);
        PropagatorResult result;
        if (Helper.IsScalarType(modelTypeUsage.EdmType))
        {
          Propagator.ExtentPlaceholderCreator.GetPropagatorResultForPrimitiveType(Helper.AsPrimitive(modelTypeUsage.EdmType), out result);
        }
        else
        {
          StructuralType edmType = (StructuralType) modelTypeUsage.EdmType;
          IBaseList<EdmMember> structuralMembers = TypeHelpers.GetAllStructuralMembers((EdmType) edmType);
          PropagatorResult[] values = new PropagatorResult[structuralMembers.Count];
          for (int index = 0; index < structuralMembers.Count; ++index)
            values[index] = this.Visit(structuralMembers[index]);
          result = PropagatorResult.CreateStructuralValue(values, edmType, false);
        }
        return result;
      }

      internal static void GetPropagatorResultForPrimitiveType(
        PrimitiveType primitiveType,
        out PropagatorResult result)
      {
        object defaultValue;
        if (!Propagator.ExtentPlaceholderCreator.TryGetDefaultValue(primitiveType, out defaultValue))
          defaultValue = (object) (byte) 0;
        result = PropagatorResult.CreateSimpleValue(PropagatorFlags.NoFlags, defaultValue);
      }
    }

    private class JoinPropagator
    {
      private static readonly Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops> _innerJoinInsertRules = new Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops>((IEqualityComparer<Propagator.JoinPropagator.Ops>) EqualityComparer<Propagator.JoinPropagator.Ops>.Default);
      private static readonly Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops> _innerJoinDeleteRules = new Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops>((IEqualityComparer<Propagator.JoinPropagator.Ops>) EqualityComparer<Propagator.JoinPropagator.Ops>.Default);
      private static readonly Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops> _leftOuterJoinInsertRules = new Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops>((IEqualityComparer<Propagator.JoinPropagator.Ops>) EqualityComparer<Propagator.JoinPropagator.Ops>.Default);
      private static readonly Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops> _leftOuterJoinDeleteRules = new Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops>((IEqualityComparer<Propagator.JoinPropagator.Ops>) EqualityComparer<Propagator.JoinPropagator.Ops>.Default);
      private readonly DbJoinExpression m_joinExpression;
      private readonly Propagator m_parent;
      private readonly Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops> m_insertRules;
      private readonly Dictionary<Propagator.JoinPropagator.Ops, Propagator.JoinPropagator.Ops> m_deleteRules;
      private readonly ReadOnlyCollection<DbExpression> m_leftKeySelectors;
      private readonly ReadOnlyCollection<DbExpression> m_rightKeySelectors;
      private readonly ChangeNode m_left;
      private readonly ChangeNode m_right;
      private readonly CompositeKey m_leftPlaceholderKey;
      private readonly CompositeKey m_rightPlaceholderKey;

      internal JoinPropagator(
        ChangeNode left,
        ChangeNode right,
        DbJoinExpression node,
        Propagator parent)
      {
        this.m_left = left;
        this.m_right = right;
        this.m_joinExpression = node;
        this.m_parent = parent;
        if (DbExpressionKind.InnerJoin == this.m_joinExpression.ExpressionKind)
        {
          this.m_insertRules = Propagator.JoinPropagator._innerJoinInsertRules;
          this.m_deleteRules = Propagator.JoinPropagator._innerJoinDeleteRules;
        }
        else
        {
          this.m_insertRules = Propagator.JoinPropagator._leftOuterJoinInsertRules;
          this.m_deleteRules = Propagator.JoinPropagator._leftOuterJoinDeleteRules;
        }
        Propagator.JoinPropagator.JoinConditionVisitor.GetKeySelectors(node.JoinCondition, out this.m_leftKeySelectors, out this.m_rightKeySelectors);
        this.m_leftPlaceholderKey = Propagator.JoinPropagator.ExtractKey(this.m_left.Placeholder, this.m_leftKeySelectors);
        this.m_rightPlaceholderKey = Propagator.JoinPropagator.ExtractKey(this.m_right.Placeholder, this.m_rightKeySelectors);
      }

      [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
      static JoinPropagator()
      {
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftUpdate | Propagator.JoinPropagator.Ops.RightUpdate, Propagator.JoinPropagator.Ops.LeftInsertJoinRightInsert, Propagator.JoinPropagator.Ops.LeftDeleteJoinRightDelete, Propagator.JoinPropagator.Ops.LeftInsertJoinRightInsert, Propagator.JoinPropagator.Ops.LeftDeleteJoinRightDelete);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftDeleteJoinRightDelete, Propagator.JoinPropagator.Ops.Nothing, Propagator.JoinPropagator.Ops.LeftDeleteJoinRightDelete, Propagator.JoinPropagator.Ops.Nothing, Propagator.JoinPropagator.Ops.LeftDeleteJoinRightDelete);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftInsertJoinRightInsert, Propagator.JoinPropagator.Ops.LeftInsertJoinRightInsert, Propagator.JoinPropagator.Ops.Nothing, Propagator.JoinPropagator.Ops.LeftInsertJoinRightInsert, Propagator.JoinPropagator.Ops.Nothing);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftUpdate, Propagator.JoinPropagator.Ops.LeftInsertUnknownExtended, Propagator.JoinPropagator.Ops.LeftDeleteUnknownExtended, Propagator.JoinPropagator.Ops.LeftInsertUnknownExtended, Propagator.JoinPropagator.Ops.LeftDeleteUnknownExtended);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.RightUpdate, Propagator.JoinPropagator.Ops.RightInsertUnknownExtended, Propagator.JoinPropagator.Ops.RightDeleteUnknownExtended, Propagator.JoinPropagator.Ops.RightInsertUnknownExtended, Propagator.JoinPropagator.Ops.RightDeleteUnknownExtended);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftUpdate | Propagator.JoinPropagator.Ops.RightDelete, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.LeftInsertNullModifiedExtended, Propagator.JoinPropagator.Ops.LeftDeleteJoinRightDelete);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftUpdate | Propagator.JoinPropagator.Ops.RightInsert, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.LeftInsertJoinRightInsert, Propagator.JoinPropagator.Ops.LeftDeleteNullModifiedExtended);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftDelete, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Nothing, Propagator.JoinPropagator.Ops.LeftDeleteNullPreserveExtended);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftInsert, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.LeftInsertNullModifiedExtended, Propagator.JoinPropagator.Ops.Nothing);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.RightDelete, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.LeftUnknownNullModifiedExtended, Propagator.JoinPropagator.Ops.RightDeleteUnknownExtended);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.RightInsert, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.RightInsertUnknownExtended, Propagator.JoinPropagator.Ops.LeftUnknownNullModifiedExtended);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.RightUpdate | Propagator.JoinPropagator.Ops.LeftDelete, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftDelete | Propagator.JoinPropagator.Ops.RightInsert, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.RightUpdate | Propagator.JoinPropagator.Ops.LeftInsert, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported);
        Propagator.JoinPropagator.InitializeRule(Propagator.JoinPropagator.Ops.LeftInsert | Propagator.JoinPropagator.Ops.RightDelete, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported, Propagator.JoinPropagator.Ops.Unsupported);
      }

      private static void InitializeRule(
        Propagator.JoinPropagator.Ops input,
        Propagator.JoinPropagator.Ops joinInsert,
        Propagator.JoinPropagator.Ops joinDelete,
        Propagator.JoinPropagator.Ops lojInsert,
        Propagator.JoinPropagator.Ops lojDelete)
      {
        Propagator.JoinPropagator._innerJoinInsertRules.Add(input, joinInsert);
        Propagator.JoinPropagator._innerJoinDeleteRules.Add(input, joinDelete);
        Propagator.JoinPropagator._leftOuterJoinInsertRules.Add(input, lojInsert);
        Propagator.JoinPropagator._leftOuterJoinDeleteRules.Add(input, lojDelete);
      }

      internal ChangeNode Propagate()
      {
        ChangeNode result = Propagator.BuildChangeNode((DbExpression) this.m_joinExpression);
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> leftDeletes = this.ProcessKeys((IEnumerable<PropagatorResult>) this.m_left.Deleted, this.m_leftKeySelectors);
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> leftInserts = this.ProcessKeys((IEnumerable<PropagatorResult>) this.m_left.Inserted, this.m_leftKeySelectors);
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> rightDeletes = this.ProcessKeys((IEnumerable<PropagatorResult>) this.m_right.Deleted, this.m_rightKeySelectors);
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> rightInserts = this.ProcessKeys((IEnumerable<PropagatorResult>) this.m_right.Inserted, this.m_rightKeySelectors);
        foreach (CompositeKey key in leftDeletes.Keys.Concat<CompositeKey>((IEnumerable<CompositeKey>) leftInserts.Keys).Concat<CompositeKey>((IEnumerable<CompositeKey>) rightDeletes.Keys).Concat<CompositeKey>((IEnumerable<CompositeKey>) rightInserts.Keys).Distinct<CompositeKey>(this.m_parent.UpdateTranslator.KeyComparer))
          this.Propagate(key, result, leftDeletes, leftInserts, rightDeletes, rightInserts);
        result.Placeholder = this.CreateResultTuple(Tuple.Create<CompositeKey, PropagatorResult>((CompositeKey) null, this.m_left.Placeholder), Tuple.Create<CompositeKey, PropagatorResult>((CompositeKey) null, this.m_right.Placeholder), result);
        return result;
      }

      private void Propagate(
        CompositeKey key,
        ChangeNode result,
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> leftDeletes,
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> leftInserts,
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> rightDeletes,
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> rightInserts)
      {
        Tuple<CompositeKey, PropagatorResult> left1 = (Tuple<CompositeKey, PropagatorResult>) null;
        Tuple<CompositeKey, PropagatorResult> left2 = (Tuple<CompositeKey, PropagatorResult>) null;
        Tuple<CompositeKey, PropagatorResult> right1 = (Tuple<CompositeKey, PropagatorResult>) null;
        Tuple<CompositeKey, PropagatorResult> right2 = (Tuple<CompositeKey, PropagatorResult>) null;
        Propagator.JoinPropagator.Ops index = Propagator.JoinPropagator.Ops.Nothing;
        if (leftInserts.TryGetValue(key, out left1))
          index |= Propagator.JoinPropagator.Ops.LeftInsert;
        if (leftDeletes.TryGetValue(key, out left2))
          index |= Propagator.JoinPropagator.Ops.LeftDelete;
        if (rightInserts.TryGetValue(key, out right1))
          index |= Propagator.JoinPropagator.Ops.RightInsert;
        if (rightDeletes.TryGetValue(key, out right2))
          index |= Propagator.JoinPropagator.Ops.RightDelete;
        Propagator.JoinPropagator.Ops insertRule = this.m_insertRules[index];
        Propagator.JoinPropagator.Ops deleteRule = this.m_deleteRules[index];
        if (Propagator.JoinPropagator.Ops.Unsupported == insertRule || Propagator.JoinPropagator.Ops.Unsupported == deleteRule)
        {
          List<IEntityStateEntry> stateEntries = new List<IEntityStateEntry>();
          Action<Tuple<CompositeKey, PropagatorResult>> action = (Action<Tuple<CompositeKey, PropagatorResult>>) (r =>
          {
            if (r == null)
              return;
            stateEntries.AddRange((IEnumerable<IEntityStateEntry>) SourceInterpreter.GetAllStateEntries(r.Item2, this.m_parent.m_updateTranslator, this.m_parent.m_table));
          });
          action(left1);
          action(left2);
          action(right1);
          action(right2);
          throw new UpdateException(Strings.Update_InvalidChanges, (Exception) null, stateEntries.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
        }
        if ((Propagator.JoinPropagator.Ops.LeftUnknown & insertRule) != Propagator.JoinPropagator.Ops.Nothing)
          left1 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.LeftPlaceholder(key, Propagator.JoinPropagator.PopulateMode.Unknown));
        if ((Propagator.JoinPropagator.Ops.LeftUnknown & deleteRule) != Propagator.JoinPropagator.Ops.Nothing)
          left2 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.LeftPlaceholder(key, Propagator.JoinPropagator.PopulateMode.Unknown));
        if ((Propagator.JoinPropagator.Ops.RightNullModified & insertRule) != Propagator.JoinPropagator.Ops.Nothing)
          right1 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.RightPlaceholder(key, Propagator.JoinPropagator.PopulateMode.NullModified));
        else if ((Propagator.JoinPropagator.Ops.RightNullPreserve & insertRule) != Propagator.JoinPropagator.Ops.Nothing)
          right1 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.RightPlaceholder(key, Propagator.JoinPropagator.PopulateMode.NullPreserve));
        else if ((Propagator.JoinPropagator.Ops.RightUnknown & insertRule) != Propagator.JoinPropagator.Ops.Nothing)
          right1 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.RightPlaceholder(key, Propagator.JoinPropagator.PopulateMode.Unknown));
        if ((Propagator.JoinPropagator.Ops.RightNullModified & deleteRule) != Propagator.JoinPropagator.Ops.Nothing)
          right2 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.RightPlaceholder(key, Propagator.JoinPropagator.PopulateMode.NullModified));
        else if ((Propagator.JoinPropagator.Ops.RightNullPreserve & deleteRule) != Propagator.JoinPropagator.Ops.Nothing)
          right2 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.RightPlaceholder(key, Propagator.JoinPropagator.PopulateMode.NullPreserve));
        else if ((Propagator.JoinPropagator.Ops.RightUnknown & deleteRule) != Propagator.JoinPropagator.Ops.Nothing)
          right2 = Tuple.Create<CompositeKey, PropagatorResult>(key, this.RightPlaceholder(key, Propagator.JoinPropagator.PopulateMode.Unknown));
        if (left1 != null && right1 != null)
          result.Inserted.Add(this.CreateResultTuple(left1, right1, result));
        if (left2 == null || right2 == null)
          return;
        result.Deleted.Add(this.CreateResultTuple(left2, right2, result));
      }

      private PropagatorResult CreateResultTuple(
        Tuple<CompositeKey, PropagatorResult> left,
        Tuple<CompositeKey, PropagatorResult> right,
        ChangeNode result)
      {
        CompositeKey compositeKey1 = left.Item1;
        CompositeKey other = right.Item1;
        Dictionary<PropagatorResult, PropagatorResult> map = (Dictionary<PropagatorResult, PropagatorResult>) null;
        if (!object.ReferenceEquals((object) null, (object) compositeKey1) && !object.ReferenceEquals((object) null, (object) other) && !object.ReferenceEquals((object) compositeKey1, (object) other))
        {
          CompositeKey compositeKey2 = compositeKey1.Merge(this.m_parent.m_updateTranslator.KeyManager, other);
          map = new Dictionary<PropagatorResult, PropagatorResult>();
          for (int index = 0; index < compositeKey1.KeyComponents.Length; ++index)
          {
            map[compositeKey1.KeyComponents[index]] = compositeKey2.KeyComponents[index];
            map[other.KeyComponents[index]] = compositeKey2.KeyComponents[index];
          }
        }
        PropagatorResult propagatorResult = PropagatorResult.CreateStructuralValue(new PropagatorResult[2]
        {
          left.Item2,
          right.Item2
        }, (StructuralType) result.ElementType.EdmType, false);
        if (map != null)
        {
          PropagatorResult replacement;
          propagatorResult = propagatorResult.Replace((Func<PropagatorResult, PropagatorResult>) (original =>
          {
            if (!map.TryGetValue(original, out replacement))
              return original;
            return replacement;
          }));
        }
        return propagatorResult;
      }

      private PropagatorResult LeftPlaceholder(
        CompositeKey key,
        Propagator.JoinPropagator.PopulateMode mode)
      {
        return Propagator.JoinPropagator.PlaceholderPopulator.Populate(this.m_left.Placeholder, key, this.m_leftPlaceholderKey, mode);
      }

      private PropagatorResult RightPlaceholder(
        CompositeKey key,
        Propagator.JoinPropagator.PopulateMode mode)
      {
        return Propagator.JoinPropagator.PlaceholderPopulator.Populate(this.m_right.Placeholder, key, this.m_rightPlaceholderKey, mode);
      }

      private Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> ProcessKeys(
        IEnumerable<PropagatorResult> instances,
        ReadOnlyCollection<DbExpression> keySelectors)
      {
        Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>> dictionary = new Dictionary<CompositeKey, Tuple<CompositeKey, PropagatorResult>>(this.m_parent.UpdateTranslator.KeyComparer);
        foreach (PropagatorResult instance in instances)
        {
          CompositeKey key = Propagator.JoinPropagator.ExtractKey(instance, keySelectors);
          dictionary[key] = Tuple.Create<CompositeKey, PropagatorResult>(key, instance);
        }
        return dictionary;
      }

      private static CompositeKey ExtractKey(
        PropagatorResult change,
        ReadOnlyCollection<DbExpression> keySelectors)
      {
        PropagatorResult[] constants = new PropagatorResult[keySelectors.Count];
        for (int index = 0; index < keySelectors.Count; ++index)
        {
          PropagatorResult propagatorResult = Propagator.Evaluator.Evaluate(keySelectors[index], change);
          constants[index] = propagatorResult;
        }
        return new CompositeKey(constants);
      }

      [Flags]
      private enum Ops : uint
      {
        Nothing = 0,
        LeftInsert = 1,
        LeftDelete = 2,
        RightInsert = 4,
        RightDelete = 8,
        LeftUnknown = 32, // 0x00000020
        RightNullModified = 128, // 0x00000080
        RightNullPreserve = 256, // 0x00000100
        RightUnknown = 512, // 0x00000200
        LeftUpdate = LeftDelete | LeftInsert, // 0x00000003
        RightUpdate = RightDelete | RightInsert, // 0x0000000C
        Unsupported = 4096, // 0x00001000
        LeftInsertJoinRightInsert = RightInsert | LeftInsert, // 0x00000005
        LeftDeleteJoinRightDelete = RightDelete | LeftDelete, // 0x0000000A
        LeftInsertNullModifiedExtended = RightNullModified | LeftInsert, // 0x00000081
        LeftInsertNullPreserveExtended = RightNullPreserve | LeftInsert, // 0x00000101
        LeftInsertUnknownExtended = RightUnknown | LeftInsert, // 0x00000201
        LeftDeleteNullModifiedExtended = RightNullModified | LeftDelete, // 0x00000082
        LeftDeleteNullPreserveExtended = RightNullPreserve | LeftDelete, // 0x00000102
        LeftDeleteUnknownExtended = RightUnknown | LeftDelete, // 0x00000202
        LeftUnknownNullModifiedExtended = RightNullModified | LeftUnknown, // 0x000000A0
        LeftUnknownNullPreserveExtended = RightNullPreserve | LeftUnknown, // 0x00000120
        RightInsertUnknownExtended = LeftUnknown | RightInsert, // 0x00000024
        RightDeleteUnknownExtended = LeftUnknown | RightDelete, // 0x00000028
      }

      private class JoinConditionVisitor : UpdateExpressionVisitor<object>
      {
        private static readonly string _visitorName = typeof (Propagator.JoinPropagator.JoinConditionVisitor).FullName;
        private readonly List<DbExpression> m_leftKeySelectors;
        private readonly List<DbExpression> m_rightKeySelectors;

        private JoinConditionVisitor()
        {
          this.m_leftKeySelectors = new List<DbExpression>();
          this.m_rightKeySelectors = new List<DbExpression>();
        }

        protected override string VisitorName
        {
          get
          {
            return Propagator.JoinPropagator.JoinConditionVisitor._visitorName;
          }
        }

        internal static void GetKeySelectors(
          DbExpression joinCondition,
          out ReadOnlyCollection<DbExpression> leftKeySelectors,
          out ReadOnlyCollection<DbExpression> rightKeySelectors)
        {
          Propagator.JoinPropagator.JoinConditionVisitor conditionVisitor = new Propagator.JoinPropagator.JoinConditionVisitor();
          joinCondition.Accept<object>((DbExpressionVisitor<object>) conditionVisitor);
          leftKeySelectors = new ReadOnlyCollection<DbExpression>((IList<DbExpression>) conditionVisitor.m_leftKeySelectors);
          rightKeySelectors = new ReadOnlyCollection<DbExpression>((IList<DbExpression>) conditionVisitor.m_rightKeySelectors);
        }

        public override object Visit(DbAndExpression node)
        {
          Check.NotNull<DbAndExpression>(node, nameof (node));
          this.Visit(node.Left);
          this.Visit(node.Right);
          return (object) null;
        }

        public override object Visit(DbComparisonExpression node)
        {
          Check.NotNull<DbComparisonExpression>(node, nameof (node));
          if (DbExpressionKind.Equals != node.ExpressionKind)
            throw this.ConstructNotSupportedException((DbExpression) node);
          this.m_leftKeySelectors.Add(node.Left);
          this.m_rightKeySelectors.Add(node.Right);
          return (object) null;
        }
      }

      private enum PopulateMode
      {
        NullModified,
        NullPreserve,
        Unknown,
      }

      private static class PlaceholderPopulator
      {
        internal static PropagatorResult Populate(
          PropagatorResult placeholder,
          CompositeKey key,
          CompositeKey placeholderKey,
          Propagator.JoinPropagator.PopulateMode mode)
        {
          bool isNull = mode == Propagator.JoinPropagator.PopulateMode.NullModified || mode == Propagator.JoinPropagator.PopulateMode.NullPreserve;
          bool flag = mode == Propagator.JoinPropagator.PopulateMode.NullPreserve || mode == Propagator.JoinPropagator.PopulateMode.Unknown;
          PropagatorFlags flags = PropagatorFlags.NoFlags;
          if (!isNull)
            flags |= PropagatorFlags.Unknown;
          if (flag)
            flags |= PropagatorFlags.Preserve;
          return placeholder.Replace((Func<PropagatorResult, PropagatorResult>) (node =>
          {
            int index1 = -1;
            for (int index2 = 0; index2 < placeholderKey.KeyComponents.Length; ++index2)
            {
              if (placeholderKey.KeyComponents[index2] == node)
              {
                index1 = index2;
                break;
              }
            }
            if (index1 != -1)
              return key.KeyComponents[index1];
            return PropagatorResult.CreateSimpleValue(flags, isNull ? (object) null : node.GetSimpleValue());
          }));
        }
      }
    }
  }
}
