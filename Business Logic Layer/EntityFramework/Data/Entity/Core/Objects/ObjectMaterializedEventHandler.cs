// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectMaterializedEventHandler
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>Delegate for the ObjectMaterialized event.</summary>
  /// <param name="sender"> The ObjectContext responsable for materializing the object. </param>
  /// <param name="e"> EventArgs containing a reference to the materialized object. </param>
  [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
  public delegate void ObjectMaterializedEventHandler(object sender, ObjectMaterializedEventArgs e);
}
