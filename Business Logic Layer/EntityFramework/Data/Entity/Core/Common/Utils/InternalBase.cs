// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.InternalBase
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Text;

namespace System.Data.Entity.Core.Common.Utils
{
  internal abstract class InternalBase
  {
    internal abstract void ToCompactString(StringBuilder builder);

    internal virtual void ToFullString(StringBuilder builder)
    {
      this.ToCompactString(builder);
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      this.ToCompactString(builder);
      return builder.ToString();
    }

    internal virtual string ToFullString()
    {
      StringBuilder builder = new StringBuilder();
      this.ToFullString(builder);
      return builder.ToString();
    }
  }
}
