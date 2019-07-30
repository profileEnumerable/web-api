// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.InternalMappingException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core
{
  [Serializable]
  internal class InternalMappingException : EntityException
  {
    private readonly ErrorLog m_errorLog;

    internal InternalMappingException()
    {
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal InternalMappingException(string message)
      : base(message)
    {
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal InternalMappingException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InternalMappingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    internal InternalMappingException(string message, ErrorLog errorLog)
      : base(message)
    {
      this.m_errorLog = errorLog;
    }

    internal InternalMappingException(string message, ErrorLog.Record record)
      : base(message)
    {
      this.m_errorLog = new ErrorLog();
      this.m_errorLog.AddEntry(record);
    }

    internal ErrorLog ErrorLog
    {
      get
      {
        return this.m_errorLog;
      }
    }
  }
}
