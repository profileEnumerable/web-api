// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.ExceptionExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core;
using System.Security;
using System.Threading;

namespace System.Data.Entity.Utilities
{
  internal static class ExceptionExtensions
  {
    public static bool IsCatchableExceptionType(this Exception e)
    {
      Type type = e.GetType();
      if (type != typeof (StackOverflowException) && type != typeof (OutOfMemoryException) && (type != typeof (ThreadAbortException) && type != typeof (NullReferenceException)) && type != typeof (AccessViolationException))
        return !typeof (SecurityException).IsAssignableFrom(type);
      return false;
    }

    public static bool IsCatchableEntityExceptionType(this Exception e)
    {
      Type type = e.GetType();
      if (e.IsCatchableExceptionType() && type != typeof (EntityCommandExecutionException) && type != typeof (EntityCommandCompilationException))
        return type != typeof (EntitySqlException);
      return false;
    }

    public static bool RequiresContext(this Exception e)
    {
      if (!e.IsCatchableExceptionType() || e is UpdateException)
        return false;
      return !(e is ProviderIncompatibleException);
    }
  }
}
