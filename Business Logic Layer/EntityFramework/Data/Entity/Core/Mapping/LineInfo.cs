// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.LineInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Xml;
using System.Xml.XPath;

namespace System.Data.Entity.Core.Mapping
{
  internal sealed class LineInfo : IXmlLineInfo
  {
    internal static readonly LineInfo Empty = new LineInfo();
    private readonly bool m_hasLineInfo;
    private readonly int m_lineNumber;
    private readonly int m_linePosition;

    internal LineInfo(XPathNavigator nav)
      : this((IXmlLineInfo) nav)
    {
    }

    internal LineInfo(IXmlLineInfo lineInfo)
    {
      this.m_hasLineInfo = lineInfo.HasLineInfo();
      this.m_lineNumber = lineInfo.LineNumber;
      this.m_linePosition = lineInfo.LinePosition;
    }

    private LineInfo()
    {
      this.m_hasLineInfo = false;
      this.m_lineNumber = 0;
      this.m_linePosition = 0;
    }

    public int LineNumber
    {
      get
      {
        return this.m_lineNumber;
      }
    }

    public int LinePosition
    {
      get
      {
        return this.m_linePosition;
      }
    }

    public bool HasLineInfo()
    {
      return this.m_hasLineInfo;
    }
  }
}
