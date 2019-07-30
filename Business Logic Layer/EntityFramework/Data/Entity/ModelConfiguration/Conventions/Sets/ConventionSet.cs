// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.Sets.ConventionSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.ModelConfiguration.Conventions.Sets
{
  internal class ConventionSet
  {
    public ConventionSet()
    {
      this.ConfigurationConventions = (IEnumerable<IConvention>) new IConvention[0];
      this.ConceptualModelConventions = (IEnumerable<IConvention>) new IConvention[0];
      this.ConceptualToStoreMappingConventions = (IEnumerable<IConvention>) new IConvention[0];
      this.StoreModelConventions = (IEnumerable<IConvention>) new IConvention[0];
    }

    public ConventionSet(
      IEnumerable<IConvention> configurationConventions,
      IEnumerable<IConvention> entityModelConventions,
      IEnumerable<IConvention> dbMappingConventions,
      IEnumerable<IConvention> dbModelConventions)
    {
      this.ConfigurationConventions = configurationConventions;
      this.ConceptualModelConventions = entityModelConventions;
      this.ConceptualToStoreMappingConventions = dbMappingConventions;
      this.StoreModelConventions = dbModelConventions;
    }

    public IEnumerable<IConvention> ConfigurationConventions { get; private set; }

    public IEnumerable<IConvention> ConceptualModelConventions { get; private set; }

    public IEnumerable<IConvention> ConceptualToStoreMappingConventions { get; private set; }

    public IEnumerable<IConvention> StoreModelConventions { get; private set; }
  }
}
