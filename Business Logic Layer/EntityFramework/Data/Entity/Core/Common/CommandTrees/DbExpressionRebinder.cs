// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbExpressionRebinder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Ensures that all metadata in a given expression tree is from the specified metadata workspace,
  /// potentially rebinding and rebuilding the expressions to appropriate replacement metadata where necessary.
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rebinder")]
  public class DbExpressionRebinder : DefaultExpressionVisitor
  {
    private readonly MetadataWorkspace _metadata;
    private readonly Perspective _perspective;

    internal DbExpressionRebinder()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionRebinder" /> class.</summary>
    /// <param name="targetWorkspace">The target workspace.</param>
    protected DbExpressionRebinder(MetadataWorkspace targetWorkspace)
    {
      this._metadata = targetWorkspace;
      this._perspective = (Perspective) new ModelPerspective(targetWorkspace);
    }

    /// <summary>Implements the visitor pattern for the entity set.</summary>
    /// <returns>The implemented visitor pattern.</returns>
    /// <param name="entitySet">The entity set.</param>
    protected override EntitySetBase VisitEntitySet(EntitySetBase entitySet)
    {
      EntityContainer entityContainer;
      if (!this._metadata.TryGetEntityContainer(entitySet.EntityContainer.Name, entitySet.EntityContainer.DataSpace, out entityContainer))
        throw new ArgumentException(Strings.Cqt_Copier_EntityContainerNotFound((object) entitySet.EntityContainer.Name));
      EntitySetBase entitySetBase = (EntitySetBase) null;
      if (entityContainer.BaseEntitySets.TryGetValue(entitySet.Name, false, out entitySetBase) && entitySetBase != null && entitySet.BuiltInTypeKind == entitySetBase.BuiltInTypeKind)
        return entitySetBase;
      throw new ArgumentException(Strings.Cqt_Copier_EntitySetNotFound((object) entitySet.EntityContainer.Name, (object) entitySet.Name));
    }

    /// <summary>Implements the visitor pattern for the function.</summary>
    /// <returns>The implemented visitor pattern.</returns>
    /// <param name="functionMetadata">The function metadata.</param>
    protected override EdmFunction VisitFunction(EdmFunction functionMetadata)
    {
      List<TypeUsage> typeUsageList = new List<TypeUsage>(functionMetadata.Parameters.Count);
      foreach (FunctionParameter parameter in functionMetadata.Parameters)
      {
        TypeUsage typeUsage = this.VisitTypeUsage(parameter.TypeUsage);
        typeUsageList.Add(typeUsage);
      }
      if (DataSpace.SSpace == functionMetadata.DataSpace)
      {
        EdmFunction function = (EdmFunction) null;
        if (this._metadata.TryGetFunction(functionMetadata.Name, functionMetadata.NamespaceName, typeUsageList.ToArray(), false, functionMetadata.DataSpace, out function) && function != null)
          return function;
      }
      else
      {
        IList<EdmFunction> functionOverloads;
        if (this._perspective.TryGetFunctionByName(functionMetadata.NamespaceName, functionMetadata.Name, false, out functionOverloads))
        {
          bool isAmbiguous;
          EdmFunction edmFunction = FunctionOverloadResolver.ResolveFunctionOverloads(functionOverloads, (IList<TypeUsage>) typeUsageList, false, out isAmbiguous);
          if (!isAmbiguous && edmFunction != null)
            return edmFunction;
        }
      }
      throw new ArgumentException(Strings.Cqt_Copier_FunctionNotFound((object) TypeHelpers.GetFullName(functionMetadata.NamespaceName, functionMetadata.Name)));
    }

    /// <summary>Implements the visitor pattern for the type.</summary>
    /// <returns>The implemented visitor pattern.</returns>
    /// <param name="type">The type.</param>
    protected override EdmType VisitType(EdmType type)
    {
      EdmType type1 = type;
      if (BuiltInTypeKind.RefType == type.BuiltInTypeKind)
      {
        RefType refType = (RefType) type;
        EntityType entityType = (EntityType) this.VisitType((EdmType) refType.ElementType);
        if (!object.ReferenceEquals((object) refType.ElementType, (object) entityType))
          type1 = (EdmType) new RefType(entityType);
      }
      else if (BuiltInTypeKind.CollectionType == type.BuiltInTypeKind)
      {
        CollectionType collectionType = (CollectionType) type;
        TypeUsage elementType = this.VisitTypeUsage(collectionType.TypeUsage);
        if (!object.ReferenceEquals((object) collectionType.TypeUsage, (object) elementType))
          type1 = (EdmType) new CollectionType(elementType);
      }
      else if (BuiltInTypeKind.RowType == type.BuiltInTypeKind)
      {
        RowType rowType = (RowType) type;
        List<KeyValuePair<string, TypeUsage>> source = (List<KeyValuePair<string, TypeUsage>>) null;
        for (int index = 0; index < rowType.Properties.Count; ++index)
        {
          EdmProperty property = rowType.Properties[index];
          TypeUsage typeUsage = this.VisitTypeUsage(property.TypeUsage);
          if (!object.ReferenceEquals((object) property.TypeUsage, (object) typeUsage))
          {
            if (source == null)
              source = new List<KeyValuePair<string, TypeUsage>>(rowType.Properties.Select<EdmProperty, KeyValuePair<string, TypeUsage>>((Func<EdmProperty, KeyValuePair<string, TypeUsage>>) (prop => new KeyValuePair<string, TypeUsage>(prop.Name, prop.TypeUsage))));
            source[index] = new KeyValuePair<string, TypeUsage>(property.Name, typeUsage);
          }
        }
        if (source != null)
          type1 = (EdmType) new RowType(source.Select<KeyValuePair<string, TypeUsage>, EdmProperty>((Func<KeyValuePair<string, TypeUsage>, EdmProperty>) (propInfo => new EdmProperty(propInfo.Key, propInfo.Value))), rowType.InitializerMetadata);
      }
      else if (!this._metadata.TryGetType(type.Name, type.NamespaceName, type.DataSpace, out type1) || type1 == null)
        throw new ArgumentException(Strings.Cqt_Copier_TypeNotFound((object) TypeHelpers.GetFullName(type.NamespaceName, type.Name)));
      return type1;
    }

    /// <summary>Implements the visitor pattern for the type usage.</summary>
    /// <returns>The implemented visitor pattern.</returns>
    /// <param name="type">The type.</param>
    protected override TypeUsage VisitTypeUsage(TypeUsage type)
    {
      EdmType edmType = this.VisitType(type.EdmType);
      if (object.ReferenceEquals((object) edmType, (object) type.EdmType))
        return type;
      Facet[] facetArray = new Facet[type.Facets.Count];
      int index = 0;
      foreach (Facet facet in type.Facets)
      {
        facetArray[index] = facet;
        ++index;
      }
      return TypeUsage.Create(edmType, (IEnumerable<Facet>) facetArray);
    }

    private static bool TryGetMember<TMember>(
      DbExpression instance,
      string memberName,
      out TMember member)
      where TMember : EdmMember
    {
      member = default (TMember);
      StructuralType edmType = instance.ResultType.EdmType as StructuralType;
      if (edmType != null)
      {
        EdmMember edmMember = (EdmMember) null;
        if (edmType.Members.TryGetValue(memberName, false, out edmMember))
          member = edmMember as TMember;
      }
      return (object) member != null;
    }

    /// <summary>Implements the visitor pattern for retrieving an instance property.</summary>
    /// <returns>The implemented visitor.</returns>
    /// <param name="expression">The expression.</param>
    public override DbExpression Visit(DbPropertyExpression expression)
    {
      Check.NotNull<DbPropertyExpression>(expression, nameof (expression));
      DbExpression dbExpression = (DbExpression) expression;
      DbExpression instance = this.VisitExpression(expression.Instance);
      if (!object.ReferenceEquals((object) expression.Instance, (object) instance))
      {
        if (Helper.IsRelationshipEndMember(expression.Property))
        {
          RelationshipEndMember member;
          if (!DbExpressionRebinder.TryGetMember<RelationshipEndMember>(instance, expression.Property.Name, out member))
          {
            EdmType edmType = instance.ResultType.EdmType;
            throw new ArgumentException(Strings.Cqt_Copier_EndNotFound((object) expression.Property.Name, (object) TypeHelpers.GetFullName(edmType.NamespaceName, edmType.Name)));
          }
          dbExpression = (DbExpression) instance.Property(member);
        }
        else if (Helper.IsNavigationProperty(expression.Property))
        {
          NavigationProperty member;
          if (!DbExpressionRebinder.TryGetMember<NavigationProperty>(instance, expression.Property.Name, out member))
          {
            EdmType edmType = instance.ResultType.EdmType;
            throw new ArgumentException(Strings.Cqt_Copier_NavPropertyNotFound((object) expression.Property.Name, (object) TypeHelpers.GetFullName(edmType.NamespaceName, edmType.Name)));
          }
          dbExpression = (DbExpression) instance.Property(member);
        }
        else
        {
          EdmProperty member;
          if (!DbExpressionRebinder.TryGetMember<EdmProperty>(instance, expression.Property.Name, out member))
          {
            EdmType edmType = instance.ResultType.EdmType;
            throw new ArgumentException(Strings.Cqt_Copier_PropertyNotFound((object) expression.Property.Name, (object) TypeHelpers.GetFullName(edmType.NamespaceName, edmType.Name)));
          }
          dbExpression = (DbExpression) instance.Property(member);
        }
      }
      return dbExpression;
    }
  }
}
