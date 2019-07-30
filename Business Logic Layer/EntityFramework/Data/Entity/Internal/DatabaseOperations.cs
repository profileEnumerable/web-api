// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DatabaseOperations
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal
{
  internal class DatabaseOperations
  {
    public virtual bool Create(ObjectContext objectContext)
    {
      objectContext.CreateDatabase();
      return true;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public virtual bool Exists(
      DbConnection connection,
      int? commandTimeout,
      Lazy<StoreItemCollection> storeItemCollection)
    {
      if (connection.State == ConnectionState.Open)
        return true;
      try
      {
        return DbProviderServices.GetProviderServices(connection).DatabaseExists(connection, commandTimeout, storeItemCollection);
      }
      catch
      {
        try
        {
          connection.Open();
          return true;
        }
        catch (Exception ex)
        {
          return false;
        }
        finally
        {
          connection.Close();
        }
      }
    }

    public virtual void Delete(ObjectContext objectContext)
    {
      objectContext.DeleteDatabase();
    }
  }
}
