// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.NodeCounter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class NodeCounter : BasicOpVisitorOfT<int>
  {
    internal static int Count(Node subTree)
    {
      return new NodeCounter().VisitNode(subTree);
    }

    protected override int VisitDefault(Node n)
    {
      int num = 1;
      foreach (Node child in n.Children)
        num += this.VisitNode(child);
      return num;
    }
  }
}
