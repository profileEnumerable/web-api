// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Utilities.EdmPropertyPath
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Text;

namespace System.Data.Entity.ModelConfiguration.Utilities
{
  internal class EdmPropertyPath : IEnumerable<EdmProperty>, IEnumerable
  {
    private static readonly EdmPropertyPath _empty = new EdmPropertyPath();
    private readonly List<EdmProperty> _components = new List<EdmProperty>();

    public EdmPropertyPath(IEnumerable<EdmProperty> components)
    {
      this._components.AddRange(components);
    }

    public EdmPropertyPath(EdmProperty component)
    {
      this._components.Add(component);
    }

    private EdmPropertyPath()
    {
    }

    public static EdmPropertyPath Empty
    {
      get
      {
        return EdmPropertyPath._empty;
      }
    }

    public override string ToString()
    {
      StringBuilder propertyPathName = new StringBuilder();
      this._components.Each<EdmProperty>((Action<EdmProperty>) (pi =>
      {
        propertyPathName.Append(pi.Name);
        propertyPathName.Append('.');
      }));
      return propertyPathName.ToString(0, propertyPathName.Length - 1);
    }

    public bool Equals(EdmPropertyPath other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      return this._components.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) other._components, (Func<EdmProperty, EdmProperty, bool>) ((p1, p2) => p1 == p2));
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != typeof (EdmPropertyPath))
        return false;
      return this.Equals((EdmPropertyPath) obj);
    }

    public override int GetHashCode()
    {
      return this._components.Aggregate<EdmProperty, int>(0, (Func<int, EdmProperty, int>) ((t, n) => t + n.GetHashCode()));
    }

    public static bool operator ==(EdmPropertyPath left, EdmPropertyPath right)
    {
      return object.Equals((object) left, (object) right);
    }

    public static bool operator !=(EdmPropertyPath left, EdmPropertyPath right)
    {
      return !object.Equals((object) left, (object) right);
    }

    IEnumerator<EdmProperty> IEnumerable<EdmProperty>.GetEnumerator()
    {
      return (IEnumerator<EdmProperty>) this._components.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._components.GetEnumerator();
    }
  }
}
