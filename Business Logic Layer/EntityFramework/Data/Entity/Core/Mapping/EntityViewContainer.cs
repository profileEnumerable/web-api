// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.EntityViewContainer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Base class for the type created at design time to store the generated views.
  /// </summary>
  [Obsolete("The mechanism to provide pre-generated views has changed. Implement a class that derives from System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCache and has a parameterless constructor, then associate it with a type that derives from DbContext or ObjectContext by using System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCacheTypeAttribute.", true)]
  public abstract class EntityViewContainer
  {
    internal IEnumerable<KeyValuePair<string, string>> ExtentViews
    {
      get
      {
        for (int i = 0; i < this.ViewCount; ++i)
          yield return this.GetViewAt(i);
      }
    }

    /// <summary>Returns the key/value pair at the specified index, which contains the view and its key.</summary>
    /// <returns>The key/value pair at  index , which contains the view and its key.</returns>
    /// <param name="index">The index of the view.</param>
    protected abstract KeyValuePair<string, string> GetViewAt(int index);

    /// <summary>
    /// Gets or sets the name of <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />.
    /// </summary>
    /// <returns>The container name.</returns>
    public string EdmEntityContainerName { get; set; }

    /// <summary>
    /// Gets or sets <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> in storage schema.
    /// </summary>
    /// <returns>Container name.</returns>
    public string StoreEntityContainerName { get; set; }

    /// <summary>Hash value.</summary>
    /// <returns>Hash value.</returns>
    public string HashOverMappingClosure { get; set; }

    /// <summary>Hash value of views.</summary>
    /// <returns>Hash value.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OverAll")]
    public string HashOverAllExtentViews { get; set; }

    /// <summary>Gets or sets view count.</summary>
    /// <returns>View count.</returns>
    public int ViewCount { get; protected set; }
  }
}
