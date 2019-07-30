// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.CodeFirstOSpaceTypeFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class CodeFirstOSpaceTypeFactory : OSpaceTypeFactory
  {
    private readonly List<Action> _referenceResolutions = new List<Action>();
    private readonly Dictionary<EdmType, EdmType> _cspaceToOspace = new Dictionary<EdmType, EdmType>();
    private readonly Dictionary<string, EdmType> _loadedTypes = new Dictionary<string, EdmType>();

    public override List<Action> ReferenceResolutions
    {
      get
      {
        return this._referenceResolutions;
      }
    }

    public override void LogLoadMessage(string message, EdmType relatedType)
    {
    }

    public override void LogError(string errorMessage, EdmType relatedType)
    {
      throw new MetadataException(Strings.InvalidSchemaEncountered((object) errorMessage));
    }

    public override void TrackClosure(Type type)
    {
    }

    public override Dictionary<EdmType, EdmType> CspaceToOspace
    {
      get
      {
        return this._cspaceToOspace;
      }
    }

    public override Dictionary<string, EdmType> LoadedTypes
    {
      get
      {
        return this._loadedTypes;
      }
    }

    public override void AddToTypesInAssembly(EdmType type)
    {
    }
  }
}
