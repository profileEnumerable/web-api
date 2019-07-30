// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.IConfigurationConvention`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  internal interface IConfigurationConvention<TMemberInfo> : IConvention where TMemberInfo : MemberInfo
  {
    void Apply(TMemberInfo memberInfo, System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration);
  }
}
