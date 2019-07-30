// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.CoordinatorScratchpad
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class CoordinatorScratchpad
  {
    private readonly Type _elementType;
    private CoordinatorScratchpad _parent;
    private readonly List<CoordinatorScratchpad> _nestedCoordinatorScratchpads;
    private readonly Dictionary<Expression, Expression> _expressionWithErrorHandlingMap;
    private readonly HashSet<LambdaExpression> _inlineDelegates;
    private List<RecordStateScratchpad> _recordStateScratchpads;

    internal CoordinatorScratchpad(Type elementType)
    {
      this._elementType = elementType;
      this._nestedCoordinatorScratchpads = new List<CoordinatorScratchpad>();
      this._expressionWithErrorHandlingMap = new Dictionary<Expression, Expression>();
      this._inlineDelegates = new HashSet<LambdaExpression>();
    }

    internal CoordinatorScratchpad Parent
    {
      get
      {
        return this._parent;
      }
    }

    internal Expression SetKeys { get; set; }

    internal Expression CheckKeys { get; set; }

    internal Expression HasData { get; set; }

    internal Expression Element { get; set; }

    internal Expression InitializeCollection { get; set; }

    internal int StateSlotNumber { get; set; }

    internal int Depth { get; set; }

    internal void AddExpressionWithErrorHandling(
      Expression expression,
      Expression expressionWithErrorHandling)
    {
      this._expressionWithErrorHandlingMap[expression] = expressionWithErrorHandling;
    }

    internal void AddInlineDelegate(LambdaExpression expression)
    {
      this._inlineDelegates.Add(expression);
    }

    internal void AddNestedCoordinator(CoordinatorScratchpad nested)
    {
      nested._parent = this;
      this._nestedCoordinatorScratchpads.Add(nested);
    }

    internal CoordinatorFactory Compile()
    {
      RecordStateFactory[] recordStateFactoryArray;
      if (this._recordStateScratchpads != null)
      {
        recordStateFactoryArray = new RecordStateFactory[this._recordStateScratchpads.Count];
        for (int index = 0; index < recordStateFactoryArray.Length; ++index)
          recordStateFactoryArray[index] = this._recordStateScratchpads[index].Compile();
      }
      else
        recordStateFactoryArray = new RecordStateFactory[0];
      CoordinatorFactory[] coordinatorFactoryArray = new CoordinatorFactory[this._nestedCoordinatorScratchpads.Count];
      for (int index = 0; index < coordinatorFactoryArray.Length; ++index)
        coordinatorFactoryArray[index] = this._nestedCoordinatorScratchpads[index].Compile();
      Expression expression1 = new CoordinatorScratchpad.ReplacementExpressionVisitor((Dictionary<Expression, Expression>) null, this._inlineDelegates).Visit(this.Element);
      Expression expression2 = new CoordinatorScratchpad.ReplacementExpressionVisitor(this._expressionWithErrorHandlingMap, this._inlineDelegates).Visit(this.Element);
      return (CoordinatorFactory) Activator.CreateInstance(typeof (CoordinatorFactory<>).MakeGenericType(this._elementType), (object) this.Depth, (object) this.StateSlotNumber, (object) this.HasData, (object) this.SetKeys, (object) this.CheckKeys, (object) coordinatorFactoryArray, (object) expression1, (object) expression2, (object) this.InitializeCollection, (object) recordStateFactoryArray);
    }

    internal RecordStateScratchpad CreateRecordStateScratchpad()
    {
      RecordStateScratchpad recordStateScratchpad = new RecordStateScratchpad();
      if (this._recordStateScratchpads == null)
        this._recordStateScratchpads = new List<RecordStateScratchpad>();
      this._recordStateScratchpads.Add(recordStateScratchpad);
      return recordStateScratchpad;
    }

    private class ReplacementExpressionVisitor : EntityExpressionVisitor
    {
      private readonly Dictionary<Expression, Expression> _replacementDictionary;
      private readonly HashSet<LambdaExpression> _inlineDelegates;

      internal ReplacementExpressionVisitor(
        Dictionary<Expression, Expression> replacementDictionary,
        HashSet<LambdaExpression> inlineDelegates)
      {
        this._replacementDictionary = replacementDictionary;
        this._inlineDelegates = inlineDelegates;
      }

      internal override Expression Visit(Expression expression)
      {
        if (expression == null)
          return expression;
        Expression expression1;
        Expression expression2;
        if (this._replacementDictionary != null && this._replacementDictionary.TryGetValue(expression, out expression1))
        {
          expression2 = expression1;
        }
        else
        {
          bool flag = false;
          LambdaExpression lambdaExpression = (LambdaExpression) null;
          if (expression.NodeType == ExpressionType.Lambda && this._inlineDelegates != null)
          {
            lambdaExpression = (LambdaExpression) expression;
            flag = this._inlineDelegates.Contains(lambdaExpression);
          }
          if (flag)
          {
            Expression body = this.Visit(lambdaExpression.Body);
            expression2 = (Expression) Expression.Constant(CodeGenEmitter.Compile(body.Type, body));
          }
          else
            expression2 = base.Visit(expression);
        }
        return expression2;
      }
    }
  }
}
