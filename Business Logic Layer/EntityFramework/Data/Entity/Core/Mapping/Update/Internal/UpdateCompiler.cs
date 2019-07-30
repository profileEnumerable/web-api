// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.UpdateCompiler
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal sealed class UpdateCompiler
  {
    private const string s_targetVarName = "target";
    internal readonly UpdateTranslator m_translator;

    internal UpdateCompiler(UpdateTranslator translator)
    {
      this.m_translator = translator;
    }

    internal UpdateCommand BuildDeleteCommand(
      PropagatorResult oldRow,
      TableChangeProcessor processor)
    {
      bool rowMustBeTouched = true;
      DbExpressionBinding target = UpdateCompiler.GetTarget(processor);
      DbExpression predicate = this.BuildPredicate(target, oldRow, (PropagatorResult) null, processor, ref rowMustBeTouched);
      DbDeleteCommandTree deleteCommandTree = new DbDeleteCommandTree(this.m_translator.MetadataWorkspace, DataSpace.SSpace, target, predicate);
      return (UpdateCommand) new DynamicUpdateCommand(processor, this.m_translator, ModificationOperator.Delete, oldRow, (PropagatorResult) null, (DbModificationCommandTree) deleteCommandTree, (Dictionary<int, string>) null);
    }

    internal UpdateCommand BuildUpdateCommand(
      PropagatorResult oldRow,
      PropagatorResult newRow,
      TableChangeProcessor processor)
    {
      bool rowMustBeTouched = false;
      DbExpressionBinding target = UpdateCompiler.GetTarget(processor);
      List<DbModificationClause> modificationClauseList = new List<DbModificationClause>();
      Dictionary<int, string> outputIdentifiers;
      DbExpression returning;
      foreach (DbModificationClause buildSetClause in this.BuildSetClauses(target, newRow, oldRow, processor, false, out outputIdentifiers, out returning, ref rowMustBeTouched))
        modificationClauseList.Add(buildSetClause);
      DbExpression predicate = this.BuildPredicate(target, oldRow, newRow, processor, ref rowMustBeTouched);
      if (modificationClauseList.Count == 0)
      {
        if (rowMustBeTouched)
        {
          List<IEntityStateEntry> source = new List<IEntityStateEntry>();
          source.AddRange((IEnumerable<IEntityStateEntry>) SourceInterpreter.GetAllStateEntries(oldRow, this.m_translator, processor.Table));
          source.AddRange((IEnumerable<IEntityStateEntry>) SourceInterpreter.GetAllStateEntries(newRow, this.m_translator, processor.Table));
          if (source.All<IEntityStateEntry>((Func<IEntityStateEntry, bool>) (it => it.State == EntityState.Unchanged)))
            rowMustBeTouched = false;
        }
        if (!rowMustBeTouched)
          return (UpdateCommand) null;
      }
      DbUpdateCommandTree updateCommandTree = new DbUpdateCommandTree(this.m_translator.MetadataWorkspace, DataSpace.SSpace, target, predicate, new ReadOnlyCollection<DbModificationClause>((IList<DbModificationClause>) modificationClauseList), returning);
      return (UpdateCommand) new DynamicUpdateCommand(processor, this.m_translator, ModificationOperator.Update, oldRow, newRow, (DbModificationCommandTree) updateCommandTree, outputIdentifiers);
    }

    internal UpdateCommand BuildInsertCommand(
      PropagatorResult newRow,
      TableChangeProcessor processor)
    {
      DbExpressionBinding target = UpdateCompiler.GetTarget(processor);
      bool rowMustBeTouched = true;
      List<DbModificationClause> modificationClauseList = new List<DbModificationClause>();
      Dictionary<int, string> outputIdentifiers;
      DbExpression returning;
      foreach (DbModificationClause buildSetClause in this.BuildSetClauses(target, newRow, (PropagatorResult) null, processor, true, out outputIdentifiers, out returning, ref rowMustBeTouched))
        modificationClauseList.Add(buildSetClause);
      DbInsertCommandTree insertCommandTree = new DbInsertCommandTree(this.m_translator.MetadataWorkspace, DataSpace.SSpace, target, new ReadOnlyCollection<DbModificationClause>((IList<DbModificationClause>) modificationClauseList), returning);
      return (UpdateCommand) new DynamicUpdateCommand(processor, this.m_translator, ModificationOperator.Insert, (PropagatorResult) null, newRow, (DbModificationCommandTree) insertCommandTree, outputIdentifiers);
    }

    private IEnumerable<DbModificationClause> BuildSetClauses(
      DbExpressionBinding target,
      PropagatorResult row,
      PropagatorResult originalRow,
      TableChangeProcessor processor,
      bool insertMode,
      out Dictionary<int, string> outputIdentifiers,
      out DbExpression returning,
      ref bool rowMustBeTouched)
    {
      Dictionary<EdmProperty, PropagatorResult> dictionary = new Dictionary<EdmProperty, PropagatorResult>();
      List<KeyValuePair<string, DbExpression>> keyValuePairList = new List<KeyValuePair<string, DbExpression>>();
      outputIdentifiers = new Dictionary<int, string>();
      PropagatorFlags propagatorFlags1 = insertMode ? PropagatorFlags.NoFlags : PropagatorFlags.Preserve | PropagatorFlags.Unknown;
      for (int index1 = 0; index1 < processor.Table.ElementType.Properties.Count; ++index1)
      {
        EdmProperty property = processor.Table.ElementType.Properties[index1];
        PropagatorResult result = row.GetMemberValue(index1);
        if (-1 != result.Identifier)
          result = result.ReplicateResultWithNewValue(this.m_translator.KeyManager.GetPrincipalValue(result));
        bool flag1 = false;
        bool flag2 = false;
        for (int index2 = 0; index2 < processor.KeyOrdinals.Length; ++index2)
        {
          if (processor.KeyOrdinals[index2] == index1)
          {
            flag2 = true;
            break;
          }
        }
        PropagatorFlags propagatorFlags2 = PropagatorFlags.NoFlags;
        if (!insertMode && flag2)
          flag1 = true;
        else
          propagatorFlags2 |= result.PropagatorFlags;
        StoreGeneratedPattern generatedPattern = MetadataHelper.GetStoreGeneratedPattern((EdmMember) property);
        bool flag3 = generatedPattern == StoreGeneratedPattern.Computed || insertMode && generatedPattern == StoreGeneratedPattern.Identity;
        if (flag3)
        {
          DbPropertyExpression propertyExpression = target.Variable.Property(property);
          keyValuePairList.Add(new KeyValuePair<string, DbExpression>(property.Name, (DbExpression) propertyExpression));
          int identifier = result.Identifier;
          if (-1 != identifier)
          {
            if (this.m_translator.KeyManager.HasPrincipals(identifier))
              throw new InvalidOperationException(Strings.Update_GeneratedDependent((object) property.Name));
            outputIdentifiers.Add(identifier, property.Name);
            if (generatedPattern != StoreGeneratedPattern.Identity && processor.IsKeyProperty(index1))
              throw new NotSupportedException(Strings.Update_NotSupportedComputedKeyColumn((object) "StoreGeneratedPattern", (object) "Computed", (object) "Identity", (object) property.Name, (object) property.DeclaringType.FullName));
          }
        }
        if ((propagatorFlags2 & propagatorFlags1) != PropagatorFlags.NoFlags)
          flag1 = true;
        else if (flag3)
        {
          flag1 = true;
          rowMustBeTouched = true;
        }
        if (!flag1 && !insertMode && generatedPattern == StoreGeneratedPattern.Identity)
        {
          PropagatorResult memberValue = originalRow.GetMemberValue(index1);
          if (!ByValueEqualityComparer.Default.Equals(memberValue.GetSimpleValue(), result.GetSimpleValue()))
            throw new InvalidOperationException(Strings.Update_ModifyingIdentityColumn((object) "Identity", (object) property.Name, (object) property.DeclaringType.FullName));
          flag1 = true;
        }
        if (!flag1)
          dictionary.Add(property, result);
      }
      returning = 0 >= keyValuePairList.Count ? (DbExpression) null : (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList);
      List<DbModificationClause> modificationClauseList = new List<DbModificationClause>(dictionary.Count);
      foreach (KeyValuePair<EdmProperty, PropagatorResult> keyValuePair in dictionary)
      {
        EdmProperty key = keyValuePair.Key;
        modificationClauseList.Add((DbModificationClause) new DbSetClause(UpdateCompiler.GeneratePropertyExpression(target, keyValuePair.Key), this.GenerateValueExpression(keyValuePair.Key, keyValuePair.Value)));
      }
      return (IEnumerable<DbModificationClause>) modificationClauseList;
    }

    private DbExpression BuildPredicate(
      DbExpressionBinding target,
      PropagatorResult referenceRow,
      PropagatorResult current,
      TableChangeProcessor processor,
      ref bool rowMustBeTouched)
    {
      Dictionary<EdmProperty, PropagatorResult> dictionary = new Dictionary<EdmProperty, PropagatorResult>();
      int ordinal = 0;
      foreach (EdmProperty property in processor.Table.ElementType.Properties)
      {
        PropagatorResult memberValue = referenceRow.GetMemberValue(ordinal);
        PropagatorResult input = current == null ? (PropagatorResult) null : current.GetMemberValue(ordinal);
        if (!rowMustBeTouched && (UpdateCompiler.HasFlag(memberValue, PropagatorFlags.ConcurrencyValue) || UpdateCompiler.HasFlag(input, PropagatorFlags.ConcurrencyValue)))
          rowMustBeTouched = true;
        if (!dictionary.ContainsKey(property) && (UpdateCompiler.HasFlag(memberValue, PropagatorFlags.ConcurrencyValue | PropagatorFlags.Key) || UpdateCompiler.HasFlag(input, PropagatorFlags.ConcurrencyValue | PropagatorFlags.Key)))
          dictionary.Add(property, memberValue);
        ++ordinal;
      }
      DbExpression left = (DbExpression) null;
      foreach (KeyValuePair<EdmProperty, PropagatorResult> keyValuePair in dictionary)
      {
        DbExpression equalityExpression = this.GenerateEqualityExpression(target, keyValuePair.Key, keyValuePair.Value);
        left = left != null ? (DbExpression) left.And(equalityExpression) : equalityExpression;
      }
      return left;
    }

    private DbExpression GenerateEqualityExpression(
      DbExpressionBinding target,
      EdmProperty property,
      PropagatorResult value)
    {
      DbExpression propertyExpression = UpdateCompiler.GeneratePropertyExpression(target, property);
      DbExpression valueExpression = this.GenerateValueExpression(property, value);
      if (valueExpression.ExpressionKind == DbExpressionKind.Null)
        return (DbExpression) propertyExpression.IsNull();
      return (DbExpression) propertyExpression.Equal(valueExpression);
    }

    private static DbExpression GeneratePropertyExpression(
      DbExpressionBinding target,
      EdmProperty property)
    {
      return (DbExpression) target.Variable.Property(property);
    }

    private DbExpression GenerateValueExpression(
      EdmProperty property,
      PropagatorResult value)
    {
      if (value.IsNull)
        return (DbExpression) Helper.GetModelTypeUsage((EdmMember) property).Null();
      object obj = this.m_translator.KeyManager.GetPrincipalValue(value);
      if (Convert.IsDBNull(obj))
        return (DbExpression) Helper.GetModelTypeUsage((EdmMember) property).Null();
      TypeUsage modelTypeUsage = Helper.GetModelTypeUsage((EdmMember) property);
      Type type = obj.GetType();
      if (type.IsEnum())
        obj = Convert.ChangeType(obj, type.GetEnumUnderlyingType(), (IFormatProvider) CultureInfo.InvariantCulture);
      Type clrEquivalentType = ((PrimitiveType) modelTypeUsage.EdmType).ClrEquivalentType;
      if (type != clrEquivalentType)
        obj = Convert.ChangeType(obj, clrEquivalentType, (IFormatProvider) CultureInfo.InvariantCulture);
      return (DbExpression) modelTypeUsage.Constant(obj);
    }

    private static bool HasFlag(PropagatorResult input, PropagatorFlags flags)
    {
      if (input == null)
        return false;
      return PropagatorFlags.NoFlags != (flags & input.PropagatorFlags);
    }

    private static DbExpressionBinding GetTarget(TableChangeProcessor processor)
    {
      return processor.Table.Scan().BindAs("target");
    }
  }
}
