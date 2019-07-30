// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.Provider.EdmProviderManifestFunctionBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm.Provider
{
  internal sealed class EdmProviderManifestFunctionBuilder
  {
    private readonly List<EdmFunction> functions = new List<EdmFunction>();
    private readonly TypeUsage[] primitiveTypes;

    internal EdmProviderManifestFunctionBuilder(
      ReadOnlyCollection<PrimitiveType> edmPrimitiveTypes)
    {
      TypeUsage[] typeUsageArray = new TypeUsage[edmPrimitiveTypes.Count];
      foreach (PrimitiveType edmPrimitiveType in edmPrimitiveTypes)
        typeUsageArray[(int) edmPrimitiveType.PrimitiveTypeKind] = TypeUsage.Create((EdmType) edmPrimitiveType);
      this.primitiveTypes = typeUsageArray;
    }

    internal ReadOnlyCollection<EdmFunction> ToFunctionCollection()
    {
      return new ReadOnlyCollection<EdmFunction>((IList<EdmFunction>) this.functions);
    }

    internal static void ForAllBasePrimitiveTypes(Action<PrimitiveTypeKind> forEachType)
    {
      for (int index = 0; index < 31; ++index)
      {
        PrimitiveTypeKind kind = (PrimitiveTypeKind) index;
        if (!Helper.IsStrongSpatialTypeKind(kind))
          forEachType(kind);
      }
    }

    internal static void ForTypes(
      IEnumerable<PrimitiveTypeKind> typeKinds,
      Action<PrimitiveTypeKind> forEachType)
    {
      foreach (PrimitiveTypeKind typeKind in typeKinds)
        forEachType(typeKind);
    }

    internal void AddAggregate(
      string aggregateFunctionName,
      PrimitiveTypeKind collectionArgumentElementTypeKind)
    {
      this.AddAggregate(collectionArgumentElementTypeKind, aggregateFunctionName, collectionArgumentElementTypeKind);
    }

    internal void AddAggregate(
      PrimitiveTypeKind returnTypeKind,
      string aggregateFunctionName,
      PrimitiveTypeKind collectionArgumentElementTypeKind)
    {
      FunctionParameter returnParameter = this.CreateReturnParameter(returnTypeKind);
      FunctionParameter aggregateParameter = this.CreateAggregateParameter(collectionArgumentElementTypeKind);
      EdmFunction edmFunction = new EdmFunction(aggregateFunctionName, "Edm", DataSpace.CSpace, new EdmFunctionPayload()
      {
        IsAggregate = new bool?(true),
        IsBuiltIn = new bool?(true),
        ReturnParameters = (IList<FunctionParameter>) new FunctionParameter[1]
        {
          returnParameter
        },
        Parameters = (IList<FunctionParameter>) new FunctionParameter[1]
        {
          aggregateParameter
        },
        IsFromProviderManifest = new bool?(true)
      });
      edmFunction.SetReadOnly();
      this.functions.Add(edmFunction);
    }

    internal void AddFunction(PrimitiveTypeKind returnType, string functionName)
    {
      this.AddFunction(returnType, functionName, new KeyValuePair<string, PrimitiveTypeKind>[0]);
    }

    internal void AddFunction(
      PrimitiveTypeKind returnType,
      string functionName,
      PrimitiveTypeKind argumentTypeKind,
      string argumentName)
    {
      this.AddFunction(returnType, functionName, new KeyValuePair<string, PrimitiveTypeKind>[1]
      {
        new KeyValuePair<string, PrimitiveTypeKind>(argumentName, argumentTypeKind)
      });
    }

    internal void AddFunction(
      PrimitiveTypeKind returnType,
      string functionName,
      PrimitiveTypeKind argument1TypeKind,
      string argument1Name,
      PrimitiveTypeKind argument2TypeKind,
      string argument2Name)
    {
      this.AddFunction(returnType, functionName, new KeyValuePair<string, PrimitiveTypeKind>[2]
      {
        new KeyValuePair<string, PrimitiveTypeKind>(argument1Name, argument1TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument2Name, argument2TypeKind)
      });
    }

    internal void AddFunction(
      PrimitiveTypeKind returnType,
      string functionName,
      PrimitiveTypeKind argument1TypeKind,
      string argument1Name,
      PrimitiveTypeKind argument2TypeKind,
      string argument2Name,
      PrimitiveTypeKind argument3TypeKind,
      string argument3Name)
    {
      this.AddFunction(returnType, functionName, new KeyValuePair<string, PrimitiveTypeKind>[3]
      {
        new KeyValuePair<string, PrimitiveTypeKind>(argument1Name, argument1TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument2Name, argument2TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument3Name, argument3TypeKind)
      });
    }

    internal void AddFunction(
      PrimitiveTypeKind returnType,
      string functionName,
      PrimitiveTypeKind argument1TypeKind,
      string argument1Name,
      PrimitiveTypeKind argument2TypeKind,
      string argument2Name,
      PrimitiveTypeKind argument3TypeKind,
      string argument3Name,
      PrimitiveTypeKind argument4TypeKind,
      string argument4Name,
      PrimitiveTypeKind argument5TypeKind,
      string argument5Name,
      PrimitiveTypeKind argument6TypeKind,
      string argument6Name)
    {
      this.AddFunction(returnType, functionName, new KeyValuePair<string, PrimitiveTypeKind>[6]
      {
        new KeyValuePair<string, PrimitiveTypeKind>(argument1Name, argument1TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument2Name, argument2TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument3Name, argument3TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument4Name, argument4TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument5Name, argument5TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument6Name, argument6TypeKind)
      });
    }

    internal void AddFunction(
      PrimitiveTypeKind returnType,
      string functionName,
      PrimitiveTypeKind argument1TypeKind,
      string argument1Name,
      PrimitiveTypeKind argument2TypeKind,
      string argument2Name,
      PrimitiveTypeKind argument3TypeKind,
      string argument3Name,
      PrimitiveTypeKind argument4TypeKind,
      string argument4Name,
      PrimitiveTypeKind argument5TypeKind,
      string argument5Name,
      PrimitiveTypeKind argument6TypeKind,
      string argument6Name,
      PrimitiveTypeKind argument7TypeKind,
      string argument7Name)
    {
      this.AddFunction(returnType, functionName, new KeyValuePair<string, PrimitiveTypeKind>[7]
      {
        new KeyValuePair<string, PrimitiveTypeKind>(argument1Name, argument1TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument2Name, argument2TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument3Name, argument3TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument4Name, argument4TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument5Name, argument5TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument6Name, argument6TypeKind),
        new KeyValuePair<string, PrimitiveTypeKind>(argument7Name, argument7TypeKind)
      });
    }

    private void AddFunction(
      PrimitiveTypeKind returnType,
      string functionName,
      KeyValuePair<string, PrimitiveTypeKind>[] parameterDefinitions)
    {
      FunctionParameter returnParameter = this.CreateReturnParameter(returnType);
      FunctionParameter[] array = ((IEnumerable<KeyValuePair<string, PrimitiveTypeKind>>) parameterDefinitions).Select<KeyValuePair<string, PrimitiveTypeKind>, FunctionParameter>((Func<KeyValuePair<string, PrimitiveTypeKind>, FunctionParameter>) (paramDef => this.CreateParameter(paramDef.Value, paramDef.Key))).ToArray<FunctionParameter>();
      EdmFunction edmFunction = new EdmFunction(functionName, "Edm", DataSpace.CSpace, new EdmFunctionPayload()
      {
        IsBuiltIn = new bool?(true),
        ReturnParameters = (IList<FunctionParameter>) new FunctionParameter[1]
        {
          returnParameter
        },
        Parameters = (IList<FunctionParameter>) array,
        IsFromProviderManifest = new bool?(true)
      });
      edmFunction.SetReadOnly();
      this.functions.Add(edmFunction);
    }

    private FunctionParameter CreateParameter(
      PrimitiveTypeKind primitiveParameterType,
      string parameterName)
    {
      return new FunctionParameter(parameterName, this.primitiveTypes[(int) primitiveParameterType], ParameterMode.In);
    }

    private FunctionParameter CreateAggregateParameter(
      PrimitiveTypeKind collectionParameterTypeElementTypeKind)
    {
      return new FunctionParameter("collection", TypeUsage.Create((EdmType) this.primitiveTypes[(int) collectionParameterTypeElementTypeKind].EdmType.GetCollectionType()), ParameterMode.In);
    }

    private FunctionParameter CreateReturnParameter(
      PrimitiveTypeKind primitiveReturnType)
    {
      return new FunctionParameter("ReturnType", this.primitiveTypes[(int) primitiveReturnType], ParameterMode.ReturnValue);
    }
  }
}
