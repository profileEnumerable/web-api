// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.EntityClient
{
  /// <summary>
  /// Class representing a connection string builder for the entity client provider
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "EntityConnectionStringBuilder follows the naming convention of DbConnectionStringBuilder.")]
  [SuppressMessage("Microsoft.Design", "CA1035:ICollectionImplementationsHaveStronglyTypedMembers", Justification = "There is no applicable strongly-typed implementation of CopyTo.")]
  public sealed class EntityConnectionStringBuilder : DbConnectionStringBuilder
  {
    internal static readonly string[] ValidKeywords = new string[4]
    {
      "name",
      "metadata",
      "provider",
      "provider connection string"
    };
    internal const string NameParameterName = "name";
    internal const string MetadataParameterName = "metadata";
    internal const string ProviderParameterName = "provider";
    internal const string ProviderConnectionStringParameterName = "provider connection string";
    private string _namedConnectionName;
    private string _providerName;
    private string _metadataLocations;
    private string _storeProviderConnectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" /> class.
    /// </summary>
    public EntityConnectionStringBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" /> class using the supplied connection string.
    /// </summary>
    /// <param name="connectionString">A provider-specific connection string to the underlying data source.</param>
    public EntityConnectionStringBuilder(string connectionString)
    {
      this.ConnectionString = connectionString;
    }

    /// <summary>Gets or sets the name of a section as defined in a configuration file.</summary>
    /// <returns>The name of a section in a configuration file.</returns>
    [EntityResDescription("EntityConnectionString_Name")]
    [RefreshProperties(RefreshProperties.All)]
    [EntityResCategory("EntityDataCategory_NamedConnectionString")]
    [DisplayName("Name")]
    public string Name
    {
      get
      {
        return this._namedConnectionName ?? "";
      }
      set
      {
        this._namedConnectionName = value;
        base["name"] = (object) value;
      }
    }

    /// <summary>Gets or sets the name of the underlying .NET Framework data provider in the connection string.</summary>
    /// <returns>The invariant name of the underlying .NET Framework data provider.</returns>
    [DisplayName("Provider")]
    [RefreshProperties(RefreshProperties.All)]
    [EntityResCategory("EntityDataCategory_Source")]
    [EntityResDescription("EntityConnectionString_Provider")]
    public string Provider
    {
      get
      {
        return this._providerName ?? "";
      }
      set
      {
        this._providerName = value;
        base["provider"] = (object) value;
      }
    }

    /// <summary>Gets or sets the metadata locations in the connection string.</summary>
    /// <returns>Gets or sets the metadata locations in the connection string.</returns>
    [EntityResCategory("EntityDataCategory_Context")]
    [RefreshProperties(RefreshProperties.All)]
    [EntityResDescription("EntityConnectionString_Metadata")]
    [DisplayName("Metadata")]
    public string Metadata
    {
      get
      {
        return this._metadataLocations ?? "";
      }
      set
      {
        this._metadataLocations = value;
        base["metadata"] = (object) value;
      }
    }

    /// <summary>Gets or sets the inner, provider-specific connection string.</summary>
    /// <returns>The inner, provider-specific connection string.</returns>
    [EntityResDescription("EntityConnectionString_ProviderConnectionString")]
    [RefreshProperties(RefreshProperties.All)]
    [EntityResCategory("EntityDataCategory_Source")]
    [DisplayName("Provider Connection String")]
    public string ProviderConnectionString
    {
      get
      {
        return this._storeProviderConnectionString ?? "";
      }
      set
      {
        this._storeProviderConnectionString = value;
        base["provider connection string"] = (object) value;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// has a fixed size.
    /// </summary>
    /// <returns>
    /// Returns true in every case, because the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// supplies a fixed-size collection of keyword/value pairs.
    /// </returns>
    public override bool IsFixedSize
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets an <see cref="T:System.Collections.ICollection" /> that contains the keys in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// .
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.ICollection" /> that contains the keys in the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// .
    /// </returns>
    public override ICollection Keys
    {
      get
      {
        return (ICollection) new ReadOnlyCollection<string>((IList<string>) EntityConnectionStringBuilder.ValidKeywords);
      }
    }

    /// <summary>Gets or sets the value associated with the specified key. In C#, this property is the indexer.</summary>
    /// <returns>The value associated with the specified key. </returns>
    /// <param name="keyword">The key of the item to get or set.</param>
    /// <exception cref="T:System.ArgumentNullException"> keyword  is a null reference (Nothing in Visual Basic).</exception>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Tried to add a key that does not exist in the available keys.</exception>
    /// <exception cref="T:System.FormatException">Invalid value in the connection string (specifically, a Boolean or numeric value was expected but not supplied).</exception>
    public override object this[string keyword]
    {
      get
      {
        Check.NotNull<string>(keyword, nameof (keyword));
        if (string.Compare(keyword, "metadata", StringComparison.OrdinalIgnoreCase) == 0)
          return (object) this.Metadata;
        if (string.Compare(keyword, "provider connection string", StringComparison.OrdinalIgnoreCase) == 0)
          return (object) this.ProviderConnectionString;
        if (string.Compare(keyword, "name", StringComparison.OrdinalIgnoreCase) == 0)
          return (object) this.Name;
        if (string.Compare(keyword, "provider", StringComparison.OrdinalIgnoreCase) == 0)
          return (object) this.Provider;
        throw new ArgumentException(Strings.EntityClient_KeywordNotSupported((object) keyword));
      }
      set
      {
        Check.NotNull<string>(keyword, nameof (keyword));
        if (value == null)
        {
          this.Remove(keyword);
        }
        else
        {
          string str = value as string;
          if (str == null)
            throw new ArgumentException(Strings.EntityClient_ValueNotString, nameof (value));
          if (string.Compare(keyword, "metadata", StringComparison.OrdinalIgnoreCase) == 0)
            this.Metadata = str;
          else if (string.Compare(keyword, "provider connection string", StringComparison.OrdinalIgnoreCase) == 0)
            this.ProviderConnectionString = str;
          else if (string.Compare(keyword, "name", StringComparison.OrdinalIgnoreCase) == 0)
          {
            this.Name = str;
          }
          else
          {
            if (string.Compare(keyword, "provider", StringComparison.OrdinalIgnoreCase) != 0)
              throw new ArgumentException(Strings.EntityClient_KeywordNotSupported((object) keyword));
            this.Provider = str;
          }
        }
      }
    }

    /// <summary>
    /// Clears the contents of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" /> instance.
    /// </summary>
    public override void Clear()
    {
      base.Clear();
      this._namedConnectionName = (string) null;
      this._providerName = (string) null;
      this._metadataLocations = (string) null;
      this._storeProviderConnectionString = (string) null;
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" /> contains a specific key.
    /// </summary>
    /// <returns>
    /// Returns true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" /> contains an element that has the specified key; otherwise, false.
    /// </returns>
    /// <param name="keyword">
    /// The key to locate in the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />.
    /// </param>
    public override bool ContainsKey(string keyword)
    {
      Check.NotNull<string>(keyword, nameof (keyword));
      foreach (string validKeyword in EntityConnectionStringBuilder.ValidKeywords)
      {
        if (validKeyword.Equals(keyword, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Retrieves a value corresponding to the supplied key from this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// .
    /// </summary>
    /// <returns>Returns true if  keyword  was found in the connection string; otherwise, false.</returns>
    /// <param name="keyword">The key of the item to retrieve.</param>
    /// <param name="value">The value corresponding to  keyword. </param>
    /// <exception cref="T:System.ArgumentNullException"> keyword  contains a null value (Nothing in Visual Basic).</exception>
    public override bool TryGetValue(string keyword, out object value)
    {
      Check.NotNull<string>(keyword, nameof (keyword));
      if (this.ContainsKey(keyword))
      {
        value = this[keyword];
        return true;
      }
      value = (object) null;
      return false;
    }

    /// <summary>
    /// Removes the entry with the specified key from the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// instance.
    /// </summary>
    /// <returns>Returns true if the key existed in the connection string and was removed; false if the key did not exist.</returns>
    /// <param name="keyword">
    /// The key of the keyword/value pair to be removed from the connection string in this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// .
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"> keyword  is null (Nothing in Visual Basic)</exception>
    public override bool Remove(string keyword)
    {
      if (string.Compare(keyword, "metadata", StringComparison.OrdinalIgnoreCase) == 0)
        this._metadataLocations = (string) null;
      else if (string.Compare(keyword, "provider connection string", StringComparison.OrdinalIgnoreCase) == 0)
        this._storeProviderConnectionString = (string) null;
      else if (string.Compare(keyword, "name", StringComparison.OrdinalIgnoreCase) == 0)
        this._namedConnectionName = (string) null;
      else if (string.Compare(keyword, "provider", StringComparison.OrdinalIgnoreCase) == 0)
        this._providerName = (string) null;
      return base.Remove(keyword);
    }
  }
}
