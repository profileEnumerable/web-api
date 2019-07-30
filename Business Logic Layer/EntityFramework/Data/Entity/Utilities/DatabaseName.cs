// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DatabaseName
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Data.Entity.Utilities
{
  internal class DatabaseName
  {
    private static readonly Regex _partExtractor = new Regex(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "^{0}(?:\\.{1})?$", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(?:(?:\\[(?<part{0}>(?:(?:\\]\\])|[^\\]])+)\\])|(?<part{0}>[^\\.\\[\\]]+))", (object) 1), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(?:(?:\\[(?<part{0}>(?:(?:\\]\\])|[^\\]])+)\\])|(?<part{0}>[^\\.\\[\\]]+))", (object) 2)), RegexOptions.Compiled);
    private const string NamePartRegex = "(?:(?:\\[(?<part{0}>(?:(?:\\]\\])|[^\\]])+)\\])|(?<part{0}>[^\\.\\[\\]]+))";
    private readonly string _name;
    private readonly string _schema;

    public static DatabaseName Parse(string name)
    {
      Match match = DatabaseName._partExtractor.Match(name.Trim());
      if (!match.Success)
        throw Error.InvalidDatabaseName((object) name);
      string str = match.Groups["part1"].Value.Replace("]]", "]");
      string name1 = match.Groups["part2"].Value.Replace("]]", "]");
      if (string.IsNullOrWhiteSpace(name1))
        return new DatabaseName(str);
      return new DatabaseName(name1, str);
    }

    public DatabaseName(string name)
      : this(name, (string) null)
    {
    }

    public DatabaseName(string name, string schema)
    {
      this._name = name;
      this._schema = schema;
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string Schema
    {
      get
      {
        return this._schema;
      }
    }

    public override string ToString()
    {
      string str = DatabaseName.Escape(this._name);
      if (this._schema != null)
        str = DatabaseName.Escape(this._schema) + "." + str;
      return str;
    }

    private static string Escape(string name)
    {
      if (name.IndexOfAny(new char[3]{ ']', '[', '.' }) == -1)
        return name;
      return "[" + name.Replace("]", "]]") + "]";
    }

    public bool Equals(DatabaseName other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if (string.Equals(other._name, this._name, StringComparison.Ordinal))
        return string.Equals(other._schema, this._schema, StringComparison.Ordinal);
      return false;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() == typeof (DatabaseName))
        return this.Equals((DatabaseName) obj);
      return false;
    }

    public override int GetHashCode()
    {
      return this._name.GetHashCode() * 397 ^ (this._schema != null ? this._schema.GetHashCode() : 0);
    }
  }
}
