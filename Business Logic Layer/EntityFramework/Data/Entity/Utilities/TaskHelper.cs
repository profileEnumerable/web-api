// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.TaskHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Threading.Tasks;

namespace System.Data.Entity.Utilities
{
  internal static class TaskHelper
  {
    internal static Task<T> FromException<T>(Exception ex)
    {
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      completionSource.SetException(ex);
      return completionSource.Task;
    }

    internal static Task<T> FromCancellation<T>()
    {
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      completionSource.SetCanceled();
      return completionSource.Task;
    }
  }
}
