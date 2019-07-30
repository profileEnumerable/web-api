// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntityRecordInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common
{
  /// <summary>
  /// EntityRecordInfo class providing a simple way to access both the type information and the column information.
  /// </summary>
  public class EntityRecordInfo : DataRecordInfo
  {
    private readonly EntityKey _entityKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Common.EntityRecordInfo" /> class of a specific entity type with an enumerable collection of data fields and with specific key and entity set information.
    /// </summary>
    /// <param name="metadata">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" /> of the entity represented by the
    /// <see cref="T:System.Data.Common.DbDataRecord" />
    /// described by this
    /// <see cref="T:System.Data.Entity.Core.Common.EntityRecordInfo" />
    /// object.
    /// </param>
    /// <param name="memberInfo">
    /// An enumerable collection of <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmMember" /> objects that represent column information.
    /// </param>
    /// <param name="entityKey">The key for the entity.</param>
    /// <param name="entitySet">The entity set to which the entity belongs.</param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EntityRecordInfo(
      EntityType metadata,
      IEnumerable<EdmMember> memberInfo,
      EntityKey entityKey,
      EntitySet entitySet)
      : base(TypeUsage.Create((EdmType) metadata), memberInfo)
    {
      Check.NotNull<EntityKey>(entityKey, nameof (entityKey));
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      this._entityKey = entityKey;
      this.ValidateEntityType((EntitySetBase) entitySet);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "entitySet")]
    internal EntityRecordInfo(EntityType metadata, EntityKey entityKey, EntitySet entitySet)
      : base(TypeUsage.Create((EdmType) metadata))
    {
      this._entityKey = entityKey;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "entitySet")]
    internal EntityRecordInfo(DataRecordInfo info, EntityKey entityKey, EntitySet entitySet)
      : base(info)
    {
      this._entityKey = entityKey;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.EntityKey" /> for the entity.
    /// </summary>
    /// <returns>The key for the entity.</returns>
    public EntityKey EntityKey
    {
      get
      {
        return this._entityKey;
      }
    }

    private void ValidateEntityType(EntitySetBase entitySet)
    {
      if (!object.ReferenceEquals((object) this.RecordType.EdmType, (object) null) && !object.ReferenceEquals((object) this._entityKey, (object) EntityKey.EntityNotValidKey) && (!object.ReferenceEquals((object) this._entityKey, (object) EntityKey.NoEntitySetKey) && !object.ReferenceEquals((object) this.RecordType.EdmType, (object) entitySet.ElementType)) && !entitySet.ElementType.IsBaseTypeOf(this.RecordType.EdmType))
        throw new ArgumentException(Strings.EntityTypesDoNotAgree);
    }
  }
}
