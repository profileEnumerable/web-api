// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionsTypeFinder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class ConventionsTypeFinder
  {
    private readonly ConventionsTypeFilter _conventionsTypeFilter;
    private readonly ConventionsTypeActivator _conventionsTypeActivator;

    public ConventionsTypeFinder()
      : this(new ConventionsTypeFilter(), new ConventionsTypeActivator())
    {
    }

    public ConventionsTypeFinder(
      ConventionsTypeFilter conventionsTypeFilter,
      ConventionsTypeActivator conventionsTypeActivator)
    {
      this._conventionsTypeFilter = conventionsTypeFilter;
      this._conventionsTypeActivator = conventionsTypeActivator;
    }

    public void AddConventions(IEnumerable<Type> types, Action<IConvention> addFunction)
    {
      foreach (Type type in types)
      {
        if (this._conventionsTypeFilter.IsConvention(type))
          addFunction(this._conventionsTypeActivator.Activate(type));
      }
    }
  }
}
