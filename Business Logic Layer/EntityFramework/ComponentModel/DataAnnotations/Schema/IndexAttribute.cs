// Decompiled with JetBrains decompiler
// Type: System.ComponentModel.DataAnnotations.Schema.IndexAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.ComponentModel.DataAnnotations.Schema
{
  /// <summary>
  /// When this attribute is placed on a property it indicates that the database column to which the
  /// property is mapped has an index.
  /// </summary>
  /// <remarks>
  /// This attribute is used by Entity Framework Migrations to create indexes on mapped database columns.
  /// Multi-column indexes are created by using the same index name in multiple attributes. The information
  /// in these attributes is then merged together to specify the actual database index.
  /// </remarks>
  [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
  public class IndexAttribute : Attribute
  {
    private int _order = -1;
    private string _name;
    private bool? _isClustered;
    private bool? _isUnique;

    /// <summary>
    /// Creates a <see cref="T:System.ComponentModel.DataAnnotations.Schema.IndexAttribute" /> instance for an index that will be named by convention and
    /// has no column order, clustering, or uniqueness specified.
    /// </summary>
    public IndexAttribute()
    {
    }

    /// <summary>
    /// Creates a <see cref="T:System.ComponentModel.DataAnnotations.Schema.IndexAttribute" /> instance for an index with the given name and
    /// has no column order, clustering, or uniqueness specified.
    /// </summary>
    /// <param name="name">The index name.</param>
    public IndexAttribute(string name)
    {
      Check.NotEmpty(name, nameof (name));
      this._name = name;
    }

    /// <summary>
    /// Creates a <see cref="T:System.ComponentModel.DataAnnotations.Schema.IndexAttribute" /> instance for an index with the given name and column order,
    /// but with no clustering or uniqueness specified.
    /// </summary>
    /// <remarks>
    /// Multi-column indexes are created by using the same index name in multiple attributes. The information
    /// in these attributes is then merged together to specify the actual database index.
    /// </remarks>
    /// <param name="name">The index name.</param>
    /// <param name="order">A number which will be used to determine column ordering for multi-column indexes.</param>
    public IndexAttribute(string name, int order)
    {
      Check.NotEmpty(name, nameof (name));
      if (order < 0)
        throw new ArgumentOutOfRangeException(nameof (order));
      this._name = name;
      this._order = order;
    }

    private IndexAttribute(string name, int order, bool? isClustered, bool? isUnique)
    {
      this._name = name;
      this._order = order;
      this._isClustered = isClustered;
      this._isUnique = isUnique;
    }

    /// <summary>The index name.</summary>
    /// <remarks>
    /// Multi-column indexes are created by using the same index name in multiple attributes. The information
    /// in these attributes is then merged together to specify the actual database index.
    /// </remarks>
    public virtual string Name
    {
      get
      {
        return this._name;
      }
      internal set
      {
        this._name = value;
      }
    }

    /// <summary>
    /// A number which will be used to determine column ordering for multi-column indexes. This will be -1 if no
    /// column order has been specified.
    /// </summary>
    /// <remarks>
    /// Multi-column indexes are created by using the same index name in multiple attributes. The information
    /// in these attributes is then merged together to specify the actual database index.
    /// </remarks>
    public virtual int Order
    {
      get
      {
        return this._order;
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException(nameof (value));
        this._order = value;
      }
    }

    /// <summary>
    /// Set this property to true to define a clustered index. Set this property to false to define a
    /// non-clustered index.
    /// </summary>
    /// <remarks>
    /// The value of this property is only relevant if <see cref="P:System.ComponentModel.DataAnnotations.Schema.IndexAttribute.IsClusteredConfigured" /> returns true.
    /// If <see cref="P:System.ComponentModel.DataAnnotations.Schema.IndexAttribute.IsClusteredConfigured" /> returns false, then the value of this property is meaningless.
    /// </remarks>
    public virtual bool IsClustered
    {
      get
      {
        if (this._isClustered.HasValue)
          return this._isClustered.Value;
        return false;
      }
      set
      {
        this._isClustered = new bool?(value);
      }
    }

    /// <summary>
    /// Returns true if <see cref="P:System.ComponentModel.DataAnnotations.Schema.IndexAttribute.IsClustered" /> has been set to a value.
    /// </summary>
    public virtual bool IsClusteredConfigured
    {
      get
      {
        return this._isClustered.HasValue;
      }
    }

    /// <summary>
    /// Set this property to true to define a unique index. Set this property to false to define a
    /// non-unique index.
    /// </summary>
    /// <remarks>
    /// The value of this property is only relevant if <see cref="P:System.ComponentModel.DataAnnotations.Schema.IndexAttribute.IsUniqueConfigured" /> returns true.
    /// If <see cref="P:System.ComponentModel.DataAnnotations.Schema.IndexAttribute.IsUniqueConfigured" /> returns false, then the value of this property is meaningless.
    /// </remarks>
    public virtual bool IsUnique
    {
      get
      {
        if (this._isUnique.HasValue)
          return this._isUnique.Value;
        return false;
      }
      set
      {
        this._isUnique = new bool?(value);
      }
    }

    /// <summary>
    /// Returns true if <see cref="P:System.ComponentModel.DataAnnotations.Schema.IndexAttribute.IsUnique" /> has been set to a value.
    /// </summary>
    public virtual bool IsUniqueConfigured
    {
      get
      {
        return this._isUnique.HasValue;
      }
    }

    /// <summary>
    /// Returns a different ID for each object instance such that type descriptors won't
    /// attempt to combine all IndexAttribute instances into a single instance.
    /// </summary>
    public override object TypeId
    {
      get
      {
        return (object) RuntimeHelpers.GetHashCode((object) this);
      }
    }

    /// <summary>
    /// Returns true if this attribute specifies the same name and configuration as the given attribute.
    /// </summary>
    /// <param name="other">The attribute to compare.</param>
    /// <returns>True if the other object is equal to this object; otherwise false.</returns>
    protected virtual bool Equals(IndexAttribute other)
    {
      if (this._name == other._name && this._order == other._order && this._isClustered.Equals((object) other._isClustered))
        return this._isUnique.Equals((object) other._isUnique);
      return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return IndexAnnotationSerializer.SerializeIndexAttribute(this);
    }

    /// <summary>
    /// Returns true if this attribute specifies the same name and configuration as the given attribute.
    /// </summary>
    /// <param name="obj">The attribute to compare.</param>
    /// <returns>True if the other object is equal to this object; otherwise false.</returns>
    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      return this.Equals((IndexAttribute) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return (((base.GetHashCode() * 397 ^ (this._name != null ? this._name.GetHashCode() : 0)) * 397 ^ this._order) * 397 ^ this._isClustered.GetHashCode()) * 397 ^ this._isUnique.GetHashCode();
    }
  }
}
