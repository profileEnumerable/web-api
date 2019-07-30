// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.DataRecordInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Common
{
  /// <summary>
  /// DataRecordInfo class providing a simple way to access both the type information and the column information.
  /// </summary>
  public class DataRecordInfo
  {
    private readonly ReadOnlyCollection<System.Data.Entity.Core.Common.FieldMetadata> _fieldMetadata;
    private readonly TypeUsage _metadata;

    internal DataRecordInfo()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Data.Common.DbDataRecord" /> object for a specific type with an enumerable collection of data fields.
    /// </summary>
    /// <param name="metadata">
    /// The metadata for the type represented by this object, supplied by
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// .
    /// </param>
    /// <param name="memberInfo">
    /// An enumerable collection of <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmMember" /> objects that represent column information.
    /// </param>
    public DataRecordInfo(TypeUsage metadata, IEnumerable<EdmMember> memberInfo)
    {
      Check.NotNull<TypeUsage>(metadata, nameof (metadata));
      IBaseList<EdmMember> structuralMembers = TypeHelpers.GetAllStructuralMembers(metadata.EdmType);
      List<System.Data.Entity.Core.Common.FieldMetadata> fieldMetadataList = new List<System.Data.Entity.Core.Common.FieldMetadata>(structuralMembers.Count);
      if (memberInfo != null)
      {
        foreach (EdmMember fieldType in memberInfo)
        {
          if (fieldType == null || 0 > structuralMembers.IndexOf(fieldType) || BuiltInTypeKind.EdmProperty != fieldType.BuiltInTypeKind && fieldType.BuiltInTypeKind != BuiltInTypeKind.AssociationEndMember)
            throw Error.InvalidEdmMemberInstance();
          if (fieldType.DeclaringType != metadata.EdmType && !fieldType.DeclaringType.IsBaseTypeOf(metadata.EdmType))
            throw new ArgumentException(Strings.EdmMembersDefiningTypeDoNotAgreeWithMetadataType);
          fieldMetadataList.Add(new System.Data.Entity.Core.Common.FieldMetadata(fieldMetadataList.Count, fieldType));
        }
      }
      if (Helper.IsStructuralType(metadata.EdmType) != 0 < fieldMetadataList.Count)
        throw Error.InvalidEdmMemberInstance();
      this._fieldMetadata = new ReadOnlyCollection<System.Data.Entity.Core.Common.FieldMetadata>((IList<System.Data.Entity.Core.Common.FieldMetadata>) fieldMetadataList);
      this._metadata = metadata;
    }

    internal DataRecordInfo(TypeUsage metadata)
    {
      IBaseList<EdmMember> structuralMembers = TypeHelpers.GetAllStructuralMembers(metadata);
      System.Data.Entity.Core.Common.FieldMetadata[] fieldMetadataArray = new System.Data.Entity.Core.Common.FieldMetadata[structuralMembers.Count];
      for (int ordinal = 0; ordinal < fieldMetadataArray.Length; ++ordinal)
      {
        EdmMember fieldType = structuralMembers[ordinal];
        fieldMetadataArray[ordinal] = new System.Data.Entity.Core.Common.FieldMetadata(ordinal, fieldType);
      }
      this._fieldMetadata = new ReadOnlyCollection<System.Data.Entity.Core.Common.FieldMetadata>((IList<System.Data.Entity.Core.Common.FieldMetadata>) fieldMetadataArray);
      this._metadata = metadata;
    }

    internal DataRecordInfo(DataRecordInfo recordInfo)
    {
      this._fieldMetadata = recordInfo._fieldMetadata;
      this._metadata = recordInfo._metadata;
    }

    /// <summary>
    /// Gets <see cref="T:System.Data.Entity.Core.Common.FieldMetadata" /> for this
    /// <see cref="P:System.Data.Entity.Core.IExtendedDataRecord.DataRecordInfo" />
    /// object.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.FieldMetadata" /> object.
    /// </returns>
    public ReadOnlyCollection<System.Data.Entity.Core.Common.FieldMetadata> FieldMetadata
    {
      get
      {
        return this._fieldMetadata;
      }
    }

    /// <summary>
    /// Gets type info for this object as a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> value.
    /// </returns>
    public virtual TypeUsage RecordType
    {
      get
      {
        return this._metadata;
      }
    }
  }
}
