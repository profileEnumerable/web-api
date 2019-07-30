// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.TypeIdPropertyRef
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class TypeIdPropertyRef : PropertyRef
  {
    internal static TypeIdPropertyRef Instance = new TypeIdPropertyRef();

    private TypeIdPropertyRef()
    {
    }

    public override string ToString()
    {
      return "TYPEID";
    }
  }
}
