// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.Constant
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal abstract class Constant : InternalBase
  {
    internal static readonly IEqualityComparer<Constant> EqualityComparer = (IEqualityComparer<Constant>) new Constant.CellConstantComparer();
    internal static readonly Constant Null = Constant.NullConstant.Instance;
    internal static readonly Constant NotNull = (Constant) new NegatedConstant((IEnumerable<Constant>) new Constant[1]
    {
      Constant.NullConstant.Instance
    });
    internal static readonly Constant Undefined = Constant.UndefinedConstant.Instance;
    internal static readonly Constant AllOtherConstants = Constant.AllOtherConstantsConstant.Instance;

    internal abstract bool IsNull();

    internal abstract bool IsNotNull();

    internal abstract bool IsUndefined();

    internal abstract bool HasNotNull();

    internal abstract StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias);

    internal abstract DbExpression AsCqt(DbExpression row, MemberPath outputMember);

    public override bool Equals(object obj)
    {
      Constant right = obj as Constant;
      if (right == null)
        return false;
      return this.IsEqualTo(right);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    protected abstract bool IsEqualTo(Constant right);

    internal abstract string ToUserString();

    internal static void ConstantsToUserString(StringBuilder builder, Set<Constant> constants)
    {
      bool flag = true;
      foreach (Constant constant in constants)
      {
        if (!flag)
          builder.Append(Strings.ViewGen_CommaBlank);
        flag = false;
        string userString = constant.ToUserString();
        builder.Append(userString);
      }
    }

    private class CellConstantComparer : IEqualityComparer<Constant>
    {
      public bool Equals(Constant left, Constant right)
      {
        if (object.ReferenceEquals((object) left, (object) right))
          return true;
        if (left == null || right == null)
          return false;
        return left.IsEqualTo(right);
      }

      public int GetHashCode(Constant key)
      {
        return key.GetHashCode();
      }
    }

    private sealed class NullConstant : Constant
    {
      internal static readonly Constant Instance = (Constant) new Constant.NullConstant();

      private NullConstant()
      {
      }

      internal override bool IsNull()
      {
        return true;
      }

      internal override bool IsNotNull()
      {
        return false;
      }

      internal override bool IsUndefined()
      {
        return false;
      }

      internal override bool HasNotNull()
      {
        return false;
      }

      internal override StringBuilder AsEsql(
        StringBuilder builder,
        MemberPath outputMember,
        string blockAlias)
      {
        EdmType edmType = Helper.GetModelTypeUsage(outputMember.LeafEdmMember).EdmType;
        builder.Append("CAST(NULL AS ");
        CqlWriter.AppendEscapedTypeName(builder, edmType);
        builder.Append(')');
        return builder;
      }

      internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
      {
        return (DbExpression) TypeUsage.Create(Helper.GetModelTypeUsage(outputMember.LeafEdmMember).EdmType).Null();
      }

      public override int GetHashCode()
      {
        return 0;
      }

      protected override bool IsEqualTo(Constant right)
      {
        return object.ReferenceEquals((object) this, (object) right);
      }

      internal override string ToUserString()
      {
        return Strings.ViewGen_Null;
      }

      internal override void ToCompactString(StringBuilder builder)
      {
        builder.Append("NULL");
      }
    }

    private sealed class UndefinedConstant : Constant
    {
      internal static readonly Constant Instance = (Constant) new Constant.UndefinedConstant();

      private UndefinedConstant()
      {
      }

      internal override bool IsNull()
      {
        return false;
      }

      internal override bool IsNotNull()
      {
        return false;
      }

      internal override bool IsUndefined()
      {
        return true;
      }

      internal override bool HasNotNull()
      {
        return false;
      }

      internal override StringBuilder AsEsql(
        StringBuilder builder,
        MemberPath outputMember,
        string blockAlias)
      {
        throw new NotSupportedException();
      }

      internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
      {
        throw new NotSupportedException();
      }

      public override int GetHashCode()
      {
        return 0;
      }

      protected override bool IsEqualTo(Constant right)
      {
        return object.ReferenceEquals((object) this, (object) right);
      }

      internal override string ToUserString()
      {
        throw new NotSupportedException();
      }

      internal override void ToCompactString(StringBuilder builder)
      {
        builder.Append("?");
      }
    }

    private sealed class AllOtherConstantsConstant : Constant
    {
      internal static readonly Constant Instance = (Constant) new Constant.AllOtherConstantsConstant();

      private AllOtherConstantsConstant()
      {
      }

      internal override bool IsNull()
      {
        return false;
      }

      internal override bool IsNotNull()
      {
        return false;
      }

      internal override bool IsUndefined()
      {
        return false;
      }

      internal override bool HasNotNull()
      {
        return false;
      }

      internal override StringBuilder AsEsql(
        StringBuilder builder,
        MemberPath outputMember,
        string blockAlias)
      {
        throw new NotSupportedException();
      }

      internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
      {
        throw new NotSupportedException();
      }

      public override int GetHashCode()
      {
        return 0;
      }

      protected override bool IsEqualTo(Constant right)
      {
        return object.ReferenceEquals((object) this, (object) right);
      }

      internal override string ToUserString()
      {
        throw new NotSupportedException();
      }

      internal override void ToCompactString(StringBuilder builder)
      {
        builder.Append("AllOtherConstants");
      }
    }
  }
}
