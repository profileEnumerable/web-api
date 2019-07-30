// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Utils.ExternalCalls
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Utils
{
  internal static class ExternalCalls
  {
    internal static bool IsReservedKeyword(string name)
    {
      return CqlLexer.IsReservedKeyword(name);
    }

    internal static DbCommandTree CompileView(
      string viewDef,
      StorageMappingItemCollection mappingItemCollection,
      ParserOptions.CompilationMode compilationMode)
    {
      Perspective perspective = (Perspective) new TargetPerspective(mappingItemCollection.Workspace);
      return CqlQuery.Compile(viewDef, perspective, new ParserOptions()
      {
        ParserCompilationMode = compilationMode
      }, (IEnumerable<DbParameterReferenceExpression>) null).CommandTree;
    }

    internal static DbExpression CompileFunctionView(
      string viewDef,
      StorageMappingItemCollection mappingItemCollection,
      ParserOptions.CompilationMode compilationMode,
      IEnumerable<DbParameterReferenceExpression> parameters)
    {
      Perspective perspective = (Perspective) new TargetPerspective(mappingItemCollection.Workspace);
      return (DbExpression) CqlQuery.CompileQueryCommandLambda(viewDef, perspective, new ParserOptions()
      {
        ParserCompilationMode = compilationMode
      }, (IEnumerable<DbParameterReferenceExpression>) null, parameters.Select<DbParameterReferenceExpression, DbVariableReferenceExpression>((Func<DbParameterReferenceExpression, DbVariableReferenceExpression>) (pInfo => pInfo.ResultType.Variable(pInfo.ParameterName)))).Invoke((IEnumerable<DbExpression>) parameters);
    }

    internal static DbLambda CompileFunctionDefinition(
      string functionDefinition,
      IList<FunctionParameter> functionParameters,
      EdmItemCollection edmItemCollection)
    {
      ModelPerspective modelPerspective = new ModelPerspective(new MetadataWorkspace((Func<EdmItemCollection>) (() => edmItemCollection), (Func<StoreItemCollection>) (() => (StoreItemCollection) null), (Func<StorageMappingItemCollection>) (() => (StorageMappingItemCollection) null)));
      return CqlQuery.CompileQueryCommandLambda(functionDefinition, (Perspective) modelPerspective, (ParserOptions) null, (IEnumerable<DbParameterReferenceExpression>) null, functionParameters.Select<FunctionParameter, DbVariableReferenceExpression>((Func<FunctionParameter, DbVariableReferenceExpression>) (pInfo => pInfo.TypeUsage.Variable(pInfo.Name))));
    }
  }
}
