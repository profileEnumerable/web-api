// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.DbExpressionValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  internal sealed class DbExpressionValidator : DbExpressionRebinder
  {
    private readonly Dictionary<string, DbParameterReferenceExpression> paramMappings = new Dictionary<string, DbParameterReferenceExpression>();
    private readonly Stack<Dictionary<string, TypeUsage>> variableScopes = new Stack<Dictionary<string, TypeUsage>>();
    private readonly DataSpace requiredSpace;
    private readonly DataSpace[] allowedMetadataSpaces;
    private readonly DataSpace[] allowedFunctionSpaces;
    private string expressionArgumentName;

    internal DbExpressionValidator(MetadataWorkspace metadata, DataSpace expectedDataSpace)
      : base(metadata)
    {
      this.requiredSpace = expectedDataSpace;
      this.allowedFunctionSpaces = new DataSpace[2]
      {
        DataSpace.CSpace,
        DataSpace.SSpace
      };
      if (expectedDataSpace == DataSpace.SSpace)
        this.allowedMetadataSpaces = new DataSpace[2]
        {
          DataSpace.SSpace,
          DataSpace.CSpace
        };
      else
        this.allowedMetadataSpaces = new DataSpace[1]
        {
          DataSpace.CSpace
        };
    }

    internal Dictionary<string, DbParameterReferenceExpression> Parameters
    {
      get
      {
        return this.paramMappings;
      }
    }

    internal void ValidateExpression(DbExpression expression, string argumentName)
    {
      this.expressionArgumentName = argumentName;
      this.VisitExpression(expression);
      this.expressionArgumentName = (string) null;
    }

    protected override EntitySetBase VisitEntitySet(EntitySetBase entitySet)
    {
      return this.ValidateMetadata<EntitySetBase>(entitySet, new Func<EntitySetBase, EntitySetBase>(((DbExpressionRebinder) this).VisitEntitySet), (Func<EntitySetBase, DataSpace>) (es => es.EntityContainer.DataSpace), this.allowedMetadataSpaces);
    }

    protected override EdmFunction VisitFunction(EdmFunction function)
    {
      return this.ValidateMetadata<EdmFunction>(function, new Func<EdmFunction, EdmFunction>(((DbExpressionRebinder) this).VisitFunction), (Func<EdmFunction, DataSpace>) (func => func.DataSpace), this.allowedFunctionSpaces);
    }

    protected override EdmType VisitType(EdmType type)
    {
      return this.ValidateMetadata<EdmType>(type, new Func<EdmType, EdmType>(((DbExpressionRebinder) this).VisitType), (Func<EdmType, DataSpace>) (et => et.DataSpace), this.allowedMetadataSpaces);
    }

    protected override TypeUsage VisitTypeUsage(TypeUsage type)
    {
      return this.ValidateMetadata<TypeUsage>(type, new Func<TypeUsage, TypeUsage>(((DbExpressionRebinder) this).VisitTypeUsage), (Func<TypeUsage, DataSpace>) (tu => tu.EdmType.DataSpace), this.allowedMetadataSpaces);
    }

    protected override void OnEnterScope(
      IEnumerable<DbVariableReferenceExpression> scopeVariables)
    {
      this.variableScopes.Push(scopeVariables.ToDictionary<DbVariableReferenceExpression, string, TypeUsage>((Func<DbVariableReferenceExpression, string>) (var => var.VariableName), (Func<DbVariableReferenceExpression, TypeUsage>) (var => var.ResultType), (IEqualityComparer<string>) StringComparer.Ordinal));
    }

    protected override void OnExitScope()
    {
      this.variableScopes.Pop();
    }

    public override DbExpression Visit(DbVariableReferenceExpression expression)
    {
      Check.NotNull<DbVariableReferenceExpression>(expression, nameof (expression));
      DbExpression dbExpression = base.Visit(expression);
      if (dbExpression.ExpressionKind == DbExpressionKind.VariableReference)
      {
        DbVariableReferenceExpression referenceExpression = (DbVariableReferenceExpression) dbExpression;
        TypeUsage type2 = (TypeUsage) null;
        foreach (Dictionary<string, TypeUsage> variableScope in this.variableScopes)
        {
          if (variableScope.TryGetValue(referenceExpression.VariableName, out type2))
            break;
        }
        if (type2 == null)
          this.ThrowInvalid(Strings.Cqt_Validator_VarRefInvalid((object) referenceExpression.VariableName));
        if (!TypeSemantics.IsEqual(referenceExpression.ResultType, type2))
          this.ThrowInvalid(Strings.Cqt_Validator_VarRefTypeMismatch((object) referenceExpression.VariableName));
      }
      return dbExpression;
    }

    public override DbExpression Visit(DbParameterReferenceExpression expression)
    {
      Check.NotNull<DbParameterReferenceExpression>(expression, nameof (expression));
      DbExpression dbExpression = base.Visit(expression);
      if (dbExpression.ExpressionKind == DbExpressionKind.ParameterReference)
      {
        DbParameterReferenceExpression referenceExpression1 = dbExpression as DbParameterReferenceExpression;
        DbParameterReferenceExpression referenceExpression2;
        if (this.paramMappings.TryGetValue(referenceExpression1.ParameterName, out referenceExpression2))
        {
          if (!TypeSemantics.IsEqual(referenceExpression1.ResultType, referenceExpression2.ResultType))
            this.ThrowInvalid(Strings.Cqt_Validator_InvalidIncompatibleParameterReferences((object) referenceExpression1.ParameterName));
        }
        else
          this.paramMappings.Add(referenceExpression1.ParameterName, referenceExpression1);
      }
      return dbExpression;
    }

    private TMetadata ValidateMetadata<TMetadata>(
      TMetadata metadata,
      Func<TMetadata, TMetadata> map,
      Func<TMetadata, DataSpace> getDataSpace,
      DataSpace[] allowedSpaces)
    {
      TMetadata metadata1 = map(metadata);
      if (!object.ReferenceEquals((object) metadata, (object) metadata1))
        this.ThrowInvalidMetadata<TMetadata>();
      DataSpace resultSpace = getDataSpace(metadata1);
      if (!((IEnumerable<DataSpace>) allowedSpaces).Any<DataSpace>((Func<DataSpace, bool>) (ds => ds == resultSpace)))
        this.ThrowInvalidSpace<TMetadata>();
      return metadata1;
    }

    private void ThrowInvalidMetadata<TMetadata>()
    {
      this.ThrowInvalid(Strings.Cqt_Validator_InvalidOtherWorkspaceMetadata((object) typeof (TMetadata).Name));
    }

    private void ThrowInvalidSpace<TMetadata>()
    {
      this.ThrowInvalid(Strings.Cqt_Validator_InvalidIncorrectDataSpaceMetadata((object) typeof (TMetadata).Name, (object) Enum.GetName(typeof (DataSpace), (object) this.requiredSpace)));
    }

    private void ThrowInvalid(string message)
    {
      throw new ArgumentException(message, this.expressionArgumentName);
    }
  }
}
