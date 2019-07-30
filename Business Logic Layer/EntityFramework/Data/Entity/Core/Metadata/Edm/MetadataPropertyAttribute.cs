// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataPropertyAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  internal sealed class MetadataPropertyAttribute : Attribute
  {
    private readonly EdmType _type;
    private readonly bool _isCollectionType;

    internal MetadataPropertyAttribute(BuiltInTypeKind builtInTypeKind, bool isCollectionType)
      : this(MetadataItem.GetBuiltInType(builtInTypeKind), isCollectionType)
    {
    }

    internal MetadataPropertyAttribute(PrimitiveTypeKind primitiveTypeKind, bool isCollectionType)
      : this((EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(primitiveTypeKind), isCollectionType)
    {
    }

    internal MetadataPropertyAttribute(System.Type type, bool isCollection)
      : this((EdmType) ClrComplexType.CreateReadonlyClrComplexType(type, type.NestingNamespace() ?? string.Empty, type.Name), isCollection)
    {
    }

    private MetadataPropertyAttribute(EdmType type, bool isCollectionType)
    {
      this._type = type;
      this._isCollectionType = isCollectionType;
    }

    internal EdmType Type
    {
      get
      {
        return this._type;
      }
    }

    internal bool IsCollectionType
    {
      get
      {
        return this._isCollectionType;
      }
    }
  }
}
