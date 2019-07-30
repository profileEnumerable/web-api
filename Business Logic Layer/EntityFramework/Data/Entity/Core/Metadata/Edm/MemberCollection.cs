// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MemberCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class MemberCollection : MetadataCollection<EdmMember>
  {
    private readonly StructuralType _declaringType;

    public MemberCollection(StructuralType declaringType)
      : this(declaringType, (IEnumerable<EdmMember>) null)
    {
    }

    public MemberCollection(StructuralType declaringType, IEnumerable<EdmMember> items)
      : base(items)
    {
      this._declaringType = declaringType;
    }

    public override ReadOnlyCollection<EdmMember> AsReadOnly
    {
      get
      {
        return new ReadOnlyCollection<EdmMember>((IList<EdmMember>) this);
      }
    }

    public override int Count
    {
      get
      {
        return this.GetBaseTypeMemberCount() + base.Count;
      }
    }

    public override EdmMember this[int index]
    {
      get
      {
        int relativeIndex = this.GetRelativeIndex(index);
        if (relativeIndex < 0)
          return ((StructuralType) this._declaringType.BaseType).Members[index];
        return base[relativeIndex];
      }
      set
      {
        int relativeIndex = this.GetRelativeIndex(index);
        if (relativeIndex < 0)
          ((StructuralType) this._declaringType.BaseType).Members.Source[index] = value;
        else
          base[relativeIndex] = value;
      }
    }

    public override void Add(EdmMember member)
    {
      this.ValidateMemberForAdd(member, nameof (member));
      base.Add(member);
      member.ChangeDeclaringTypeWithoutCollectionFixup(this._declaringType);
    }

    public override bool ContainsIdentity(string identity)
    {
      if (base.ContainsIdentity(identity))
        return true;
      EdmType baseType = this._declaringType.BaseType;
      return baseType != null && ((StructuralType) baseType).Members.Contains(identity);
    }

    public override int IndexOf(EdmMember item)
    {
      int num = base.IndexOf(item);
      if (num != -1)
        return num + this.GetBaseTypeMemberCount();
      StructuralType baseType = this._declaringType.BaseType as StructuralType;
      if (baseType != null)
        return baseType.Members.IndexOf(item);
      return -1;
    }

    public override void CopyTo(EdmMember[] array, int arrayIndex)
    {
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (arrayIndex));
      int baseTypeMemberCount = this.GetBaseTypeMemberCount();
      if (base.Count + baseTypeMemberCount > array.Length - arrayIndex)
        throw new ArgumentOutOfRangeException(nameof (arrayIndex));
      if (baseTypeMemberCount > 0)
        ((StructuralType) this._declaringType.BaseType).Members.CopyTo(array, arrayIndex);
      base.CopyTo(array, arrayIndex + baseTypeMemberCount);
    }

    public override bool TryGetValue(string identity, bool ignoreCase, out EdmMember item)
    {
      if (!base.TryGetValue(identity, ignoreCase, out item))
        ((StructuralType) this._declaringType.BaseType)?.Members.TryGetValue(identity, ignoreCase, out item);
      return item != null;
    }

    internal ReadOnlyMetadataCollection<T> GetDeclaredOnlyMembers<T>() where T : EdmMember
    {
      MetadataCollection<T> collection = new MetadataCollection<T>();
      for (int index = 0; index < base.Count; ++index)
      {
        T obj = base[index] as T;
        if ((object) obj != null)
          collection.Add(obj);
      }
      return new ReadOnlyMetadataCollection<T>(collection);
    }

    private int GetBaseTypeMemberCount()
    {
      StructuralType baseType = this._declaringType.BaseType as StructuralType;
      if (baseType != null)
        return baseType.Members.Count;
      return 0;
    }

    private int GetRelativeIndex(int index)
    {
      int baseTypeMemberCount = this.GetBaseTypeMemberCount();
      int count = base.Count;
      if (index < 0 || index >= baseTypeMemberCount + count)
        throw new ArgumentOutOfRangeException(nameof (index));
      return index - baseTypeMemberCount;
    }

    private void ValidateMemberForAdd(EdmMember member, string argumentName)
    {
      Check.NotNull<EdmMember>(member, argumentName);
      this._declaringType.ValidateMemberForAdd(member);
    }
  }
}
