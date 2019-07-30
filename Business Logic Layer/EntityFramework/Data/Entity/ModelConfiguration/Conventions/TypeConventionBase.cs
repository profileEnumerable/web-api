// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.TypeConventionBase
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  internal abstract class TypeConventionBase : IConfigurationConvention<Type, EntityTypeConfiguration>, IConfigurationConvention<Type, ComplexTypeConfiguration>, IConfigurationConvention<Type>, IConvention
  {
    private readonly IEnumerable<Func<Type, bool>> _predicates;

    protected TypeConventionBase(IEnumerable<Func<Type, bool>> predicates)
    {
      this._predicates = predicates;
    }

    internal IEnumerable<Func<Type, bool>> Predicates
    {
      get
      {
        return this._predicates;
      }
    }

    public void Apply(Type memberInfo, System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      if (!this._predicates.All<Func<Type, bool>>((Func<Func<Type, bool>, bool>) (p => p(memberInfo))))
        return;
      this.ApplyCore(memberInfo, modelConfiguration);
    }

    protected abstract void ApplyCore(Type memberInfo, System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration);

    public void Apply(
      Type memberInfo,
      Func<EntityTypeConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      if (!this._predicates.All<Func<Type, bool>>((Func<Func<Type, bool>, bool>) (p => p(memberInfo))))
        return;
      this.ApplyCore(memberInfo, configuration, modelConfiguration);
    }

    protected abstract void ApplyCore(
      Type memberInfo,
      Func<EntityTypeConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration);

    public void Apply(
      Type memberInfo,
      Func<ComplexTypeConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      if (!this._predicates.All<Func<Type, bool>>((Func<Func<Type, bool>, bool>) (p => p(memberInfo))))
        return;
      this.ApplyCore(memberInfo, configuration, modelConfiguration);
    }

    protected abstract void ApplyCore(
      Type memberInfo,
      Func<ComplexTypeConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration);
  }
}
