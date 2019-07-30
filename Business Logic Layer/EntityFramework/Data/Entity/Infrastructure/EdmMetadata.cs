// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.EdmMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Represents an entity used to store metadata about an EDM in the database.
  /// </summary>
  [Obsolete("EdmMetadata is no longer used. The Code First Migrations <see cref=\"EdmModelDiffer\" /> is used instead.")]
  public class EdmMetadata
  {
    /// <summary>
    /// Gets or sets the ID of the metadata entity, which is currently always 1.
    /// </summary>
    /// <value> The id. </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the model hash which is used to check whether the model has
    /// changed since the database was created from it.
    /// </summary>
    /// <value> The model hash. </value>
    public string ModelHash { get; set; }

    /// <summary>
    /// Attempts to get the model hash calculated by Code First for the given context.
    /// This method will return null if the context is not being used in Code First mode.
    /// </summary>
    /// <param name="context"> The context. </param>
    /// <returns> The hash string. </returns>
    public static string TryGetModelHash(DbContext context)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      DbCompiledModel codeFirstModel = context.InternalContext.CodeFirstModel;
      if (codeFirstModel != null)
        return new ModelHashCalculator().Calculate(codeFirstModel);
      return (string) null;
    }
  }
}
