// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Utils.ExceptionHelpers
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Utils
{
  internal static class ExceptionHelpers
  {
    internal static void ThrowMappingException(
      ErrorLog.Record errorRecord,
      ConfigViewGenerator config)
    {
      InternalMappingException mappingException = new InternalMappingException(errorRecord.ToUserString(), errorRecord);
      if (config.IsNormalTracing)
        mappingException.ErrorLog.PrintTrace();
      throw mappingException;
    }

    internal static void ThrowMappingException(ErrorLog errorLog, ConfigViewGenerator config)
    {
      InternalMappingException mappingException = new InternalMappingException(errorLog.ToUserString(), errorLog);
      if (config.IsNormalTracing)
        mappingException.ErrorLog.PrintTrace();
      throw mappingException;
    }
  }
}
