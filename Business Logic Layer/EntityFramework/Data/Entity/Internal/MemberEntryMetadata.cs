// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.MemberEntryMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Internal
{
  internal abstract class MemberEntryMetadata
  {
    private readonly Type _declaringType;
    private readonly Type _elementType;
    private readonly string _memberName;

    protected MemberEntryMetadata(Type declaringType, Type elementType, string memberName)
    {
      this._declaringType = declaringType;
      this._elementType = elementType;
      this._memberName = memberName;
    }

    public abstract InternalMemberEntry CreateMemberEntry(
      InternalEntityEntry internalEntityEntry,
      InternalPropertyEntry parentPropertyEntry);

    public abstract MemberEntryType MemberEntryType { get; }

    public string MemberName
    {
      get
      {
        return this._memberName;
      }
    }

    public Type DeclaringType
    {
      get
      {
        return this._declaringType;
      }
    }

    public Type ElementType
    {
      get
      {
        return this._elementType;
      }
    }

    public abstract Type MemberType { get; }
  }
}
