// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.TableChangeProcessor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class TableChangeProcessor
  {
    private readonly EntitySet m_table;
    private readonly int[] m_keyOrdinals;

    internal TableChangeProcessor(EntitySet table)
    {
      this.m_table = table;
      this.m_keyOrdinals = TableChangeProcessor.InitializeKeyOrdinals(table);
    }

    protected TableChangeProcessor()
    {
    }

    internal EntitySet Table
    {
      get
      {
        return this.m_table;
      }
    }

    internal int[] KeyOrdinals
    {
      get
      {
        return this.m_keyOrdinals;
      }
    }

    internal bool IsKeyProperty(int propertyOrdinal)
    {
      foreach (int keyOrdinal in this.m_keyOrdinals)
      {
        if (propertyOrdinal == keyOrdinal)
          return true;
      }
      return false;
    }

    private static int[] InitializeKeyOrdinals(EntitySet table)
    {
      EntityType elementType = table.ElementType;
      IList<EdmMember> keyMembers = (IList<EdmMember>) elementType.KeyMembers;
      IBaseList<EdmMember> structuralMembers = TypeHelpers.GetAllStructuralMembers((EdmType) elementType);
      int[] numArray = new int[keyMembers.Count];
      for (int index = 0; index < keyMembers.Count; ++index)
      {
        EdmMember edmMember = keyMembers[index];
        numArray[index] = structuralMembers.IndexOf(edmMember);
      }
      return numArray;
    }

    internal List<UpdateCommand> CompileCommands(
      ChangeNode changeNode,
      UpdateCompiler compiler)
    {
      Set<CompositeKey> keys = new Set<CompositeKey>(compiler.m_translator.KeyComparer);
      Dictionary<CompositeKey, PropagatorResult> dictionary1 = this.ProcessKeys(compiler, changeNode.Deleted, keys);
      Dictionary<CompositeKey, PropagatorResult> dictionary2 = this.ProcessKeys(compiler, changeNode.Inserted, keys);
      List<UpdateCommand> updateCommandList = new List<UpdateCommand>(dictionary1.Count + dictionary2.Count);
      foreach (CompositeKey key in keys)
      {
        PropagatorResult propagatorResult1;
        bool flag1 = dictionary1.TryGetValue(key, out propagatorResult1);
        PropagatorResult propagatorResult2;
        bool flag2 = dictionary2.TryGetValue(key, out propagatorResult2);
        try
        {
          if (!flag1)
            updateCommandList.Add(compiler.BuildInsertCommand(propagatorResult2, this));
          else if (!flag2)
          {
            updateCommandList.Add(compiler.BuildDeleteCommand(propagatorResult1, this));
          }
          else
          {
            UpdateCommand updateCommand = compiler.BuildUpdateCommand(propagatorResult1, propagatorResult2, this);
            if (updateCommand != null)
              updateCommandList.Add(updateCommand);
          }
        }
        catch (Exception ex)
        {
          if (ex.RequiresContext())
          {
            List<IEntityStateEntry> source = new List<IEntityStateEntry>();
            if (propagatorResult1 != null)
              source.AddRange((IEnumerable<IEntityStateEntry>) SourceInterpreter.GetAllStateEntries(propagatorResult1, compiler.m_translator, this.m_table));
            if (propagatorResult2 != null)
              source.AddRange((IEnumerable<IEntityStateEntry>) SourceInterpreter.GetAllStateEntries(propagatorResult2, compiler.m_translator, this.m_table));
            throw new UpdateException(Strings.Update_GeneralExecutionException, ex, source.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
          }
          throw;
        }
      }
      return updateCommandList;
    }

    private Dictionary<CompositeKey, PropagatorResult> ProcessKeys(
      UpdateCompiler compiler,
      List<PropagatorResult> changes,
      Set<CompositeKey> keys)
    {
      Dictionary<CompositeKey, PropagatorResult> dictionary = new Dictionary<CompositeKey, PropagatorResult>(compiler.m_translator.KeyComparer);
      foreach (PropagatorResult change in changes)
      {
        PropagatorResult row = change;
        CompositeKey compositeKey = new CompositeKey(this.GetKeyConstants(row));
        PropagatorResult other;
        if (dictionary.TryGetValue(compositeKey, out other))
          this.DiagnoseKeyCollision(compiler, change, compositeKey, other);
        dictionary.Add(compositeKey, row);
        keys.Add(compositeKey);
      }
      return dictionary;
    }

    [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCode", Justification = "Based on Bug VSTS Pioneer #433188: IsVisibleOutsideAssembly is wrong on generic instantiations.")]
    private void DiagnoseKeyCollision(
      UpdateCompiler compiler,
      PropagatorResult change,
      CompositeKey key,
      PropagatorResult other)
    {
      KeyManager keyManager = compiler.m_translator.KeyManager;
      CompositeKey compositeKey = new CompositeKey(this.GetKeyConstants(other));
      bool flag = true;
      for (int index = 0; flag && index < key.KeyComponents.Length; ++index)
      {
        int identifier1 = key.KeyComponents[index].Identifier;
        int identifier2 = compositeKey.KeyComponents[index].Identifier;
        if (!keyManager.GetPrincipals(identifier1).Intersect<int>(keyManager.GetPrincipals(identifier2)).Any<int>())
          flag = false;
      }
      if (flag)
        throw new UpdateException(Strings.Update_DuplicateKeys, (Exception) null, SourceInterpreter.GetAllStateEntries(change, compiler.m_translator, this.m_table).Concat<IEntityStateEntry>((IEnumerable<IEntityStateEntry>) SourceInterpreter.GetAllStateEntries(other, compiler.m_translator, this.m_table)).Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
      HashSet<IEntityStateEntry> source = (HashSet<IEntityStateEntry>) null;
      foreach (PropagatorResult propagatorResult in ((IEnumerable<PropagatorResult>) key.KeyComponents).Concat<PropagatorResult>((IEnumerable<PropagatorResult>) compositeKey.KeyComponents))
      {
        HashSet<IEntityStateEntry> entityStateEntrySet = new HashSet<IEntityStateEntry>();
        foreach (int dependent in keyManager.GetDependents(propagatorResult.Identifier))
        {
          PropagatorResult owner;
          if (keyManager.TryGetIdentifierOwner(dependent, out owner) && owner.StateEntry != null)
            entityStateEntrySet.Add(owner.StateEntry);
        }
        if (source == null)
          source = new HashSet<IEntityStateEntry>((IEnumerable<IEntityStateEntry>) entityStateEntrySet);
        else
          source.IntersectWith((IEnumerable<IEntityStateEntry>) entityStateEntrySet);
      }
      throw new UpdateException(Strings.Update_GeneralExecutionException, (Exception) new ConstraintException(Strings.Update_ReferentialConstraintIntegrityViolation), source.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
    }

    private PropagatorResult[] GetKeyConstants(PropagatorResult row)
    {
      PropagatorResult[] propagatorResultArray = new PropagatorResult[this.m_keyOrdinals.Length];
      for (int index = 0; index < this.m_keyOrdinals.Length; ++index)
      {
        PropagatorResult memberValue = row.GetMemberValue(this.m_keyOrdinals[index]);
        propagatorResultArray[index] = memberValue;
      }
      return propagatorResultArray;
    }
  }
}
