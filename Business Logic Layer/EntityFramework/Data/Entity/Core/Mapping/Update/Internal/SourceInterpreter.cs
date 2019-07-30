// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.SourceInterpreter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class SourceInterpreter
  {
    private readonly List<IEntityStateEntry> m_stateEntries;
    private readonly UpdateTranslator m_translator;
    private readonly EntitySet m_sourceTable;

    private SourceInterpreter(UpdateTranslator translator, EntitySet sourceTable)
    {
      this.m_stateEntries = new List<IEntityStateEntry>();
      this.m_translator = translator;
      this.m_sourceTable = sourceTable;
    }

    internal static ReadOnlyCollection<IEntityStateEntry> GetAllStateEntries(
      PropagatorResult source,
      UpdateTranslator translator,
      EntitySet sourceTable)
    {
      SourceInterpreter sourceInterpreter = new SourceInterpreter(translator, sourceTable);
      sourceInterpreter.RetrieveResultMarkup(source);
      return new ReadOnlyCollection<IEntityStateEntry>((IList<IEntityStateEntry>) sourceInterpreter.m_stateEntries);
    }

    private void RetrieveResultMarkup(PropagatorResult source)
    {
      if (source.Identifier != -1)
      {
        do
        {
          if (source.StateEntry != null)
          {
            this.m_stateEntries.Add(source.StateEntry);
            if (source.Identifier != -1)
            {
              PropagatorResult owner;
              if (this.m_translator.KeyManager.TryGetIdentifierOwner(source.Identifier, out owner) && owner.StateEntry != null && this.ExtentInScope(owner.StateEntry.EntitySet))
                this.m_stateEntries.Add(owner.StateEntry);
              foreach (IEntityStateEntry dependentStateEntry in this.m_translator.KeyManager.GetDependentStateEntries(source.Identifier))
                this.m_stateEntries.Add(dependentStateEntry);
            }
          }
          source = source.Next;
        }
        while (source != null);
      }
      else
      {
        if (source.IsSimple || source.IsNull)
          return;
        foreach (PropagatorResult memberValue in source.GetMemberValues())
          this.RetrieveResultMarkup(memberValue);
      }
    }

    private bool ExtentInScope(EntitySetBase extent)
    {
      if (extent == null)
        return false;
      return this.m_translator.ViewLoader.GetAffectedTables(extent, this.m_translator.MetadataWorkspace).Contains(this.m_sourceTable);
    }
  }
}
