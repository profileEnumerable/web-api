// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Utilities.PropertyPath
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Entity.ModelConfiguration.Utilities
{
  internal class PropertyPath : IEnumerable<PropertyInfo>, IEnumerable
  {
    private static readonly PropertyPath _empty = new PropertyPath();
    private readonly List<PropertyInfo> _components = new List<PropertyInfo>();

    public PropertyPath(IEnumerable<PropertyInfo> components)
    {
      this._components.AddRange(components);
    }

    public PropertyPath(PropertyInfo component)
    {
      this._components.Add(component);
    }

    private PropertyPath()
    {
    }

    public int Count
    {
      get
      {
        return this._components.Count;
      }
    }

    public static PropertyPath Empty
    {
      get
      {
        return PropertyPath._empty;
      }
    }

    public PropertyInfo this[int index]
    {
      get
      {
        return this._components[index];
      }
    }

    public override string ToString()
    {
      StringBuilder propertyPathName = new StringBuilder();
      this._components.Each<PropertyInfo>((Action<PropertyInfo>) (pi =>
      {
        propertyPathName.Append(pi.Name);
        propertyPathName.Append('.');
      }));
      return propertyPathName.ToString(0, propertyPathName.Length - 1);
    }

    public bool Equals(PropertyPath other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      return this._components.SequenceEqual<PropertyInfo>((IEnumerable<PropertyInfo>) other._components, (Func<PropertyInfo, PropertyInfo, bool>) ((p1, p2) => p1.IsSameAs(p2)));
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != typeof (PropertyPath))
        return false;
      return this.Equals((PropertyPath) obj);
    }

    public override int GetHashCode()
    {
      return this._components.Aggregate<PropertyInfo, int>(0, (Func<int, PropertyInfo, int>) ((t, n) => t ^ n.DeclaringType.GetHashCode() * n.Name.GetHashCode() * 397));
    }

    public static bool operator ==(PropertyPath left, PropertyPath right)
    {
      return object.Equals((object) left, (object) right);
    }

    public static bool operator !=(PropertyPath left, PropertyPath right)
    {
      return !object.Equals((object) left, (object) right);
    }

    IEnumerator<PropertyInfo> IEnumerable<PropertyInfo>.GetEnumerator()
    {
      return (IEnumerator<PropertyInfo>) this._components.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._components.GetEnumerator();
    }
  }
}
