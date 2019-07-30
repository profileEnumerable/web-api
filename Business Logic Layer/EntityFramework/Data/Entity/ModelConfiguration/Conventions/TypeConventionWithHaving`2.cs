// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.TypeConventionWithHaving`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  internal class TypeConventionWithHaving<T, TValue> : TypeConventionWithHavingBase<TValue>
    where T : class
    where TValue : class
  {
    private readonly Action<ConventionTypeConfiguration<T>, TValue> _entityConfigurationAction;

    public TypeConventionWithHaving(
      IEnumerable<Func<Type, bool>> predicates,
      Func<Type, TValue> capturingPredicate,
      Action<ConventionTypeConfiguration<T>, TValue> entityConfigurationAction)
      : base(predicates.Prepend<Func<Type, bool>>(TypeConvention<T>.OfTypePredicate), capturingPredicate)
    {
      this._entityConfigurationAction = entityConfigurationAction;
    }

    internal Action<ConventionTypeConfiguration<T>, TValue> EntityConfigurationAction
    {
      get
      {
        return this._entityConfigurationAction;
      }
    }

    protected override void InvokeAction(
      Type memberInfo,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration,
      TValue value)
    {
      this._entityConfigurationAction(new ConventionTypeConfiguration<T>(memberInfo, modelConfiguration), value);
    }

    protected override void InvokeAction(
      Type memberInfo,
      Func<EntityTypeConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration,
      TValue value)
    {
      this._entityConfigurationAction(new ConventionTypeConfiguration<T>(memberInfo, configuration, modelConfiguration), value);
    }

    protected override void InvokeAction(
      Type memberInfo,
      Func<ComplexTypeConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration,
      TValue value)
    {
      this._entityConfigurationAction(new ConventionTypeConfiguration<T>(memberInfo, configuration, modelConfiguration), value);
    }
  }
}
