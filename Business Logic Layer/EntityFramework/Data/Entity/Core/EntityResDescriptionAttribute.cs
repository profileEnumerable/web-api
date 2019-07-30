// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityResDescriptionAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class EntityResDescriptionAttribute : DescriptionAttribute
  {
    private bool _replaced;

    public override string Description
    {
      get
      {
        if (!this._replaced)
        {
          this._replaced = true;
          this.DescriptionValue = EntityRes.GetString(base.Description);
        }
        return base.Description;
      }
    }

    public EntityResDescriptionAttribute(string description)
      : base(description)
    {
    }
  }
}
