// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.TranslatorResult
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects.Internal;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class TranslatorResult
  {
    private readonly Expression ReturnedExpression;
    private readonly Type RequestedType;

    internal TranslatorResult(Expression returnedExpression, Type requestedType)
    {
      this.RequestedType = requestedType;
      this.ReturnedExpression = returnedExpression;
    }

    internal Expression Expression
    {
      get
      {
        return CodeGenEmitter.Emit_EnsureType(this.ReturnedExpression, this.RequestedType);
      }
    }

    internal Expression UnconvertedExpression
    {
      get
      {
        return this.ReturnedExpression;
      }
    }

    internal Expression UnwrappedExpression
    {
      get
      {
        if (!typeof (IEntityWrapper).IsAssignableFrom(this.ReturnedExpression.Type))
          return this.ReturnedExpression;
        return CodeGenEmitter.Emit_UnwrapAndEnsureType(this.ReturnedExpression, this.RequestedType);
      }
    }
  }
}
