// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.ScalarConstant
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class ScalarConstant : Constant
  {
    private readonly object m_scalar;

    internal ScalarConstant(object value)
    {
      this.m_scalar = value;
    }

    internal object Value
    {
      get
      {
        return this.m_scalar;
      }
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
      TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(outputMember.LeafEdmMember);
      EdmType edmType = modelTypeUsage.EdmType;
      if (BuiltInTypeKind.PrimitiveType == edmType.BuiltInTypeKind)
      {
        switch (((PrimitiveType) edmType).PrimitiveTypeKind)
        {
          case PrimitiveTypeKind.Boolean:
            string str = StringUtil.FormatInvariant("{0}", (object) (bool) this.m_scalar);
            builder.Append(str);
            return builder;
          case PrimitiveTypeKind.String:
            bool isUnicode;
            if (!TypeHelpers.TryGetIsUnicode(modelTypeUsage, out isUnicode))
              isUnicode = true;
            if (isUnicode)
              builder.Append('N');
            this.AppendEscapedScalar(builder);
            return builder;
        }
      }
      else if (BuiltInTypeKind.EnumType == edmType.BuiltInTypeKind)
      {
        EnumMember scalar = (EnumMember) this.m_scalar;
        builder.Append(scalar.Name);
        return builder;
      }
      builder.Append("CAST(");
      this.AppendEscapedScalar(builder);
      builder.Append(" AS ");
      CqlWriter.AppendEscapedTypeName(builder, edmType);
      builder.Append(')');
      return builder;
    }

    private StringBuilder AppendEscapedScalar(StringBuilder builder)
    {
      string str = StringUtil.FormatInvariant("{0}", this.m_scalar);
      if (str.Contains("'"))
        str = str.Replace("'", "''");
      StringUtil.FormatStringBuilder(builder, "'{0}'", (object) str);
      return builder;
    }

    internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
    {
      return (DbExpression) Helper.GetModelTypeUsage(outputMember.LeafEdmMember).Constant(this.m_scalar);
    }

    protected override bool IsEqualTo(Constant right)
    {
      ScalarConstant scalarConstant = right as ScalarConstant;
      if (scalarConstant == null)
        return false;
      return ByValueEqualityComparer.Default.Equals(this.m_scalar, scalarConstant.m_scalar);
    }

    public override int GetHashCode()
    {
      return this.m_scalar.GetHashCode();
    }

    internal override string ToUserString()
    {
      StringBuilder builder = new StringBuilder();
      this.ToCompactString(builder);
      return builder.ToString();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      EnumMember scalar = this.m_scalar as EnumMember;
      if (scalar != null)
        builder.Append(scalar.Name);
      else
        builder.Append(StringUtil.FormatInvariant("'{0}'", this.m_scalar));
    }
  }
}
