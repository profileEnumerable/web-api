// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.Patterns
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  internal static class Patterns
  {
    internal static Func<DbExpression, bool> And(
      Func<DbExpression, bool> pattern1,
      Func<DbExpression, bool> pattern2)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (pattern1(e))
          return pattern2(e);
        return false;
      });
    }

    internal static Func<DbExpression, bool> And(
      Func<DbExpression, bool> pattern1,
      Func<DbExpression, bool> pattern2,
      Func<DbExpression, bool> pattern3)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (pattern1(e) && pattern2(e))
          return pattern3(e);
        return false;
      });
    }

    internal static Func<DbExpression, bool> Or(
      Func<DbExpression, bool> pattern1,
      Func<DbExpression, bool> pattern2)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (!pattern1(e))
          return pattern2(e);
        return true;
      });
    }

    internal static Func<DbExpression, bool> Or(
      Func<DbExpression, bool> pattern1,
      Func<DbExpression, bool> pattern2,
      Func<DbExpression, bool> pattern3)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (!pattern1(e) && !pattern2(e))
          return pattern3(e);
        return true;
      });
    }

    internal static Func<DbExpression, bool> AnyExpression
    {
      get
      {
        return (Func<DbExpression, bool>) (e => true);
      }
    }

    internal static Func<IEnumerable<DbExpression>, bool> AnyExpressions
    {
      get
      {
        return (Func<IEnumerable<DbExpression>, bool>) (elems => true);
      }
    }

    internal static Func<DbExpression, bool> MatchComplexType
    {
      get
      {
        return (Func<DbExpression, bool>) (e => TypeSemantics.IsComplexType(e.ResultType));
      }
    }

    internal static Func<DbExpression, bool> MatchEntityType
    {
      get
      {
        return (Func<DbExpression, bool>) (e => TypeSemantics.IsEntityType(e.ResultType));
      }
    }

    internal static Func<DbExpression, bool> MatchRowType
    {
      get
      {
        return (Func<DbExpression, bool>) (e => TypeSemantics.IsRowType(e.ResultType));
      }
    }

    internal static Func<DbExpression, bool> MatchKind(DbExpressionKind kindToMatch)
    {
      return (Func<DbExpression, bool>) (e => e.ExpressionKind == kindToMatch);
    }

    internal static Func<IEnumerable<DbExpression>, bool> MatchForAll(
      Func<DbExpression, bool> elementPattern)
    {
      return (Func<IEnumerable<DbExpression>, bool>) (elems => elems.FirstOrDefault<DbExpression>((Func<DbExpression, bool>) (e => !elementPattern(e))) == null);
    }

    internal static Func<DbExpression, bool> MatchBinary()
    {
      return (Func<DbExpression, bool>) (e => e is DbBinaryExpression);
    }

    internal static Func<DbExpression, bool> MatchFilter(
      Func<DbExpression, bool> inputPattern,
      Func<DbExpression, bool> predicatePattern)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (e.ExpressionKind != DbExpressionKind.Filter)
          return false;
        DbFilterExpression filterExpression = (DbFilterExpression) e;
        if (inputPattern(filterExpression.Input.Expression))
          return predicatePattern(filterExpression.Predicate);
        return false;
      });
    }

    internal static Func<DbExpression, bool> MatchProject(
      Func<DbExpression, bool> inputPattern,
      Func<DbExpression, bool> projectionPattern)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (e.ExpressionKind != DbExpressionKind.Project)
          return false;
        DbProjectExpression projectExpression = (DbProjectExpression) e;
        if (inputPattern(projectExpression.Input.Expression))
          return projectionPattern(projectExpression.Projection);
        return false;
      });
    }

    internal static Func<DbExpression, bool> MatchCase(
      Func<IEnumerable<DbExpression>, bool> whenPattern,
      Func<IEnumerable<DbExpression>, bool> thenPattern,
      Func<DbExpression, bool> elsePattern)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (e.ExpressionKind != DbExpressionKind.Case)
          return false;
        DbCaseExpression dbCaseExpression = (DbCaseExpression) e;
        if (whenPattern((IEnumerable<DbExpression>) dbCaseExpression.When) && thenPattern((IEnumerable<DbExpression>) dbCaseExpression.Then))
          return elsePattern(dbCaseExpression.Else);
        return false;
      });
    }

    internal static Func<DbExpression, bool> MatchNewInstance()
    {
      return (Func<DbExpression, bool>) (e => e.ExpressionKind == DbExpressionKind.NewInstance);
    }

    internal static Func<DbExpression, bool> MatchNewInstance(
      Func<IEnumerable<DbExpression>, bool> argumentsPattern)
    {
      return (Func<DbExpression, bool>) (e =>
      {
        if (e.ExpressionKind != DbExpressionKind.NewInstance)
          return false;
        return argumentsPattern((IEnumerable<DbExpression>) ((DbNewInstanceExpression) e).Arguments);
      });
    }
  }
}
