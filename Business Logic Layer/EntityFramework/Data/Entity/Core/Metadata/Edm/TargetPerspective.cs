// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.TargetPerspective
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class TargetPerspective : Perspective
  {
    internal const DataSpace TargetPerspectiveDataSpace = DataSpace.SSpace;
    private readonly ModelPerspective _modelPerspective;

    internal TargetPerspective(MetadataWorkspace metadataWorkspace)
      : base(metadataWorkspace, DataSpace.SSpace)
    {
      this._modelPerspective = new ModelPerspective(metadataWorkspace);
    }

    internal override bool TryGetTypeByName(string fullName, bool ignoreCase, out TypeUsage usage)
    {
      Check.NotEmpty(fullName, nameof (fullName));
      EdmType edmType = (EdmType) null;
      if (!this.MetadataWorkspace.TryGetItem<EdmType>(fullName, ignoreCase, this.TargetDataspace, out edmType))
        return this._modelPerspective.TryGetTypeByName(fullName, ignoreCase, out usage);
      usage = TypeUsage.Create(edmType);
      usage = Helper.GetModelTypeUsage(usage);
      return true;
    }

    internal override bool TryGetEntityContainer(
      string name,
      bool ignoreCase,
      out EntityContainer entityContainer)
    {
      if (!base.TryGetEntityContainer(name, ignoreCase, out entityContainer))
        return this._modelPerspective.TryGetEntityContainer(name, ignoreCase, out entityContainer);
      return true;
    }
  }
}
