// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation.ConstraintConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Types;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation
{
  internal abstract class ConstraintConfiguration
  {
    internal abstract ConstraintConfiguration Clone();

    internal abstract void Configure(
      AssociationType associationType,
      AssociationEndMember dependentEnd,
      EntityTypeConfiguration entityTypeConfiguration);

    public virtual bool IsFullySpecified
    {
      get
      {
        return true;
      }
    }
  }
}
