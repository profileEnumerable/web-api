// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.StringHashBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Entity.Core.Mapping
{
  internal class StringHashBuilder
  {
    private readonly List<string> _strings = new List<string>();
    private const string NewLine = "\n";
    private readonly HashAlgorithm _hashAlgorithm;
    private int _totalLength;
    private byte[] _cachedBuffer;

    internal StringHashBuilder(HashAlgorithm hashAlgorithm)
    {
      this._hashAlgorithm = hashAlgorithm;
    }

    internal StringHashBuilder(HashAlgorithm hashAlgorithm, int startingBufferSize)
      : this(hashAlgorithm)
    {
      this._cachedBuffer = new byte[startingBufferSize];
    }

    internal int CharCount
    {
      get
      {
        return this._totalLength;
      }
    }

    internal virtual void Append(string s)
    {
      this.InternalAppend(s);
    }

    internal virtual void AppendLine(string s)
    {
      this.InternalAppend(s);
      this.InternalAppend("\n");
    }

    private void InternalAppend(string s)
    {
      if (s.Length == 0)
        return;
      this._strings.Add(s);
      this._totalLength += s.Length;
    }

    internal string ComputeHash()
    {
      int byteCount = this.GetByteCount();
      if (this._cachedBuffer == null)
        this._cachedBuffer = new byte[byteCount];
      else if (this._cachedBuffer.Length < byteCount)
        this._cachedBuffer = new byte[Math.Max(this._cachedBuffer.Length + this._cachedBuffer.Length / 2, byteCount)];
      int byteIndex = 0;
      foreach (string s in this._strings)
        byteIndex += Encoding.Unicode.GetBytes(s, 0, s.Length, this._cachedBuffer, byteIndex);
      return StringHashBuilder.ConvertHashToString(this._hashAlgorithm.ComputeHash(this._cachedBuffer, 0, byteCount));
    }

    internal void Clear()
    {
      this._strings.Clear();
      this._totalLength = 0;
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      this._strings.Each<string, StringBuilder>((Func<string, StringBuilder>) (s => builder.Append(s)));
      return builder.ToString();
    }

    private int GetByteCount()
    {
      int num = 0;
      foreach (string s in this._strings)
        num += Encoding.Unicode.GetByteCount(s);
      return num;
    }

    private static string ConvertHashToString(byte[] hash)
    {
      StringBuilder stringBuilder = new StringBuilder(hash.Length * 2);
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture));
      return stringBuilder.ToString();
    }

    public static string ComputeHash(HashAlgorithm hashAlgorithm, string source)
    {
      StringHashBuilder stringHashBuilder = new StringHashBuilder(hashAlgorithm);
      stringHashBuilder.Append(source);
      return stringHashBuilder.ComputeHash();
    }
  }
}
