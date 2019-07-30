// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.RecordConverter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class RecordConverter
  {
    private readonly UpdateTranslator m_updateTranslator;

    internal RecordConverter(UpdateTranslator updateTranslator)
    {
      this.m_updateTranslator = updateTranslator;
    }

    internal PropagatorResult ConvertOriginalValuesToPropagatorResult(
      IEntityStateEntry stateEntry,
      ModifiedPropertiesBehavior modifiedPropertiesBehavior)
    {
      return this.ConvertStateEntryToPropagatorResult(stateEntry, false, modifiedPropertiesBehavior);
    }

    internal PropagatorResult ConvertCurrentValuesToPropagatorResult(
      IEntityStateEntry stateEntry,
      ModifiedPropertiesBehavior modifiedPropertiesBehavior)
    {
      return this.ConvertStateEntryToPropagatorResult(stateEntry, true, modifiedPropertiesBehavior);
    }

    private PropagatorResult ConvertStateEntryToPropagatorResult(
      IEntityStateEntry stateEntry,
      bool useCurrentValues,
      ModifiedPropertiesBehavior modifiedPropertiesBehavior)
    {
      try
      {
        IExtendedDataRecord record = useCurrentValues ? (IExtendedDataRecord) stateEntry.CurrentValues : (IExtendedDataRecord) stateEntry.OriginalValues;
        bool isModified = false;
        return ExtractorMetadata.ExtractResultFromRecord(stateEntry, isModified, record, useCurrentValues, this.m_updateTranslator, modifiedPropertiesBehavior);
      }
      catch (Exception ex)
      {
        if (ex.RequiresContext())
          throw EntityUtil.Update(Strings.Update_ErrorLoadingRecord, ex, stateEntry);
        throw;
      }
    }
  }
}
