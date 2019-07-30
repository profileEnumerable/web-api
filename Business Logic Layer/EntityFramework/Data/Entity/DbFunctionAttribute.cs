// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DbFunctionAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity
{
  /// <summary>
  /// Indicates that the given method is a proxy for an EDM function.
  /// </summary>
  /// <remarks>
  /// Note that this class was called EdmFunctionAttribute in some previous versions of Entity Framework.
  /// </remarks>
  [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class DbFunctionAttribute : Attribute
  {
    private readonly string _namespaceName;
    private readonly string _functionName;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.DbFunctionAttribute" /> class.
    /// </summary>
    /// <param name="namespaceName">The namespace of the mapped-to function.</param>
    /// <param name="functionName">The name of the mapped-to function.</param>
    public DbFunctionAttribute(string namespaceName, string functionName)
    {
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      Check.NotEmpty(functionName, nameof (functionName));
      this._namespaceName = namespaceName;
      this._functionName = functionName;
    }

    /// <summary>The namespace of the mapped-to function.</summary>
    /// <returns>The namespace of the mapped-to function.</returns>
    public string NamespaceName
    {
      get
      {
        return this._namespaceName;
      }
    }

    /// <summary>The name of the mapped-to function.</summary>
    /// <returns>The name of the mapped-to function.</returns>
    public string FunctionName
    {
      get
      {
        return this._functionName;
      }
    }
  }
}
