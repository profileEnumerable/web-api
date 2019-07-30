// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.LoadMessageLogger
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Text;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class LoadMessageLogger
  {
    private readonly Dictionary<EdmType, StringBuilder> _messages = new Dictionary<EdmType, StringBuilder>();
    private readonly Action<string> _logLoadMessage;

    internal LoadMessageLogger(Action<string> logLoadMessage)
    {
      this._logLoadMessage = logLoadMessage;
    }

    internal virtual void LogLoadMessage(string message, EdmType relatedType)
    {
      if (this._logLoadMessage != null)
        this._logLoadMessage(message);
      this.LogMessagesWithTypeInfo(message, relatedType);
    }

    internal virtual string CreateErrorMessageWithTypeSpecificLoadLogs(
      string errorMessage,
      EdmType relatedType)
    {
      return new StringBuilder(errorMessage).AppendLine(this.GetTypeRelatedLogMessage(relatedType)).ToString();
    }

    private string GetTypeRelatedLogMessage(EdmType relatedType)
    {
      if (this._messages.ContainsKey(relatedType))
        return new StringBuilder().AppendLine().AppendLine(Strings.ExtraInfo).AppendLine(this._messages[relatedType].ToString()).ToString();
      return string.Empty;
    }

    private void LogMessagesWithTypeInfo(string message, EdmType relatedType)
    {
      if (this._messages.ContainsKey(relatedType))
        this._messages[relatedType].AppendLine(message);
      else
        this._messages.Add(relatedType, new StringBuilder(message));
    }
  }
}
