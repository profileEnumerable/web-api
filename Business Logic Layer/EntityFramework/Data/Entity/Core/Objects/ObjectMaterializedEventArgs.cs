// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectMaterializedEventArgs
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Objects
{
  /// <summary>EventArgs for the ObjectMaterialized event.</summary>
  public class ObjectMaterializedEventArgs : EventArgs
  {
    private readonly object _entity;

    internal ObjectMaterializedEventArgs(object entity)
    {
      this._entity = entity;
    }

    /// <summary>Gets the entity object that was created.</summary>
    /// <returns>The entity object that was created.</returns>
    public object Entity
    {
      get
      {
        return this._entity;
      }
    }
  }
}
