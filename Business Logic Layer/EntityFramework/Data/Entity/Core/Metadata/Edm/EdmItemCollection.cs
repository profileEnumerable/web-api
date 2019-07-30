// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmItemCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.SchemaObjectModel;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Class for representing a collection of items in Edm space.
  /// </summary>
  public sealed class EdmItemCollection : ItemCollection
  {
    private readonly CacheForPrimitiveTypes _primitiveTypeMaps = new CacheForPrimitiveTypes();
    private readonly OcAssemblyCache _conventionalOcCache = new OcAssemblyCache();
    private double _edmVersion;
    private Memoizer<InitializerMetadata, InitializerMetadata> _getCanonicalInitializerMetadataMemoizer;
    private Memoizer<EdmFunction, DbLambda> _getGeneratedFunctionDefinitionsMemoizer;

    internal EdmItemCollection(
      IEnumerable<XmlReader> xmlReaders,
      IEnumerable<string> filePaths,
      bool skipInitialization = false)
      : base(DataSpace.CSpace)
    {
      if (skipInitialization)
        return;
      this.Init(xmlReaders, filePaths, true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" /> class by using the collection of the XMLReader objects where the conceptual schema definition language (CSDL) files exist.
    /// </summary>
    /// <param name="xmlReaders">The collection of the XMLReader objects where the conceptual schema definition language (CSDL) files exist.</param>
    public EdmItemCollection(IEnumerable<XmlReader> xmlReaders)
      : base(DataSpace.CSpace)
    {
      Check.NotNull<IEnumerable<XmlReader>>(xmlReaders, nameof (xmlReaders));
      EntityUtil.CheckArgumentContainsNull<XmlReader>(ref xmlReaders, nameof (xmlReaders));
      MetadataArtifactLoader compositeFromXmlReaders = MetadataArtifactLoader.CreateCompositeFromXmlReaders(xmlReaders);
      this.Init((IEnumerable<XmlReader>) compositeFromXmlReaders.GetReaders(), (IEnumerable<string>) compositeFromXmlReaders.GetPaths(), true);
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" /> class.</summary>
    /// <param name="model">The entity data model.</param>
    public EdmItemCollection(EdmModel model)
      : base(DataSpace.CSpace)
    {
      Check.NotNull<EdmModel>(model, nameof (model));
      this.Init();
      this._edmVersion = model.SchemaVersion;
      model.Validate();
      foreach (GlobalItem globalItem in model.GlobalItems)
      {
        globalItem.SetReadOnly();
        this.AddInternal(globalItem);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" /> class by using the paths where the conceptual schema definition language (CSDL) files exist.
    /// </summary>
    /// <param name="filePaths">The paths where the conceptual schema definition language (CSDL) files exist.</param>
    public EdmItemCollection(params string[] filePaths)
      : base(DataSpace.CSpace)
    {
      Check.NotNull<string[]>(filePaths, nameof (filePaths));
      List<XmlReader> xmlReaderList = (List<XmlReader>) null;
      try
      {
        MetadataArtifactLoader compositeFromFilePaths = MetadataArtifactLoader.CreateCompositeFromFilePaths((IEnumerable<string>) filePaths, ".csdl");
        xmlReaderList = compositeFromFilePaths.CreateReaders(DataSpace.CSpace);
        this.Init((IEnumerable<XmlReader>) xmlReaderList, (IEnumerable<string>) compositeFromFilePaths.GetPaths(DataSpace.CSpace), true);
      }
      finally
      {
        if (xmlReaderList != null)
          Helper.DisposeXmlReaders((IEnumerable<XmlReader>) xmlReaderList);
      }
    }

    private EdmItemCollection(
      IEnumerable<XmlReader> xmlReaders,
      ReadOnlyCollection<string> filePaths,
      out IList<EdmSchemaError> errors)
      : base(DataSpace.CSpace)
    {
      errors = this.Init(xmlReaders, (IEnumerable<string>) filePaths, false);
    }

    private void Init()
    {
      this.LoadEdmPrimitiveTypesAndFunctions();
    }

    private IList<EdmSchemaError> Init(
      IEnumerable<XmlReader> xmlReaders,
      IEnumerable<string> filePaths,
      bool throwOnError)
    {
      this.Init();
      return EdmItemCollection.LoadItems(xmlReaders, filePaths, SchemaDataModelOption.EntityDataModel, (DbProviderManifest) MetadataItem.EdmProviderManifest, (ItemCollection) this, throwOnError);
    }

    /// <summary>Gets the conceptual model version for this collection.</summary>
    /// <returns>The conceptual model version for this collection.</returns>
    public double EdmVersion
    {
      get
      {
        return this._edmVersion;
      }
      internal set
      {
        this._edmVersion = value;
      }
    }

    internal OcAssemblyCache ConventionalOcCache
    {
      get
      {
        return this._conventionalOcCache;
      }
    }

    internal InitializerMetadata GetCanonicalInitializerMetadata(
      InitializerMetadata metadata)
    {
      if (this._getCanonicalInitializerMetadataMemoizer == null)
        Interlocked.CompareExchange<Memoizer<InitializerMetadata, InitializerMetadata>>(ref this._getCanonicalInitializerMetadataMemoizer, new Memoizer<InitializerMetadata, InitializerMetadata>((Func<InitializerMetadata, InitializerMetadata>) (m => m), (IEqualityComparer<InitializerMetadata>) EqualityComparer<InitializerMetadata>.Default), (Memoizer<InitializerMetadata, InitializerMetadata>) null);
      return this._getCanonicalInitializerMetadataMemoizer.Evaluate(metadata);
    }

    internal static bool IsSystemNamespace(DbProviderManifest manifest, string namespaceName)
    {
      if (manifest == MetadataItem.EdmProviderManifest)
      {
        if (!(namespaceName == "Transient") && !(namespaceName == "Edm"))
          return namespaceName == "System";
        return true;
      }
      if (namespaceName == "Transient" || namespaceName == "Edm" || namespaceName == "System")
        return true;
      if (manifest != null)
        return namespaceName == manifest.NamespaceName;
      return false;
    }

    internal static IList<EdmSchemaError> LoadItems(
      IEnumerable<XmlReader> xmlReaders,
      IEnumerable<string> sourceFilePaths,
      SchemaDataModelOption dataModelOption,
      DbProviderManifest providerManifest,
      ItemCollection itemCollection,
      bool throwOnError)
    {
      IList<Schema> schemaCollection = (IList<Schema>) null;
      IList<EdmSchemaError> andValidate = SchemaManager.ParseAndValidate(xmlReaders, sourceFilePaths, dataModelOption, providerManifest, out schemaCollection);
      if (MetadataHelper.CheckIfAllErrorsAreWarnings(andValidate))
      {
        foreach (EdmSchemaError loadItem in EdmItemCollection.LoadItems(providerManifest, schemaCollection, itemCollection))
          andValidate.Add(loadItem);
      }
      if (!MetadataHelper.CheckIfAllErrorsAreWarnings(andValidate) && throwOnError)
        throw EntityUtil.InvalidSchemaEncountered(Helper.CombineErrorMessage((IEnumerable<EdmSchemaError>) andValidate));
      return andValidate;
    }

    internal static List<EdmSchemaError> LoadItems(
      DbProviderManifest manifest,
      IList<Schema> somSchemas,
      ItemCollection itemCollection)
    {
      List<EdmSchemaError> edmSchemaErrorList = new List<EdmSchemaError>();
      IEnumerable<GlobalItem> globalItems = EdmItemCollection.LoadSomSchema(somSchemas, manifest, itemCollection);
      List<string> stringList = new List<string>();
      foreach (GlobalItem globalItem in globalItems)
      {
        if (globalItem.BuiltInTypeKind == BuiltInTypeKind.EdmFunction && globalItem.DataSpace == DataSpace.SSpace)
        {
          EdmFunction edmFunction = (EdmFunction) globalItem;
          StringBuilder builder = new StringBuilder();
          EdmFunction.BuildIdentity<FunctionParameter>(builder, edmFunction.FullName, (IEnumerable<FunctionParameter>) edmFunction.Parameters, (Func<FunctionParameter, TypeUsage>) (param => MetadataHelper.ConvertStoreTypeUsageToEdmTypeUsage(param.TypeUsage)), (Func<FunctionParameter, ParameterMode>) (param => param.Mode));
          string str = builder.ToString();
          if (stringList.Contains(str))
          {
            edmSchemaErrorList.Add(new EdmSchemaError(Strings.DuplicatedFunctionoverloads((object) edmFunction.FullName, (object) str.Substring(edmFunction.FullName.Length)).Trim(), 174, EdmSchemaErrorSeverity.Error));
            continue;
          }
          stringList.Add(str);
        }
        globalItem.SetReadOnly();
        itemCollection.AddInternal(globalItem);
      }
      return edmSchemaErrorList;
    }

    internal static IEnumerable<GlobalItem> LoadSomSchema(
      IList<Schema> somSchemas,
      DbProviderManifest providerManifest,
      ItemCollection itemCollection)
    {
      return Converter.ConvertSchema(somSchemas, providerManifest, itemCollection);
    }

    /// <summary>
    /// Returns a collection of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> objects.
    /// </summary>
    /// <returns>
    /// A ReadOnlyCollection object that represents a collection of the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" />
    /// objects.
    /// </returns>
    public ReadOnlyCollection<PrimitiveType> GetPrimitiveTypes()
    {
      return this._primitiveTypeMaps.GetTypes();
    }

    /// <summary>
    /// Returns a collection of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> objects with the specified conceptual model version.
    /// </summary>
    /// <returns>
    /// A ReadOnlyCollection object that represents a collection of the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" />
    /// objects.
    /// </returns>
    /// <param name="edmVersion">The conceptual model version.</param>
    public ReadOnlyCollection<PrimitiveType> GetPrimitiveTypes(
      double edmVersion)
    {
      if (edmVersion == 1.0 || edmVersion == 1.1 || edmVersion == 2.0)
        return new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) this._primitiveTypeMaps.GetTypes().Where<PrimitiveType>((Func<PrimitiveType, bool>) (type => !Helper.IsSpatialType(type))).ToList<PrimitiveType>());
      if (edmVersion == 3.0)
        return this._primitiveTypeMaps.GetTypes();
      throw new ArgumentException(Strings.InvalidEDMVersion((object) edmVersion.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
    }

    internal override PrimitiveType GetMappedPrimitiveType(
      PrimitiveTypeKind primitiveTypeKind)
    {
      PrimitiveType type = (PrimitiveType) null;
      this._primitiveTypeMaps.TryGetType(primitiveTypeKind, (IEnumerable<Facet>) null, out type);
      return type;
    }

    private void LoadEdmPrimitiveTypesAndFunctions()
    {
      EdmProviderManifest instance = EdmProviderManifest.Instance;
      ReadOnlyCollection<PrimitiveType> storeTypes = instance.GetStoreTypes();
      for (int index = 0; index < storeTypes.Count; ++index)
      {
        this.AddInternal((GlobalItem) storeTypes[index]);
        this._primitiveTypeMaps.Add(storeTypes[index]);
      }
      ReadOnlyCollection<EdmFunction> storeFunctions = instance.GetStoreFunctions();
      for (int index = 0; index < storeFunctions.Count; ++index)
        this.AddInternal((GlobalItem) storeFunctions[index]);
    }

    internal DbLambda GetGeneratedFunctionDefinition(EdmFunction function)
    {
      if (this._getGeneratedFunctionDefinitionsMemoizer == null)
        Interlocked.CompareExchange<Memoizer<EdmFunction, DbLambda>>(ref this._getGeneratedFunctionDefinitionsMemoizer, new Memoizer<EdmFunction, DbLambda>(new Func<EdmFunction, DbLambda>(this.GenerateFunctionDefinition), (IEqualityComparer<EdmFunction>) null), (Memoizer<EdmFunction, DbLambda>) null);
      return this._getGeneratedFunctionDefinitionsMemoizer.Evaluate(function);
    }

    internal DbLambda GenerateFunctionDefinition(EdmFunction function)
    {
      if (!function.HasUserDefinedBody)
        throw new InvalidOperationException(Strings.Cqt_UDF_FunctionHasNoDefinition((object) function.Identity));
      DbLambda dbLambda = ExternalCalls.CompileFunctionDefinition(function.CommandTextAttribute, (IList<FunctionParameter>) function.Parameters, this);
      if (!TypeSemantics.IsStructurallyEqual(function.ReturnParameter.TypeUsage, dbLambda.Body.ResultType))
        throw new InvalidOperationException(Strings.Cqt_UDF_FunctionDefinitionResultTypeMismatch((object) function.ReturnParameter.TypeUsage.ToString(), (object) function.FullName, (object) dbLambda.Body.ResultType.ToString()));
      return dbLambda;
    }

    /// <summary>
    /// Factory method that creates an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" />.
    /// </summary>
    /// <param name="xmlReaders">
    /// CSDL artifacts to load. Must not be <c>null</c>.
    /// </param>
    /// <param name="filePaths">
    /// Paths to CSDL artifacts. Used in error messages. Can be <c>null</c> in which case
    /// the base Uri of the XmlReader will be used as a path.
    /// </param>
    /// <param name="errors">
    /// The collection of errors encountered while loading.
    /// </param>
    /// <returns>
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" /> instance if no errors encountered. Otherwise <c>null</c>.
    /// </returns>
    public static EdmItemCollection Create(
      IEnumerable<XmlReader> xmlReaders,
      ReadOnlyCollection<string> filePaths,
      out IList<EdmSchemaError> errors)
    {
      Check.NotNull<IEnumerable<XmlReader>>(xmlReaders, nameof (xmlReaders));
      EntityUtil.CheckArgumentContainsNull<XmlReader>(ref xmlReaders, nameof (xmlReaders));
      EdmItemCollection edmItemCollection = new EdmItemCollection(xmlReaders, filePaths, out errors);
      if (errors == null || errors.Count <= 0)
        return edmItemCollection;
      return (EdmItemCollection) null;
    }
  }
}
