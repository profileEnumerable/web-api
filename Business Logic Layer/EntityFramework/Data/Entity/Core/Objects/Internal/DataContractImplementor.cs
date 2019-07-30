// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.DataContractImplementor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class DataContractImplementor
  {
    internal static readonly ConstructorInfo DataContractAttributeConstructor = typeof (DataContractAttribute).GetDeclaredConstructor();
    internal static readonly PropertyInfo[] DataContractProperties = new PropertyInfo[1]
    {
      typeof (DataContractAttribute).GetDeclaredProperty("IsReference")
    };
    private readonly Type _baseClrType;
    private readonly DataContractAttribute _dataContract;

    internal DataContractImplementor(EntityType ospaceEntityType)
    {
      this._baseClrType = ospaceEntityType.ClrType;
      this._dataContract = this._baseClrType.GetCustomAttributes<DataContractAttribute>(false).FirstOrDefault<DataContractAttribute>();
    }

    internal void Implement(TypeBuilder typeBuilder)
    {
      if (this._dataContract == null)
        return;
      object[] propertyValues = new object[1]
      {
        (object) this._dataContract.IsReference
      };
      CustomAttributeBuilder customBuilder = new CustomAttributeBuilder(DataContractImplementor.DataContractAttributeConstructor, new object[0], DataContractImplementor.DataContractProperties, propertyValues);
      typeBuilder.SetCustomAttribute(customBuilder);
    }
  }
}
