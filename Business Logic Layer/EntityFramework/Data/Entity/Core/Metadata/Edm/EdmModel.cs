// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmModel
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Represents a conceptual or store model. This class can be used to access information about the shape of the model
  /// and the way the that it has been configured.
  /// </summary>
  public class EdmModel : MetadataItem
  {
    private readonly List<AssociationType> _associationTypes = new List<AssociationType>();
    private readonly List<ComplexType> _complexTypes = new List<ComplexType>();
    private readonly List<EntityType> _entityTypes = new List<EntityType>();
    private readonly List<EnumType> _enumTypes = new List<EnumType>();
    private readonly List<EdmFunction> _functions = new List<EdmFunction>();
    private readonly EntityContainer _container;
    private double _schemaVersion;
    private DbProviderInfo _providerInfo;
    private DbProviderManifest _providerManifest;

    private EdmModel(EntityContainer entityContainer, double version = 3.0)
    {
      this._container = entityContainer;
      this.SchemaVersion = version;
    }

    internal EdmModel(DataSpace dataSpace, double schemaVersion = 3.0)
    {
      if (dataSpace != DataSpace.CSpace && dataSpace != DataSpace.SSpace)
        throw new ArgumentException(Strings.MetadataItem_InvalidDataSpace((object) dataSpace, (object) typeof (EdmModel).Name), nameof (dataSpace));
      this._container = new EntityContainer(dataSpace == DataSpace.CSpace ? "CodeFirstContainer" : "CodeFirstDatabase", dataSpace);
      this._schemaVersion = schemaVersion;
    }

    /// <summary>Gets the built-in type kind for this type.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this type.
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.MetadataItem;
      }
    }

    internal override string Identity
    {
      get
      {
        return nameof (EdmModel) + this.Container.Identity;
      }
    }

    /// <summary>
    /// Gets the data space associated with the model, which indicates whether
    /// it is a conceptual model (DataSpace.CSpace) or a store model (DataSpace.SSpace).
    /// </summary>
    public DataSpace DataSpace
    {
      get
      {
        return this.Container.DataSpace;
      }
    }

    /// <summary>Gets the association types in the model.</summary>
    public IEnumerable<AssociationType> AssociationTypes
    {
      get
      {
        return (IEnumerable<AssociationType>) this._associationTypes;
      }
    }

    /// <summary>Gets the complex types in the model.</summary>
    public IEnumerable<ComplexType> ComplexTypes
    {
      get
      {
        return (IEnumerable<ComplexType>) this._complexTypes;
      }
    }

    /// <summary>Gets the entity types in the model.</summary>
    public IEnumerable<EntityType> EntityTypes
    {
      get
      {
        return (IEnumerable<EntityType>) this._entityTypes;
      }
    }

    /// <summary>Gets the enum types in the model.</summary>
    public IEnumerable<EnumType> EnumTypes
    {
      get
      {
        return (IEnumerable<EnumType>) this._enumTypes;
      }
    }

    /// <summary>Gets the functions in the model.</summary>
    public IEnumerable<EdmFunction> Functions
    {
      get
      {
        return (IEnumerable<EdmFunction>) this._functions;
      }
    }

    /// <summary>
    /// Gets the container that stores entity and association sets, and function imports.
    /// </summary>
    public EntityContainer Container
    {
      get
      {
        return this._container;
      }
    }

    internal double SchemaVersion
    {
      get
      {
        return this._schemaVersion;
      }
      set
      {
        this._schemaVersion = value;
      }
    }

    internal DbProviderInfo ProviderInfo
    {
      get
      {
        return this._providerInfo;
      }
      private set
      {
        this._providerInfo = value;
      }
    }

    internal DbProviderManifest ProviderManifest
    {
      get
      {
        return this._providerManifest;
      }
      private set
      {
        this._providerManifest = value;
      }
    }

    internal virtual IEnumerable<string> NamespaceNames
    {
      get
      {
        return this.NamespaceItems.Select<EdmType, string>((Func<EdmType, string>) (t => t.NamespaceName)).Distinct<string>();
      }
    }

    internal IEnumerable<EdmType> NamespaceItems
    {
      get
      {
        return ((IEnumerable<EdmType>) this._associationTypes).Concat<EdmType>((IEnumerable<EdmType>) this._complexTypes).Concat<EdmType>((IEnumerable<EdmType>) this._entityTypes).Concat<EdmType>((IEnumerable<EdmType>) this._enumTypes).Concat<EdmType>((IEnumerable<EdmType>) this._functions);
      }
    }

    /// <summary>Gets the global items associated with the model.</summary>
    /// <returns>The global items associated with the model.</returns>
    public IEnumerable<GlobalItem> GlobalItems
    {
      get
      {
        return ((IEnumerable<GlobalItem>) this.NamespaceItems).Concat<GlobalItem>((IEnumerable<GlobalItem>) this.Containers);
      }
    }

    internal virtual IEnumerable<EntityContainer> Containers
    {
      get
      {
        yield return this.Container;
      }
    }

    /// <summary>Adds an association type to the model.</summary>
    /// <param name="item">The AssociationType instance to be added.</param>
    public void AddItem(AssociationType item)
    {
      Check.NotNull<AssociationType>(item, nameof (item));
      this.ValidateSpace((EdmType) item);
      this._associationTypes.Add(item);
    }

    /// <summary>Adds a complex type to the model.</summary>
    /// <param name="item">The ComplexType instance to be added.</param>
    public void AddItem(ComplexType item)
    {
      Check.NotNull<ComplexType>(item, nameof (item));
      this.ValidateSpace((EdmType) item);
      this._complexTypes.Add(item);
    }

    /// <summary>Adds an entity type to the model.</summary>
    /// <param name="item">The EntityType instance to be added.</param>
    public void AddItem(EntityType item)
    {
      Check.NotNull<EntityType>(item, nameof (item));
      this.ValidateSpace((EdmType) item);
      this._entityTypes.Add(item);
    }

    /// <summary>Adds an enumeration type to the model.</summary>
    /// <param name="item">The EnumType instance to be added.</param>
    public void AddItem(EnumType item)
    {
      Check.NotNull<EnumType>(item, nameof (item));
      this.ValidateSpace((EdmType) item);
      this._enumTypes.Add(item);
    }

    /// <summary>Adds a function to the model.</summary>
    /// <param name="item">The EdmFunction instance to be added.</param>
    public void AddItem(EdmFunction item)
    {
      Check.NotNull<EdmFunction>(item, nameof (item));
      this.ValidateSpace((EdmType) item);
      this._functions.Add(item);
    }

    /// <summary>Removes an association type from the model.</summary>
    /// <param name="item">The AssociationType instance to be removed.</param>
    public void RemoveItem(AssociationType item)
    {
      Check.NotNull<AssociationType>(item, nameof (item));
      this._associationTypes.Remove(item);
    }

    /// <summary>Removes a complex type from the model.</summary>
    /// <param name="item">The ComplexType instance to be removed.</param>
    public void RemoveItem(ComplexType item)
    {
      Check.NotNull<ComplexType>(item, nameof (item));
      this._complexTypes.Remove(item);
    }

    /// <summary>Removes an entity type from the model.</summary>
    /// <param name="item">The EntityType instance to be removed.</param>
    public void RemoveItem(EntityType item)
    {
      Check.NotNull<EntityType>(item, nameof (item));
      this._entityTypes.Remove(item);
    }

    /// <summary>Removes an enumeration type from the model.</summary>
    /// <param name="item">The EnumType instance to be removed.</param>
    public void RemoveItem(EnumType item)
    {
      Check.NotNull<EnumType>(item, nameof (item));
      this._enumTypes.Remove(item);
    }

    /// <summary>Removes a function from the model.</summary>
    /// <param name="item">The EdmFunction instance to be removed.</param>
    public void RemoveItem(EdmFunction item)
    {
      Check.NotNull<EdmFunction>(item, nameof (item));
      this._functions.Remove(item);
    }

    internal virtual void Validate()
    {
      List<DataModelErrorEventArgs> validationErrors = new List<DataModelErrorEventArgs>();
      DataModelValidator dataModelValidator = new DataModelValidator();
      dataModelValidator.OnError += (EventHandler<DataModelErrorEventArgs>) ((_, e) => validationErrors.Add(e));
      dataModelValidator.Validate(this, true);
      if (validationErrors.Count > 0)
        throw new ModelValidationException((IEnumerable<DataModelErrorEventArgs>) validationErrors);
    }

    private void ValidateSpace(EdmType item)
    {
      if (item.DataSpace != this.DataSpace)
        throw new ArgumentException(Strings.EdmModel_AddItem_NonMatchingNamespace, nameof (item));
    }

    internal static EdmModel CreateStoreModel(
      DbProviderInfo providerInfo,
      DbProviderManifest providerManifest,
      double schemaVersion = 3.0)
    {
      return new EdmModel(DataSpace.SSpace, schemaVersion)
      {
        ProviderInfo = providerInfo,
        ProviderManifest = providerManifest
      };
    }

    internal static EdmModel CreateStoreModel(
      EntityContainer entityContainer,
      DbProviderInfo providerInfo,
      DbProviderManifest providerManifest,
      double schemaVersion = 3.0)
    {
      EdmModel edmModel = new EdmModel(entityContainer, schemaVersion);
      if (providerInfo != null)
        edmModel.ProviderInfo = providerInfo;
      if (providerManifest != null)
        edmModel.ProviderManifest = providerManifest;
      return edmModel;
    }

    internal static EdmModel CreateConceptualModel(double schemaVersion = 3.0)
    {
      return new EdmModel(DataSpace.CSpace, schemaVersion);
    }

    internal static EdmModel CreateConceptualModel(
      EntityContainer entityContainer,
      double schemaVersion = 3.0)
    {
      return new EdmModel(entityContainer, schemaVersion);
    }
  }
}
