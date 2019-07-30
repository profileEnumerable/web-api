// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ItemCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Class for representing a collection of items.
  /// Most of the implementation for actual maintenance of the collection is
  /// done by MetadataCollection
  /// </summary>
  public abstract class ItemCollection : ReadOnlyMetadataCollection<GlobalItem>
  {
    private readonly DataSpace _space;
    private Dictionary<string, ReadOnlyCollection<EdmFunction>> _functionLookUpTable;
    private Memoizer<Type, ICollection> _itemsCache;
    private int _itemCount;

    internal ItemCollection()
    {
    }

    internal ItemCollection(DataSpace dataspace)
      : base(new MetadataCollection<GlobalItem>())
    {
      this._space = dataspace;
    }

    /// <summary>Gets the data model associated with this item collection. </summary>
    /// <returns>The data model associated with this item collection. </returns>
    public DataSpace DataSpace
    {
      get
      {
        return this._space;
      }
    }

    internal Dictionary<string, ReadOnlyCollection<EdmFunction>> FunctionLookUpTable
    {
      get
      {
        if (this._functionLookUpTable == null)
          Interlocked.CompareExchange<Dictionary<string, ReadOnlyCollection<EdmFunction>>>(ref this._functionLookUpTable, ItemCollection.PopulateFunctionLookUpTable(this), (Dictionary<string, ReadOnlyCollection<EdmFunction>>) null);
        return this._functionLookUpTable;
      }
    }

    internal void AddInternal(GlobalItem item)
    {
      this.Source.Add(item);
    }

    internal void AddRange(List<GlobalItem> items)
    {
      this.Source.AddRange(items);
    }

    /// <summary>
    /// Returns a strongly typed <see cref="T:System.Data.Entity.Core.Metadata.Edm.GlobalItem" /> object by using the specified identity.
    /// </summary>
    /// <returns>The item that is specified by the identity.</returns>
    /// <param name="identity">The identity of the item.</param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public T GetItem<T>(string identity) where T : GlobalItem
    {
      return this.GetItem<T>(identity, false);
    }

    /// <summary>
    /// Returns a strongly typed <see cref="T:System.Data.Entity.Core.Metadata.Edm.GlobalItem" /> object by using the specified identity from this item collection.
    /// </summary>
    /// <returns>true if there is an item that matches the search criteria; otherwise, false.</returns>
    /// <param name="identity">The identity of the item.</param>
    /// <param name="item">
    /// When this method returns, the output parameter contains a
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.GlobalItem" />
    /// object. If there is no global item with the specified identity in the item collection, this output parameter contains null.
    /// </param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public bool TryGetItem<T>(string identity, out T item) where T : GlobalItem
    {
      return this.TryGetItem<T>(identity, false, out item);
    }

    /// <summary>
    /// Returns a strongly typed <see cref="T:System.Data.Entity.Core.Metadata.Edm.GlobalItem" /> object by using the specified identity from this item collection.
    /// </summary>
    /// <returns>true if there is an item that matches the search criteria; otherwise, false.</returns>
    /// <param name="identity">The identity of the item.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="item">
    /// When this method returns, the output parameter contains a
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.GlobalItem" />
    /// object. If there is no global item with the specified identity in the item collection, this output parameter contains null.
    /// </param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public bool TryGetItem<T>(string identity, bool ignoreCase, out T item) where T : GlobalItem
    {
      GlobalItem globalItem = (GlobalItem) null;
      this.TryGetValue(identity, ignoreCase, out globalItem);
      item = globalItem as T;
      return (object) item != null;
    }

    /// <summary>
    /// Returns a strongly typed <see cref="T:System.Data.Entity.Core.Metadata.Edm.GlobalItem" /> object by using the specified identity with either case-sensitive or case-insensitive search.
    /// </summary>
    /// <returns>The item that is specified by the identity.</returns>
    /// <param name="identity">The identity of the item.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public T GetItem<T>(string identity, bool ignoreCase) where T : GlobalItem
    {
      T obj;
      if (this.TryGetItem<T>(identity, ignoreCase, out obj))
        return obj;
      throw new ArgumentException(Strings.ItemInvalidIdentity((object) identity), nameof (identity));
    }

    /// <summary>Returns all the items of the specified type from this item collection.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the items of the specified type.
    /// </returns>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public virtual ReadOnlyCollection<T> GetItems<T>() where T : GlobalItem
    {
      Memoizer<Type, ICollection> itemsCache = this._itemsCache;
      if (this._itemsCache == null || this._itemCount != this.Count)
      {
        Interlocked.CompareExchange<Memoizer<Type, ICollection>>(ref this._itemsCache, new Memoizer<Type, ICollection>(new Func<Type, ICollection>(this.InternalGetItems), (IEqualityComparer<Type>) null), itemsCache);
        this._itemCount = this.Count;
      }
      return this._itemsCache.Evaluate(typeof (T)) as ReadOnlyCollection<T>;
    }

    internal ICollection InternalGetItems(Type type)
    {
      return typeof (ItemCollection).GetOnlyDeclaredMethod("GenericGetItems").MakeGenericMethod(type).Invoke((object) null, new object[1]
      {
        (object) this
      }) as ICollection;
    }

    private static ReadOnlyCollection<TItem> GenericGetItems<TItem>(
      ItemCollection collection)
      where TItem : GlobalItem
    {
      List<TItem> objList = new List<TItem>();
      foreach (GlobalItem globalItem in (ReadOnlyMetadataCollection<GlobalItem>) collection)
      {
        TItem obj = globalItem as TItem;
        if ((object) obj != null)
          objList.Add(obj);
      }
      return new ReadOnlyCollection<TItem>((IList<TItem>) objList);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name and the namespace name in this item collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object that represents the type that matches the specified type name and the namespace name in this item collection. If there is no matched type, this method returns null.
    /// </returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    public EdmType GetType(string name, string namespaceName)
    {
      return this.GetType(name, namespaceName, false);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name and the namespace name from this item collection.
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    /// <param name="type">
    /// When this method returns, this output parameter contains an
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// object. If there is no type with the specified name and namespace name in this item collection, this output parameter contains null.
    /// </param>
    public bool TryGetType(string name, string namespaceName, out EdmType type)
    {
      return this.TryGetType(name, namespaceName, false, out type);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name and the namespace name from this item collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object that represents the type that matches the specified type name and the namespace name in this item collection. If there is no matched type, this method returns null.
    /// </returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    public EdmType GetType(string name, string namespaceName, bool ignoreCase)
    {
      Check.NotNull<string>(name, nameof (name));
      Check.NotNull<string>(namespaceName, nameof (namespaceName));
      return this.GetItem<EdmType>(EdmType.CreateEdmTypeIdentity(namespaceName, name), ignoreCase);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name and the namespace name from this item collection.
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false. </returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="type">
    /// When this method returns, this output parameter contains an
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// object. If there is no type with the specified name and namespace name in this item collection, this output parameter contains null.
    /// </param>
    public bool TryGetType(string name, string namespaceName, bool ignoreCase, out EdmType type)
    {
      Check.NotNull<string>(name, nameof (name));
      Check.NotNull<string>(namespaceName, nameof (namespaceName));
      GlobalItem globalItem = (GlobalItem) null;
      this.TryGetValue(EdmType.CreateEdmTypeIdentity(namespaceName, name), ignoreCase, out globalItem);
      type = globalItem as EdmType;
      return type != null;
    }

    /// <summary>Returns all the overloads of the functions by using the specified name from this item collection.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the functions that have the specified name.
    /// </returns>
    /// <param name="functionName">The full name of the function.</param>
    public ReadOnlyCollection<EdmFunction> GetFunctions(
      string functionName)
    {
      return this.GetFunctions(functionName, false);
    }

    /// <summary>Returns all the overloads of the functions by using the specified name from this item collection.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the functions that have the specified name.
    /// </returns>
    /// <param name="functionName">The full name of the function.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    public ReadOnlyCollection<EdmFunction> GetFunctions(
      string functionName,
      bool ignoreCase)
    {
      return ItemCollection.GetFunctions(this.FunctionLookUpTable, functionName, ignoreCase);
    }

    /// <summary>Returns all the overloads of the functions by using the specified name from this item collection.</summary>
    /// <returns>A collection of type ReadOnlyCollection that contains all the functions that have the specified name.</returns>
    /// <param name="functionCollection">A dictionary of functions.</param>
    /// <param name="functionName">The full name of the function.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    protected static ReadOnlyCollection<EdmFunction> GetFunctions(
      Dictionary<string, ReadOnlyCollection<EdmFunction>> functionCollection,
      string functionName,
      bool ignoreCase)
    {
      ReadOnlyCollection<EdmFunction> functionOverloads;
      if (!functionCollection.TryGetValue(functionName, out functionOverloads))
        return Helper.EmptyEdmFunctionReadOnlyCollection;
      if (ignoreCase)
        return functionOverloads;
      return ItemCollection.GetCaseSensitiveFunctions(functionOverloads, functionName);
    }

    internal static ReadOnlyCollection<EdmFunction> GetCaseSensitiveFunctions(
      ReadOnlyCollection<EdmFunction> functionOverloads,
      string functionName)
    {
      List<EdmFunction> edmFunctionList = new List<EdmFunction>(functionOverloads.Count);
      for (int index = 0; index < functionOverloads.Count; ++index)
      {
        if (functionOverloads[index].FullName == functionName)
          edmFunctionList.Add(functionOverloads[index]);
      }
      if (edmFunctionList.Count != functionOverloads.Count)
        functionOverloads = new ReadOnlyCollection<EdmFunction>((IList<EdmFunction>) edmFunctionList);
      return functionOverloads;
    }

    internal bool TryGetFunction(
      string functionName,
      TypeUsage[] parameterTypes,
      bool ignoreCase,
      out EdmFunction function)
    {
      Check.NotNull<string>(functionName, nameof (functionName));
      Check.NotNull<TypeUsage[]>(parameterTypes, nameof (parameterTypes));
      string identity = EdmFunction.BuildIdentity(functionName, (IEnumerable<TypeUsage>) parameterTypes);
      GlobalItem globalItem = (GlobalItem) null;
      function = (EdmFunction) null;
      if (!this.TryGetValue(identity, ignoreCase, out globalItem) || !Helper.IsEdmFunction(globalItem))
        return false;
      function = (EdmFunction) globalItem;
      return true;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name.
    /// </summary>
    /// <returns>If there is no entity container, this method returns null; otherwise, it returns the first one.</returns>
    /// <param name="name">The name of the entity container.</param>
    public EntityContainer GetEntityContainer(string name)
    {
      Check.NotNull<string>(name, nameof (name));
      return this.GetEntityContainer(name, false);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name. If there is no entity container, the output parameter contains null; otherwise, it contains the first entity container.
    /// </summary>
    /// <returns>true if there is an entity container that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the entity container.</param>
    /// <param name="entityContainer">
    /// When this method returns, it contains an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object. If there is no entity container, this output parameter contains null; otherwise, it contains the first entity container.
    /// </param>
    public bool TryGetEntityContainer(string name, out EntityContainer entityContainer)
    {
      Check.NotNull<string>(name, nameof (name));
      return this.TryGetEntityContainer(name, false, out entityContainer);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name.
    /// </summary>
    /// <returns>If there is no entity container, this method returns null; otherwise, it returns the first entity container.</returns>
    /// <param name="name">The name of the entity container.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    public EntityContainer GetEntityContainer(string name, bool ignoreCase)
    {
      EntityContainer entityContainer = this.GetValue(name, ignoreCase) as EntityContainer;
      if (entityContainer != null)
        return entityContainer;
      throw new ArgumentException(Strings.ItemInvalidIdentity((object) name), nameof (name));
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name. If there is no entity container, this output parameter contains null; otherwise, it contains the first entity container.
    /// </summary>
    /// <returns>true if there is an entity container that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the entity container.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="entityContainer">
    /// When this method returns, it contains an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object. If there is no entity container, this output parameter contains null; otherwise, it contains the first entity container.
    /// </param>
    public bool TryGetEntityContainer(
      string name,
      bool ignoreCase,
      out EntityContainer entityContainer)
    {
      Check.NotNull<string>(name, nameof (name));
      GlobalItem globalItem = (GlobalItem) null;
      if (this.TryGetValue(name, ignoreCase, out globalItem) && Helper.IsEntityContainer(globalItem))
      {
        entityContainer = (EntityContainer) globalItem;
        return true;
      }
      entityContainer = (EntityContainer) null;
      return false;
    }

    internal virtual PrimitiveType GetMappedPrimitiveType(
      PrimitiveTypeKind primitiveTypeKind)
    {
      throw Error.NotSupported();
    }

    internal virtual bool MetadataEquals(ItemCollection other)
    {
      return object.ReferenceEquals((object) this, (object) other);
    }

    private static Dictionary<string, ReadOnlyCollection<EdmFunction>> PopulateFunctionLookUpTable(
      ItemCollection itemCollection)
    {
      Dictionary<string, List<EdmFunction>> dictionary1 = new Dictionary<string, List<EdmFunction>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (EdmFunction edmFunction in itemCollection.GetItems<EdmFunction>())
      {
        List<EdmFunction> edmFunctionList;
        if (!dictionary1.TryGetValue(edmFunction.FullName, out edmFunctionList))
        {
          edmFunctionList = new List<EdmFunction>();
          dictionary1[edmFunction.FullName] = edmFunctionList;
        }
        edmFunctionList.Add(edmFunction);
      }
      Dictionary<string, ReadOnlyCollection<EdmFunction>> dictionary2 = new Dictionary<string, ReadOnlyCollection<EdmFunction>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (List<EdmFunction> edmFunctionList in dictionary1.Values)
        dictionary2.Add(edmFunctionList[0].FullName, new ReadOnlyCollection<EdmFunction>((IList<EdmFunction>) edmFunctionList.ToArray()));
      return dictionary2;
    }
  }
}
