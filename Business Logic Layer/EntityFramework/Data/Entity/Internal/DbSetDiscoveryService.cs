// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DbSetDiscoveryService
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Internal
{
  internal class DbSetDiscoveryService
  {
    private static readonly ConcurrentDictionary<Type, DbContextTypesInitializersPair> _objectSetInitializers = new ConcurrentDictionary<Type, DbContextTypesInitializersPair>();
    public static readonly MethodInfo SetMethod = typeof (DbContext).GetDeclaredMethod("Set");
    private readonly DbContext _context;

    public DbSetDiscoveryService(DbContext context)
    {
      this._context = context;
    }

    private Dictionary<Type, List<string>> GetSets()
    {
      DbContextTypesInitializersPair initializersPair;
      if (!DbSetDiscoveryService._objectSetInitializers.TryGetValue(this._context.GetType(), out initializersPair))
      {
        ParameterExpression parameterExpression = Expression.Parameter(typeof (DbContext), "dbContext");
        List<Action<DbContext>> initDelegates = new List<Action<DbContext>>();
        Dictionary<Type, List<string>> entityTypeToPropertyNameMap = new Dictionary<Type, List<string>>();
        foreach (PropertyInfo propertyInfo in this._context.GetType().GetInstanceProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
        {
          if (p.GetIndexParameters().Length == 0)
            return p.DeclaringType != typeof (DbContext);
          return false;
        })))
        {
          Type setType = DbSetDiscoveryService.GetSetType(propertyInfo.PropertyType);
          if (setType != (Type) null)
          {
            if (!setType.IsValidStructuralType())
              throw Error.InvalidEntityType((object) setType);
            List<string> stringList;
            if (!entityTypeToPropertyNameMap.TryGetValue(setType, out stringList))
            {
              stringList = new List<string>();
              entityTypeToPropertyNameMap[setType] = stringList;
            }
            stringList.Add(propertyInfo.Name);
            if (DbSetDiscoveryService.DbSetPropertyShouldBeInitialized(propertyInfo))
            {
              MethodInfo method1 = propertyInfo.Setter();
              if (method1 != (MethodInfo) null && method1.IsPublic)
              {
                MethodInfo method2 = DbSetDiscoveryService.SetMethod.MakeGenericMethod(setType);
                MethodCallExpression methodCallExpression1 = Expression.Call((Expression) parameterExpression, method2);
                MethodCallExpression methodCallExpression2 = Expression.Call((Expression) Expression.Convert((Expression) parameterExpression, this._context.GetType()), method1, (Expression) methodCallExpression1);
                initDelegates.Add(Expression.Lambda<Action<DbContext>>((Expression) methodCallExpression2, parameterExpression).Compile());
              }
            }
          }
        }
        Action<DbContext> setsInitializer = (Action<DbContext>) (dbContext =>
        {
          foreach (Action<DbContext> action in initDelegates)
            action(dbContext);
        });
        initializersPair = new DbContextTypesInitializersPair(entityTypeToPropertyNameMap, setsInitializer);
        DbSetDiscoveryService._objectSetInitializers.TryAdd(this._context.GetType(), initializersPair);
      }
      return initializersPair.EntityTypeToPropertyNameMap;
    }

    public void InitializeSets()
    {
      this.GetSets();
      DbSetDiscoveryService._objectSetInitializers[this._context.GetType()].SetsInitializer(this._context);
    }

    public void RegisterSets(DbModelBuilder modelBuilder)
    {
      IEnumerable<KeyValuePair<Type, List<string>>> source = (IEnumerable<KeyValuePair<Type, List<string>>>) this.GetSets();
      if (modelBuilder.Version.IsEF6OrHigher())
        source = (IEnumerable<KeyValuePair<Type, List<string>>>) source.OrderBy<KeyValuePair<Type, List<string>>, string>((Func<KeyValuePair<Type, List<string>>, string>) (s => s.Value[0]));
      foreach (KeyValuePair<Type, List<string>> keyValuePair in source)
      {
        if (keyValuePair.Value.Count > 1)
          throw Error.Mapping_MESTNotSupported((object) keyValuePair.Value[0], (object) keyValuePair.Value[1], (object) keyValuePair.Key);
        modelBuilder.Entity(keyValuePair.Key).EntitySetName = keyValuePair.Value[0];
      }
    }

    private static bool DbSetPropertyShouldBeInitialized(PropertyInfo propertyInfo)
    {
      if (!propertyInfo.GetCustomAttributes<SuppressDbSetInitializationAttribute>(false).Any<SuppressDbSetInitializationAttribute>())
        return !propertyInfo.DeclaringType.GetCustomAttributes<SuppressDbSetInitializationAttribute>(false).Any<SuppressDbSetInitializationAttribute>();
      return false;
    }

    private static Type GetSetType(Type declaredType)
    {
      if (!declaredType.IsArray)
      {
        Type setElementType = DbSetDiscoveryService.GetSetElementType(declaredType);
        if (setElementType != (Type) null)
        {
          Type c = typeof (DbSet<>).MakeGenericType(setElementType);
          if (declaredType.IsAssignableFrom(c))
            return setElementType;
        }
      }
      return (Type) null;
    }

    private static Type GetSetElementType(Type setType)
    {
      try
      {
        Type type = !setType.IsGenericType() || !typeof (IDbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()) ? setType.GetInterface(typeof (IDbSet<>).FullName) : setType;
        if (type != (Type) null)
        {
          if (!type.ContainsGenericParameters())
            return type.GetGenericArguments()[0];
        }
      }
      catch (AmbiguousMatchException ex)
      {
      }
      return (Type) null;
    }
  }
}
