// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectParameterCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Text;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// This class represents a collection of query parameters at the object layer.
  /// </summary>
  public class ObjectParameterCollection : ICollection<ObjectParameter>, IEnumerable<ObjectParameter>, IEnumerable
  {
    private bool _locked;
    private readonly List<ObjectParameter> _parameters;
    private readonly ClrPerspective _perspective;
    private string _cacheKey;

    internal ObjectParameterCollection(ClrPerspective perspective)
    {
      this._perspective = perspective;
      this._parameters = new List<ObjectParameter>();
    }

    /// <summary>Gets the number of parameters currently in the collection.</summary>
    /// <returns>
    /// The number of <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> objects that are currently in the collection.
    /// </returns>
    public int Count
    {
      get
      {
        return this._parameters.Count;
      }
    }

    bool ICollection<ObjectParameter>.IsReadOnly
    {
      get
      {
        return this._locked;
      }
    }

    /// <summary>Provides an indexer that allows callers to retrieve parameters by name.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> instance.
    /// </returns>
    /// <param name="name">The name of the parameter to find. This name should not include the "@" parameter marker that is used in the Entity SQL statements, only the actual name.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">No parameter with the specified name is found in the collection.</exception>
    public ObjectParameter this[string name]
    {
      get
      {
        int index = this.IndexOf(name);
        if (index == -1)
          throw new ArgumentOutOfRangeException(nameof (name), Strings.ObjectParameterCollection_ParameterNameNotFound((object) name));
        return this._parameters[index];
      }
    }

    /// <summary>
    /// Adds the specified <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> to the collection.
    /// </summary>
    /// <param name="item">The parameter to add to the collection.</param>
    /// <exception cref="T:System.ArgumentNullException">The  parameter  argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// The  parameter  argument already exists in the collection. This behavior differs from that of most collections that allow duplicate entries. -or-Another parameter with the same name as the  parameter  argument already exists in the collection. Note that the lookup is case-insensitive. This behavior differs from that of most collections, and is more like that of a
    /// <see cref="T:System.Collections.Generic.Dictionary" />
    /// .
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The type of the  parameter  is not valid.</exception>
    public void Add(ObjectParameter item)
    {
      Check.NotNull<ObjectParameter>(item, nameof (item));
      this.CheckUnlocked();
      if (this.Contains(item))
        throw new ArgumentException(Strings.ObjectParameterCollection_ParameterAlreadyExists((object) item.Name), nameof (item));
      if (this.Contains(item.Name))
        throw new ArgumentException(Strings.ObjectParameterCollection_DuplicateParameterName((object) item.Name), nameof (item));
      if (!item.ValidateParameterType(this._perspective))
        throw new ArgumentOutOfRangeException(nameof (item), Strings.ObjectParameter_InvalidParameterType((object) item.ParameterType.FullName));
      this._parameters.Add(item);
      this._cacheKey = (string) null;
    }

    /// <summary>
    /// Deletes all <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> instances from the collection.
    /// </summary>
    public void Clear()
    {
      this.CheckUnlocked();
      this._parameters.Clear();
      this._cacheKey = (string) null;
    }

    /// <summary>
    /// Checks for the existence of a specified <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> in the collection by reference.
    /// </summary>
    /// <returns>Returns true if the parameter object was found in the collection; otherwise, false.  </returns>
    /// <param name="item">
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> to find in the collection.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The  parameter  argument is null.</exception>
    public bool Contains(ObjectParameter item)
    {
      Check.NotNull<ObjectParameter>(item, nameof (item));
      return this._parameters.Contains(item);
    }

    /// <summary>
    /// Determines whether an <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> with the specified name is in the collection.
    /// </summary>
    /// <returns>Returns true if a parameter with the specified name was found in the collection; otherwise, false.</returns>
    /// <param name="name">The name of the parameter to look for in the collection. This name should not include the "@" parameter marker that is used in the Entity SQL statements, only the actual name.</param>
    /// <exception cref="T:System.ArgumentNullException">The  name  parameter is null.</exception>
    public bool Contains(string name)
    {
      Check.NotNull<string>(name, nameof (name));
      return this.IndexOf(name) != -1;
    }

    /// <summary>Allows the parameters in the collection to be copied into a supplied array, starting with the object at the specified index.</summary>
    /// <param name="array">The array into which to copy the parameters.</param>
    /// <param name="arrayIndex">The index in the array at which to start copying the parameters.</param>
    public void CopyTo(ObjectParameter[] array, int arrayIndex)
    {
      this._parameters.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes an instance of an <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> from the collection by reference if it exists in the collection.
    /// </summary>
    /// <returns>Returns true if the parameter object was found and removed from the collection; otherwise, false. </returns>
    /// <param name="item">An object to remove from the collection.</param>
    /// <exception cref="T:System.ArgumentNullException">The  parameter  argument is null.</exception>
    public bool Remove(ObjectParameter item)
    {
      Check.NotNull<ObjectParameter>(item, nameof (item));
      this.CheckUnlocked();
      bool flag = this._parameters.Remove(item);
      if (flag)
        this._cacheKey = (string) null;
      return flag;
    }

    /// <summary>
    /// These methods return enumerator instances, which allow the collection to
    /// be iterated through and traversed.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> object that can be used to iterate through the collection.</returns>
    public virtual IEnumerator<ObjectParameter> GetEnumerator()
    {
      return ((IEnumerable<ObjectParameter>) this._parameters).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) this._parameters).GetEnumerator();
    }

    internal string GetCacheKey()
    {
      if (this._cacheKey == null && this._parameters.Count > 0)
      {
        if (1 == this._parameters.Count)
        {
          ObjectParameter parameter = this._parameters[0];
          this._cacheKey = "@@1" + parameter.Name + ":" + parameter.ParameterType.FullName;
        }
        else
        {
          StringBuilder stringBuilder = new StringBuilder(this._parameters.Count * 20);
          stringBuilder.Append("@@");
          stringBuilder.Append(this._parameters.Count);
          for (int index = 0; index < this._parameters.Count; ++index)
          {
            if (index > 0)
              stringBuilder.Append(";");
            ObjectParameter parameter = this._parameters[index];
            stringBuilder.Append(parameter.Name);
            stringBuilder.Append(":");
            stringBuilder.Append(parameter.ParameterType.FullName);
          }
          this._cacheKey = stringBuilder.ToString();
        }
      }
      return this._cacheKey;
    }

    internal void SetReadOnly(bool isReadOnly)
    {
      this._locked = isReadOnly;
    }

    internal static ObjectParameterCollection DeepCopy(
      ObjectParameterCollection copyParams)
    {
      if (copyParams == null)
        return (ObjectParameterCollection) null;
      ObjectParameterCollection parameterCollection = new ObjectParameterCollection(copyParams._perspective);
      foreach (ObjectParameter copyParam in copyParams)
        parameterCollection.Add(copyParam.ShallowCopy());
      return parameterCollection;
    }

    private int IndexOf(string name)
    {
      int num = 0;
      foreach (ObjectParameter parameter in this._parameters)
      {
        if (string.Compare(name, parameter.Name, StringComparison.OrdinalIgnoreCase) == 0)
          return num;
        ++num;
      }
      return -1;
    }

    private void CheckUnlocked()
    {
      if (this._locked)
        throw new InvalidOperationException(Strings.ObjectParameterCollection_ParametersLocked);
    }
  }
}
