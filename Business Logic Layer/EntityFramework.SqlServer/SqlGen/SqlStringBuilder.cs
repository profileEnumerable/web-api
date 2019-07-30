// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SqlStringBuilder
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class SqlStringBuilder
  {
    private readonly StringBuilder _sql;

    public SqlStringBuilder()
    {
      this._sql = new StringBuilder();
    }

    public SqlStringBuilder(int capacity)
    {
      this._sql = new StringBuilder(capacity);
    }

    public bool UpperCaseKeywords { get; set; }

    internal StringBuilder InnerBuilder
    {
      get
      {
        return this._sql;
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Keywords are known safe for lowercasing")]
    public SqlStringBuilder AppendKeyword(string keyword)
    {
      this._sql.Append(this.UpperCaseKeywords ? keyword.ToUpperInvariant() : keyword.ToLowerInvariant());
      return this;
    }

    public SqlStringBuilder AppendLine()
    {
      this._sql.AppendLine();
      return this;
    }

    public SqlStringBuilder AppendLine(string s)
    {
      this._sql.AppendLine(s);
      return this;
    }

    public SqlStringBuilder Append(string s)
    {
      this._sql.Append(s);
      return this;
    }

    public int Length
    {
      get
      {
        return this._sql.Length;
      }
    }

    public override string ToString()
    {
      return this._sql.ToString();
    }
  }
}
