// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.ComplexTypeExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class ComplexTypeExtensions
  {
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public static EdmProperty AddComplexProperty(
      this ComplexType complexType,
      string name,
      ComplexType targetComplexType)
    {
      EdmProperty complex = EdmProperty.CreateComplex(name, targetComplexType);
      complexType.AddMember((EdmMember) complex);
      return complex;
    }

    public static object GetConfiguration(this ComplexType complexType)
    {
      return complexType.Annotations.GetConfiguration();
    }

    public static Type GetClrType(this ComplexType complexType)
    {
      return complexType.Annotations.GetClrType();
    }

    internal static IEnumerable<ComplexType> ToHierarchy(
      this ComplexType edmType)
    {
      return EdmType.SafeTraverseHierarchy<ComplexType>(edmType);
    }
  }
}
