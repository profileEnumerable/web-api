// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.TypeResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class TypeResolver
  {
    private readonly Perspective _perspective;
    private readonly ParserOptions _parserOptions;
    private readonly Dictionary<string, MetadataNamespace> _aliasedNamespaces;
    private readonly HashSet<MetadataNamespace> _namespaces;
    private readonly Dictionary<string, List<InlineFunctionInfo>> _functionDefinitions;
    private bool _includeInlineFunctions;
    private bool _resolveLeftMostUnqualifiedNameAsNamespaceOnly;

    internal TypeResolver(Perspective perspective, ParserOptions parserOptions)
    {
      this._perspective = perspective;
      this._parserOptions = parserOptions;
      this._aliasedNamespaces = new Dictionary<string, MetadataNamespace>((IEqualityComparer<string>) parserOptions.NameComparer);
      this._namespaces = new HashSet<MetadataNamespace>((IEqualityComparer<MetadataNamespace>) MetadataMember.CreateMetadataMemberNameEqualityComparer(parserOptions.NameComparer));
      this._functionDefinitions = new Dictionary<string, List<InlineFunctionInfo>>((IEqualityComparer<string>) parserOptions.NameComparer);
      this._includeInlineFunctions = true;
      this._resolveLeftMostUnqualifiedNameAsNamespaceOnly = false;
    }

    internal Perspective Perspective
    {
      get
      {
        return this._perspective;
      }
    }

    internal ICollection<MetadataNamespace> NamespaceImports
    {
      get
      {
        return (ICollection<MetadataNamespace>) this._namespaces;
      }
    }

    internal static TypeUsage StringType
    {
      get
      {
        return MetadataWorkspace.GetCanonicalModelTypeUsage(PrimitiveTypeKind.String);
      }
    }

    internal static TypeUsage BooleanType
    {
      get
      {
        return MetadataWorkspace.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Boolean);
      }
    }

    internal static TypeUsage Int64Type
    {
      get
      {
        return MetadataWorkspace.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Int64);
      }
    }

    internal void AddAliasedNamespaceImport(
      string alias,
      MetadataNamespace @namespace,
      ErrorContext errCtx)
    {
      if (this._aliasedNamespaces.ContainsKey(alias))
      {
        string errorMessage = Strings.NamespaceAliasAlreadyUsed((object) alias);
        throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
      }
      this._aliasedNamespaces.Add(alias, @namespace);
    }

    internal void AddNamespaceImport(MetadataNamespace @namespace, ErrorContext errCtx)
    {
      if (this._namespaces.Contains(@namespace))
      {
        string errorMessage = Strings.NamespaceAlreadyImported((object) @namespace.Name);
        throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
      }
      this._namespaces.Add(@namespace);
    }

    internal void DeclareInlineFunction(string name, InlineFunctionInfo functionInfo)
    {
      List<InlineFunctionInfo> inlineFunctionInfoList;
      if (!this._functionDefinitions.TryGetValue(name, out inlineFunctionInfoList))
      {
        inlineFunctionInfoList = new List<InlineFunctionInfo>();
        this._functionDefinitions.Add(name, inlineFunctionInfoList);
      }
      if (inlineFunctionInfoList.Exists((Predicate<InlineFunctionInfo>) (overload => overload.Parameters.Select<DbVariableReferenceExpression, TypeUsage>((Func<DbVariableReferenceExpression, TypeUsage>) (p => p.ResultType)).SequenceEqual<TypeUsage>(functionInfo.Parameters.Select<DbVariableReferenceExpression, TypeUsage>((Func<DbVariableReferenceExpression, TypeUsage>) (p => p.ResultType)), (IEqualityComparer<TypeUsage>) TypeResolver.TypeUsageStructuralComparer.Instance))))
        throw EntitySqlException.Create(functionInfo.FunctionDefAst.ErrCtx, Strings.DuplicatedInlineFunctionOverload((object) name), (Exception) null);
      inlineFunctionInfoList.Add(functionInfo);
    }

    internal IDisposable EnterFunctionNameResolution(bool includeInlineFunctions)
    {
      bool savedIncludeInlineFunctions = this._includeInlineFunctions;
      this._includeInlineFunctions = includeInlineFunctions;
      return (IDisposable) new Disposer((Action) (() => this._includeInlineFunctions = savedIncludeInlineFunctions));
    }

    internal IDisposable EnterBackwardCompatibilityResolution()
    {
      this._resolveLeftMostUnqualifiedNameAsNamespaceOnly = true;
      return (IDisposable) new Disposer((Action) (() => this._resolveLeftMostUnqualifiedNameAsNamespaceOnly = false));
    }

    internal MetadataMember ResolveMetadataMemberName(
      string[] name,
      ErrorContext errCtx)
    {
      return name.Length != 1 ? this.ResolveFullyQualifiedName(name, name.Length, errCtx) : this.ResolveUnqualifiedName(name[0], false, errCtx);
    }

    internal MetadataMember ResolveMetadataMemberAccess(
      MetadataMember qualifier,
      string name,
      ErrorContext errCtx)
    {
      string fullName = TypeResolver.GetFullName(qualifier.Name, name);
      if (qualifier.MetadataMemberClass == MetadataMemberClass.Namespace)
      {
        MetadataType type;
        if (this.TryGetTypeFromMetadata(fullName, out type))
          return (MetadataMember) type;
        MetadataFunctionGroup functionGroup;
        if (this.TryGetFunctionFromMetadata(qualifier.Name, name, out functionGroup))
          return (MetadataMember) functionGroup;
        return (MetadataMember) new MetadataNamespace(fullName);
      }
      if (qualifier.MetadataMemberClass == MetadataMemberClass.Type)
      {
        MetadataType metadataType = (MetadataType) qualifier;
        if (TypeSemantics.IsEnumerationType(metadataType.TypeUsage))
        {
          EnumMember outMember;
          if (this._perspective.TryGetEnumMember((EnumType) metadataType.TypeUsage.EdmType, name, this._parserOptions.NameComparisonCaseInsensitive, out outMember))
            return (MetadataMember) new MetadataEnumMember(fullName, metadataType.TypeUsage, outMember);
          string errorMessage = Strings.NotAMemberOfType((object) name, (object) qualifier.Name);
          throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
        }
      }
      string errorMessage1 = Strings.InvalidMetadataMemberClassResolution((object) qualifier.Name, (object) qualifier.MetadataMemberClassName, (object) MetadataNamespace.NamespaceClassName);
      throw EntitySqlException.Create(errCtx, errorMessage1, (Exception) null);
    }

    internal MetadataMember ResolveUnqualifiedName(
      string name,
      bool partOfQualifiedName,
      ErrorContext errCtx)
    {
      bool flag1 = partOfQualifiedName && this._resolveLeftMostUnqualifiedNameAsNamespaceOnly;
      bool flag2 = !partOfQualifiedName;
      InlineFunctionGroup inlineFunctionGroup;
      if (!flag1 && flag2 && this.TryGetInlineFunction(name, out inlineFunctionGroup))
        return (MetadataMember) inlineFunctionGroup;
      MetadataNamespace metadataNamespace;
      if (this._aliasedNamespaces.TryGetValue(name, out metadataNamespace))
        return (MetadataMember) metadataNamespace;
      if (!flag1)
      {
        MetadataType type1 = (MetadataType) null;
        MetadataFunctionGroup functionGroup1 = (MetadataFunctionGroup) null;
        if (!this.TryGetTypeFromMetadata(name, out type1) && flag2)
        {
          string[] strArray = name.Split('.');
          if (strArray.Length > 1 && ((IEnumerable<string>) strArray).All<string>((Func<string, bool>) (p => p.Length > 0)))
          {
            string functionName = strArray[strArray.Length - 1];
            this.TryGetFunctionFromMetadata(name.Substring(0, name.Length - functionName.Length - 1), functionName, out functionGroup1);
          }
        }
        MetadataNamespace ns2 = (MetadataNamespace) null;
        foreach (MetadataNamespace ns1 in this._namespaces)
        {
          MetadataType type2;
          if (this.TryGetTypeFromMetadata(TypeResolver.GetFullName(ns1.Name, name), out type2))
          {
            if (type1 != null || functionGroup1 != null)
              throw TypeResolver.AmbiguousMetadataMemberName(errCtx, name, ns1, ns2);
            type1 = type2;
            ns2 = ns1;
          }
          MetadataFunctionGroup functionGroup2;
          if (flag2 && this.TryGetFunctionFromMetadata(ns1.Name, name, out functionGroup2))
          {
            if (type1 != null || functionGroup1 != null)
              throw TypeResolver.AmbiguousMetadataMemberName(errCtx, name, ns1, ns2);
            functionGroup1 = functionGroup2;
            ns2 = ns1;
          }
        }
        if (type1 != null)
          return (MetadataMember) type1;
        if (functionGroup1 != null)
          return (MetadataMember) functionGroup1;
      }
      return (MetadataMember) new MetadataNamespace(name);
    }

    private MetadataMember ResolveFullyQualifiedName(
      string[] name,
      int length,
      ErrorContext errCtx)
    {
      return this.ResolveMetadataMemberAccess(length != 2 ? this.ResolveFullyQualifiedName(name, length - 1, errCtx) : this.ResolveUnqualifiedName(name[0], true, errCtx), name[length - 1], errCtx);
    }

    private static Exception AmbiguousMetadataMemberName(
      ErrorContext errCtx,
      string name,
      MetadataNamespace ns1,
      MetadataNamespace ns2)
    {
      string errorMessage = Strings.AmbiguousMetadataMemberName((object) name, (object) ns1.Name, (object) ns2?.Name);
      throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
    }

    private bool TryGetTypeFromMetadata(string typeFullName, out MetadataType type)
    {
      TypeUsage typeUsage;
      if (this._perspective.TryGetTypeByName(typeFullName, this._parserOptions.NameComparisonCaseInsensitive, out typeUsage))
      {
        type = new MetadataType(typeFullName, typeUsage);
        return true;
      }
      type = (MetadataType) null;
      return false;
    }

    internal bool TryGetFunctionFromMetadata(
      string namespaceName,
      string functionName,
      out MetadataFunctionGroup functionGroup)
    {
      IList<EdmFunction> functionOverloads;
      if (this._perspective.TryGetFunctionByName(namespaceName, functionName, this._parserOptions.NameComparisonCaseInsensitive, out functionOverloads))
      {
        functionGroup = new MetadataFunctionGroup(TypeResolver.GetFullName(namespaceName, functionName), functionOverloads);
        return true;
      }
      functionGroup = (MetadataFunctionGroup) null;
      return false;
    }

    private bool TryGetInlineFunction(
      string functionName,
      out InlineFunctionGroup inlineFunctionGroup)
    {
      List<InlineFunctionInfo> inlineFunctionInfoList;
      if (this._includeInlineFunctions && this._functionDefinitions.TryGetValue(functionName, out inlineFunctionInfoList))
      {
        inlineFunctionGroup = new InlineFunctionGroup(functionName, (IList<InlineFunctionInfo>) inlineFunctionInfoList);
        return true;
      }
      inlineFunctionGroup = (InlineFunctionGroup) null;
      return false;
    }

    internal static string GetFullName(params string[] names)
    {
      return string.Join(".", names);
    }

    private sealed class TypeUsageStructuralComparer : IEqualityComparer<TypeUsage>
    {
      private static readonly TypeResolver.TypeUsageStructuralComparer _instance = new TypeResolver.TypeUsageStructuralComparer();

      private TypeUsageStructuralComparer()
      {
      }

      public static TypeResolver.TypeUsageStructuralComparer Instance
      {
        get
        {
          return TypeResolver.TypeUsageStructuralComparer._instance;
        }
      }

      public bool Equals(TypeUsage x, TypeUsage y)
      {
        return TypeSemantics.IsStructurallyEqual(x, y);
      }

      public int GetHashCode(TypeUsage obj)
      {
        return 0;
      }
    }
  }
}
