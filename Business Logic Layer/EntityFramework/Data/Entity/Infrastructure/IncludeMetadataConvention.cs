// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IncludeMetadataConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// This <see cref="T:System.Data.Entity.DbModelBuilder" /> convention causes DbModelBuilder to include metadata about the model
  /// when it builds the model. When <see cref="T:System.Data.Entity.DbContext" /> creates a model by convention it will
  /// add this convention to the list of those used by the DbModelBuilder.  This will then result in
  /// model metadata being written to the database if the DbContext is used to create the database.
  /// This can then be used as a quick check to see if the model has changed since the last time it was
  /// used against the database.
  /// This convention can be removed from the <see cref="T:System.Data.Entity.DbModelBuilder" /> conventions by overriding
  /// the OnModelCreating method on a derived DbContext class.
  /// </summary>
  [Obsolete("The IncludeMetadataConvention is no longer used. EdmMetadata is not included in the model. <see cref=\"EdmModelDiffer\" /> is now used to detect changes in the model.")]
  public class IncludeMetadataConvention : Convention
  {
    internal virtual void Apply(System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      Check.NotNull<System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration>(modelConfiguration, nameof (modelConfiguration));
      EdmMetadataContext.ConfigureEdmMetadata(modelConfiguration);
    }
  }
}
