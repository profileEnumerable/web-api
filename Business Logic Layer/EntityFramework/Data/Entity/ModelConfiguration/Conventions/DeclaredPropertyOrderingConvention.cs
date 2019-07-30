// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.DeclaredPropertyOrderingConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Mappers;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to move primary key properties to appear first.
  /// </summary>
  public class DeclaredPropertyOrderingConvention : IConceptualModelConvention<EntityType>, IConvention
  {
    /// <inheritdoc />
    public virtual void Apply(EntityType item, DbModel model)
    {
      Check.NotNull<EntityType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      if (item.BaseType != null)
        return;
      foreach (EdmProperty keyProperty in item.KeyProperties)
      {
        item.RemoveMember((EdmMember) keyProperty);
        item.AddKeyMember((EdmMember) keyProperty);
      }
      foreach (PropertyInfo property in new PropertyFilter(DbModelBuilderVersion.Latest).GetProperties(EntityTypeExtensions.GetClrType(item), false, (IEnumerable<PropertyInfo>) null, (IEnumerable<Type>) null, true))
      {
        PropertyInfo p = property;
        EdmProperty edmProperty = item.DeclaredProperties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (ep => ep.Name == p.Name));
        if (edmProperty != null && !item.KeyProperties.Contains(edmProperty))
        {
          item.RemoveMember((EdmMember) edmProperty);
          item.AddMember((EdmMember) edmProperty);
        }
      }
    }
  }
}
