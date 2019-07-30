// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ModelCompatibilityChecker
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Internal
{
  internal class ModelCompatibilityChecker
  {
    public virtual bool CompatibleWithModel(
      InternalContext internalContext,
      ModelHashCalculator modelHashCalculator,
      bool throwIfNoMetadata,
      DatabaseExistenceState existenceState = DatabaseExistenceState.Unknown)
    {
      if (internalContext.CodeFirstModel == null)
      {
        if (throwIfNoMetadata)
          throw Error.Database_NonCodeFirstCompatibilityCheck();
        return true;
      }
      VersionedModel model = internalContext.QueryForModel(existenceState);
      if (model != null)
        return internalContext.ModelMatches(model);
      string a = internalContext.QueryForModelHash();
      if (a != null)
        return string.Equals(a, modelHashCalculator.Calculate(internalContext.CodeFirstModel), StringComparison.Ordinal);
      if (throwIfNoMetadata)
        throw Error.Database_NoDatabaseMetadata();
      return true;
    }
  }
}
