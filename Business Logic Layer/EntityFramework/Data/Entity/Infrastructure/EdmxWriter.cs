// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.EdmxWriter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.ModelConfiguration.Edm.Serialization;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Xml;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Contains methods used to access the Entity Data Model created by Code First in the EDMX form.
  /// These methods are typically used for debugging when there is a need to look at the model that
  /// Code First creates internally.
  /// </summary>
  public static class EdmxWriter
  {
    /// <summary>
    /// Uses Code First with the given context and writes the resulting Entity Data Model to the given
    /// writer in EDMX form.  This method can only be used with context instances that use Code First
    /// and create the model internally.  The method cannot be used for contexts created using Database
    /// First or Model First, for contexts created using a pre-existing <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />, or
    /// for contexts created using a pre-existing <see cref="T:System.Data.Entity.Infrastructure.DbCompiledModel" />.
    /// </summary>
    /// <param name="context"> The context. </param>
    /// <param name="writer"> The writer. </param>
    public static void WriteEdmx(DbContext context, XmlWriter writer)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      Check.NotNull<XmlWriter>(writer, nameof (writer));
      InternalContext internalContext = context.InternalContext;
      if (internalContext is EagerInternalContext)
        throw Error.EdmxWriter_EdmxFromObjectContextNotSupported();
      DbModel beingInitialized = internalContext.ModelBeingInitialized;
      if (beingInitialized != null)
      {
        EdmxWriter.WriteEdmx(beingInitialized, writer);
      }
      else
      {
        DbCompiledModel codeFirstModel = internalContext.CodeFirstModel;
        if (codeFirstModel == null)
          throw Error.EdmxWriter_EdmxFromModelFirstNotSupported();
        DbModelBuilder dbModelBuilder = codeFirstModel.CachedModelBuilder.Clone();
        EdmxWriter.WriteEdmx(internalContext.ModelProviderInfo == null ? dbModelBuilder.Build(internalContext.Connection) : dbModelBuilder.Build(internalContext.ModelProviderInfo), writer);
      }
    }

    /// <summary>
    /// Writes the Entity Data Model represented by the given <see cref="T:System.Data.Entity.Infrastructure.DbModel" /> to the
    /// given writer in EDMX form.
    /// </summary>
    /// <param name="model"> An object representing the EDM. </param>
    /// <param name="writer"> The writer. </param>
    public static void WriteEdmx(DbModel model, XmlWriter writer)
    {
      Check.NotNull<DbModel>(model, nameof (model));
      Check.NotNull<XmlWriter>(writer, nameof (writer));
      new EdmxSerializer().Serialize(model.DatabaseMapping, writer);
    }
  }
}
