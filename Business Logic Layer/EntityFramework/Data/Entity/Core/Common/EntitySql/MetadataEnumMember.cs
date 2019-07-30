// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.MetadataEnumMember
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class MetadataEnumMember : MetadataMember
  {
    internal readonly TypeUsage EnumType;
    internal readonly EnumMember EnumMember;

    internal MetadataEnumMember(string name, TypeUsage enumType, EnumMember enumMember)
      : base(MetadataMemberClass.EnumMember, name)
    {
      this.EnumType = enumType;
      this.EnumMember = enumMember;
    }

    internal override string MetadataMemberClassName
    {
      get
      {
        return MetadataEnumMember.EnumMemberClassName;
      }
    }

    internal static string EnumMemberClassName
    {
      get
      {
        return Strings.LocalizedEnumMember;
      }
    }
  }
}
