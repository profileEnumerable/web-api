// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.NameValuePair
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.EntityClient
{
  internal sealed class NameValuePair
  {
    private NameValuePair _next;

    internal NameValuePair Next
    {
      get
      {
        return this._next;
      }
      set
      {
        if (this._next != null || value == null)
          throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1014));
        this._next = value;
      }
    }
  }
}
