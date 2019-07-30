// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.FieldNameLookup
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core
{
  internal sealed class FieldNameLookup
  {
    private readonly Dictionary<string, int> _fieldNameLookup = new Dictionary<string, int>();
    private readonly string[] _fieldNames;

    public FieldNameLookup(ReadOnlyCollection<string> columnNames)
    {
      int count = columnNames.Count;
      this._fieldNames = new string[count];
      for (int index = 0; index < count; ++index)
        this._fieldNames[index] = columnNames[index];
      this.GenerateLookup();
    }

    public FieldNameLookup(IDataRecord reader)
    {
      int fieldCount = reader.FieldCount;
      this._fieldNames = new string[fieldCount];
      for (int i = 0; i < fieldCount; ++i)
        this._fieldNames[i] = reader.GetName(i);
      this.GenerateLookup();
    }

    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    public int GetOrdinal(string fieldName)
    {
      Check.NotNull<string>(fieldName, nameof (fieldName));
      int num = this.IndexOf(fieldName);
      if (num == -1)
        throw new IndexOutOfRangeException(fieldName);
      return num;
    }

    private int IndexOf(string fieldName)
    {
      int num;
      if (!this._fieldNameLookup.TryGetValue(fieldName, out num))
      {
        num = this.LinearIndexOf(fieldName, CompareOptions.IgnoreCase);
        if (num == -1)
          num = this.LinearIndexOf(fieldName, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
      }
      return num;
    }

    private int LinearIndexOf(string fieldName, CompareOptions compareOptions)
    {
      for (int index = 0; index < this._fieldNames.Length; ++index)
      {
        if (CultureInfo.InvariantCulture.CompareInfo.Compare(fieldName, this._fieldNames[index], compareOptions) == 0)
        {
          this._fieldNameLookup[fieldName] = index;
          return index;
        }
      }
      return -1;
    }

    private void GenerateLookup()
    {
      for (int index = this._fieldNames.Length - 1; 0 <= index; --index)
        this._fieldNameLookup[this._fieldNames[index]] = index;
    }
  }
}
