// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.DbXmlEnabledProviderManifest
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.SchemaObjectModel;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace System.Data.Entity.Core.Common
{
  /// <summary>
  /// A specialization of the ProviderManifest that accepts an XmlReader
  /// </summary>
  public abstract class DbXmlEnabledProviderManifest : DbProviderManifest
  {
    private readonly Dictionary<PrimitiveType, ReadOnlyCollection<FacetDescription>> _facetDescriptions = new Dictionary<PrimitiveType, ReadOnlyCollection<FacetDescription>>();
    private readonly Dictionary<string, PrimitiveType> _storeTypeNameToEdmPrimitiveType = new Dictionary<string, PrimitiveType>();
    private readonly Dictionary<string, PrimitiveType> _storeTypeNameToStorePrimitiveType = new Dictionary<string, PrimitiveType>();
    private string _namespaceName;
    private ReadOnlyCollection<PrimitiveType> _primitiveTypes;
    private ReadOnlyCollection<EdmFunction> _functions;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Common.DbXmlEnabledProviderManifest" /> class.
    /// </summary>
    /// <param name="reader">
    /// An <see cref="T:System.Xml.XmlReader" /> object that provides access to the XML data in the provider manifest file.
    /// </param>
    protected DbXmlEnabledProviderManifest(XmlReader reader)
    {
      if (reader == null)
        throw new ProviderIncompatibleException(Strings.IncorrectProviderManifest, (Exception) new ArgumentNullException(nameof (reader)));
      this.Load(reader);
    }

    /// <summary>Gets the namespace name supported by this provider manifest.</summary>
    /// <returns>The namespace name supported by this provider manifest.</returns>
    public override string NamespaceName
    {
      get
      {
        return this._namespaceName;
      }
    }

    /// <summary>Gets the best mapped equivalent Entity Data Model (EDM) type for a specified storage type name.</summary>
    /// <returns>The best mapped equivalent EDM type for a specified storage type name.</returns>
    protected Dictionary<string, PrimitiveType> StoreTypeNameToEdmPrimitiveType
    {
      get
      {
        return this._storeTypeNameToEdmPrimitiveType;
      }
    }

    /// <summary>Gets the best mapped equivalent storage primitive type for a specified storage type name.</summary>
    /// <returns>The best mapped equivalent storage primitive type for a specified storage type name.</returns>
    protected Dictionary<string, PrimitiveType> StoreTypeNameToStorePrimitiveType
    {
      get
      {
        return this._storeTypeNameToStorePrimitiveType;
      }
    }

    /// <summary>Returns the list of facet descriptions for the specified Entity Data Model (EDM) type.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains the list of facet descriptions for the specified EDM type.
    /// </returns>
    /// <param name="edmType">
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> for which the facet descriptions are to be retrieved.
    /// </param>
    public override ReadOnlyCollection<FacetDescription> GetFacetDescriptions(
      EdmType edmType)
    {
      return DbXmlEnabledProviderManifest.GetReadOnlyCollection<FacetDescription>(edmType as PrimitiveType, this._facetDescriptions, Helper.EmptyFacetDescriptionEnumerable);
    }

    /// <summary>Returns the list of primitive types supported by the storage provider.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains the list of primitive types supported by the storage provider.
    /// </returns>
    public override ReadOnlyCollection<PrimitiveType> GetStoreTypes()
    {
      return this._primitiveTypes;
    }

    /// <summary>Returns the list of provider-supported functions.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains the list of provider-supported functions.
    /// </returns>
    public override ReadOnlyCollection<EdmFunction> GetStoreFunctions()
    {
      return this._functions;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    private void Load(XmlReader reader)
    {
      Schema schema;
      IList<EdmSchemaError> edmSchemaErrorList = SchemaManager.LoadProviderManifest(reader, reader.BaseURI.Length > 0 ? reader.BaseURI : (string) null, true, out schema);
      if (edmSchemaErrorList.Count != 0)
        throw new ProviderIncompatibleException(Strings.IncorrectProviderManifest + Helper.CombineErrorMessage((IEnumerable<EdmSchemaError>) edmSchemaErrorList));
      this._namespaceName = schema.Namespace;
      List<PrimitiveType> primitiveTypeList = new List<PrimitiveType>();
      foreach (System.Data.Entity.Core.SchemaObjectModel.SchemaType schemaType in schema.SchemaTypes)
      {
        TypeElement typeElement = schemaType as TypeElement;
        if (typeElement != null)
        {
          PrimitiveType primitiveType = typeElement.PrimitiveType;
          primitiveType.ProviderManifest = (DbProviderManifest) this;
          primitiveType.DataSpace = DataSpace.SSpace;
          primitiveType.SetReadOnly();
          primitiveTypeList.Add(primitiveType);
          this._storeTypeNameToStorePrimitiveType.Add(primitiveType.Name.ToLowerInvariant(), primitiveType);
          this._storeTypeNameToEdmPrimitiveType.Add(primitiveType.Name.ToLowerInvariant(), EdmProviderManifest.Instance.GetPrimitiveType(primitiveType.PrimitiveTypeKind));
          ReadOnlyCollection<FacetDescription> collection;
          if (DbXmlEnabledProviderManifest.EnumerableToReadOnlyCollection<FacetDescription, FacetDescription>(typeElement.FacetDescriptions, out collection))
            this._facetDescriptions.Add(primitiveType, collection);
        }
      }
      this._primitiveTypes = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) primitiveTypeList.ToArray());
      ItemCollection itemCollection = (ItemCollection) new DbXmlEnabledProviderManifest.EmptyItemCollection();
      if (!DbXmlEnabledProviderManifest.EnumerableToReadOnlyCollection<EdmFunction, GlobalItem>(Converter.ConvertSchema(schema, (DbProviderManifest) this, itemCollection), out this._functions))
        this._functions = Helper.EmptyEdmFunctionReadOnlyCollection;
      foreach (MetadataItem function in this._functions)
        function.SetReadOnly();
    }

    private static ReadOnlyCollection<T> GetReadOnlyCollection<T>(
      PrimitiveType type,
      Dictionary<PrimitiveType, ReadOnlyCollection<T>> typeDictionary,
      ReadOnlyCollection<T> useIfEmpty)
    {
      ReadOnlyCollection<T> readOnlyCollection;
      if (typeDictionary.TryGetValue(type, out readOnlyCollection))
        return readOnlyCollection;
      return useIfEmpty;
    }

    private static bool EnumerableToReadOnlyCollection<Target, BaseType>(
      IEnumerable<BaseType> enumerable,
      out ReadOnlyCollection<Target> collection)
      where Target : BaseType
    {
      List<Target> argetList = new List<Target>();
      foreach (BaseType baseType in enumerable)
      {
        if (typeof (Target) == typeof (BaseType) || (object) baseType is Target)
          argetList.Add((Target) (object) baseType);
      }
      if (argetList.Count != 0)
      {
        collection = new ReadOnlyCollection<Target>((IList<Target>) argetList);
        return true;
      }
      collection = (ReadOnlyCollection<Target>) null;
      return false;
    }

    private class EmptyItemCollection : ItemCollection
    {
      public EmptyItemCollection()
        : base(DataSpace.SSpace)
      {
      }
    }
  }
}
