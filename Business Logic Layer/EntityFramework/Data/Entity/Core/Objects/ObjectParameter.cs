// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectParameter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// This class represents a query parameter at the object layer, which consists
  /// of a Name, a Type and a Value.
  /// </summary>
  public sealed class ObjectParameter
  {
    private readonly string _name;
    private readonly Type _type;
    private readonly Type _mappableType;
    private TypeUsage _effectiveType;
    private object _value;

    internal static bool ValidateParameterName(string name)
    {
      return DbCommandTree.IsValidParameterName(name);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> class with the specified name and type.
    /// </summary>
    /// <param name="name">The parameter name. This name should not include the "@" parameter marker that is used in the Entity SQL statements, only the actual name. The first character of the expression must be a letter. Any successive characters in the expression must be either letters, numbers, or an underscore (_) character.</param>
    /// <param name="type">The common language runtime (CLR) type of the parameter.</param>
    /// <exception cref="T:System.ArgumentNullException">If the value of either argument is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">If the value of the name argument is invalid. Parameter names must start with a letter and can only contain letters, numbers, and underscores.</exception>
    public ObjectParameter(string name, Type type)
    {
      Check.NotNull<string>(name, nameof (name));
      Check.NotNull<Type>(type, nameof (type));
      if (!ObjectParameter.ValidateParameterName(name))
        throw new ArgumentException(Strings.ObjectParameter_InvalidParameterName((object) name), nameof (name));
      this._name = name;
      this._type = type;
      this._mappableType = TypeSystem.GetNonNullableType(this._type);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> class with the specified name and value.
    /// </summary>
    /// <param name="name">The parameter name. This name should not include the "@" parameter marker that is used in Entity SQL statements, only the actual name. The first character of the expression must be a letter. Any successive characters in the expression must be either letters, numbers, or an underscore (_) character.</param>
    /// <param name="value">The initial value (and inherently, the type) of the parameter.</param>
    /// <exception cref="T:System.ArgumentNullException">If the value of either argument is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">If the value of the name argument is not valid. Parameter names must start with a letter and can only contain letters, numbers, and underscores.</exception>
    public ObjectParameter(string name, object value)
    {
      Check.NotNull<string>(name, nameof (name));
      Check.NotNull<object>(value, nameof (value));
      if (!ObjectParameter.ValidateParameterName(name))
        throw new ArgumentException(Strings.ObjectParameter_InvalidParameterName((object) name), nameof (name));
      this._name = name;
      this._type = value.GetType();
      this._value = value;
      this._mappableType = TypeSystem.GetNonNullableType(this._type);
    }

    private ObjectParameter(ObjectParameter template)
    {
      this._name = template._name;
      this._type = template._type;
      this._mappableType = template._mappableType;
      this._effectiveType = template._effectiveType;
      this._value = template._value;
    }

    /// <summary>Gets the parameter name, which can only be set through a constructor.</summary>
    /// <returns>The parameter name, which can only be set through a constructor.</returns>
    public string Name
    {
      get
      {
        return this._name;
      }
    }

    /// <summary>Gets the parameter type.</summary>
    /// <returns>
    /// The <see cref="T:System.Type" /> of the parameter.
    /// </returns>
    public Type ParameterType
    {
      get
      {
        return this._type;
      }
    }

    /// <summary>Gets or sets the parameter value.</summary>
    /// <returns>The parameter value.</returns>
    public object Value
    {
      get
      {
        return this._value;
      }
      set
      {
        this._value = value;
      }
    }

    internal TypeUsage TypeUsage
    {
      get
      {
        return this._effectiveType;
      }
      set
      {
        this._effectiveType = value;
      }
    }

    internal Type MappableType
    {
      get
      {
        return this._mappableType;
      }
    }

    internal ObjectParameter ShallowCopy()
    {
      return new ObjectParameter(this);
    }

    internal bool ValidateParameterType(ClrPerspective perspective)
    {
      TypeUsage outTypeUsage;
      return perspective.TryGetType(this._mappableType, out outTypeUsage) && TypeSemantics.IsScalarType(outTypeUsage);
    }
  }
}
