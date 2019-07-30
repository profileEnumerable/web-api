// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.RecordStateScratchpad
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class RecordStateScratchpad
  {
    private readonly List<RecordStateScratchpad> _nestedRecordStateScratchpads = new List<RecordStateScratchpad>();

    internal int StateSlotNumber { get; set; }

    internal int ColumnCount { get; set; }

    internal DataRecordInfo DataRecordInfo { get; set; }

    internal Expression GatherData { get; set; }

    internal string[] PropertyNames { get; set; }

    internal TypeUsage[] TypeUsages { get; set; }

    internal RecordStateFactory Compile()
    {
      RecordStateFactory[] recordStateFactoryArray = new RecordStateFactory[this._nestedRecordStateScratchpads.Count];
      for (int index = 0; index < recordStateFactoryArray.Length; ++index)
        recordStateFactoryArray[index] = this._nestedRecordStateScratchpads[index].Compile();
      return (RecordStateFactory) Activator.CreateInstance(typeof (RecordStateFactory), (object) this.StateSlotNumber, (object) this.ColumnCount, (object) recordStateFactoryArray, (object) this.DataRecordInfo, (object) this.GatherData, (object) this.PropertyNames, (object) this.TypeUsages);
    }
  }
}
