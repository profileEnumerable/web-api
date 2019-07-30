// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.EntityParameterCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.EntityClient
{
  /// <summary>
  /// Class representing a parameter collection used in EntityCommand
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
  public sealed class EntityParameterCollection : DbParameterCollection
  {
    private static readonly Type _itemType = typeof (EntityParameter);
    private List<EntityParameter> _items;
    private bool _isDirty;

    /// <summary>
    /// Gets an Integer that contains the number of elements in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <returns>
    /// The number of elements in the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> as an Integer.
    /// </returns>
    public override int Count
    {
      get
      {
        if (this._items == null)
          return 0;
        return this._items.Count;
      }
    }

    private List<EntityParameter> InnerList
    {
      get
      {
        List<EntityParameter> entityParameterList = this._items;
        if (entityParameterList == null)
        {
          entityParameterList = new List<EntityParameter>();
          this._items = entityParameterList;
        }
        return entityParameterList;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// has a fixed size.
    /// </summary>
    /// <returns>
    /// Returns true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> has a fixed size; otherwise false.
    /// </returns>
    public override bool IsFixedSize
    {
      get
      {
        return ((IList) this.InnerList).IsFixedSize;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// is read-only.
    /// </summary>
    /// <returns>
    /// Returns true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> is read only; otherwise false.
    /// </returns>
    public override bool IsReadOnly
    {
      get
      {
        return ((IList) this.InnerList).IsReadOnly;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// is synchronized.
    /// </summary>
    /// <returns>
    /// Returns true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> is synchronized; otherwise false.
    /// </returns>
    public override bool IsSynchronized
    {
      get
      {
        return ((ICollection) this.InnerList).IsSynchronized;
      }
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </returns>
    public override object SyncRoot
    {
      get
      {
        return ((ICollection) this.InnerList).SyncRoot;
      }
    }

    /// <summary>
    /// Adds the specified object to the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />.
    /// </summary>
    /// <returns>
    /// The index of the new <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object.
    /// </returns>
    /// <param name="value">
    /// An <see cref="T:System.Object" />.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int Add(object value)
    {
      this.OnChange();
      Check.NotNull<object>(value, nameof (value));
      EntityParameterCollection.ValidateType(value);
      this.Validate(-1, value);
      this.InnerList.Add((EntityParameter) value);
      return this.Count - 1;
    }

    /// <summary>
    /// Adds an array of values to the end of the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <param name="values">
    /// The <see cref="T:System.Array" /> values to add.
    /// </param>
    public override void AddRange(Array values)
    {
      this.OnChange();
      Check.NotNull<Array>(values, nameof (values));
      foreach (object obj in values)
        EntityParameterCollection.ValidateType(obj);
      foreach (EntityParameter entityParameter in values)
      {
        this.Validate(-1, (object) entityParameter);
        this.InnerList.Add(entityParameter);
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    private int CheckName(string parameterName)
    {
      int num = this.IndexOf(parameterName);
      if (num < 0)
        throw new IndexOutOfRangeException(Strings.EntityParameterCollectionInvalidParameterName((object) parameterName));
      return num;
    }

    /// <summary>
    /// Removes all the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> objects from the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    public override void Clear()
    {
      this.OnChange();
      List<EntityParameter> innerList = this.InnerList;
      if (innerList == null)
        return;
      foreach (EntityParameter entityParameter in innerList)
        entityParameter.ResetParent();
      innerList.Clear();
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is in this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> contains the value; otherwise false.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Object" /> value.
    /// </param>
    public override bool Contains(object value)
    {
      return -1 != this.IndexOf(value);
    }

    /// <summary>
    /// Copies all the elements of the current <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> to the specified one-dimensional
    /// <see cref="T:System.Array" />
    /// starting at the specified destination <see cref="T:System.Array" /> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the current
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </param>
    /// <param name="index">
    /// A 32-bit integer that represents the index in the <see cref="T:System.Array" /> at which copying starts.
    /// </param>
    public override void CopyTo(Array array, int index)
    {
      ((ICollection) this.InnerList).CopyTo(array, index);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> for the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </returns>
    public override IEnumerator GetEnumerator()
    {
      return ((IEnumerable) this.InnerList).GetEnumerator();
    }

    /// <inhertidoc />
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented")]
    protected override DbParameter GetParameter(int index)
    {
      this.RangeCheck(index);
      return (DbParameter) this.InnerList[index];
    }

    /// <inhertidoc />
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented")]
    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented")]
    protected override DbParameter GetParameter(string parameterName)
    {
      int index = this.IndexOf(parameterName);
      if (index < 0)
        throw new IndexOutOfRangeException(Strings.EntityParameterCollectionInvalidParameterName((object) parameterName));
      return (DbParameter) this.InnerList[index];
    }

    private static int IndexOf(IEnumerable items, string parameterName)
    {
      if (items != null)
      {
        int num1 = 0;
        foreach (EntityParameter entityParameter in items)
        {
          if (EntityUtil.SrcCompare(parameterName, entityParameter.ParameterName) == 0)
            return num1;
          ++num1;
        }
        int num2 = 0;
        foreach (EntityParameter entityParameter in items)
        {
          if (EntityUtil.DstCompare(parameterName, entityParameter.ParameterName) == 0)
            return num2;
          ++num2;
        }
      }
      return -1;
    }

    /// <summary>
    /// Gets the location of the specified <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> with the specified name.
    /// </summary>
    /// <returns>
    /// The zero-based location of the specified <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> with the specified case-sensitive name. Returns -1 when the object does not exist in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </returns>
    /// <param name="parameterName">
    /// The case-sensitive name of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> to find.
    /// </param>
    public override int IndexOf(string parameterName)
    {
      return EntityParameterCollection.IndexOf((IEnumerable) this.InnerList, parameterName);
    }

    /// <summary>
    /// Gets the location of the specified <see cref="T:System.Object" /> in the collection.
    /// </summary>
    /// <returns>
    /// The zero-based location of the specified <see cref="T:System.Object" /> that is a
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" />
    /// in the collection. Returns -1 when the object does not exist in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Object" /> to find.
    /// </param>
    public override int IndexOf(object value)
    {
      if (value != null)
      {
        EntityParameterCollection.ValidateType(value);
        List<EntityParameter> innerList = this.InnerList;
        if (innerList != null)
        {
          int count = innerList.Count;
          for (int index = 0; index < count; ++index)
          {
            if (value == innerList[index])
              return index;
          }
        }
      }
      return -1;
    }

    /// <summary>
    /// Inserts an <see cref="T:System.Object" /> into the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">
    /// An <see cref="T:System.Object" /> to be inserted in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </param>
    public override void Insert(int index, object value)
    {
      this.OnChange();
      Check.NotNull<object>(value, nameof (value));
      EntityParameterCollection.ValidateType(value);
      this.Validate(-1, value);
      this.InnerList.Insert(index, (EntityParameter) value);
    }

    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    private void RangeCheck(int index)
    {
      if (index < 0 || this.Count <= index)
        throw new IndexOutOfRangeException(Strings.EntityParameterCollectionInvalidIndex((object) index.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) this.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }

    /// <summary>Removes the specified parameter from the collection.</summary>
    /// <param name="value">
    /// A <see cref="T:System.Object" /> object to remove from the collection.
    /// </param>
    public override void Remove(object value)
    {
      this.OnChange();
      Check.NotNull<object>(value, nameof (value));
      EntityParameterCollection.ValidateType(value);
      int index = this.IndexOf(value);
      if (-1 != index)
        this.RemoveIndex(index);
      else if (this != ((EntityParameter) value).CompareExchangeParent((object) null, (object) this))
        throw new ArgumentException(Strings.EntityParameterCollectionRemoveInvalidObject);
    }

    /// <summary>
    /// Removes the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> from the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object to remove.
    /// </param>
    public override void RemoveAt(int index)
    {
      this.OnChange();
      this.RangeCheck(index);
      this.RemoveIndex(index);
    }

    /// <summary>
    /// Removes the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> from the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// at the specified parameter name.
    /// </summary>
    /// <param name="parameterName">
    /// The name of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> to remove.
    /// </param>
    public override void RemoveAt(string parameterName)
    {
      this.OnChange();
      this.RemoveIndex(this.CheckName(parameterName));
    }

    private void RemoveIndex(int index)
    {
      List<EntityParameter> innerList = this.InnerList;
      EntityParameter entityParameter = innerList[index];
      innerList.RemoveAt(index);
      entityParameter.ResetParent();
    }

    private void Replace(int index, object newValue)
    {
      List<EntityParameter> innerList = this.InnerList;
      EntityParameterCollection.ValidateType(newValue);
      this.Validate(index, newValue);
      EntityParameter entityParameter = innerList[index];
      innerList[index] = (EntityParameter) newValue;
      entityParameter.ResetParent();
    }

    /// <inhertidoc />
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary")]
    protected override void SetParameter(int index, DbParameter value)
    {
      this.OnChange();
      this.RangeCheck(index);
      this.Replace(index, (object) value);
    }

    /// <inhertidoc />
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary")]
    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    protected override void SetParameter(string parameterName, DbParameter value)
    {
      this.OnChange();
      int index = this.IndexOf(parameterName);
      if (index < 0)
        throw new IndexOutOfRangeException(Strings.EntityParameterCollectionInvalidParameterName((object) parameterName));
      this.Replace(index, (object) value);
    }

    private void Validate(int index, object value)
    {
      Check.NotNull<object>(value, nameof (value));
      EntityParameter entityParameter = (EntityParameter) value;
      object obj = entityParameter.CompareExchangeParent((object) this, (object) null);
      if (obj != null)
      {
        if (this != obj)
          throw new ArgumentException(Strings.EntityParameterContainedByAnotherCollection);
        if (index != this.IndexOf(value))
          throw new ArgumentException(Strings.EntityParameterContainedByAnotherCollection);
      }
      if (entityParameter.ParameterName.Length != 0)
        return;
      index = 1;
      string parameterName;
      do
      {
        parameterName = "Parameter" + index.ToString((IFormatProvider) CultureInfo.CurrentCulture);
        ++index;
      }
      while (-1 != this.IndexOf(parameterName));
      entityParameter.ParameterName = parameterName;
    }

    private static void ValidateType(object value)
    {
      Check.NotNull<object>(value, nameof (value));
      if (!EntityParameterCollection._itemType.IsInstanceOfType(value))
        throw new InvalidCastException(Strings.InvalidEntityParameterType((object) value.GetType().Name));
    }

    internal EntityParameterCollection()
    {
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> at the specified index.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> at the specified index.
    /// </returns>
    /// <param name="index">The zero-based index of the parameter to retrieve. </param>
    /// <exception cref="T:System.IndexOutOfRangeException">The specified index does not exist. </exception>
    public EntityParameter this[int index]
    {
      get
      {
        return (EntityParameter) this.GetParameter(index);
      }
      set
      {
        this.SetParameter(index, (DbParameter) value);
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> with the specified name.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> with the specified name.
    /// </returns>
    /// <param name="parameterName">The name of the parameter to retrieve. </param>
    /// <exception cref="T:System.IndexOutOfRangeException">The specified name does not exist. </exception>
    public EntityParameter this[string parameterName]
    {
      get
      {
        return (EntityParameter) this.GetParameter(parameterName);
      }
      set
      {
        this.SetParameter(parameterName, (DbParameter) value);
      }
    }

    internal bool IsDirty
    {
      get
      {
        if (this._isDirty)
          return true;
        foreach (EntityParameter entityParameter in (DbParameterCollection) this)
        {
          if (entityParameter.IsDirty)
            return true;
        }
        return false;
      }
    }

    /// <summary>
    /// Adds the specified <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object to the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> to add to the collection.
    /// </param>
    /// <exception cref="T:System.ArgumentException">
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> specified in the  value  parameter is already added to this or another
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidCastException">
    /// The parameter passed was not a <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">The  value  parameter is null. </exception>
    public EntityParameter Add(EntityParameter value)
    {
      this.Add((object) value);
      return value;
    }

    /// <summary>
    /// Adds a value to the end of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object.
    /// </returns>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value to be added.</param>
    public EntityParameter AddWithValue(string parameterName, object value)
    {
      EntityParameter entityParameter = new EntityParameter();
      entityParameter.ParameterName = parameterName;
      entityParameter.Value = value;
      return this.Add(entityParameter);
    }

    /// <summary>
    /// Adds a <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> to the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// given the parameter name and the data type.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object.
    /// </returns>
    /// <param name="parameterName">The name of the parameter. </param>
    /// <param name="dbType">
    /// One of the <see cref="T:System.Data.DbType" /> values.
    /// </param>
    public EntityParameter Add(string parameterName, DbType dbType)
    {
      return this.Add(new EntityParameter(parameterName, dbType));
    }

    /// <summary>
    /// Adds a <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> to the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// with the parameter name, the data type, and the column length.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object.
    /// </returns>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">
    /// One of the <see cref="T:System.Data.DbType" /> values.
    /// </param>
    /// <param name="size">The column length.</param>
    public EntityParameter Add(string parameterName, DbType dbType, int size)
    {
      return this.Add(new EntityParameter(parameterName, dbType, size));
    }

    /// <summary>
    /// Adds an array of <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> values to the end of the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <param name="values">
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> values to add.
    /// </param>
    public void AddRange(EntityParameter[] values)
    {
      this.AddRange((Array) values);
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> is in this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> contains the value; otherwise false.
    /// </returns>
    /// <param name="parameterName">
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> value.
    /// </param>
    public override bool Contains(string parameterName)
    {
      return this.IndexOf(parameterName) != -1;
    }

    /// <summary>
    /// Copies all the elements of the current <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> to the specified
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// starting at the specified destination index.
    /// </summary>
    /// <param name="array">
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" /> that is the destination of the elements copied from the current
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </param>
    /// <param name="index">
    /// A 32-bit integer that represents the index in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// at which copying starts.
    /// </param>
    public void CopyTo(EntityParameter[] array, int index)
    {
      this.CopyTo((Array) array, index);
    }

    /// <summary>
    /// Gets the location of the specified <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> in the collection.
    /// </summary>
    /// <returns>
    /// The zero-based location of the specified <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> that is a
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" />
    /// in the collection. Returns -1 when the object does not exist in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> to find.
    /// </param>
    public int IndexOf(EntityParameter value)
    {
      return this.IndexOf((object) value);
    }

    /// <summary>
    /// Inserts a <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object into the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">
    /// A <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object to be inserted in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameterCollection" />
    /// .
    /// </param>
    public void Insert(int index, EntityParameter value)
    {
      this.Insert(index, (object) value);
    }

    private void OnChange()
    {
      this._isDirty = true;
    }

    /// <summary>
    /// Removes the specified <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> from the collection.
    /// </summary>
    /// <param name="value">
    /// A <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object to remove from the collection.
    /// </param>
    /// <exception cref="T:System.InvalidCastException">
    /// The parameter is not a <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" />.
    /// </exception>
    /// <exception cref="T:System.SystemException">The parameter does not exist in the collection. </exception>
    public void Remove(EntityParameter value)
    {
      this.Remove((object) value);
    }

    internal void ResetIsDirty()
    {
      this._isDirty = false;
      foreach (EntityParameter entityParameter in (DbParameterCollection) this)
        entityParameter.ResetIsDirty();
    }
  }
}
