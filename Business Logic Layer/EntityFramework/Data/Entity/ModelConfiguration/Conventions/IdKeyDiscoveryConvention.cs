// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.IdKeyDiscoveryConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to detect primary key properties.
  /// Recognized naming patterns in order of precedence are:
  /// 1. 'Id'
  /// 2. [type name]Id
  /// Primary key detection is case insensitive.
  /// </summary>
  public class IdKeyDiscoveryConvention : KeyDiscoveryConvention
  {
    private const string Id = "Id";

    /// <inheritdoc />
    protected override IEnumerable<EdmProperty> MatchKeyProperty(
      EntityType entityType,
      IEnumerable<EdmProperty> primitiveProperties)
    {
      Check.NotNull<EntityType>(entityType, nameof (entityType));
      Check.NotNull<IEnumerable<EdmProperty>>(primitiveProperties, nameof (primitiveProperties));
      IEnumerable<EdmProperty> source = primitiveProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => "Id".Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
      if (!source.Any<EdmProperty>())
        source = primitiveProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => (entityType.Name + "Id").Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
      if (source.Count<EdmProperty>() > 1)
        throw Error.MultiplePropertiesMatchedAsKeys((object) source.First<EdmProperty>().Name, (object) entityType.Name);
      return source;
    }
  }
}
