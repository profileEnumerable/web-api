// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.CellLabel
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class CellLabel
  {
    private readonly int m_startLineNumber;
    private readonly int m_startLinePosition;
    private readonly string m_sourceLocation;

    internal CellLabel(CellLabel source)
    {
      this.m_startLineNumber = source.m_startLineNumber;
      this.m_startLinePosition = source.m_startLinePosition;
      this.m_sourceLocation = source.m_sourceLocation;
    }

    internal CellLabel(MappingFragment fragmentInfo)
      : this(fragmentInfo.StartLineNumber, fragmentInfo.StartLinePosition, fragmentInfo.SourceLocation)
    {
    }

    internal CellLabel(int startLineNumber, int startLinePosition, string sourceLocation)
    {
      this.m_startLineNumber = startLineNumber;
      this.m_startLinePosition = startLinePosition;
      this.m_sourceLocation = sourceLocation;
    }

    internal int StartLineNumber
    {
      get
      {
        return this.m_startLineNumber;
      }
    }

    internal int StartLinePosition
    {
      get
      {
        return this.m_startLinePosition;
      }
    }

    internal string SourceLocation
    {
      get
      {
        return this.m_sourceLocation;
      }
    }
  }
}
