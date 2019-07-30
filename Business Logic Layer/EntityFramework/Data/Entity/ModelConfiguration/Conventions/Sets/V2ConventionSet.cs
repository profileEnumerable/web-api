// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.Sets.V2ConventionSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Conventions.Sets
{
  internal static class V2ConventionSet
  {
    private static readonly ConventionSet _conventions;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static V2ConventionSet()
    {
      List<IConvention> conventionList = new List<IConvention>(V1ConventionSet.Conventions.StoreModelConventions);
      int index = conventionList.FindIndex((Predicate<IConvention>) (c => c.GetType() == typeof (ColumnOrderingConvention)));
      conventionList[index] = (IConvention) new ColumnOrderingConventionStrict();
      V2ConventionSet._conventions = new ConventionSet(V1ConventionSet.Conventions.ConfigurationConventions, V1ConventionSet.Conventions.ConceptualModelConventions, V1ConventionSet.Conventions.ConceptualToStoreMappingConventions, (IEnumerable<IConvention>) conventionList);
    }

    public static ConventionSet Conventions
    {
      get
      {
        return V2ConventionSet._conventions;
      }
    }
  }
}
