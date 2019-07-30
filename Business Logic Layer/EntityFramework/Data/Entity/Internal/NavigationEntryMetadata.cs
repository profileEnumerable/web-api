// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.NavigationEntryMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Internal
{
  internal class NavigationEntryMetadata : MemberEntryMetadata
  {
    private readonly bool _isCollection;

    public NavigationEntryMetadata(
      Type declaringType,
      Type propertyType,
      string propertyName,
      bool isCollection)
      : base(declaringType, propertyType, propertyName)
    {
      this._isCollection = isCollection;
    }

    public override MemberEntryType MemberEntryType
    {
      get
      {
        return !this._isCollection ? MemberEntryType.ReferenceNavigationProperty : MemberEntryType.CollectionNavigationProperty;
      }
    }

    public override Type MemberType
    {
      get
      {
        if (!this._isCollection)
          return this.ElementType;
        return DbHelpers.CollectionType(this.ElementType);
      }
    }

    public override InternalMemberEntry CreateMemberEntry(
      InternalEntityEntry internalEntityEntry,
      InternalPropertyEntry parentPropertyEntry)
    {
      if (!this._isCollection)
        return (InternalMemberEntry) new InternalReferenceEntry(internalEntityEntry, this);
      return (InternalMemberEntry) new InternalCollectionEntry(internalEntityEntry, this);
    }
  }
}
