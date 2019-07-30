// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.PropertyConventionWithHaving`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  internal class PropertyConventionWithHaving<T> : PropertyConventionBase where T : class
  {
    private readonly Func<PropertyInfo, T> _capturingPredicate;
    private readonly Action<ConventionPrimitivePropertyConfiguration, T> _propertyConfigurationAction;

    public PropertyConventionWithHaving(
      IEnumerable<Func<PropertyInfo, bool>> predicates,
      Func<PropertyInfo, T> capturingPredicate,
      Action<ConventionPrimitivePropertyConfiguration, T> propertyConfigurationAction)
      : base(predicates)
    {
      this._capturingPredicate = capturingPredicate;
      this._propertyConfigurationAction = propertyConfigurationAction;
    }

    internal Func<PropertyInfo, T> CapturingPredicate
    {
      get
      {
        return this._capturingPredicate;
      }
    }

    internal Action<ConventionPrimitivePropertyConfiguration, T> PropertyConfigurationAction
    {
      get
      {
        return this._propertyConfigurationAction;
      }
    }

    protected override void ApplyCore(
      PropertyInfo memberInfo,
      Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      T obj = this._capturingPredicate(memberInfo);
      if ((object) obj == null)
        return;
      this._propertyConfigurationAction(new ConventionPrimitivePropertyConfiguration(memberInfo, configuration), obj);
    }
  }
}
