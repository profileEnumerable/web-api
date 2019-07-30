// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.DomainVariable`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal class DomainVariable<T_Variable, T_Element>
  {
    private readonly T_Variable _identifier;
    private readonly Set<T_Element> _domain;
    private readonly int _hashCode;
    private readonly IEqualityComparer<T_Variable> _identifierComparer;

    internal DomainVariable(
      T_Variable identifier,
      Set<T_Element> domain,
      IEqualityComparer<T_Variable> identifierComparer)
    {
      this._identifier = identifier;
      this._domain = domain.AsReadOnly();
      this._identifierComparer = identifierComparer ?? (IEqualityComparer<T_Variable>) EqualityComparer<T_Variable>.Default;
      this._hashCode = this._domain.GetElementsHashCode() ^ this._identifierComparer.GetHashCode(this._identifier);
    }

    internal DomainVariable(T_Variable identifier, Set<T_Element> domain)
      : this(identifier, domain, (IEqualityComparer<T_Variable>) null)
    {
    }

    internal T_Variable Identifier
    {
      get
      {
        return this._identifier;
      }
    }

    internal Set<T_Element> Domain
    {
      get
      {
        return this._domain;
      }
    }

    public override int GetHashCode()
    {
      return this._hashCode;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) this, obj))
        return true;
      DomainVariable<T_Variable, T_Element> domainVariable = obj as DomainVariable<T_Variable, T_Element>;
      if (domainVariable == null || this._hashCode != domainVariable._hashCode || !this._identifierComparer.Equals(this._identifier, domainVariable._identifier))
        return false;
      return this._domain.SetEquals(domainVariable._domain);
    }

    public override string ToString()
    {
      return StringUtil.FormatInvariant("{0}{{{1}}}", (object) this._identifier.ToString(), (object) this._domain);
    }
  }
}
