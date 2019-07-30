// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation.IndependentConstraintConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation
{
  internal class IndependentConstraintConfiguration : ConstraintConfiguration
  {
    private static readonly ConstraintConfiguration _instance = (ConstraintConfiguration) new IndependentConstraintConfiguration();

    private IndependentConstraintConfiguration()
    {
    }

    public static ConstraintConfiguration Instance
    {
      get
      {
        return IndependentConstraintConfiguration._instance;
      }
    }

    internal override ConstraintConfiguration Clone()
    {
      return IndependentConstraintConfiguration._instance;
    }

    internal override void Configure(
      AssociationType associationType,
      AssociationEndMember dependentEnd,
      EntityTypeConfiguration entityTypeConfiguration)
    {
      associationType.MarkIndependent();
    }
  }
}
