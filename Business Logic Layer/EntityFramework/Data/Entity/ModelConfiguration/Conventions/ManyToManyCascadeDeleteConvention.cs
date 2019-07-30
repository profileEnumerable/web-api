// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to add a cascade delete to the join table from both tables involved in a many to many relationship.
  /// </summary>
  public class ManyToManyCascadeDeleteConvention : IDbMappingConvention, IConvention
  {
    void IDbMappingConvention.Apply(DbDatabaseMapping databaseMapping)
    {
      Check.NotNull<DbDatabaseMapping>(databaseMapping, nameof (databaseMapping));
      databaseMapping.EntityContainerMappings.SelectMany<EntityContainerMapping, AssociationSetMapping>((Func<EntityContainerMapping, IEnumerable<AssociationSetMapping>>) (ecm => ecm.AssociationSetMappings)).Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm =>
      {
        if (asm.AssociationSet.ElementType.IsManyToMany())
          return !asm.AssociationSet.ElementType.IsSelfReferencing();
        return false;
      })).SelectMany<AssociationSetMapping, ForeignKeyBuilder>((Func<AssociationSetMapping, IEnumerable<ForeignKeyBuilder>>) (asm => asm.Table.ForeignKeyBuilders)).Each<ForeignKeyBuilder, OperationAction>((Func<ForeignKeyBuilder, OperationAction>) (fk => fk.DeleteAction = OperationAction.Cascade));
    }
  }
}
