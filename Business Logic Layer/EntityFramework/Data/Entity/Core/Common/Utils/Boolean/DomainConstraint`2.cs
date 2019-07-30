// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.DomainConstraint`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal class DomainConstraint<T_Variable, T_Element>
  {
    private readonly DomainVariable<T_Variable, T_Element> _variable;
    private readonly Set<T_Element> _range;
    private readonly int _hashCode;

    internal DomainConstraint(DomainVariable<T_Variable, T_Element> variable, Set<T_Element> range)
    {
      this._variable = variable;
      this._range = range.AsReadOnly();
      this._hashCode = this._variable.GetHashCode() ^ this._range.GetElementsHashCode();
    }

    internal DomainConstraint(DomainVariable<T_Variable, T_Element> variable, T_Element element)
      : this(variable, new Set<T_Element>((IEnumerable<T_Element>) new T_Element[1]
      {
        element
      }).MakeReadOnly())
    {
    }

    internal DomainVariable<T_Variable, T_Element> Variable
    {
      get
      {
        return this._variable;
      }
    }

    internal Set<T_Element> Range
    {
      get
      {
        return this._range;
      }
    }

    internal DomainConstraint<T_Variable, T_Element> InvertDomainConstraint()
    {
      return new DomainConstraint<T_Variable, T_Element>(this._variable, this._variable.Domain.Difference((IEnumerable<T_Element>) this._range).AsReadOnly());
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) this, obj))
        return true;
      DomainConstraint<T_Variable, T_Element> domainConstraint = obj as DomainConstraint<T_Variable, T_Element>;
      if (domainConstraint == null || this._hashCode != domainConstraint._hashCode || !this._range.SetEquals(domainConstraint._range))
        return false;
      return this._variable.Equals((object) domainConstraint._variable);
    }

    public override int GetHashCode()
    {
      return this._hashCode;
    }

    public override string ToString()
    {
      return StringUtil.FormatInvariant("{0} in [{1}]", (object) this._variable, (object) this._range);
    }
  }
}
