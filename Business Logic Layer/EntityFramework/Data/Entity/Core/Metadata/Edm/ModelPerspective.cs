// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ModelPerspective
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class ModelPerspective : Perspective
  {
    internal ModelPerspective(MetadataWorkspace metadataWorkspace)
      : base(metadataWorkspace, DataSpace.CSpace)
    {
    }

    internal override bool TryGetTypeByName(
      string fullName,
      bool ignoreCase,
      out TypeUsage typeUsage)
    {
      Check.NotEmpty(fullName, nameof (fullName));
      typeUsage = (TypeUsage) null;
      EdmType edmType = (EdmType) null;
      if (this.MetadataWorkspace.TryGetItem<EdmType>(fullName, ignoreCase, this.TargetDataspace, out edmType))
        typeUsage = !Helper.IsPrimitiveType(edmType) ? TypeUsage.Create(edmType) : MetadataWorkspace.GetCanonicalModelTypeUsage(((PrimitiveType) edmType).PrimitiveTypeKind);
      return typeUsage != null;
    }
  }
}
