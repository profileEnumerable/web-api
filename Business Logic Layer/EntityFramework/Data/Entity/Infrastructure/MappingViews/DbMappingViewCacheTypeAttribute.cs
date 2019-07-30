// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCacheTypeAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure.MappingViews
{
  /// <summary>
  /// Defines a custom attribute that specifies the mapping view cache type (subclass of <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCache" />)
  /// associated with a context type (subclass of <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> or <see cref="T:System.Data.Entity.DbContext" />).
  /// The cache type is instantiated at runtime and used to retrieve pre-generated views in the
  /// corresponding context.
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
  public sealed class DbMappingViewCacheTypeAttribute : Attribute
  {
    private readonly Type _contextType;
    private readonly Type _cacheType;

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCacheTypeAttribute" />  instance that associates a context type
    /// with a mapping view cache type.
    /// </summary>
    /// <param name="contextType">
    /// A subclass of <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> or <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="cacheType">
    /// A subclass of <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCache" />.
    /// </param>
    public DbMappingViewCacheTypeAttribute(Type contextType, Type cacheType)
    {
      Check.NotNull<Type>(contextType, nameof (contextType));
      Check.NotNull<Type>(cacheType, nameof (cacheType));
      if (!contextType.IsSubclassOf(typeof (ObjectContext)) && !contextType.IsSubclassOf(typeof (DbContext)))
        throw new ArgumentException(Strings.DbMappingViewCacheTypeAttribute_InvalidContextType((object) contextType), nameof (contextType));
      if (!cacheType.IsSubclassOf(typeof (DbMappingViewCache)))
        throw new ArgumentException(Strings.Generated_View_Type_Super_Class((object) cacheType), nameof (cacheType));
      this._contextType = contextType;
      this._cacheType = cacheType;
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCacheTypeAttribute" /> instance that associates a context type
    /// with a mapping view cache type.
    /// </summary>
    /// <param name="contextType">
    /// A subclass of <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> or <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="cacheTypeName">The assembly qualified full name of the cache type.</param>
    public DbMappingViewCacheTypeAttribute(Type contextType, string cacheTypeName)
    {
      Check.NotNull<Type>(contextType, nameof (contextType));
      Check.NotEmpty(cacheTypeName, nameof (cacheTypeName));
      if (!contextType.IsSubclassOf(typeof (ObjectContext)) && !contextType.IsSubclassOf(typeof (DbContext)))
        throw new ArgumentException(Strings.DbMappingViewCacheTypeAttribute_InvalidContextType((object) contextType), nameof (contextType));
      this._contextType = contextType;
      try
      {
        this._cacheType = Type.GetType(cacheTypeName, true);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(Strings.DbMappingViewCacheTypeAttribute_CacheTypeNotFound((object) cacheTypeName), nameof (cacheTypeName), ex);
      }
    }

    internal Type ContextType
    {
      get
      {
        return this._contextType;
      }
    }

    internal Type CacheType
    {
      get
      {
        return this._cacheType;
      }
    }
  }
}
