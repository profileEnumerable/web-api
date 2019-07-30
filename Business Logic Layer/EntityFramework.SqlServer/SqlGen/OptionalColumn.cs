// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.OptionalColumn
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal sealed class OptionalColumn
  {
    private readonly SqlBuilder m_builder = new SqlBuilder();
    private readonly SymbolUsageManager m_usageManager;
    private readonly Symbol m_symbol;

    internal void Append(object s)
    {
      this.m_builder.Append(s);
    }

    internal void MarkAsUsed()
    {
      this.m_usageManager.MarkAsUsed(this.m_symbol);
    }

    internal OptionalColumn(SymbolUsageManager usageManager, Symbol symbol)
    {
      this.m_usageManager = usageManager;
      this.m_symbol = symbol;
    }

    public bool WriteSqlIfUsed(SqlWriter writer, SqlGenerator sqlGenerator, string separator)
    {
      if (!this.m_usageManager.IsUsed(this.m_symbol))
        return false;
      writer.Write(separator);
      this.m_builder.WriteSql(writer, sqlGenerator);
      return true;
    }
  }
}
