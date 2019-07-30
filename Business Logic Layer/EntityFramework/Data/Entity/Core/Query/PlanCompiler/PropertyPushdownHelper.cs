// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.PropertyPushdownHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class PropertyPushdownHelper : BasicOpVisitor
  {
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, PropertyRefList> m_nodePropertyRefMap;
    private readonly Dictionary<Var, PropertyRefList> m_varPropertyRefMap;

    private PropertyPushdownHelper()
    {
      this.m_varPropertyRefMap = new Dictionary<Var, PropertyRefList>();
      this.m_nodePropertyRefMap = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, PropertyRefList>();
    }

    internal static void Process(
      Command itree,
      out Dictionary<Var, PropertyRefList> varPropertyRefs,
      out Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, PropertyRefList> nodePropertyRefs)
    {
      PropertyPushdownHelper propertyPushdownHelper = new PropertyPushdownHelper();
      propertyPushdownHelper.Process(itree.Root);
      varPropertyRefs = propertyPushdownHelper.m_varPropertyRefMap;
      nodePropertyRefs = propertyPushdownHelper.m_nodePropertyRefMap;
    }

    private void Process(System.Data.Entity.Core.Query.InternalTrees.Node rootNode)
    {
      rootNode.Op.Accept((BasicOpVisitor) this, rootNode);
    }

    private PropertyRefList GetPropertyRefList(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      PropertyRefList propertyRefList;
      if (!this.m_nodePropertyRefMap.TryGetValue(node, out propertyRefList))
      {
        propertyRefList = new PropertyRefList();
        this.m_nodePropertyRefMap[node] = propertyRefList;
      }
      return propertyRefList;
    }

    private void AddPropertyRefs(System.Data.Entity.Core.Query.InternalTrees.Node node, PropertyRefList propertyRefs)
    {
      this.GetPropertyRefList(node).Append(propertyRefs);
    }

    private PropertyRefList GetPropertyRefList(Var v)
    {
      PropertyRefList propertyRefList;
      if (!this.m_varPropertyRefMap.TryGetValue(v, out propertyRefList))
      {
        propertyRefList = new PropertyRefList();
        this.m_varPropertyRefMap[v] = propertyRefList;
      }
      return propertyRefList;
    }

    private void AddPropertyRefs(Var v, PropertyRefList propertyRefs)
    {
      this.GetPropertyRefList(v).Append(propertyRefs);
    }

    private static PropertyRefList GetIdentityProperties(EntityType type)
    {
      PropertyRefList keyProperties = PropertyPushdownHelper.GetKeyProperties(type);
      keyProperties.Add((PropertyRef) EntitySetIdPropertyRef.Instance);
      return keyProperties;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-EdmProperty")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EntityType")]
    private static PropertyRefList GetKeyProperties(EntityType entityType)
    {
      PropertyRefList propertyRefList = new PropertyRefList();
      foreach (EdmMember keyMember in entityType.KeyMembers)
      {
        EdmProperty edmProperty = keyMember as EdmProperty;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(edmProperty != null, "EntityType had non-EdmProperty key member?");
        SimplePropertyRef simplePropertyRef = new SimplePropertyRef((EdmMember) edmProperty);
        propertyRefList.Add((PropertyRef) simplePropertyRef);
      }
      return propertyRefList;
    }

    protected override void VisitDefault(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        ScalarOp op = child.Op as ScalarOp;
        if (op != null && TypeUtils.IsStructuredType(op.Type))
          this.AddPropertyRefs(child, PropertyRefList.All);
      }
      this.VisitChildren(n);
    }

    public override void Visit(SoftCastOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      PropertyRefList propertyRefs = (PropertyRefList) null;
      if (TypeSemantics.IsReferenceType(op.Type))
        propertyRefs = PropertyRefList.All;
      else if (TypeSemantics.IsNominalType(op.Type))
        propertyRefs = this.m_nodePropertyRefMap[n].Clone();
      else if (TypeSemantics.IsRowType(op.Type))
        propertyRefs = PropertyRefList.All;
      if (propertyRefs != null)
        this.AddPropertyRefs(n.Child0, propertyRefs);
      this.VisitChildren(n);
    }

    public override void Visit(CaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      PropertyRefList propertyRefList = this.GetPropertyRefList(n);
      for (int index = 1; index < n.Children.Count - 1; index += 2)
      {
        PropertyRefList propertyRefs = propertyRefList.Clone();
        this.AddPropertyRefs(n.Children[index], propertyRefs);
      }
      this.AddPropertyRefs(n.Children[n.Children.Count - 1], propertyRefList.Clone());
      this.VisitChildren(n);
    }

    public override void Visit(CollectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "childOpType")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override void Visit(ComparisonOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      TypeUsage type = (n.Child0.Op as ScalarOp).Type;
      if (!TypeUtils.IsStructuredType(type))
        this.VisitChildren(n);
      else if (TypeSemantics.IsRowType(type) || TypeSemantics.IsReferenceType(type))
      {
        this.VisitDefault(n);
      }
      else
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsEntityType(type), "unexpected childOpType?");
        PropertyRefList identityProperties = PropertyPushdownHelper.GetIdentityProperties(TypeHelpers.GetEdmType<EntityType>(type));
        foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
          this.AddPropertyRefs(child, identityProperties);
        this.VisitChildren(n);
      }
    }

    public override void Visit(ElementOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScalarOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetEntityRefOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override void Visit(GetEntityRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      ScalarOp op1 = n.Child0.Op as ScalarOp;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op1 != null, "input to GetEntityRefOp is not a ScalarOp?");
      PropertyRefList identityProperties = PropertyPushdownHelper.GetIdentityProperties(TypeHelpers.GetEdmType<EntityType>(op1.Type));
      this.AddPropertyRefs(n.Child0, identityProperties);
      this.VisitNode(n.Child0);
    }

    public override void Visit(IsOfOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      PropertyRefList propertyRefs = new PropertyRefList();
      propertyRefs.Add((PropertyRef) TypeIdPropertyRef.Instance);
      this.AddPropertyRefs(n.Child0, propertyRefs);
      this.VisitChildren(n);
    }

    private void VisitPropertyOp(Op op, System.Data.Entity.Core.Query.InternalTrees.Node n, PropertyRef propertyRef)
    {
      PropertyRefList propertyRefs = new PropertyRefList();
      if (!TypeUtils.IsStructuredType(op.Type))
      {
        propertyRefs.Add(propertyRef);
      }
      else
      {
        PropertyRefList propertyRefList = this.GetPropertyRefList(n);
        if (propertyRefList.AllProperties)
        {
          propertyRefs = propertyRefList;
        }
        else
        {
          foreach (PropertyRef property in propertyRefList.Properties)
            propertyRefs.Add(property.CreateNestedPropertyRef(propertyRef));
        }
      }
      this.AddPropertyRefs(n.Child0, propertyRefs);
      this.VisitChildren(n);
    }

    public override void Visit(RelPropertyOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitPropertyOp((Op) op, n, (PropertyRef) new RelPropertyRef(op.PropertyInfo));
    }

    public override void Visit(PropertyOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitPropertyOp((Op) op, n, (PropertyRef) new SimplePropertyRef(op.PropertyInfo));
    }

    public override void Visit(TreatOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      PropertyRefList propertyRefs = this.GetPropertyRefList(n).Clone();
      propertyRefs.Add((PropertyRef) TypeIdPropertyRef.Instance);
      this.AddPropertyRefs(n.Child0, propertyRefs);
      this.VisitChildren(n);
    }

    public override void Visit(VarRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (!TypeUtils.IsStructuredType(op.Var.Type))
        return;
      PropertyRefList propertyRefList = this.GetPropertyRefList(n);
      this.AddPropertyRefs(op.Var, propertyRefList);
    }

    public override void Visit(VarDefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (TypeUtils.IsStructuredType(op.Var.Type))
      {
        PropertyRefList propertyRefList = this.GetPropertyRefList(op.Var);
        this.AddPropertyRefs(n.Child0, propertyRefList);
      }
      this.VisitChildren(n);
    }

    public override void Visit(VarDefListOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
    }

    protected override void VisitApplyOp(ApplyBaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitNode(n.Child1);
      this.VisitNode(n.Child0);
    }

    public override void Visit(DistinctOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (Var key in op.Keys)
      {
        if (TypeUtils.IsStructuredType(key.Type))
          this.AddPropertyRefs(key, PropertyRefList.All);
      }
      this.VisitChildren(n);
    }

    public override void Visit(FilterOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitNode(n.Child1);
      this.VisitNode(n.Child0);
    }

    protected override void VisitGroupByOp(GroupByBaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (Var key in op.Keys)
      {
        if (TypeUtils.IsStructuredType(key.Type))
          this.AddPropertyRefs(key, PropertyRefList.All);
      }
      this.VisitChildrenReverse(n);
    }

    protected override void VisitJoinOp(JoinBaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (n.Op.OpType == OpType.CrossJoin)
      {
        this.VisitChildren(n);
      }
      else
      {
        this.VisitNode(n.Child2);
        this.VisitNode(n.Child0);
        this.VisitNode(n.Child1);
      }
    }

    public override void Visit(ProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitNode(n.Child1);
      this.VisitNode(n.Child0);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "scanTableOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override void Visit(ScanTableOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild0, "scanTableOp with an input?");
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScanViewOp's")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScanViewOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override void Visit(ScanViewOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.Table.Columns.Count == 1, "ScanViewOp with multiple columns?");
      PropertyRefList propertyRefList = this.GetPropertyRefList(op.Table.Columns[0]);
      Var singletonVar = NominalTypeEliminator.GetSingletonVar(n.Child0);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(singletonVar != null, "cannot determine single Var from ScanViewOp's input");
      this.AddPropertyRefs(singletonVar, propertyRefList.Clone());
      this.VisitChildren(n);
    }

    protected override void VisitSetOp(SetOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (Dictionary<Var, Var> var in op.VarMap)
      {
        foreach (KeyValuePair<Var, Var> keyValuePair in var)
        {
          if (TypeUtils.IsStructuredType(keyValuePair.Key.Type))
          {
            PropertyRefList propertyRefList = this.GetPropertyRefList(keyValuePair.Key);
            PropertyRefList propertyRefs;
            if (op.OpType == OpType.Intersect || op.OpType == OpType.Except)
            {
              propertyRefs = PropertyRefList.All;
              this.AddPropertyRefs(keyValuePair.Key, propertyRefs);
            }
            else
              propertyRefs = propertyRefList.Clone();
            this.AddPropertyRefs(keyValuePair.Value, propertyRefs);
          }
        }
      }
      this.VisitChildren(n);
    }

    protected override void VisitSortOp(SortBaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (SortKey key in op.Keys)
      {
        if (TypeUtils.IsStructuredType(key.Var.Type))
          this.AddPropertyRefs(key.Var, PropertyRefList.All);
      }
      if (n.HasChild1)
        this.VisitNode(n.Child1);
      this.VisitNode(n.Child0);
    }

    public override void Visit(UnnestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
    }

    public override void Visit(PhysicalProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (Var output in (List<Var>) op.Outputs)
      {
        if (TypeUtils.IsStructuredType(output.Type))
          this.AddPropertyRefs(output, PropertyRefList.All);
      }
      this.VisitChildren(n);
    }

    public override void Visit(MultiStreamNestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override void Visit(SingleStreamNestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }
  }
}
