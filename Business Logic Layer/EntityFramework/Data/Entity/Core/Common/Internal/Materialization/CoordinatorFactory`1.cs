// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.CoordinatorFactory`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Objects.Internal;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class CoordinatorFactory<TElement> : CoordinatorFactory
  {
    internal readonly Func<Shaper, IEntityWrapper> WrappedElement;
    internal readonly Func<Shaper, TElement> Element;
    internal readonly Func<Shaper, TElement> ElementWithErrorHandling;
    internal readonly Func<Shaper, ICollection<TElement>> InitializeCollection;
    private readonly string Description;

    internal CoordinatorFactory(
      int depth,
      int stateSlot,
      Expression<Func<Shaper, bool>> hasData,
      Expression<Func<Shaper, bool>> setKeys,
      Expression<Func<Shaper, bool>> checkKeys,
      CoordinatorFactory[] nestedCoordinators,
      Expression<Func<Shaper, TElement>> element,
      Expression<Func<Shaper, IEntityWrapper>> wrappedElement,
      Expression<Func<Shaper, TElement>> elementWithErrorHandling,
      Expression<Func<Shaper, ICollection<TElement>>> initializeCollection,
      RecordStateFactory[] recordStateFactories)
      : base(depth, stateSlot, CoordinatorFactory<TElement>.CompilePredicate(hasData), CoordinatorFactory<TElement>.CompilePredicate(setKeys), CoordinatorFactory<TElement>.CompilePredicate(checkKeys), nestedCoordinators, recordStateFactories)
    {
      this.WrappedElement = wrappedElement == null ? (Func<Shaper, IEntityWrapper>) null : wrappedElement.Compile();
      this.Element = element == null ? (Func<Shaper, TElement>) null : element.Compile();
      this.ElementWithErrorHandling = elementWithErrorHandling.Compile();
      this.InitializeCollection = initializeCollection == null ? (Func<Shaper, ICollection<TElement>>) (s => (ICollection<TElement>) new List<TElement>()) : initializeCollection.Compile();
      this.Description = new StringBuilder().Append("HasData: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression((Expression) hasData)).Append("SetKeys: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression((Expression) setKeys)).Append("CheckKeys: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression((Expression) checkKeys)).Append("Element: ").AppendLine(element == null ? CoordinatorFactory<TElement>.DescribeExpression((Expression) wrappedElement) : CoordinatorFactory<TElement>.DescribeExpression((Expression) element)).Append("ElementWithExceptionHandling: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression((Expression) elementWithErrorHandling)).Append("InitializeCollection: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression((Expression) initializeCollection)).ToString();
    }

    public CoordinatorFactory(
      int depth,
      int stateSlot,
      Expression hasData,
      Expression setKeys,
      Expression checkKeys,
      CoordinatorFactory[] nestedCoordinators,
      Expression element,
      Expression elementWithErrorHandling,
      Expression initializeCollection,
      RecordStateFactory[] recordStateFactories)
      : this(depth, stateSlot, CodeGenEmitter.BuildShaperLambda<bool>(hasData), CodeGenEmitter.BuildShaperLambda<bool>(setKeys), CodeGenEmitter.BuildShaperLambda<bool>(checkKeys), nestedCoordinators, typeof (IEntityWrapper).IsAssignableFrom(element.Type) ? (Expression<Func<Shaper, TElement>>) null : CodeGenEmitter.BuildShaperLambda<TElement>(element), typeof (IEntityWrapper).IsAssignableFrom(element.Type) ? CodeGenEmitter.BuildShaperLambda<IEntityWrapper>(element) : (Expression<Func<Shaper, IEntityWrapper>>) null, CodeGenEmitter.BuildShaperLambda<TElement>(typeof (IEntityWrapper).IsAssignableFrom(element.Type) ? CodeGenEmitter.Emit_UnwrapAndEnsureType(elementWithErrorHandling, typeof (TElement)) : elementWithErrorHandling), CodeGenEmitter.BuildShaperLambda<ICollection<TElement>>(initializeCollection), recordStateFactories)
    {
    }

    private static Func<Shaper, bool> CompilePredicate(
      Expression<Func<Shaper, bool>> predicate)
    {
      return predicate?.Compile();
    }

    private static string DescribeExpression(Expression expression)
    {
      return expression != null ? expression.ToString() : "undefined";
    }

    internal override Coordinator CreateCoordinator(Coordinator parent, Coordinator next)
    {
      return (Coordinator) new Coordinator<TElement>(this, parent, next);
    }

    internal RecordState GetDefaultRecordState(Shaper<RecordState> shaper)
    {
      RecordState recordState = (RecordState) null;
      if (this.RecordStateFactories.Count > 0)
      {
        recordState = (RecordState) shaper.State[this.RecordStateFactories[0].StateSlotNumber];
        recordState.ResetToDefaultState();
      }
      return recordState;
    }

    public override string ToString()
    {
      return this.Description;
    }
  }
}
