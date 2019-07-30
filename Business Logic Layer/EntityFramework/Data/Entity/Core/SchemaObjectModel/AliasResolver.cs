// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.AliasResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class AliasResolver
  {
    private readonly Dictionary<string, string> _aliasToNamespaceMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
    private readonly List<UsingElement> _usingElementCollection = new List<UsingElement>();
    private readonly Schema _definingSchema;

    public AliasResolver(Schema schema)
    {
      this._definingSchema = schema;
      if (string.IsNullOrEmpty(schema.Alias))
        return;
      this._aliasToNamespaceMap.Add(schema.Alias, schema.Namespace);
    }

    public void Add(UsingElement usingElement)
    {
      string name = usingElement.NamespaceName;
      string str = usingElement.Alias;
      if (this.CheckForSystemNamespace(usingElement, str, AliasResolver.NameKind.Alias))
        str = (string) null;
      if (this.CheckForSystemNamespace(usingElement, name, AliasResolver.NameKind.Namespace))
        name = (string) null;
      if (str != null && this._aliasToNamespaceMap.ContainsKey(str))
      {
        usingElement.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) Strings.AliasNameIsAlreadyDefined((object) str));
        str = (string) null;
      }
      if (str == null)
        return;
      this._aliasToNamespaceMap.Add(str, name);
      this._usingElementCollection.Add(usingElement);
    }

    public bool TryResolveAlias(string alias, out string namespaceName)
    {
      return this._aliasToNamespaceMap.TryGetValue(alias, out namespaceName);
    }

    public void ResolveNamespaces()
    {
      foreach (UsingElement usingElement in this._usingElementCollection)
      {
        if (!this._definingSchema.SchemaManager.IsValidNamespaceName(usingElement.NamespaceName))
          usingElement.AddError(ErrorCode.InvalidNamespaceInUsing, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidNamespaceInUsing((object) usingElement.NamespaceName));
      }
    }

    private bool CheckForSystemNamespace(
      UsingElement refSchema,
      string name,
      AliasResolver.NameKind nameKind)
    {
      if (!EdmItemCollection.IsSystemNamespace(this._definingSchema.ProviderManifest, name))
        return false;
      if (nameKind == AliasResolver.NameKind.Alias)
        refSchema.AddError(ErrorCode.CannotUseSystemNamespaceAsAlias, EdmSchemaErrorSeverity.Error, (object) Strings.CannotUseSystemNamespaceAsAlias((object) name));
      else
        refSchema.AddError(ErrorCode.NeedNotUseSystemNamespaceInUsing, EdmSchemaErrorSeverity.Error, (object) Strings.NeedNotUseSystemNamespaceInUsing((object) name));
      return true;
    }

    private enum NameKind
    {
      Alias,
      Namespace,
    }
  }
}
