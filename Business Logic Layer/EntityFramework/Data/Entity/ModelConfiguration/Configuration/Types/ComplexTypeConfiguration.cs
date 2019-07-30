// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Types.ComplexTypeConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Configuration.Types
{
  internal class ComplexTypeConfiguration : StructuralTypeConfiguration
  {
    internal ComplexTypeConfiguration(Type structuralType)
      : base(structuralType)
    {
    }

    private ComplexTypeConfiguration(ComplexTypeConfiguration source)
      : base((StructuralTypeConfiguration) source)
    {
    }

    internal virtual ComplexTypeConfiguration Clone()
    {
      return new ComplexTypeConfiguration(this);
    }

    internal virtual void Configure(ComplexType complexType)
    {
      this.Configure(complexType.Name, (IEnumerable<EdmProperty>) complexType.Properties, (ICollection<MetadataProperty>) complexType.GetMetadataProperties());
    }
  }
}
