// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.Dump
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class Dump : BasicOpVisitor, IDisposable
  {
    internal static readonly Encoding DefaultEncoding = Encoding.UTF8;
    private readonly XmlWriter _writer;

    private Dump(Stream stream)
      : this(stream, Dump.DefaultEncoding)
    {
    }

    private Dump(Stream stream, Encoding encoding)
    {
      this._writer = XmlWriter.Create(stream, new XmlWriterSettings()
      {
        CheckCharacters = false,
        Indent = true,
        Encoding = encoding
      });
      this._writer.WriteStartDocument(true);
    }

    internal static string ToXml(Command itree)
    {
      return Dump.ToXml(itree.Root);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    internal static string ToXml(Node subtree)
    {
      MemoryStream memoryStream = new MemoryStream();
      using (Dump dumper = new Dump((Stream) memoryStream))
      {
        using (new Dump.AutoXml(dumper, "nodes"))
          dumper.VisitNode(subtree);
      }
      return Dump.DefaultEncoding.GetString(memoryStream.ToArray());
    }

    void IDisposable.Dispose()
    {
      GC.SuppressFinalize((object) this);
      try
      {
        this._writer.WriteEndDocument();
        this._writer.Flush();
        this._writer.Close();
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
          return;
        throw;
      }
    }

    internal void Begin(string name, Dictionary<string, object> attrs)
    {
      this._writer.WriteStartElement(name);
      if (attrs == null)
        return;
      foreach (KeyValuePair<string, object> attr in attrs)
        this._writer.WriteAttributeString(attr.Key, attr.Value.ToString());
    }

    internal void BeginExpression()
    {
      this.WriteString("(");
    }

    internal void EndExpression()
    {
      this.WriteString(")");
    }

    internal void End()
    {
      this._writer.WriteEndElement();
    }

    internal void WriteString(string value)
    {
      this._writer.WriteString(value);
    }

    protected override void VisitDefault(Node n)
    {
      using (new Dump.AutoXml(this, n.Op))
        base.VisitDefault(n);
    }

    protected override void VisitScalarOpDefault(ScalarOp op, Node n)
    {
      using (new Dump.AutoString(this, (Op) op))
      {
        string str = string.Empty;
        foreach (Node child in n.Children)
        {
          this.WriteString(str);
          this.VisitNode(child);
          str = ",";
        }
      }
    }

    protected override void VisitJoinOp(JoinBaseOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
      {
        if (n.Children.Count > 2)
        {
          using (new Dump.AutoXml(this, "condition"))
            this.VisitNode(n.Child2);
        }
        using (new Dump.AutoXml(this, "input"))
          this.VisitNode(n.Child0);
        using (new Dump.AutoXml(this, "input"))
          this.VisitNode(n.Child1);
      }
    }

    public override void Visit(CaseOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
      {
        int num = 0;
        while (num < n.Children.Count)
        {
          if (num + 1 < n.Children.Count)
          {
            using (new Dump.AutoXml(this, "when"))
              this.VisitNode(n.Children[num++]);
            using (new Dump.AutoXml(this, "then"))
              this.VisitNode(n.Children[num++]);
          }
          else
          {
            using (new Dump.AutoXml(this, "else"))
              this.VisitNode(n.Children[num++]);
          }
        }
      }
    }

    public override void Visit(CollectOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
        this.VisitChildren(n);
    }

    protected override void VisitConstantOp(ConstantBaseOp op, Node n)
    {
      using (new Dump.AutoString(this, (Op) op))
      {
        if (op.Value == null)
        {
          this.WriteString("null");
        }
        else
        {
          this.WriteString("(");
          this.WriteString(op.Type.EdmType.FullName);
          this.WriteString(")");
          this.WriteString(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", op.Value));
        }
        this.VisitChildren(n);
      }
    }

    public override void Visit(DistinctOp op, Node n)
    {
      Dictionary<string, object> attrs = new Dictionary<string, object>();
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      foreach (Var key in op.Keys)
      {
        stringBuilder.Append(str);
        stringBuilder.Append(key.Id);
        str = ",";
      }
      if (stringBuilder.Length != 0)
        attrs.Add("Keys", (object) stringBuilder.ToString());
      using (new Dump.AutoXml(this, (Op) op, attrs))
        this.VisitChildren(n);
    }

    protected override void VisitGroupByOp(GroupByBaseOp op, Node n)
    {
      Dictionary<string, object> attrs = new Dictionary<string, object>();
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      foreach (Var key in op.Keys)
      {
        stringBuilder.Append(str);
        stringBuilder.Append(key.Id);
        str = ",";
      }
      if (stringBuilder.Length != 0)
        attrs.Add("Keys", (object) stringBuilder.ToString());
      using (new Dump.AutoXml(this, (Op) op, attrs))
      {
        using (new Dump.AutoXml(this, "outputs"))
        {
          foreach (Var output in op.Outputs)
            this.DumpVar(output);
        }
        this.VisitChildren(n);
      }
    }

    public override void Visit(IsOfOp op, Node n)
    {
      using (new Dump.AutoXml(this, op.IsOfOnly ? "IsOfOnly" : "IsOf"))
      {
        string str = string.Empty;
        foreach (Node child in n.Children)
        {
          this.WriteString(str);
          this.VisitNode(child);
          str = ",";
        }
      }
    }

    protected override void VisitNestOp(NestBaseOp op, Node n)
    {
      Dictionary<string, object> attrs1 = new Dictionary<string, object>();
      SingleStreamNestOp singleStreamNestOp = op as SingleStreamNestOp;
      if (singleStreamNestOp != null)
        attrs1.Add("Discriminator", singleStreamNestOp.Discriminator == null ? (object) "<null>" : (object) singleStreamNestOp.Discriminator.ToString());
      StringBuilder sb = new StringBuilder();
      if (singleStreamNestOp != null)
      {
        sb.Length = 0;
        string str = string.Empty;
        foreach (Var key in singleStreamNestOp.Keys)
        {
          sb.Append(str);
          sb.Append(key.Id);
          str = ",";
        }
        if (sb.Length != 0)
          attrs1.Add("Keys", (object) sb.ToString());
      }
      using (new Dump.AutoXml(this, (Op) op, attrs1))
      {
        using (new Dump.AutoXml(this, "outputs"))
        {
          foreach (Var output in op.Outputs)
            this.DumpVar(output);
        }
        foreach (CollectionInfo collectionInfo in op.CollectionInfo)
        {
          Dictionary<string, object> attrs2 = new Dictionary<string, object>();
          attrs2.Add("CollectionVar", (object) collectionInfo.CollectionVar);
          if (collectionInfo.DiscriminatorValue != null)
            attrs2.Add("DiscriminatorValue", collectionInfo.DiscriminatorValue);
          if (collectionInfo.FlattenedElementVars.Count != 0)
            attrs2.Add("FlattenedElementVars", (object) Dump.FormatVarList(sb, collectionInfo.FlattenedElementVars));
          if (collectionInfo.Keys.Count != 0)
            attrs2.Add("Keys", (object) collectionInfo.Keys);
          if (collectionInfo.SortKeys.Count != 0)
            attrs2.Add("SortKeys", (object) Dump.FormatVarList(sb, collectionInfo.SortKeys));
          using (new Dump.AutoXml(this, "collection", attrs2))
            collectionInfo.ColumnMap.Accept<Dump>((ColumnMapVisitor<Dump>) Dump.ColumnMapDumper.Instance, this);
        }
        this.VisitChildren(n);
      }
    }

    private static string FormatVarList(StringBuilder sb, VarList varList)
    {
      sb.Length = 0;
      string str = string.Empty;
      foreach (Var var in (List<Var>) varList)
      {
        sb.Append(str);
        sb.Append(var.Id);
        str = ",";
      }
      return sb.ToString();
    }

    private static string FormatVarList(StringBuilder sb, List<SortKey> varList)
    {
      sb.Length = 0;
      string str = string.Empty;
      foreach (SortKey var in varList)
      {
        sb.Append(str);
        sb.Append(var.Var.Id);
        str = ",";
      }
      return sb.ToString();
    }

    private void VisitNewOp(Op op, Node n)
    {
      using (new Dump.AutoXml(this, op))
      {
        foreach (Node child in n.Children)
        {
          using (new Dump.AutoXml(this, "argument", (Dictionary<string, object>) null))
            this.VisitNode(child);
        }
      }
    }

    public override void Visit(NewEntityOp op, Node n)
    {
      this.VisitNewOp((Op) op, n);
    }

    public override void Visit(NewInstanceOp op, Node n)
    {
      this.VisitNewOp((Op) op, n);
    }

    public override void Visit(DiscriminatedNewEntityOp op, Node n)
    {
      this.VisitNewOp((Op) op, n);
    }

    public override void Visit(NewMultisetOp op, Node n)
    {
      this.VisitNewOp((Op) op, n);
    }

    public override void Visit(NewRecordOp op, Node n)
    {
      this.VisitNewOp((Op) op, n);
    }

    public override void Visit(PhysicalProjectOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
      {
        using (new Dump.AutoXml(this, "outputs"))
        {
          foreach (Var output in (List<Var>) op.Outputs)
            this.DumpVar(output);
        }
        using (new Dump.AutoXml(this, "columnMap"))
          op.ColumnMap.Accept<Dump>((ColumnMapVisitor<Dump>) Dump.ColumnMapDumper.Instance, this);
        using (new Dump.AutoXml(this, "input"))
          this.VisitChildren(n);
      }
    }

    public override void Visit(ProjectOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
      {
        using (new Dump.AutoXml(this, "outputs"))
        {
          foreach (Var output in op.Outputs)
            this.DumpVar(output);
        }
        this.VisitChildren(n);
      }
    }

    public override void Visit(PropertyOp op, Node n)
    {
      using (new Dump.AutoString(this, (Op) op))
      {
        this.VisitChildren(n);
        this.WriteString(".");
        this.WriteString(op.PropertyInfo.Name);
      }
    }

    public override void Visit(RelPropertyOp op, Node n)
    {
      using (new Dump.AutoString(this, (Op) op))
      {
        this.VisitChildren(n);
        this.WriteString(".NAVIGATE(");
        this.WriteString(op.PropertyInfo.Relationship.Name);
        this.WriteString(",");
        this.WriteString(op.PropertyInfo.FromEnd.Name);
        this.WriteString(",");
        this.WriteString(op.PropertyInfo.ToEnd.Name);
        this.WriteString(")");
      }
    }

    public override void Visit(ScanTableOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
      {
        this.DumpTable(op.Table);
        this.VisitChildren(n);
      }
    }

    public override void Visit(ScanViewOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
      {
        this.DumpTable(op.Table);
        this.VisitChildren(n);
      }
    }

    protected override void VisitSetOp(SetOp op, Node n)
    {
      Dictionary<string, object> attrs = new Dictionary<string, object>();
      if (OpType.UnionAll == op.OpType)
      {
        UnionAllOp unionAllOp = (UnionAllOp) op;
        if (unionAllOp.BranchDiscriminator != null)
          attrs.Add("branchDiscriminator", (object) unionAllOp.BranchDiscriminator);
      }
      using (new Dump.AutoXml(this, (Op) op, attrs))
      {
        using (new Dump.AutoXml(this, "outputs"))
        {
          foreach (Var output in op.Outputs)
            this.DumpVar(output);
        }
        int num = 0;
        foreach (Node child in n.Children)
        {
          using (new Dump.AutoXml(this, "input", new Dictionary<string, object>()
          {
            {
              "VarMap",
              (object) op.VarMap[num++].ToString()
            }
          }))
            this.VisitNode(child);
        }
      }
    }

    public override void Visit(SortOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op))
        base.Visit(op, n);
    }

    public override void Visit(ConstrainedSortOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op, new Dictionary<string, object>()
      {
        {
          "WithTies",
          (object) op.WithTies
        }
      }))
        base.Visit(op, n);
    }

    protected override void VisitSortOp(SortBaseOp op, Node n)
    {
      using (new Dump.AutoXml(this, "keys"))
      {
        foreach (SortKey key in op.Keys)
        {
          using (new Dump.AutoXml(this, "sortKey", new Dictionary<string, object>()
          {
            {
              "Var",
              (object) key.Var
            },
            {
              "Ascending",
              (object) key.AscendingSort
            },
            {
              "Collation",
              (object) key.Collation
            }
          }))
            ;
        }
      }
      this.VisitChildren(n);
    }

    public override void Visit(UnnestOp op, Node n)
    {
      Dictionary<string, object> attrs = new Dictionary<string, object>();
      if (op.Var != null)
        attrs.Add("Var", (object) op.Var.Id);
      using (new Dump.AutoXml(this, (Op) op, attrs))
      {
        this.DumpTable(op.Table);
        this.VisitChildren(n);
      }
    }

    public override void Visit(VarDefOp op, Node n)
    {
      using (new Dump.AutoXml(this, (Op) op, new Dictionary<string, object>()
      {
        {
          "Var",
          (object) op.Var.Id
        }
      }))
        this.VisitChildren(n);
    }

    public override void Visit(VarRefOp op, Node n)
    {
      using (new Dump.AutoString(this, (Op) op))
      {
        this.VisitChildren(n);
        if (op.Type != null)
        {
          this.WriteString("Type=");
          this.WriteString(op.Type.ToString());
          this.WriteString(", ");
        }
        this.WriteString("Var=");
        this.WriteString(op.Var.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
    }

    private void DumpVar(Var v)
    {
      Dictionary<string, object> attrs = new Dictionary<string, object>();
      attrs.Add("Var", (object) v.Id);
      ColumnVar columnVar = v as ColumnVar;
      if (columnVar != null)
      {
        attrs.Add("Name", (object) columnVar.ColumnMetadata.Name);
        attrs.Add("Type", (object) columnVar.ColumnMetadata.Type.ToString());
      }
      using (new Dump.AutoXml(this, v.GetType().Name, attrs))
        ;
    }

    private void DumpVars(List<Var> vars)
    {
      foreach (Var var in vars)
        this.DumpVar(var);
    }

    private void DumpTable(Table table)
    {
      Dictionary<string, object> attrs = new Dictionary<string, object>();
      attrs.Add("Table", (object) table.TableId);
      if (table.TableMetadata.Extent != null)
        attrs.Add("Extent", (object) table.TableMetadata.Extent.Name);
      using (new Dump.AutoXml(this, "Table", attrs))
        this.DumpVars((List<Var>) table.Columns);
    }

    internal class ColumnMapDumper : ColumnMapVisitor<Dump>
    {
      internal static Dump.ColumnMapDumper Instance = new Dump.ColumnMapDumper();

      private ColumnMapDumper()
      {
      }

      private void DumpCollection(CollectionColumnMap columnMap, Dump dumper)
      {
        if (columnMap.ForeignKeys.Length > 0)
        {
          using (new Dump.AutoXml(dumper, "foreignKeys"))
            this.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, dumper);
        }
        if (columnMap.Keys.Length > 0)
        {
          using (new Dump.AutoXml(dumper, "keys"))
            this.VisitList<SimpleColumnMap>(columnMap.Keys, dumper);
        }
        using (new Dump.AutoXml(dumper, "element"))
          columnMap.Element.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
      }

      private static Dictionary<string, object> GetAttributes(ColumnMap columnMap)
      {
        return new Dictionary<string, object>()
        {
          {
            "Type",
            (object) columnMap.Type.ToString()
          }
        };
      }

      internal override void Visit(ComplexTypeColumnMap columnMap, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "ComplexType", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
        {
          if (columnMap.NullSentinel != null)
          {
            using (new Dump.AutoXml(dumper, "nullSentinel"))
              columnMap.NullSentinel.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
          }
          this.VisitList<ColumnMap>(columnMap.Properties, dumper);
        }
      }

      internal override void Visit(DiscriminatedCollectionColumnMap columnMap, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "DiscriminatedCollection", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
        {
          using (new Dump.AutoXml(dumper, "discriminator", new Dictionary<string, object>()
          {
            {
              "Value",
              columnMap.DiscriminatorValue
            }
          }))
            columnMap.Discriminator.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
          this.DumpCollection((CollectionColumnMap) columnMap, dumper);
        }
      }

      internal override void Visit(EntityColumnMap columnMap, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "Entity", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
        {
          using (new Dump.AutoXml(dumper, "entityIdentity"))
            this.VisitEntityIdentity(columnMap.EntityIdentity, dumper);
          this.VisitList<ColumnMap>(columnMap.Properties, dumper);
        }
      }

      internal override void Visit(SimplePolymorphicColumnMap columnMap, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "SimplePolymorphic", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
        {
          using (new Dump.AutoXml(dumper, "typeDiscriminator"))
            columnMap.TypeDiscriminator.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
          Dictionary<string, object> attrs = new Dictionary<string, object>();
          foreach (KeyValuePair<object, TypedColumnMap> typeChoice in columnMap.TypeChoices)
          {
            attrs.Clear();
            attrs.Add("DiscriminatorValue", typeChoice.Key);
            using (new Dump.AutoXml(dumper, "choice", attrs))
              typeChoice.Value.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
          }
          using (new Dump.AutoXml(dumper, "default"))
            this.VisitList<ColumnMap>(columnMap.Properties, dumper);
        }
      }

      internal override void Visit(
        MultipleDiscriminatorPolymorphicColumnMap columnMap,
        Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "MultipleDiscriminatorPolymorphic", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
        {
          using (new Dump.AutoXml(dumper, "typeDiscriminators"))
            this.VisitList<SimpleColumnMap>(columnMap.TypeDiscriminators, dumper);
          Dictionary<string, object> attrs = new Dictionary<string, object>();
          foreach (KeyValuePair<System.Data.Entity.Core.Metadata.Edm.EntityType, TypedColumnMap> typeChoice in columnMap.TypeChoices)
          {
            attrs.Clear();
            attrs.Add("EntityType", (object) typeChoice.Key);
            using (new Dump.AutoXml(dumper, "choice", attrs))
              typeChoice.Value.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
          }
          using (new Dump.AutoXml(dumper, "default"))
            this.VisitList<ColumnMap>(columnMap.Properties, dumper);
        }
      }

      internal override void Visit(RecordColumnMap columnMap, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "Record", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
        {
          if (columnMap.NullSentinel != null)
          {
            using (new Dump.AutoXml(dumper, "nullSentinel"))
              columnMap.NullSentinel.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
          }
          this.VisitList<ColumnMap>(columnMap.Properties, dumper);
        }
      }

      internal override void Visit(RefColumnMap columnMap, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "Ref", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
        {
          using (new Dump.AutoXml(dumper, "entityIdentity"))
            this.VisitEntityIdentity(columnMap.EntityIdentity, dumper);
        }
      }

      internal override void Visit(SimpleCollectionColumnMap columnMap, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "SimpleCollection", Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap)))
          this.DumpCollection((CollectionColumnMap) columnMap, dumper);
      }

      internal override void Visit(ScalarColumnMap columnMap, Dump dumper)
      {
        Dictionary<string, object> attributes = Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap);
        attributes.Add("CommandId", (object) columnMap.CommandId);
        attributes.Add("ColumnPos", (object) columnMap.ColumnPos);
        using (new Dump.AutoXml(dumper, "AssignedSimple", attributes))
          ;
      }

      internal override void Visit(VarRefColumnMap columnMap, Dump dumper)
      {
        Dictionary<string, object> attributes = Dump.ColumnMapDumper.GetAttributes((ColumnMap) columnMap);
        attributes.Add("Var", (object) columnMap.Var.Id);
        using (new Dump.AutoXml(dumper, "VarRef", attributes))
          ;
      }

      protected override void VisitEntityIdentity(
        DiscriminatedEntityIdentity entityIdentity,
        Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "DiscriminatedEntityIdentity"))
        {
          using (new Dump.AutoXml(dumper, "entitySetId"))
            entityIdentity.EntitySetColumnMap.Accept<Dump>((ColumnMapVisitor<Dump>) this, dumper);
          if (entityIdentity.Keys.Length <= 0)
            return;
          using (new Dump.AutoXml(dumper, "keys"))
            this.VisitList<SimpleColumnMap>(entityIdentity.Keys, dumper);
        }
      }

      protected override void VisitEntityIdentity(SimpleEntityIdentity entityIdentity, Dump dumper)
      {
        using (new Dump.AutoXml(dumper, "SimpleEntityIdentity"))
        {
          if (entityIdentity.Keys.Length <= 0)
            return;
          using (new Dump.AutoXml(dumper, "keys"))
            this.VisitList<SimpleColumnMap>(entityIdentity.Keys, dumper);
        }
      }
    }

    internal struct AutoString : IDisposable
    {
      private readonly Dump _dumper;

      internal AutoString(Dump dumper, Op op)
      {
        this._dumper = dumper;
        this._dumper.WriteString(Dump.AutoString.ToString(op.OpType));
        this._dumper.BeginExpression();
      }

      public void Dispose()
      {
        try
        {
          this._dumper.EndExpression();
        }
        catch (Exception ex)
        {
          if (ex.IsCatchableExceptionType())
            return;
          throw;
        }
      }

      [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
      internal static string ToString(OpType op)
      {
        switch (op)
        {
          case OpType.Constant:
            return "Constant";
          case OpType.InternalConstant:
            return "InternalConstant";
          case OpType.NullSentinel:
            return "NullSentinel";
          case OpType.Null:
            return "Null";
          case OpType.ConstantPredicate:
            return "ConstantPredicate";
          case OpType.VarRef:
            return "VarRef";
          case OpType.GT:
            return "GT";
          case OpType.GE:
            return "GE";
          case OpType.LE:
            return "LE";
          case OpType.LT:
            return "LT";
          case OpType.EQ:
            return "EQ";
          case OpType.NE:
            return "NE";
          case OpType.Like:
            return "Like";
          case OpType.Plus:
            return "Plus";
          case OpType.Minus:
            return "Minus";
          case OpType.Multiply:
            return "Multiply";
          case OpType.Divide:
            return "Divide";
          case OpType.Modulo:
            return "Modulo";
          case OpType.UnaryMinus:
            return "UnaryMinus";
          case OpType.And:
            return "And";
          case OpType.Or:
            return "Or";
          case OpType.In:
            return "In";
          case OpType.Not:
            return "Not";
          case OpType.IsNull:
            return "IsNull";
          case OpType.Case:
            return "Case";
          case OpType.Treat:
            return "Treat";
          case OpType.IsOf:
            return "IsOf";
          case OpType.Cast:
            return "Cast";
          case OpType.SoftCast:
            return "SoftCast";
          case OpType.Aggregate:
            return "Aggregate";
          case OpType.Function:
            return "Function";
          case OpType.RelProperty:
            return "RelProperty";
          case OpType.Property:
            return "Property";
          case OpType.NewEntity:
            return "NewEntity";
          case OpType.NewInstance:
            return "NewInstance";
          case OpType.DiscriminatedNewEntity:
            return "DiscriminatedNewEntity";
          case OpType.NewMultiset:
            return "NewMultiset";
          case OpType.NewRecord:
            return "NewRecord";
          case OpType.GetRefKey:
            return "GetRefKey";
          case OpType.GetEntityRef:
            return "GetEntityRef";
          case OpType.Ref:
            return "Ref";
          case OpType.Exists:
            return "Exists";
          case OpType.Element:
            return "Element";
          case OpType.Collect:
            return "Collect";
          case OpType.Deref:
            return "Deref";
          case OpType.Navigate:
            return "Navigate";
          case OpType.ScanTable:
            return "ScanTable";
          case OpType.ScanView:
            return "ScanView";
          case OpType.Filter:
            return "Filter";
          case OpType.Project:
            return "Project";
          case OpType.InnerJoin:
            return "InnerJoin";
          case OpType.LeftOuterJoin:
            return "LeftOuterJoin";
          case OpType.FullOuterJoin:
            return "FullOuterJoin";
          case OpType.CrossJoin:
            return "CrossJoin";
          case OpType.CrossApply:
            return "CrossApply";
          case OpType.OuterApply:
            return "OuterApply";
          case OpType.Unnest:
            return "Unnest";
          case OpType.Sort:
            return "Sort";
          case OpType.ConstrainedSort:
            return "ConstrainedSort";
          case OpType.GroupBy:
            return "GroupBy";
          case OpType.GroupByInto:
            return "GroupByInto";
          case OpType.UnionAll:
            return "UnionAll";
          case OpType.Intersect:
            return "Intersect";
          case OpType.Except:
            return "Except";
          case OpType.Distinct:
            return "Distinct";
          case OpType.SingleRow:
            return "SingleRow";
          case OpType.SingleRowTable:
            return "SingleRowTable";
          case OpType.VarDef:
            return "VarDef";
          case OpType.VarDefList:
            return "VarDefList";
          case OpType.Leaf:
            return "Leaf";
          case OpType.PhysicalProject:
            return "PhysicalProject";
          case OpType.SingleStreamNest:
            return "SingleStreamNest";
          case OpType.MultiStreamNest:
            return "MultiStreamNest";
          default:
            return op.ToString();
        }
      }
    }

    internal struct AutoXml : IDisposable
    {
      private readonly string _nodeName;
      private readonly Dump _dumper;

      internal AutoXml(Dump dumper, Op op)
      {
        this._dumper = dumper;
        this._nodeName = Dump.AutoString.ToString(op.OpType);
        Dictionary<string, object> attrs = new Dictionary<string, object>();
        if (op.Type != null)
          attrs.Add("Type", (object) op.Type.ToString());
        this._dumper.Begin(this._nodeName, attrs);
      }

      internal AutoXml(Dump dumper, Op op, Dictionary<string, object> attrs)
      {
        this._dumper = dumper;
        this._nodeName = Dump.AutoString.ToString(op.OpType);
        Dictionary<string, object> attrs1 = new Dictionary<string, object>();
        if (op.Type != null)
          attrs1.Add("Type", (object) op.Type.ToString());
        foreach (KeyValuePair<string, object> attr in attrs)
          attrs1.Add(attr.Key, attr.Value);
        this._dumper.Begin(this._nodeName, attrs1);
      }

      internal AutoXml(Dump dumper, string nodeName)
      {
        this = new Dump.AutoXml(dumper, nodeName, (Dictionary<string, object>) null);
      }

      internal AutoXml(Dump dumper, string nodeName, Dictionary<string, object> attrs)
      {
        this._dumper = dumper;
        this._nodeName = nodeName;
        this._dumper.Begin(this._nodeName, attrs);
      }

      public void Dispose()
      {
        this._dumper.End();
      }
    }
  }
}
