// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Base EdmType class for all the model types</summary>
  public abstract class EdmType : GlobalItem, INamedDataModelItem
  {
    private CollectionType _collectionType;
    private string _name;
    private string _namespace;
    private EdmType _baseType;

    internal static IEnumerable<T> SafeTraverseHierarchy<T>(T startFrom) where T : EdmType
    {
      HashSet<T> visitedTypes = new HashSet<T>();
      for (T thisType = startFrom; (object) thisType != null && !visitedTypes.Contains(thisType); thisType = thisType.BaseType as T)
      {
        visitedTypes.Add(thisType);
        yield return thisType;
      }
    }

    internal EdmType()
    {
    }

    internal EdmType(string name, string namespaceName, DataSpace dataSpace)
    {
      Check.NotNull<string>(name, nameof (name));
      Check.NotNull<string>(namespaceName, nameof (namespaceName));
      EdmType.Initialize(this, name, namespaceName, dataSpace, false, (EdmType) null);
    }

    internal string CacheIdentity { get; private set; }

    string INamedDataModelItem.Identity
    {
      get
      {
        return this.Identity;
      }
    }

    internal override string Identity
    {
      get
      {
        if (this.CacheIdentity == null)
        {
          StringBuilder builder = new StringBuilder(50);
          this.BuildIdentity(builder);
          this.CacheIdentity = builder.ToString();
        }
        return this.CacheIdentity;
      }
    }

    /// <summary>Gets the name of this type.</summary>
    /// <returns>The name of this type.</returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public virtual string Name
    {
      get
      {
        return this._name;
      }
      internal set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._name = value;
      }
    }

    /// <summary>Gets the namespace of this type.</summary>
    /// <returns>The namespace of this type.</returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public virtual string NamespaceName
    {
      get
      {
        return this._namespace;
      }
      internal set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._namespace = value;
      }
    }

    /// <summary>Gets a value indicating whether this type is abstract or not. </summary>
    /// <returns>true if this type is abstract; otherwise, false. </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called on instance that is in ReadOnly state</exception>
    [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
    public bool Abstract
    {
      get
      {
        return this.GetFlag(MetadataItem.MetadataFlags.IsAbstract);
      }
      internal set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this.SetFlag(MetadataItem.MetadataFlags.IsAbstract, value);
      }
    }

    /// <summary>Gets the base type of this type.</summary>
    /// <returns>The base type of this type.</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called on instance that is in ReadOnly state</exception>
    /// <exception cref="T:System.ArgumentException">Thrown if the value passed in for setter will create a loop in the inheritance chain</exception>
    [MetadataProperty(BuiltInTypeKind.EdmType, false)]
    public virtual EdmType BaseType
    {
      get
      {
        return this._baseType;
      }
      internal set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this.CheckBaseType(value);
        this._baseType = value;
      }
    }

    private void CheckBaseType(EdmType baseType)
    {
      for (EdmType edmType = baseType; edmType != null; edmType = edmType.BaseType)
      {
        if (edmType == this)
          throw new ArgumentException(Strings.CannotSetBaseTypeCyclicInheritance((object) baseType.Name, (object) this.Name));
      }
      if (baseType != null && Helper.IsEntityTypeBase(this) && (((EntityTypeBase) baseType).KeyMembers.Count != 0 && ((EntityTypeBase) this).KeyMembers.Count != 0))
        throw new ArgumentException(Strings.CannotDefineKeysOnBothBaseAndDerivedTypes);
    }

    /// <summary>Gets the full name of this type.</summary>
    /// <returns>The full name of this type. </returns>
    public virtual string FullName
    {
      get
      {
        return this.Identity;
      }
    }

    internal virtual Type ClrType
    {
      get
      {
        return (Type) null;
      }
    }

    internal override void BuildIdentity(StringBuilder builder)
    {
      if (this.CacheIdentity != null)
        builder.Append(this.CacheIdentity);
      else
        builder.Append(EdmType.CreateEdmTypeIdentity(this.NamespaceName, this.Name));
    }

    internal static string CreateEdmTypeIdentity(string namespaceName, string name)
    {
      string str = string.Empty;
      if (!string.IsNullOrEmpty(namespaceName))
        str = namespaceName + ".";
      return str + name;
    }

    internal static void Initialize(
      EdmType type,
      string name,
      string namespaceName,
      DataSpace dataSpace,
      bool isAbstract,
      EdmType baseType)
    {
      type._baseType = baseType;
      type._name = name;
      type._namespace = namespaceName;
      type.DataSpace = dataSpace;
      type.Abstract = isAbstract;
    }

    /// <summary>Returns the full name of this type.</summary>
    /// <returns>The full name of this type. </returns>
    public override string ToString()
    {
      return this.FullName;
    }

    /// <summary>
    /// Returns an instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.CollectionType" /> whose element type is this type.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.CollectionType" /> object whose element type is this type.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public CollectionType GetCollectionType()
    {
      if (this._collectionType == null)
        Interlocked.CompareExchange<CollectionType>(ref this._collectionType, new CollectionType(this), (CollectionType) null);
      return this._collectionType;
    }

    internal virtual bool IsSubtypeOf(EdmType otherType)
    {
      return Helper.IsSubtypeOf(this, otherType);
    }

    internal virtual bool IsBaseTypeOf(EdmType otherType)
    {
      if (otherType == null)
        return false;
      return otherType.IsSubtypeOf(this);
    }

    internal virtual bool IsAssignableFrom(EdmType otherType)
    {
      return Helper.IsAssignableFrom(this, otherType);
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
      this.BaseType?.SetReadOnly();
    }

    internal virtual IEnumerable<FacetDescription> GetAssociatedFacetDescriptions()
    {
      return (IEnumerable<FacetDescription>) MetadataItem.GetGeneralFacetDescriptions();
    }
  }
}
