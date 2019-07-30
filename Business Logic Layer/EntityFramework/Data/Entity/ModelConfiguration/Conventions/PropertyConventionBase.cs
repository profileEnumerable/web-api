// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.PropertyConventionBase
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  internal abstract class PropertyConventionBase : IConfigurationConvention<PropertyInfo, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>, IConvention
  {
    private readonly IEnumerable<Func<PropertyInfo, bool>> _predicates;

    public PropertyConventionBase(IEnumerable<Func<PropertyInfo, bool>> predicates)
    {
      this._predicates = predicates;
    }

    internal IEnumerable<Func<PropertyInfo, bool>> Predicates
    {
      get
      {
        return this._predicates;
      }
    }

    public void Apply(
      PropertyInfo memberInfo,
      Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      if (!this._predicates.All<Func<PropertyInfo, bool>>((Func<Func<PropertyInfo, bool>, bool>) (p => p(memberInfo))))
        return;
      this.ApplyCore(memberInfo, configuration, modelConfiguration);
    }

    protected abstract void ApplyCore(
      PropertyInfo memberInfo,
      Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration);
  }
}
