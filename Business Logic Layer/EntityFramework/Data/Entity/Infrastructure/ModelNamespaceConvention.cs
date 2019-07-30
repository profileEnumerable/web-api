// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.ModelNamespaceConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.ModelConfiguration.Conventions;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// This <see cref="T:System.Data.Entity.DbModelBuilder" /> convention uses the namespace of the derived
  /// <see cref="T:System.Data.Entity.DbContext" /> class as the namespace of the conceptual model built by
  /// Code First.
  /// </summary>
  public class ModelNamespaceConvention : Convention
  {
    private readonly string _modelNamespace;

    internal ModelNamespaceConvention(string modelNamespace)
    {
      this._modelNamespace = modelNamespace;
    }

    internal override void ApplyModelConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      base.ApplyModelConfiguration(modelConfiguration);
      modelConfiguration.ModelNamespace = this._modelNamespace;
    }
  }
}
