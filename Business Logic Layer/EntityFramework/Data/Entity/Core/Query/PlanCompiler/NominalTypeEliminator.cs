// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.NominalTypeEliminator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class NominalTypeEliminator : BasicOpVisitorOfNode
  {
    private const string PrefixMatchCharacter = "%";
    private readonly Dictionary<Var, PropertyRefList> m_varPropertyMap;
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, PropertyRefList> m_nodePropertyMap;
    private readonly VarInfoMap m_varInfoMap;
    private readonly System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler m_compilerState;
    private readonly StructuredTypeInfo m_typeInfo;
    private readonly Dictionary<EdmFunction, EdmProperty[]> m_tvfResultKeys;
    private readonly Dictionary<TypeUsage, TypeUsage> m_typeToNewTypeMap;

    private Command m_command
    {
      get
      {
        return this.m_compilerState.Command;
      }
    }

    private NominalTypeEliminator(
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState,
      StructuredTypeInfo typeInfo,
      Dictionary<Var, PropertyRefList> varPropertyMap,
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, PropertyRefList> nodePropertyMap,
      Dictionary<EdmFunction, EdmProperty[]> tvfResultKeys)
    {
      this.m_compilerState = compilerState;
      this.m_typeInfo = typeInfo;
      this.m_varPropertyMap = varPropertyMap;
      this.m_nodePropertyMap = nodePropertyMap;
      this.m_varInfoMap = new VarInfoMap();
      this.m_tvfResultKeys = tvfResultKeys;
      this.m_typeToNewTypeMap = new Dictionary<TypeUsage, TypeUsage>((IEqualityComparer<TypeUsage>) TypeUsageEqualityComparer.Instance);
    }

    internal static void Process(
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState,
      StructuredTypeInfo structuredTypeInfo,
      Dictionary<EdmFunction, EdmProperty[]> tvfResultKeys)
    {
      Dictionary<Var, PropertyRefList> varPropertyRefs;
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, PropertyRefList> nodePropertyRefs;
      PropertyPushdownHelper.Process(compilerState.Command, out varPropertyRefs, out nodePropertyRefs);
      new NominalTypeEliminator(compilerState, structuredTypeInfo, varPropertyRefs, nodePropertyRefs, tvfResultKeys).Process();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PhysicalProjectOp")]
    private void Process()
    {
      foreach (ParameterVar oldVar in this.m_command.Vars.OfType<ParameterVar>().Where<ParameterVar>((Func<ParameterVar, bool>) (v =>
      {
        if (!TypeSemantics.IsEnumerationType(v.Type))
          return TypeSemantics.IsStrongSpatialType(v.Type);
        return true;
      })).ToArray<ParameterVar>())
      {
        ParameterVar parameterVar = TypeSemantics.IsEnumerationType(oldVar.Type) ? this.m_command.ReplaceEnumParameterVar(oldVar) : this.m_command.ReplaceStrongSpatialParameterVar(oldVar);
        this.m_varInfoMap.CreatePrimitiveTypeVarInfo((Var) oldVar, (Var) parameterVar);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node root = this.m_command.Root;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(root.Op.OpType == OpType.PhysicalProject, "root node is not PhysicalProjectOp?");
      root.Op.Accept<System.Data.Entity.Core.Query.InternalTrees.Node>((BasicOpVisitorOfT<System.Data.Entity.Core.Query.InternalTrees.Node>) this, root);
    }

    private TypeUsage DefaultTypeIdType
    {
      get
      {
        return this.m_command.StringType;
      }
    }

    private TypeUsage GetNewType(TypeUsage type)
    {
      TypeUsage typeUsage1;
      if (this.m_typeToNewTypeMap.TryGetValue(type, out typeUsage1))
        return typeUsage1;
      CollectionType type1;
      TypeUsage typeUsage2 = !TypeHelpers.TryGetEdmType<CollectionType>(type, out type1) ? (!TypeUtils.IsStructuredType(type) ? (!TypeSemantics.IsEnumerationType(type) ? (!TypeSemantics.IsStrongSpatialType(type) ? type : TypeHelpers.CreateSpatialUnionTypeUsage(type)) : TypeHelpers.CreateEnumUnderlyingTypeUsage(type)) : this.m_typeInfo.GetTypeInfo(type).FlattenedTypeUsage) : TypeUtils.CreateCollectionType(this.GetNewType(type1.TypeUsage));
      this.m_typeToNewTypeMap[type] = typeUsage2;
      return typeUsage2;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildAccessor(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      EdmProperty property)
    {
      Op op = input.Op;
      NewRecordOp newRecordOp = op as NewRecordOp;
      if (newRecordOp != null)
      {
        int fieldPosition;
        if (newRecordOp.GetFieldPosition(property, out fieldPosition))
          return this.Copy(input.Children[fieldPosition]);
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      }
      if (op.OpType == OpType.Null)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      return this.m_command.CreateNode((Op) this.m_command.CreatePropertyOp((EdmMember) property), this.Copy(input));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildAccessorWithNulls(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      EdmProperty property)
    {
      return this.BuildAccessor(input, property) ?? this.CreateNullConstantNode(Helper.GetModelTypeUsage((EdmMember) property));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildTypeIdAccessor(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      TypeInfo typeInfo)
    {
      return !typeInfo.HasTypeIdProperty ? this.CreateTypeIdConstant(typeInfo) : this.BuildAccessorWithNulls(input, typeInfo.TypeIdProperty);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SoftCast")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-ScalarOp")]
    private System.Data.Entity.Core.Query.InternalTrees.Node BuildSoftCast(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      TypeUsage targetType)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node.Op.IsScalarOp, "Attempting SoftCast around non-ScalarOp?");
      if (Command.EqualTypes(node.Op.Type, targetType))
        return node;
      while (node.Op.OpType == OpType.SoftCast)
        node = node.Child0;
      return this.m_command.CreateNode((Op) this.m_command.CreateSoftCastOp(targetType), node);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node Copy(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return OpCopier.Copy(this.m_command, n);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateNullConstantNode(
      TypeUsage type)
    {
      return this.m_command.CreateNode((Op) this.m_command.CreateNullOp(type));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateNullSentinelConstant()
    {
      return this.m_command.CreateNode((Op) this.m_command.CreateNullSentinelOp());
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateTypeIdConstant(
      TypeInfo typeInfo)
    {
      object typeId = typeInfo.TypeId;
      return this.m_command.CreateNode((Op) this.m_command.CreateInternalConstantOp(typeInfo.RootType.DiscriminatorMap == null ? this.DefaultTypeIdType : Helper.GetModelTypeUsage(typeInfo.RootType.DiscriminatorMap.DiscriminatorProperty), typeId));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateTypeIdConstantForPrefixMatch(
      TypeInfo typeInfo)
    {
      return this.m_command.CreateNode((Op) this.m_command.CreateInternalConstantOp(this.DefaultTypeIdType, (object) (typeInfo.TypeId.ToString() + "%")));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "isNull")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "opKind")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IsNull")]
    private IEnumerable<PropertyRef> GetPropertyRefsForComparisonAndIsNull(
      TypeInfo typeInfo,
      NominalTypeEliminator.OperationKind opKind)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(opKind == NominalTypeEliminator.OperationKind.IsNull || opKind == NominalTypeEliminator.OperationKind.Equality, "Unexpected opKind: " + (object) opKind + "; Can only handle IsNull and Equality");
      TypeUsage currentType = typeInfo.Type;
      RowType recordType = (RowType) null;
      if (TypeHelpers.TryGetEdmType<RowType>(currentType, out recordType))
      {
        if (opKind == NominalTypeEliminator.OperationKind.IsNull && typeInfo.HasNullSentinelProperty)
        {
          yield return (PropertyRef) NullSentinelPropertyRef.Instance;
        }
        else
        {
          foreach (EdmProperty property in recordType.Properties)
          {
            if (!TypeUtils.IsStructuredType(Helper.GetModelTypeUsage((EdmMember) property)))
            {
              yield return (PropertyRef) new SimplePropertyRef((EdmMember) property);
            }
            else
            {
              TypeInfo nestedTypeInfo = this.m_typeInfo.GetTypeInfo(Helper.GetModelTypeUsage((EdmMember) property));
              foreach (PropertyRef propertyRef in this.GetPropertyRefs(nestedTypeInfo, opKind))
              {
                PropertyRef nestedPropertyRef = propertyRef.CreateNestedPropertyRef((EdmMember) property);
                yield return nestedPropertyRef;
              }
            }
          }
        }
      }
      else
      {
        EntityType entityType = (EntityType) null;
        if (TypeHelpers.TryGetEdmType<EntityType>(currentType, out entityType))
        {
          if (opKind == NominalTypeEliminator.OperationKind.Equality || opKind == NominalTypeEliminator.OperationKind.IsNull && !typeInfo.HasTypeIdProperty)
          {
            foreach (PropertyRef identityPropertyRef in typeInfo.GetIdentityPropertyRefs())
              yield return identityPropertyRef;
          }
          else
            yield return (PropertyRef) TypeIdPropertyRef.Instance;
        }
        else
        {
          ComplexType complexType = (ComplexType) null;
          if (TypeHelpers.TryGetEdmType<ComplexType>(currentType, out complexType))
          {
            System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(opKind == NominalTypeEliminator.OperationKind.IsNull, "complex types not equality-comparable");
            System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(typeInfo.HasNullSentinelProperty, "complex type with no null sentinel property: can't handle isNull");
            yield return (PropertyRef) NullSentinelPropertyRef.Instance;
          }
          else
          {
            RefType refType = (RefType) null;
            if (TypeHelpers.TryGetEdmType<RefType>(currentType, out refType))
            {
              foreach (PropertyRef allPropertyRef in typeInfo.GetAllPropertyRefs())
                yield return allPropertyRef;
            }
            else
              System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Unknown type");
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OperationKind")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetPropertyRefs")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private IEnumerable<PropertyRef> GetPropertyRefs(
      TypeInfo typeInfo,
      NominalTypeEliminator.OperationKind opKind)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(opKind != NominalTypeEliminator.OperationKind.All, "unexpected attempt to GetPropertyRefs(...,OperationKind.All)");
      if (opKind == NominalTypeEliminator.OperationKind.GetKeys)
        return typeInfo.GetKeyPropertyRefs();
      if (opKind == NominalTypeEliminator.OperationKind.GetIdentity)
        return typeInfo.GetIdentityPropertyRefs();
      return this.GetPropertyRefsForComparisonAndIsNull(typeInfo, opKind);
    }

    private IEnumerable<EdmProperty> GetProperties(
      TypeInfo typeInfo,
      NominalTypeEliminator.OperationKind opKind)
    {
      if (opKind == NominalTypeEliminator.OperationKind.All)
      {
        foreach (EdmProperty allProperty in typeInfo.GetAllProperties())
          yield return allProperty;
      }
      else
      {
        foreach (PropertyRef propertyRef in this.GetPropertyRefs(typeInfo, opKind))
          yield return typeInfo.GetNewProperty(propertyRef);
      }
    }

    private void GetPropertyValues(
      TypeInfo typeInfo,
      NominalTypeEliminator.OperationKind opKind,
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      bool ignoreMissingProperties,
      out List<EdmProperty> properties,
      out List<System.Data.Entity.Core.Query.InternalTrees.Node> values)
    {
      values = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      properties = new List<EdmProperty>();
      foreach (EdmProperty property in this.GetProperties(typeInfo, opKind))
      {
        KeyValuePair<EdmProperty, System.Data.Entity.Core.Query.InternalTrees.Node> propertyValue = this.GetPropertyValue(input, property, ignoreMissingProperties);
        if (propertyValue.Value != null)
        {
          properties.Add(propertyValue.Key);
          values.Add(propertyValue.Value);
        }
      }
    }

    private KeyValuePair<EdmProperty, System.Data.Entity.Core.Query.InternalTrees.Node> GetPropertyValue(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      EdmProperty property,
      bool ignoreMissingProperties)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node = ignoreMissingProperties ? this.BuildAccessor(input, property) : this.BuildAccessorWithNulls(input, property);
      return new KeyValuePair<EdmProperty, System.Data.Entity.Core.Query.InternalTrees.Node>(property, node);
    }

    private List<System.Data.Entity.Core.Query.InternalTrees.SortKey> HandleSortKeys(
      List<System.Data.Entity.Core.Query.InternalTrees.SortKey> keys)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.SortKey> sortKeyList = new List<System.Data.Entity.Core.Query.InternalTrees.SortKey>();
      bool flag = false;
      foreach (System.Data.Entity.Core.Query.InternalTrees.SortKey key in keys)
      {
        VarInfo varInfo;
        if (!this.m_varInfoMap.TryGetVarInfo(key.Var, out varInfo))
        {
          sortKeyList.Add(key);
        }
        else
        {
          StructuredVarInfo structuredVarInfo = varInfo as StructuredVarInfo;
          if (structuredVarInfo != null && structuredVarInfo.NewVarsIncludeNullSentinelVar)
            this.m_compilerState.HasSortingOnNullSentinels = true;
          foreach (Var newVar in varInfo.NewVars)
          {
            System.Data.Entity.Core.Query.InternalTrees.SortKey sortKey = Command.CreateSortKey(newVar, key.AscendingSort, key.Collation);
            sortKeyList.Add(sortKey);
          }
          flag = true;
        }
      }
      return flag ? sortKeyList : keys;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TVFs")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node CreateTVFProjection(
      System.Data.Entity.Core.Query.InternalTrees.Node unnestNode,
      List<Var> unnestOpTableColumns,
      TypeInfo unnestOpTableTypeInfo,
      out List<Var> newVars)
    {
      RowType edmType = unnestOpTableTypeInfo.Type.EdmType as RowType;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(edmType != null, "Unexpected TVF return type (must be row): " + (object) unnestOpTableTypeInfo.Type);
      List<Var> varList = new List<Var>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      PropertyRef[] array = unnestOpTableTypeInfo.PropertyRefList.ToArray<PropertyRef>();
      Dictionary<EdmProperty, PropertyRef> dictionary = new Dictionary<EdmProperty, PropertyRef>();
      foreach (PropertyRef propertyRef in array)
        dictionary.Add(unnestOpTableTypeInfo.GetNewProperty(propertyRef), propertyRef);
      foreach (EdmProperty property in unnestOpTableTypeInfo.FlattenedType.Properties)
      {
        PropertyRef propertyRef = dictionary[property];
        Var computedVar = (Var) null;
        SimplePropertyRef simplePropertyRef = propertyRef as SimplePropertyRef;
        if (simplePropertyRef != null)
        {
          int index = edmType.Members.IndexOf(simplePropertyRef.Property);
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(index >= 0, "Can't find a column in the TVF result type");
          args.Add(this.m_command.CreateVarDefNode(this.m_command.CreateNode((Op) this.m_command.CreateVarRefOp(unnestOpTableColumns[index])), out computedVar));
        }
        else if (propertyRef is NullSentinelPropertyRef)
          args.Add(this.m_command.CreateVarDefNode(this.CreateNullSentinelConstant(), out computedVar));
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(computedVar != null, "TVFs returning a collection of rows with non-primitive properties are not supported");
        varList.Add(computedVar);
      }
      newVars = varList;
      return this.m_command.CreateNode((Op) this.m_command.CreateProjectOp(this.m_command.CreateVarVec((IEnumerable<Var>) varList)), unnestNode, this.m_command.CreateNode((Op) this.m_command.CreateVarDefListOp(), args));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      VarDefListOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(child.Op is VarDefOp, "VarDefOp expected");
        VarDefOp op1 = (VarDefOp) child.Op;
        if (TypeUtils.IsStructuredType(op1.Var.Type) || TypeUtils.IsCollectionType(op1.Var.Type))
        {
          List<System.Data.Entity.Core.Query.InternalTrees.Node> newNodes;
          TypeUsage newType;
          this.FlattenComputedVar((ComputedVar) op1.Var, child, out newNodes, out newType);
          foreach (System.Data.Entity.Core.Query.InternalTrees.Node node in newNodes)
            args.Add(node);
        }
        else if (TypeSemantics.IsEnumerationType(op1.Var.Type) || TypeSemantics.IsStrongSpatialType(op1.Var.Type))
          args.Add(this.FlattenEnumOrStrongSpatialVar(op1, child.Child0));
        else
          args.Add(child);
      }
      return this.m_command.CreateNode(n.Op, args);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void FlattenComputedVar(
      ComputedVar v,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      out List<System.Data.Entity.Core.Query.InternalTrees.Node> newNodes,
      out TypeUsage newType)
    {
      newNodes = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = node.Child0;
      newType = (TypeUsage) null;
      if (TypeUtils.IsCollectionType(v.Type))
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(child0.Op.OpType != OpType.Function, "Flattening of TVF output is not allowed.");
        newType = this.GetNewType(v.Type);
        Var computedVar;
        System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = this.m_command.CreateVarDefNode(child0, out computedVar);
        newNodes.Add(varDefNode);
        this.m_varInfoMap.CreateCollectionVarInfo((Var) v, computedVar);
      }
      else
      {
        TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(v.Type);
        PropertyRefList varProperty = this.m_varPropertyMap[(Var) v];
        List<Var> newVars = new List<Var>();
        List<EdmProperty> newProperties = new List<EdmProperty>();
        newNodes = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        bool newVarsIncludeNullSentinelVar = false;
        foreach (PropertyRef propertyRef in typeInfo.PropertyRefList)
        {
          if (varProperty.Contains(propertyRef))
          {
            EdmProperty newProperty = typeInfo.GetNewProperty(propertyRef);
            System.Data.Entity.Core.Query.InternalTrees.Node definingExpr;
            if (varProperty.AllProperties)
            {
              definingExpr = this.BuildAccessorWithNulls(child0, newProperty);
            }
            else
            {
              definingExpr = this.BuildAccessor(child0, newProperty);
              if (definingExpr == null)
                continue;
            }
            newProperties.Add(newProperty);
            Var computedVar;
            System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = this.m_command.CreateVarDefNode(definingExpr, out computedVar);
            newNodes.Add(varDefNode);
            newVars.Add(computedVar);
            if (!newVarsIncludeNullSentinelVar && NominalTypeEliminator.IsNullSentinelPropertyRef(propertyRef))
              newVarsIncludeNullSentinelVar = true;
          }
        }
        this.m_varInfoMap.CreateStructuredVarInfo((Var) v, typeInfo.FlattenedType, newVars, newProperties, newVarsIncludeNullSentinelVar);
      }
    }

    private static bool IsNullSentinelPropertyRef(PropertyRef propertyRef)
    {
      if (propertyRef is NullSentinelPropertyRef)
        return true;
      NestedPropertyRef nestedPropertyRef = propertyRef as NestedPropertyRef;
      if (nestedPropertyRef == null)
        return false;
      return nestedPropertyRef.OuterProperty is NullSentinelPropertyRef;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node FlattenEnumOrStrongSpatialVar(
      VarDefOp varDefOp,
      System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      Var computedVar;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = this.m_command.CreateVarDefNode(node, out computedVar);
      this.m_varInfoMap.CreatePrimitiveTypeVarInfo(varDefOp.Var, computedVar);
      return varDefNode;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      PhysicalProjectOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      PhysicalProjectOp physicalProjectOp = this.m_command.CreatePhysicalProjectOp(this.FlattenVarList(op.Outputs), this.ExpandColumnMap(op.ColumnMap));
      n.Op = (Op) physicalProjectOp;
      return n;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarRefColumnMap")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SimpleCollectionColumnMap")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NominalTypeEliminator")]
    private SimpleCollectionColumnMap ExpandColumnMap(
      SimpleCollectionColumnMap columnMap)
    {
      VarRefColumnMap element = columnMap.Element as VarRefColumnMap;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(element != null, "Encountered a SimpleCollectionColumnMap element that is not VarRefColumnMap when expanding a column map in NominalTypeEliminator.");
      VarInfo varInfo;
      if (!this.m_varInfoMap.TryGetVarInfo(element.Var, out varInfo))
        return columnMap;
      if (TypeUtils.IsStructuredType(element.Var.Type))
      {
        TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(element.Var.Type);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((typeInfo.RootType.FlattenedType.Properties.Count == varInfo.NewVars.Count ? 1 : 0) != 0, "Var count mismatch; Expected " + (object) typeInfo.RootType.FlattenedType.Properties.Count + "; got " + (object) varInfo.NewVars.Count + " instead.");
      }
      ColumnMap elementMap = new ColumnMapProcessor(element, varInfo, this.m_typeInfo).ExpandColumnMap();
      return new SimpleCollectionColumnMap(TypeUtils.CreateCollectionType(elementMap.Type), elementMap.Name, elementMap, columnMap.Keys, columnMap.ForeignKeys);
    }

    private IEnumerable<Var> FlattenVars(IEnumerable<Var> vars)
    {
      foreach (Var var in vars)
      {
        VarInfo varInfo;
        if (!this.m_varInfoMap.TryGetVarInfo(var, out varInfo))
        {
          yield return var;
        }
        else
        {
          foreach (Var newVar in varInfo.NewVars)
            yield return newVar;
        }
      }
    }

    private VarVec FlattenVarSet(VarVec varSet)
    {
      return this.m_command.CreateVarVec(this.FlattenVars((IEnumerable<Var>) varSet));
    }

    private VarList FlattenVarList(VarList varList)
    {
      return Command.CreateVarList(this.FlattenVars((IEnumerable<Var>) varList));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DistinctOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      VarVec keyVars = this.FlattenVarSet(op.Keys);
      n.Op = (Op) this.m_command.CreateDistinctOp(keyVars);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(GroupByOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      VarVec gbyKeys = this.FlattenVarSet(op.Keys);
      VarVec outputs = this.FlattenVarSet(op.Outputs);
      if (gbyKeys != op.Keys || outputs != op.Outputs)
        n.Op = (Op) this.m_command.CreateGroupByOp(gbyKeys, outputs);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      GroupByIntoOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      VarVec gbyKeys = this.FlattenVarSet(op.Keys);
      VarVec inputs = this.FlattenVarSet(op.Inputs);
      VarVec outputs = this.FlattenVarSet(op.Outputs);
      if (gbyKeys != op.Keys || inputs != op.Inputs || outputs != op.Outputs)
        n.Op = (Op) this.m_command.CreateGroupByIntoOp(gbyKeys, inputs, outputs);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(ProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      VarVec vars = this.FlattenVarSet(op.Outputs);
      if (op.Outputs != vars)
      {
        if (vars.IsEmpty)
          return n.Child0;
        n.Op = (Op) this.m_command.CreateProjectOp(vars);
      }
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ScanTableOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      Var column = op.Table.Columns[0];
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(column.Type);
      RowType flattenedType = typeInfo.FlattenedType;
      List<EdmProperty> newProperties = new List<EdmProperty>();
      List<EdmMember> edmMemberList = new List<EdmMember>();
      HashSet<string> stringSet = new HashSet<string>();
      foreach (EdmProperty structuralMember in (IEnumerable) TypeHelpers.GetAllStructuralMembers(column.Type.EdmType))
        stringSet.Add(structuralMember.Name);
      foreach (EdmProperty property in flattenedType.Properties)
      {
        if (stringSet.Contains(property.Name))
          newProperties.Add(property);
      }
      foreach (PropertyRef keyPropertyRef in typeInfo.GetKeyPropertyRefs())
      {
        EdmProperty newProperty = typeInfo.GetNewProperty(keyPropertyRef);
        edmMemberList.Add((EdmMember) newProperty);
      }
      Table tableInstance = this.m_command.CreateTableInstance(this.m_command.CreateFlatTableDefinition((IEnumerable<EdmProperty>) newProperties, (IEnumerable<EdmMember>) edmMemberList, op.Table.TableMetadata.Extent));
      this.m_varInfoMap.CreateStructuredVarInfo(column, flattenedType, (List<Var>) tableInstance.Columns, newProperties);
      n.Op = (Op) this.m_command.CreateScanTableOp(tableInstance);
      return n;
    }

    internal static Var GetSingletonVar(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      switch (n.Op.OpType)
      {
        case OpType.ScanTable:
          ScanTableOp op1 = (ScanTableOp) n.Op;
          if (op1.Table.Columns.Count != 1)
            return (Var) null;
          return op1.Table.Columns[0];
        case OpType.Filter:
        case OpType.Sort:
        case OpType.ConstrainedSort:
        case OpType.SingleRow:
          return NominalTypeEliminator.GetSingletonVar(n.Child0);
        case OpType.Project:
          ProjectOp op2 = (ProjectOp) n.Op;
          if (op2.Outputs.Count != 1)
            return (Var) null;
          return op2.Outputs.First;
        case OpType.Unnest:
          UnnestOp op3 = (UnnestOp) n.Op;
          if (op3.Table.Columns.Count != 1)
            return (Var) null;
          return op3.Table.Columns[0];
        case OpType.UnionAll:
        case OpType.Intersect:
        case OpType.Except:
          SetOp op4 = (SetOp) n.Op;
          if (op4.Outputs.Count != 1)
            return (Var) null;
          return op4.Outputs.First;
        case OpType.Distinct:
          DistinctOp op5 = (DistinctOp) n.Op;
          if (op5.Keys.Count != 1)
            return (Var) null;
          return op5.Keys.First;
        default:
          return (Var) null;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "inputVar")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "scanViewOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScanViewOp")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ScanViewOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      Var singletonVar = NominalTypeEliminator.GetSingletonVar(n.Child0);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(singletonVar != null, "cannot identify Var for the input node to the ScanViewOp");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.Table.Columns.Count == 1, "table for scanViewOp has more than on column?");
      Var column = op.Table.Columns[0];
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitNode(n.Child0);
      VarInfo varInfo;
      if (!this.m_varInfoMap.TryGetVarInfo(singletonVar, out varInfo))
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "didn't find inputVar for scanViewOp?");
      StructuredVarInfo structuredVarInfo = (StructuredVarInfo) varInfo;
      this.m_typeInfo.GetTypeInfo(column.Type);
      this.m_varInfoMap.CreateStructuredVarInfo(column, structuredVarInfo.NewType, structuredVarInfo.NewVars, structuredVarInfo.Fields);
      return node;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(SortOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      List<System.Data.Entity.Core.Query.InternalTrees.SortKey> sortKeys = this.HandleSortKeys(op.Keys);
      if (sortKeys != op.Keys)
        n.Op = (Op) this.m_command.CreateSortOp(sortKeys);
      return n;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "newUnnestVar")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TVFs")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "unnest")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(UnnestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      Var v = (Var) null;
      EdmFunction tvf = (EdmFunction) null;
      if (n.HasChild0)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
        VarDefOp op1 = child0.Op as VarDefOp;
        if (op1 != null && TypeUtils.IsCollectionType(op1.Var.Type))
        {
          ComputedVar var = (ComputedVar) op1.Var;
          if (child0.HasChild0 && child0.Child0.Op.OpType == OpType.Function)
          {
            v = (Var) var;
            tvf = ((FunctionOp) child0.Child0.Op).Function;
          }
          else
          {
            List<System.Data.Entity.Core.Query.InternalTrees.Node> newNodes = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
            TypeUsage newType;
            this.FlattenComputedVar(var, child0, out newNodes, out newType);
            System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(newNodes.Count == 1, "Flattening unnest var produced more than one Var.");
            n.Child0 = newNodes[0];
          }
        }
      }
      if (tvf != null)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(v != null, "newUnnestVar must be initialized in the TVF case.");
      }
      else
      {
        VarInfo varInfo;
        if (!this.m_varInfoMap.TryGetVarInfo(op.Var, out varInfo) || varInfo.Kind != VarInfoKind.CollectionVarInfo)
          throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1006));
        v = ((CollectionVarInfo) varInfo).NewVar;
      }
      Var column = op.Table.Columns[0];
      if (!TypeUtils.IsStructuredType(column.Type))
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(tvf == null, "TVFs returning a collection of values of a non-structured type are not supported");
        if (TypeSemantics.IsEnumerationType(column.Type) || TypeSemantics.IsStrongSpatialType(column.Type))
        {
          UnnestOp unnestOp = this.m_command.CreateUnnestOp(v);
          this.m_varInfoMap.CreatePrimitiveTypeVarInfo(column, unnestOp.Table.Columns[0]);
          n.Op = (Op) unnestOp;
        }
        else
          n.Op = (Op) this.m_command.CreateUnnestOp(v, op.Table);
      }
      else
      {
        TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(column.Type);
        TableMD flatTableDefinition;
        if (tvf != null)
        {
          RowType tvfReturnType = TypeHelpers.GetTvfReturnType(tvf);
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(Command.EqualTypes((EdmType) tvfReturnType, column.Type.EdmType), "Unexpected TVF return type (row type is expected).");
          flatTableDefinition = this.m_command.CreateFlatTableDefinition((IEnumerable<EdmProperty>) tvfReturnType.Properties, (IEnumerable<EdmMember>) this.GetTvfResultKeys(tvf), (EntitySetBase) null);
        }
        else
          flatTableDefinition = this.m_command.CreateFlatTableDefinition(typeInfo.FlattenedType);
        Table tableInstance = this.m_command.CreateTableInstance(flatTableDefinition);
        n.Op = (Op) this.m_command.CreateUnnestOp(v, tableInstance);
        List<Var> newVars;
        if (tvf != null)
          n = this.CreateTVFProjection(n, (List<Var>) tableInstance.Columns, typeInfo, out newVars);
        else
          newVars = (List<Var>) tableInstance.Columns;
        this.m_varInfoMap.CreateStructuredVarInfo(column, typeInfo.FlattenedType, newVars, typeInfo.FlattenedType.Properties.ToList<EdmProperty>());
      }
      return n;
    }

    private IEnumerable<EdmProperty> GetTvfResultKeys(EdmFunction tvf)
    {
      EdmProperty[] edmPropertyArray;
      if (this.m_tvfResultKeys.TryGetValue(tvf, out edmPropertyArray))
        return (IEnumerable<EdmProperty>) edmPropertyArray;
      return Enumerable.Empty<EdmProperty>();
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitSetOp(
      SetOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      for (int index = 0; index < op.VarMap.Length; ++index)
      {
        List<ComputedVar> newComputedVars;
        op.VarMap[index] = this.FlattenVarMap(op.VarMap[index], out newComputedVars);
        if (newComputedVars != null)
          n.Children[index] = this.FixupSetOpChild(n.Children[index], op.VarMap[index], newComputedVars);
      }
      op.Outputs.Clear();
      foreach (Var key in op.VarMap[0].Keys)
        op.Outputs.Set(key);
      return n;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "newComputedVars")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "varMap")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "setOpChild")]
    private System.Data.Entity.Core.Query.InternalTrees.Node FixupSetOpChild(
      System.Data.Entity.Core.Query.InternalTrees.Node setOpChild,
      VarMap varMap,
      List<ComputedVar> newComputedVars)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(null != setOpChild, "null setOpChild?");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(null != varMap, "null varMap?");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(null != newComputedVars, "null newComputedVars?");
      VarVec varVec = this.m_command.CreateVarVec();
      foreach (KeyValuePair<Var, Var> var in (Dictionary<Var, Var>) varMap)
        varVec.Set(var.Value);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (Var newComputedVar in newComputedVars)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node = this.m_command.CreateNode((Op) this.m_command.CreateVarDefOp(newComputedVar), this.CreateNullConstantNode(newComputedVar.Type));
        args.Add(node);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.m_command.CreateNode((Op) this.m_command.CreateVarDefListOp(), args);
      return this.m_command.CreateNode((Op) this.m_command.CreateProjectOp(varVec), setOpChild, node1);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarInfo")]
    private VarMap FlattenVarMap(VarMap varMap, out List<ComputedVar> newComputedVars)
    {
      newComputedVars = (List<ComputedVar>) null;
      VarMap varMap1 = new VarMap();
      foreach (KeyValuePair<Var, Var> var in (Dictionary<Var, Var>) varMap)
      {
        VarInfo varInfo1;
        if (!this.m_varInfoMap.TryGetVarInfo(var.Value, out varInfo1))
        {
          varMap1.Add(var.Key, var.Value);
        }
        else
        {
          VarInfo varInfo2;
          if (!this.m_varInfoMap.TryGetVarInfo(var.Key, out varInfo2))
            varInfo2 = this.FlattenSetOpVar((SetOpVar) var.Key);
          if (varInfo2.Kind == VarInfoKind.CollectionVarInfo)
            varMap1.Add(((CollectionVarInfo) varInfo2).NewVar, ((CollectionVarInfo) varInfo1).NewVar);
          else if (varInfo2.Kind == VarInfoKind.PrimitiveTypeVarInfo)
          {
            varMap1.Add(((PrimitiveTypeVarInfo) varInfo2).NewVar, ((PrimitiveTypeVarInfo) varInfo1).NewVar);
          }
          else
          {
            StructuredVarInfo structuredVarInfo1 = (StructuredVarInfo) varInfo2;
            StructuredVarInfo structuredVarInfo2 = (StructuredVarInfo) varInfo1;
            foreach (EdmProperty field in structuredVarInfo1.Fields)
            {
              Var v1;
              System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(structuredVarInfo1.TryGetVar(field, out v1), "Could not find VarInfo for prop " + field.Name);
              Var v2;
              if (!structuredVarInfo2.TryGetVar(field, out v2))
              {
                v2 = (Var) this.m_command.CreateComputedVar(v1.Type);
                if (newComputedVars == null)
                  newComputedVars = new List<ComputedVar>();
                newComputedVars.Add((ComputedVar) v2);
              }
              varMap1.Add(v1, v2);
            }
          }
        }
      }
      return varMap1;
    }

    private VarInfo FlattenSetOpVar(SetOpVar v)
    {
      if (TypeUtils.IsCollectionType(v.Type))
      {
        Var setOpVar = (Var) this.m_command.CreateSetOpVar(this.GetNewType(v.Type));
        return this.m_varInfoMap.CreateCollectionVarInfo((Var) v, setOpVar);
      }
      if (TypeSemantics.IsEnumerationType(v.Type) || TypeSemantics.IsStrongSpatialType(v.Type))
      {
        Var setOpVar = (Var) this.m_command.CreateSetOpVar(this.GetNewType(v.Type));
        return this.m_varInfoMap.CreatePrimitiveTypeVarInfo((Var) v, setOpVar);
      }
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(v.Type);
      PropertyRefList varProperty = this.m_varPropertyMap[(Var) v];
      List<Var> newVars = new List<Var>();
      List<EdmProperty> newProperties = new List<EdmProperty>();
      bool newVarsIncludeNullSentinelVar = false;
      foreach (PropertyRef propertyRef in typeInfo.PropertyRefList)
      {
        if (varProperty.Contains(propertyRef))
        {
          EdmProperty newProperty = typeInfo.GetNewProperty(propertyRef);
          newProperties.Add(newProperty);
          SetOpVar setOpVar = this.m_command.CreateSetOpVar(Helper.GetModelTypeUsage((EdmMember) newProperty));
          newVars.Add((Var) setOpVar);
          if (!newVarsIncludeNullSentinelVar && NominalTypeEliminator.IsNullSentinelPropertyRef(propertyRef))
            newVarsIncludeNullSentinelVar = true;
        }
      }
      return this.m_varInfoMap.CreateStructuredVarInfo((Var) v, typeInfo.FlattenedType, newVars, newProperties, newVarsIncludeNullSentinelVar);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NullSentinelProperty")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      SoftCastOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      TypeUsage type1 = n.Child0.Op.Type;
      TypeUsage type2 = op.Type;
      this.VisitChildren(n);
      TypeUsage newType = this.GetNewType(type2);
      if (TypeSemantics.IsRowType(type2))
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.Op.OpType == OpType.NewRecord, "Expected a record constructor here. Found " + (object) n.Child0.Op.OpType + " instead");
        TypeInfo typeInfo1 = this.m_typeInfo.GetTypeInfo(type1);
        TypeInfo typeInfo2 = this.m_typeInfo.GetTypeInfo(op.Type);
        NewRecordOp newRecordOp = this.m_command.CreateNewRecordOp(newType);
        List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        IEnumerator<EdmProperty> enumerator1 = (IEnumerator<EdmProperty>) newRecordOp.Properties.GetEnumerator();
        int count1 = newRecordOp.Properties.Count;
        enumerator1.MoveNext();
        IEnumerator<System.Data.Entity.Core.Query.InternalTrees.Node> enumerator2 = (IEnumerator<System.Data.Entity.Core.Query.InternalTrees.Node>) n.Child0.Children.GetEnumerator();
        int count2 = n.Child0.Children.Count;
        enumerator2.MoveNext();
        for (; count2 < count1; --count1)
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(typeInfo2.HasNullSentinelProperty && !typeInfo1.HasNullSentinelProperty, "NullSentinelProperty mismatch on input?");
          args.Add(this.CreateNullSentinelConstant());
          enumerator1.MoveNext();
        }
        for (; count2 > count1; --count2)
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!typeInfo2.HasNullSentinelProperty && typeInfo1.HasNullSentinelProperty, "NullSentinelProperty mismatch on output?");
          enumerator2.MoveNext();
        }
        do
        {
          EdmProperty current = enumerator1.Current;
          System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildSoftCast(enumerator2.Current, Helper.GetModelTypeUsage((EdmMember) current));
          args.Add(node);
          enumerator1.MoveNext();
        }
        while (enumerator2.MoveNext());
        return this.m_command.CreateNode((Op) newRecordOp, args);
      }
      if (TypeSemantics.IsCollectionType(type2))
        return this.BuildSoftCast(n.Child0, newType);
      if (TypeSemantics.IsPrimitiveType(type2))
        return n;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsNominalType(type2) || TypeSemantics.IsReferenceType(type2), "Gasp! Not a nominal type or even a reference type");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(Command.EqualTypes(newType, n.Child0.Op.Type), "Types are not equal");
      return n.Child0;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(CastOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      if (TypeSemantics.IsEnumerationType(op.Type))
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsPrimitiveType(n.Child0.Op.Type), "Primitive type expected.");
        return this.RewriteAsCastToUnderlyingType(Helper.GetUnderlyingEdmTypeForEnumType(op.Type.EdmType), op, n);
      }
      if (!TypeSemantics.IsSpatialType(op.Type))
        return n;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsPrimitiveType(n.Child0.Op.Type, PrimitiveTypeKind.Geography) || TypeSemantics.IsPrimitiveType(n.Child0.Op.Type, PrimitiveTypeKind.Geometry), "Union spatial type expected.");
      return this.RewriteAsCastToUnderlyingType(Helper.GetSpatialNormalizedPrimitiveType(op.Type.EdmType), op, n);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node RewriteAsCastToUnderlyingType(
      PrimitiveType underlyingType,
      CastOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (underlyingType.PrimitiveTypeKind == ((PrimitiveType) n.Child0.Op.Type.EdmType).PrimitiveTypeKind)
        return n.Child0;
      return this.m_command.CreateNode((Op) this.m_command.CreateCastOp(TypeUsage.Create((EdmType) underlyingType, (IEnumerable<Facet>) op.Type.Facets)), n.Child0);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ConstantOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count == 0, "Constant operations don't have children.");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.Value != null, "Value must not be null");
      if (TypeSemantics.IsEnumerationType(op.Type))
      {
        object obj = op.Value.GetType().IsEnum() ? Convert.ChangeType(op.Value, op.Value.GetType().GetEnumUnderlyingType(), (IFormatProvider) CultureInfo.InvariantCulture) : op.Value;
        return this.m_command.CreateNode((Op) this.m_command.CreateConstantOp(TypeHelpers.CreateEnumUnderlyingTypeUsage(op.Type), obj));
      }
      if (TypeSemantics.IsStrongSpatialType(op.Type))
        op.Type = TypeHelpers.CreateSpatialUnionTypeUsage(op.Type);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(CaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      bool thenClauseIsNull;
      bool flag = PlanCompilerUtil.IsRowTypeCaseOpWithNullability(op, n, out thenClauseIsNull);
      this.VisitChildren(n);
      System.Data.Entity.Core.Query.InternalTrees.Node rewrittenNode;
      if (flag && this.TryRewriteCaseOp(n, thenClauseIsNull, out rewrittenNode))
        return rewrittenNode;
      if (TypeUtils.IsCollectionType(op.Type) || TypeSemantics.IsEnumerationType(op.Type) || TypeSemantics.IsStrongSpatialType(op.Type))
      {
        TypeUsage newType = this.GetNewType(op.Type);
        n.Op = (Op) this.m_command.CreateCaseOp(newType);
        return n;
      }
      if (!TypeUtils.IsStructuredType(op.Type))
        return n;
      PropertyRefList nodeProperty = this.m_nodePropertyMap[n];
      return this.FlattenCaseOp(n, this.m_typeInfo.GetTypeInfo(op.Type), nodeProperty);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private bool TryRewriteCaseOp(System.Data.Entity.Core.Query.InternalTrees.Node n, bool thenClauseIsNull, out System.Data.Entity.Core.Query.InternalTrees.Node rewrittenNode)
    {
      rewrittenNode = n;
      if (!this.m_typeInfo.GetTypeInfo(n.Op.Type).HasNullSentinelProperty)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = thenClauseIsNull ? n.Child2 : n.Child1;
      if (node1.Op.OpType != OpType.NewRecord)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = node1.Child0;
      TypeUsage integerType = this.m_command.IntegerType;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(child0.Op.Type.EdmEquals((MetadataItem) integerType), "Column that is expected to be a null sentinel is not of Integer type.");
      CaseOp caseOp = this.m_command.CreateCaseOp(integerType);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(3);
      args.Add(n.Child0);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.m_command.CreateNode((Op) this.m_command.CreateNullOp(integerType));
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = thenClauseIsNull ? node2 : child0;
      System.Data.Entity.Core.Query.InternalTrees.Node node4 = thenClauseIsNull ? child0 : node2;
      args.Add(node3);
      args.Add(node4);
      node1.Child0 = this.m_command.CreateNode((Op) caseOp, args);
      rewrittenNode = node1;
      return true;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node FlattenCaseOp(
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      TypeInfo typeInfo,
      PropertyRefList desiredProperties)
    {
      List<EdmProperty> fields = new List<EdmProperty>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (PropertyRef propertyRef in typeInfo.PropertyRefList)
      {
        if (desiredProperties.Contains(propertyRef))
        {
          EdmProperty newProperty = typeInfo.GetNewProperty(propertyRef);
          List<System.Data.Entity.Core.Query.InternalTrees.Node> args2 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
          int index1;
          for (int index2 = 0; index2 < n.Children.Count - 1; index2 = index1 + 1)
          {
            System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.Copy(n.Children[index2]);
            args2.Add(node1);
            index1 = index2 + 1;
            System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.BuildAccessorWithNulls(n.Children[index1], newProperty);
            args2.Add(node2);
          }
          System.Data.Entity.Core.Query.InternalTrees.Node node3 = this.BuildAccessorWithNulls(n.Children[n.Children.Count - 1], newProperty);
          args2.Add(node3);
          System.Data.Entity.Core.Query.InternalTrees.Node node4 = this.m_command.CreateNode((Op) this.m_command.CreateCaseOp(Helper.GetModelTypeUsage((EdmMember) newProperty)), args2);
          fields.Add(newProperty);
          args1.Add(node4);
        }
      }
      return this.m_command.CreateNode((Op) this.m_command.CreateNewRecordOp(typeInfo.FlattenedTypeUsage, fields), args1);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(CollectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      n.Op = (Op) this.m_command.CreateCollectOp(this.GetNewType(op.Type));
      return n;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ComparisonOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      TypeUsage type1 = n.Child0.Op.Type;
      TypeUsage type2 = n.Child1.Op.Type;
      if (!TypeUtils.IsStructuredType(type1))
        return this.VisitScalarOpDefault((ScalarOp) op, n);
      this.VisitChildren(n);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!TypeSemantics.IsComplexType(type1) && !TypeSemantics.IsComplexType(type2), "complex type?");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.EQ || op.OpType == OpType.NE, "non-equality comparison of structured types?");
      TypeInfo typeInfo1 = this.m_typeInfo.GetTypeInfo(type1);
      TypeInfo typeInfo2 = this.m_typeInfo.GetTypeInfo(type2);
      List<EdmProperty> properties1;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> values1;
      this.GetPropertyValues(typeInfo1, NominalTypeEliminator.OperationKind.Equality, n.Child0, false, out properties1, out values1);
      List<EdmProperty> properties2;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> values2;
      this.GetPropertyValues(typeInfo2, NominalTypeEliminator.OperationKind.Equality, n.Child1, false, out properties2, out values2);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(properties1.Count == properties2.Count && values1.Count == values2.Count, "different shaped structured types?");
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      for (int index = 0; index < values1.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.m_command.CreateNode((Op) this.m_command.CreateComparisonOp(op.OpType, op.UseDatabaseNullSemantics), values1[index], values2[index]);
        node1 = node1 != null ? this.m_command.CreateNode((Op) this.m_command.CreateConditionalOp(OpType.And), node1, node2) : node2;
      }
      return node1;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetPropertyValues")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IsNull")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ConditionalOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (op.OpType != OpType.IsNull)
        return this.VisitScalarOpDefault((ScalarOp) op, n);
      TypeUsage type = n.Child0.Op.Type;
      if (!TypeUtils.IsStructuredType(type))
        return this.VisitScalarOpDefault((ScalarOp) op, n);
      this.VisitChildren(n);
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(type);
      List<EdmProperty> properties = (List<EdmProperty>) null;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> values = (List<System.Data.Entity.Core.Query.InternalTrees.Node>) null;
      this.GetPropertyValues(typeInfo, NominalTypeEliminator.OperationKind.IsNull, n.Child0, false, out properties, out values);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(properties.Count == values.Count && properties.Count > 0, "No properties returned from GetPropertyValues(IsNull)?");
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node node2 in values)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node3 = this.m_command.CreateNode((Op) this.m_command.CreateConditionalOp(OpType.IsNull), node2);
        node1 = node1 != null ? this.m_command.CreateNode((Op) this.m_command.CreateConditionalOp(OpType.And), node1, node3) : node3;
      }
      return node1;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ConstrainedSortOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      List<System.Data.Entity.Core.Query.InternalTrees.SortKey> sortKeys = this.HandleSortKeys(op.Keys);
      if (sortKeys != op.Keys)
        n.Op = (Op) this.m_command.CreateConstrainedSortOp(sortKeys, op.WithTies);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      GetEntityRefOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.FlattenGetKeyOp((ScalarOp) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      GetRefKeyOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.FlattenGetKeyOp((ScalarOp) op, n);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "fieldTypes")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OpType")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetEntityRef")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetRefKey")]
    private System.Data.Entity.Core.Query.InternalTrees.Node FlattenGetKeyOp(
      ScalarOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.GetEntityRef || op.OpType == OpType.GetRefKey, "Expecting GetEntityRef or GetRefKey ops");
      TypeInfo typeInfo1 = this.m_typeInfo.GetTypeInfo(n.Child0.Op.Type);
      TypeInfo typeInfo2 = this.m_typeInfo.GetTypeInfo(op.Type);
      this.VisitChildren(n);
      List<EdmProperty> properties;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> values;
      if (op.OpType == OpType.GetRefKey)
      {
        this.GetPropertyValues(typeInfo1, NominalTypeEliminator.OperationKind.GetKeys, n.Child0, false, out properties, out values);
      }
      else
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.GetEntityRef, "Expected OpType.GetEntityRef: Found " + (object) op.OpType);
        this.GetPropertyValues(typeInfo1, NominalTypeEliminator.OperationKind.GetIdentity, n.Child0, false, out properties, out values);
      }
      if (typeInfo2.HasNullSentinelProperty && !typeInfo1.HasNullSentinelProperty)
        values.Insert(0, this.CreateNullSentinelConstant());
      List<EdmProperty> fields = new List<EdmProperty>((IEnumerable<EdmProperty>) typeInfo2.FlattenedType.Properties);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(values.Count == fields.Count, "fieldTypes.Count mismatch?");
      return this.m_command.CreateNode((Op) this.m_command.CreateNewRecordOp(typeInfo2.FlattenedTypeUsage, fields), values);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "optype")]
    private System.Data.Entity.Core.Query.InternalTrees.Node VisitPropertyOp(
      Op op,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      PropertyRef propertyRef,
      bool throwIfMissing)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.Property || op.OpType == OpType.RelProperty, "Unexpected optype: " + (object) op.OpType);
      TypeUsage type1 = n.Child0.Op.Type;
      TypeUsage type2 = op.Type;
      this.VisitChildren(n);
      TypeInfo typeInfo1 = this.m_typeInfo.GetTypeInfo(type1);
      System.Data.Entity.Core.Query.InternalTrees.Node node1;
      if (TypeUtils.IsStructuredType(type2))
      {
        TypeInfo typeInfo2 = this.m_typeInfo.GetTypeInfo(type2);
        List<EdmProperty> fields = new List<EdmProperty>();
        List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        PropertyRefList nodeProperty = this.m_nodePropertyMap[n];
        foreach (PropertyRef propertyRef1 in typeInfo2.PropertyRefList)
        {
          if (nodeProperty.Contains(propertyRef1))
          {
            PropertyRef nestedPropertyRef = propertyRef1.CreateNestedPropertyRef(propertyRef);
            EdmProperty newProperty1;
            if (typeInfo1.TryGetNewProperty(nestedPropertyRef, throwIfMissing, out newProperty1))
            {
              EdmProperty newProperty2 = typeInfo2.GetNewProperty(propertyRef1);
              System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.BuildAccessor(n.Child0, newProperty1);
              if (node2 != null)
              {
                fields.Add(newProperty2);
                args.Add(node2);
              }
            }
          }
        }
        node1 = this.m_command.CreateNode((Op) this.m_command.CreateNewRecordOp(typeInfo2.FlattenedTypeUsage, fields), args);
      }
      else
      {
        EdmProperty newProperty = typeInfo1.GetNewProperty(propertyRef);
        node1 = this.BuildAccessorWithNulls(n.Child0, newProperty);
      }
      return node1;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      PropertyOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitPropertyOp((Op) op, n, (PropertyRef) new SimplePropertyRef(op.PropertyInfo), true);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      RelPropertyOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitPropertyOp((Op) op, n, (PropertyRef) new RelPropertyRef(op.PropertyInfo), false);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "entitySetId")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(RefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      TypeInfo typeInfo1 = this.m_typeInfo.GetTypeInfo(n.Child0.Op.Type);
      TypeInfo typeInfo2 = this.m_typeInfo.GetTypeInfo(op.Type);
      this.VisitChildren(n);
      List<EdmProperty> properties;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> values;
      this.GetPropertyValues(typeInfo1, NominalTypeEliminator.OperationKind.All, n.Child0, false, out properties, out values);
      List<EdmProperty> fields = new List<EdmProperty>((IEnumerable<EdmProperty>) typeInfo2.FlattenedType.Properties);
      if (typeInfo2.HasEntitySetIdProperty)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(fields[0] == typeInfo2.EntitySetIdProperty, "OutputField0 must be the entitySetId property");
        if (typeInfo1.HasNullSentinelProperty && !typeInfo2.HasNullSentinelProperty)
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((fields.Count == properties.Count ? 1 : 0) != 0, "Mismatched field count: Expected " + (object) properties.Count + "; Got " + (object) fields.Count);
          NominalTypeEliminator.RemoveNullSentinel(typeInfo1, properties, values);
        }
        else
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((fields.Count == properties.Count + 1 ? 1 : 0) != 0, "Mismatched field count: Expected " + (object) (properties.Count + 1) + "; Got " + (object) fields.Count);
        int entitySetId = this.m_typeInfo.GetEntitySetId(op.EntitySet);
        values.Insert(0, this.m_command.CreateNode((Op) this.m_command.CreateInternalConstantOp(Helper.GetModelTypeUsage((EdmMember) typeInfo2.EntitySetIdProperty), (object) entitySetId)));
      }
      else
      {
        if (typeInfo1.HasNullSentinelProperty && !typeInfo2.HasNullSentinelProperty)
          NominalTypeEliminator.RemoveNullSentinel(typeInfo1, properties, values);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((fields.Count == properties.Count ? 1 : 0) != 0, "Mismatched field count: Expected " + (object) properties.Count + "; Got " + (object) fields.Count);
      }
      return this.m_command.CreateNode((Op) this.m_command.CreateNewRecordOp(typeInfo2.FlattenedTypeUsage, fields), values);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static void RemoveNullSentinel(
      TypeInfo inputTypeInfo,
      List<EdmProperty> inputFields,
      List<System.Data.Entity.Core.Query.InternalTrees.Node> inputFieldValues)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputFields[0] == inputTypeInfo.NullSentinelProperty, "InputField0 must be the null sentinel property");
      inputFields.RemoveAt(0);
      inputFieldValues.RemoveAt(0);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "varInfo")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(VarRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      VarInfo varInfo;
      if (!this.m_varInfoMap.TryGetVarInfo(op.Var, out varInfo))
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((!TypeUtils.IsStructuredType(op.Type) ? 1 : 0) != 0, "No varInfo for a structured type var: Id = " + (object) op.Var.Id + " Type = " + (object) op.Type);
        return n;
      }
      if (varInfo.Kind == VarInfoKind.CollectionVarInfo)
      {
        n.Op = (Op) this.m_command.CreateVarRefOp(((CollectionVarInfo) varInfo).NewVar);
        return n;
      }
      if (varInfo.Kind == VarInfoKind.PrimitiveTypeVarInfo)
      {
        n.Op = (Op) this.m_command.CreateVarRefOp(((PrimitiveTypeVarInfo) varInfo).NewVar);
        return n;
      }
      StructuredVarInfo structuredVarInfo = (StructuredVarInfo) varInfo;
      NewRecordOp newRecordOp = this.m_command.CreateNewRecordOp(structuredVarInfo.NewTypeUsage, structuredVarInfo.Fields);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (Var newVar in varInfo.NewVars)
      {
        VarRefOp varRefOp = this.m_command.CreateVarRefOp(newVar);
        args.Add(this.m_command.CreateNode((Op) varRefOp));
      }
      return this.m_command.CreateNode((Op) newRecordOp, args);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      NewEntityOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.FlattenConstructor((ScalarOp) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      NewInstanceOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.FlattenConstructor((ScalarOp) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DiscriminatedNewEntityOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.FlattenConstructor((ScalarOp) op, n);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node NormalizeTypeDiscriminatorValues(
      DiscriminatedNewEntityOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node discriminator)
    {
      CaseOp caseOp = this.m_command.CreateCaseOp(this.m_typeInfo.GetTypeInfo(op.Type).RootType.TypeIdProperty.TypeUsage);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(op.DiscriminatorMap.TypeMap.Count * 2 - 1);
      for (int index = 0; index < op.DiscriminatorMap.TypeMap.Count; ++index)
      {
        object key = op.DiscriminatorMap.TypeMap[index].Key;
        System.Data.Entity.Core.Query.InternalTrees.Node typeIdConstant = this.CreateTypeIdConstant(this.m_typeInfo.GetTypeInfo(TypeUsage.Create((EdmType) op.DiscriminatorMap.TypeMap[index].Value)));
        if (index == op.DiscriminatorMap.TypeMap.Count - 1)
        {
          args.Add(typeIdConstant);
        }
        else
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.m_command.CreateNode((Op) this.m_command.CreateConstantOp(Helper.GetModelTypeUsage(op.DiscriminatorMap.DiscriminatorProperty.TypeUsage), key));
          System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.m_command.CreateNode((Op) this.m_command.CreateComparisonOp(OpType.EQ, false), discriminator, node1);
          args.Add(node2);
          args.Add(typeIdConstant);
        }
      }
      discriminator = this.m_command.CreateNode((Op) caseOp, args);
      return discriminator;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      NewRecordOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.FlattenConstructor((ScalarOp) op, n);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node GetEntitySetIdExpr(
      EdmProperty entitySetIdProperty,
      NewEntityBaseOp op)
    {
      EntitySet entitySet = op.EntitySet;
      System.Data.Entity.Core.Query.InternalTrees.Node node;
      if (entitySet != null)
      {
        int entitySetId = this.m_typeInfo.GetEntitySetId(entitySet);
        node = this.m_command.CreateNode((Op) this.m_command.CreateInternalConstantOp(Helper.GetModelTypeUsage((EdmMember) entitySetIdProperty), (object) entitySetId));
      }
      else
        node = this.CreateNullConstantNode(Helper.GetModelTypeUsage((EdmMember) entitySetIdProperty));
      return node;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "optype")]
    private System.Data.Entity.Core.Query.InternalTrees.Node FlattenConstructor(
      ScalarOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.NewInstance || op.OpType == OpType.NewRecord || op.OpType == OpType.DiscriminatedNewEntity || op.OpType == OpType.NewEntity, "unexpected op: " + (object) op.OpType + "?");
      this.VisitChildren(n);
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.Type);
      RowType flattenedType1 = typeInfo.FlattenedType;
      NewEntityBaseOp op1 = op as NewEntityBaseOp;
      DiscriminatedNewEntityOp op2 = (DiscriminatedNewEntityOp) null;
      IEnumerable enumerable;
      if (op.OpType == OpType.NewRecord)
        enumerable = (IEnumerable) ((NewRecordOp) op).Properties;
      else if (op.OpType == OpType.DiscriminatedNewEntity)
      {
        op2 = (DiscriminatedNewEntityOp) op;
        enumerable = (IEnumerable) op2.DiscriminatorMap.Properties;
      }
      else
        enumerable = (IEnumerable) TypeHelpers.GetAllStructuralMembers(op.Type);
      List<EdmProperty> fields = new List<EdmProperty>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      if (typeInfo.HasTypeIdProperty)
      {
        fields.Add(typeInfo.TypeIdProperty);
        if (op2 == null)
        {
          args.Add(this.CreateTypeIdConstant(typeInfo));
        }
        else
        {
          System.Data.Entity.Core.Query.InternalTrees.Node discriminator = n.Children[0];
          if (typeInfo.RootType.DiscriminatorMap == null)
            discriminator = this.NormalizeTypeDiscriminatorValues(op2, discriminator);
          args.Add(discriminator);
        }
      }
      if (typeInfo.HasEntitySetIdProperty)
      {
        fields.Add(typeInfo.EntitySetIdProperty);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op1 != null, "unexpected optype:" + (object) op.OpType);
        System.Data.Entity.Core.Query.InternalTrees.Node entitySetIdExpr = this.GetEntitySetIdExpr(typeInfo.EntitySetIdProperty, op1);
        args.Add(entitySetIdExpr);
      }
      if (typeInfo.HasNullSentinelProperty)
      {
        fields.Add(typeInfo.NullSentinelProperty);
        args.Add(this.CreateNullSentinelConstant());
      }
      int index = op2 == null ? 0 : 1;
      foreach (EdmMember edmMember in enumerable)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child = n.Children[index];
        if (TypeUtils.IsStructuredType(Helper.GetModelTypeUsage(edmMember)))
        {
          RowType flattenedType2 = this.m_typeInfo.GetTypeInfo(Helper.GetModelTypeUsage(edmMember)).FlattenedType;
          int nestedStructureOffset = typeInfo.RootType.GetNestedStructureOffset((PropertyRef) new SimplePropertyRef(edmMember));
          foreach (EdmProperty property in flattenedType2.Properties)
          {
            System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildAccessor(child, property);
            if (node != null)
            {
              fields.Add(flattenedType1.Properties[nestedStructureOffset]);
              args.Add(node);
            }
            ++nestedStructureOffset;
          }
        }
        else
        {
          PropertyRef propertyRef = (PropertyRef) new SimplePropertyRef(edmMember);
          EdmProperty newProperty = typeInfo.GetNewProperty(propertyRef);
          fields.Add(newProperty);
          args.Add(child);
        }
        ++index;
      }
      if (op1 != null)
      {
        foreach (RelProperty relationshipProperty in op1.RelationshipProperties)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node child = n.Children[index];
          RowType flattenedType2 = this.m_typeInfo.GetTypeInfo(relationshipProperty.ToEnd.TypeUsage).FlattenedType;
          int nestedStructureOffset = typeInfo.RootType.GetNestedStructureOffset((PropertyRef) new RelPropertyRef(relationshipProperty));
          foreach (EdmProperty property in flattenedType2.Properties)
          {
            System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildAccessor(child, property);
            if (node != null)
            {
              fields.Add(flattenedType1.Properties[nestedStructureOffset]);
              args.Add(node);
            }
            ++nestedStructureOffset;
          }
          ++index;
        }
      }
      return this.m_command.CreateNode((Op) this.m_command.CreateNewRecordOp(typeInfo.FlattenedTypeUsage, fields), args);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(NullOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (!TypeUtils.IsStructuredType(op.Type))
      {
        if (TypeSemantics.IsEnumerationType(op.Type))
          op.Type = TypeHelpers.CreateEnumUnderlyingTypeUsage(op.Type);
        else if (TypeSemantics.IsStrongSpatialType(op.Type))
          op.Type = TypeHelpers.CreateSpatialUnionTypeUsage(op.Type);
        return n;
      }
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.Type);
      List<EdmProperty> fields = new List<EdmProperty>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      if (typeInfo.HasTypeIdProperty)
      {
        fields.Add(typeInfo.TypeIdProperty);
        TypeUsage modelTypeUsage = Helper.GetModelTypeUsage((EdmMember) typeInfo.TypeIdProperty);
        args.Add(this.CreateNullConstantNode(modelTypeUsage));
      }
      return this.m_command.CreateNode((Op) new NewRecordOp(typeInfo.FlattenedTypeUsage, fields), args);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(IsOfOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      if (!TypeUtils.IsStructuredType(op.IsOfType))
        return n;
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.IsOfType);
      return this.CreateTypeComparisonOp(n.Child0, typeInfo, op.IsOfOnly);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(TreatOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      ScalarOp op1 = (ScalarOp) n.Child0.Op;
      if (op.IsFakeTreat || TypeSemantics.IsStructurallyEqual(op1.Type, op.Type) || TypeSemantics.IsSubTypeOf(op1.Type, op.Type))
        return n.Child0;
      if (!TypeUtils.IsStructuredType(op.Type))
        return n;
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.Type);
      System.Data.Entity.Core.Query.InternalTrees.Node typeComparisonOp = this.CreateTypeComparisonOp(n.Child0, typeInfo, false);
      CaseOp caseOp = this.m_command.CreateCaseOp(typeInfo.FlattenedTypeUsage);
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.m_command.CreateNode((Op) caseOp, typeComparisonOp, n.Child0, this.CreateNullConstantNode(caseOp.Type));
      PropertyRefList nodeProperty = this.m_nodePropertyMap[n];
      return this.FlattenCaseOp(node, typeInfo, nodeProperty);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateTypeComparisonOp(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      TypeInfo typeInfo,
      bool isExact)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node typeIdProperty = this.BuildTypeIdAccessor(input, typeInfo);
      System.Data.Entity.Core.Query.InternalTrees.Node node;
      if (isExact)
        node = this.CreateTypeEqualsOp(typeInfo, typeIdProperty);
      else if (typeInfo.RootType.DiscriminatorMap != null)
      {
        node = this.CreateDisjunctiveTypeComparisonOp(typeInfo, typeIdProperty);
      }
      else
      {
        System.Data.Entity.Core.Query.InternalTrees.Node constantForPrefixMatch = this.CreateTypeIdConstantForPrefixMatch(typeInfo);
        node = this.m_command.CreateNode((Op) this.m_command.CreateLikeOp(), typeIdProperty, constantForPrefixMatch, this.CreateNullConstantNode(this.DefaultTypeIdType));
      }
      return node;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DiscriminatorMap")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node CreateDisjunctiveTypeComparisonOp(
      TypeInfo typeInfo,
      System.Data.Entity.Core.Query.InternalTrees.Node typeIdProperty)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(typeInfo.RootType.DiscriminatorMap != null, "should be used only for DiscriminatorMap type checks");
      IEnumerable<TypeInfo> typeInfos = typeInfo.GetTypeHierarchy().Where<TypeInfo>((Func<TypeInfo, bool>) (t => !t.Type.EdmType.Abstract));
      System.Data.Entity.Core.Query.InternalTrees.Node node = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      foreach (TypeInfo typeInfo1 in typeInfos)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node typeEqualsOp = this.CreateTypeEqualsOp(typeInfo1, typeIdProperty);
        node = node != null ? this.m_command.CreateNode((Op) this.m_command.CreateConditionalOp(OpType.Or), node, typeEqualsOp) : typeEqualsOp;
      }
      if (node == null)
        node = this.m_command.CreateNode((Op) this.m_command.CreateFalseOp());
      return node;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateTypeEqualsOp(
      TypeInfo typeInfo,
      System.Data.Entity.Core.Query.InternalTrees.Node typeIdProperty)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node typeIdConstant = this.CreateTypeIdConstant(typeInfo);
      return this.m_command.CreateNode((Op) this.m_command.CreateComparisonOp(OpType.EQ, false), typeIdProperty, typeIdConstant);
    }

    internal enum OperationKind
    {
      Equality,
      IsNull,
      GetIdentity,
      GetKeys,
      All,
    }
  }
}
