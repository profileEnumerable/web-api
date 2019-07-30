// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.Perspective
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal abstract class Perspective
  {
    private readonly MetadataWorkspace _metadataWorkspace;
    private readonly DataSpace _targetDataspace;

    internal Perspective(MetadataWorkspace metadataWorkspace, DataSpace targetDataspace)
    {
      this._metadataWorkspace = metadataWorkspace;
      this._targetDataspace = targetDataspace;
    }

    internal virtual bool TryGetMember(
      StructuralType type,
      string memberName,
      bool ignoreCase,
      out EdmMember outMember)
    {
      Check.NotEmpty(memberName, nameof (memberName));
      outMember = (EdmMember) null;
      return type.Members.TryGetValue(memberName, ignoreCase, out outMember);
    }

    internal virtual bool TryGetEnumMember(
      EnumType type,
      string memberName,
      bool ignoreCase,
      out EnumMember outMember)
    {
      Check.NotEmpty(memberName, nameof (memberName));
      outMember = (EnumMember) null;
      return type.Members.TryGetValue(memberName, ignoreCase, out outMember);
    }

    internal virtual bool TryGetExtent(
      EntityContainer entityContainer,
      string extentName,
      bool ignoreCase,
      out EntitySetBase outSet)
    {
      return entityContainer.BaseEntitySets.TryGetValue(extentName, ignoreCase, out outSet);
    }

    internal virtual bool TryGetFunctionImport(
      EntityContainer entityContainer,
      string functionImportName,
      bool ignoreCase,
      out EdmFunction functionImport)
    {
      functionImport = (EdmFunction) null;
      functionImport = !ignoreCase ? entityContainer.FunctionImports.Where<EdmFunction>((Func<EdmFunction, bool>) (fi => fi.Name == functionImportName)).SingleOrDefault<EdmFunction>() : entityContainer.FunctionImports.Where<EdmFunction>((Func<EdmFunction, bool>) (fi => string.Equals(fi.Name, functionImportName, StringComparison.OrdinalIgnoreCase))).SingleOrDefault<EdmFunction>();
      return functionImport != null;
    }

    internal virtual EntityContainer GetDefaultContainer()
    {
      return (EntityContainer) null;
    }

    internal virtual bool TryGetEntityContainer(
      string name,
      bool ignoreCase,
      out EntityContainer entityContainer)
    {
      return this.MetadataWorkspace.TryGetEntityContainer(name, ignoreCase, this.TargetDataspace, out entityContainer);
    }

    internal abstract bool TryGetTypeByName(
      string fullName,
      bool ignoreCase,
      out TypeUsage typeUsage);

    internal bool TryGetFunctionByName(
      string namespaceName,
      string functionName,
      bool ignoreCase,
      out IList<EdmFunction> functionOverloads)
    {
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      Check.NotEmpty(functionName, nameof (functionName));
      string functionName1 = namespaceName + "." + functionName;
      ItemCollection itemCollection = this._metadataWorkspace.GetItemCollection(this._targetDataspace);
      IList<EdmFunction> edmFunctionList = this._targetDataspace == DataSpace.SSpace ? (IList<EdmFunction>) ((StoreItemCollection) itemCollection).GetCTypeFunctions(functionName1, ignoreCase) : (IList<EdmFunction>) itemCollection.GetFunctions(functionName1, ignoreCase);
      if (this._targetDataspace == DataSpace.CSpace)
      {
        EntityContainer entityContainer;
        EdmFunction functionImport;
        if ((edmFunctionList == null || edmFunctionList.Count == 0) && (this.TryGetEntityContainer(namespaceName, false, out entityContainer) && this.TryGetFunctionImport(entityContainer, functionName, false, out functionImport)))
          edmFunctionList = (IList<EdmFunction>) new EdmFunction[1]
          {
            functionImport
          };
        ItemCollection collection;
        if ((edmFunctionList == null || edmFunctionList.Count == 0) && this._metadataWorkspace.TryGetItemCollection(DataSpace.SSpace, out collection))
          edmFunctionList = (IList<EdmFunction>) ((StoreItemCollection) collection).GetCTypeFunctions(functionName1, ignoreCase);
      }
      functionOverloads = edmFunctionList == null || edmFunctionList.Count <= 0 ? (IList<EdmFunction>) null : edmFunctionList;
      return functionOverloads != null;
    }

    internal MetadataWorkspace MetadataWorkspace
    {
      get
      {
        return this._metadataWorkspace;
      }
    }

    internal virtual bool TryGetMappedPrimitiveType(
      PrimitiveTypeKind primitiveTypeKind,
      out PrimitiveType primitiveType)
    {
      primitiveType = this._metadataWorkspace.GetMappedPrimitiveType(primitiveTypeKind, DataSpace.CSpace);
      return null != primitiveType;
    }

    internal DataSpace TargetDataspace
    {
      get
      {
        return this._targetDataspace;
      }
    }
  }
}
