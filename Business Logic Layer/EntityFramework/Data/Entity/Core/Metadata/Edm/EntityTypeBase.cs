// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EntityTypeBase
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represents the Entity Type</summary>
  public abstract class EntityTypeBase : StructuralType
  {
    private readonly object _keyPropertiesSync = new object();
    private readonly ReadOnlyMetadataCollection<EdmMember> _keyMembers;
    private ReadOnlyMetadataCollection<EdmProperty> _keyProperties;
    private string[] _keyMemberNames;

    internal EntityTypeBase(string name, string namespaceName, DataSpace dataSpace)
      : base(name, namespaceName, dataSpace)
    {
      this._keyMembers = new ReadOnlyMetadataCollection<EdmMember>(new MetadataCollection<EdmMember>());
    }

    /// <summary>Gets the list of all the key members for the current entity or relationship type.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> object that represents the list of key members for the current entity or relationship type.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EdmMember, true)]
    public virtual ReadOnlyMetadataCollection<EdmMember> KeyMembers
    {
      get
      {
        if (this.BaseType != null && ((EntityTypeBase) this.BaseType).KeyMembers.Count != 0)
          return ((EntityTypeBase) this.BaseType).KeyMembers;
        return this._keyMembers;
      }
    }

    /// <summary>Gets the list of all the key properties for this entity type.</summary>
    /// <returns>The list of all the key properties for this entity type.</returns>
    public virtual ReadOnlyMetadataCollection<EdmProperty> KeyProperties
    {
      get
      {
        ReadOnlyMetadataCollection<EdmProperty> keyProperties = this._keyProperties;
        if (keyProperties == null)
        {
          lock (this._keyPropertiesSync)
          {
            if (this._keyProperties == null)
            {
              this.KeyMembers.SourceAccessed += new EventHandler(this.KeyMembersSourceAccessedEventHandler);
              this._keyProperties = new ReadOnlyMetadataCollection<EdmProperty>(this.KeyMembers.Cast<EdmProperty>().ToList<EdmProperty>());
            }
            keyProperties = this._keyProperties;
          }
        }
        return keyProperties;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ResetKeyPropertiesCache()
    {
      if (this._keyProperties == null)
        return;
      lock (this._keyPropertiesSync)
      {
        if (this._keyProperties == null)
          return;
        this._keyProperties = (ReadOnlyMetadataCollection<EdmProperty>) null;
        this.KeyMembers.SourceAccessed -= new EventHandler(this.KeyMembersSourceAccessedEventHandler);
      }
    }

    private void KeyMembersSourceAccessedEventHandler(object sender, EventArgs e)
    {
      this.ResetKeyPropertiesCache();
    }

    internal virtual string[] KeyMemberNames
    {
      get
      {
        if (this._keyMemberNames == null)
        {
          string[] strArray = new string[this.KeyMembers.Count];
          for (int index = 0; index < strArray.Length; ++index)
            strArray[index] = this.KeyMembers[index].Name;
          this._keyMemberNames = strArray;
        }
        return this._keyMemberNames;
      }
    }

    /// <summary>
    /// Adds the specified property to the list of keys for the current entity.
    /// </summary>
    /// <param name="member">The property to add.</param>
    /// <exception cref="T:System.ArgumentNullException">if member argument is null</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the EntityType has a base type of another EntityTypeBase. In this case KeyMembers should be added to the base type</exception>
    /// <exception cref="T:System.InvalidOperationException">If the EntityType instance is in ReadOnly state</exception>
    public void AddKeyMember(EdmMember member)
    {
      Check.NotNull<EdmMember>(member, nameof (member));
      Util.ThrowIfReadOnly((MetadataItem) this);
      if (!this.Members.Contains(member))
        this.AddMember(member);
      this._keyMembers.Source.Add(member);
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      this._keyMembers.Source.SetReadOnly();
      base.SetReadOnly();
    }

    internal static void CheckAndAddMembers(IEnumerable<EdmMember> members, EntityType entityType)
    {
      foreach (EdmMember member in members)
      {
        if (member == null)
          throw new ArgumentException(Strings.ADP_CollectionParameterElementIsNull((object) nameof (members)));
        entityType.AddMember(member);
      }
    }

    internal void CheckAndAddKeyMembers(IEnumerable<string> keyMembers)
    {
      foreach (string keyMember in keyMembers)
      {
        if (keyMember == null)
          throw new ArgumentException(Strings.ADP_CollectionParameterElementIsNull((object) nameof (keyMembers)));
        EdmMember member;
        if (!this.Members.TryGetValue(keyMember, false, out member))
          throw new ArgumentException(Strings.InvalidKeyMember((object) keyMember));
        this.AddKeyMember(member);
      }
    }

    /// <summary>Removes the specified key member from the collection.</summary>
    /// <param name="member">The key member to remove.</param>
    public override void RemoveMember(EdmMember member)
    {
      Check.NotNull<EdmMember>(member, nameof (member));
      Util.ThrowIfReadOnly((MetadataItem) this);
      if (this._keyMembers.Contains(member))
        this._keyMembers.Source.Remove(member);
      base.RemoveMember(member);
    }

    internal override void NotifyItemIdentityChanged(EdmMember item, string initialIdentity)
    {
      base.NotifyItemIdentityChanged(item, initialIdentity);
      this._keyMembers.Source.HandleIdentityChange(item, initialIdentity);
    }
  }
}
