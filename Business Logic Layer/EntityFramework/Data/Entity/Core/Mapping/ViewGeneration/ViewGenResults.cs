// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.ViewGenResults
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class ViewGenResults : InternalBase
  {
    private readonly KeyToListMap<EntitySetBase, GeneratedView> m_views;
    private readonly ErrorLog m_errorLog;

    internal ViewGenResults()
    {
      this.m_views = new KeyToListMap<EntitySetBase, GeneratedView>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      this.m_errorLog = new ErrorLog();
    }

    internal KeyToListMap<EntitySetBase, GeneratedView> Views
    {
      get
      {
        return this.m_views;
      }
    }

    internal IEnumerable<EdmSchemaError> Errors
    {
      get
      {
        return this.m_errorLog.Errors;
      }
    }

    internal bool HasErrors
    {
      get
      {
        return this.m_errorLog.Count > 0;
      }
    }

    internal void AddErrors(ErrorLog errorLog)
    {
      this.m_errorLog.Merge(errorLog);
    }

    internal string ErrorsToString()
    {
      return this.m_errorLog.ToString();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append(this.m_errorLog.Count);
      builder.Append(" ");
      this.m_errorLog.ToCompactString(builder);
    }
  }
}
