// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.SafeLinkCollection`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class SafeLinkCollection<TParent, TChild> : ReadOnlyMetadataCollection<TChild>
    where TParent : class
    where TChild : MetadataItem
  {
    public SafeLinkCollection(
      TParent parent,
      Func<TChild, SafeLink<TParent>> getLink,
      MetadataCollection<TChild> children)
      : base((MetadataCollection<TChild>) SafeLink<TParent>.BindChildren<TChild>(parent, getLink, (IEnumerable<TChild>) children))
    {
    }
  }
}
