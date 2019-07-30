// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.InternalSet`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Internal.Linq
{
  internal class InternalSet<TEntity> : InternalQuery<TEntity>, IInternalSet<TEntity>, IInternalSet, IInternalQuery<TEntity>, IInternalQuery
    where TEntity : class
  {
    private DbLocalView<TEntity> _localView;
    private EntitySet _entitySet;
    private string _entitySetName;
    private string _quotedEntitySetName;
    private Type _baseType;

    public InternalSet(InternalContext internalContext)
      : base(internalContext)
    {
    }

    public override void ResetQuery()
    {
      this._entitySet = (EntitySet) null;
      this._localView = (DbLocalView<TEntity>) null;
      base.ResetQuery();
    }

    public TEntity Find(params object[] keyValues)
    {
      this.InternalContext.ObjectContext.AsyncMonitor.EnsureNotEntered();
      this.InternalContext.DetectChanges(false);
      WrappedEntityKey key = new WrappedEntityKey(this.EntitySet, this.EntitySetName, keyValues, nameof (keyValues));
      object obj = this.FindInStateManager(key) ?? this.FindInStore(key, nameof (keyValues));
      if (obj != null && !(obj is TEntity))
        throw Error.DbSet_WrongEntityTypeFound((object) obj.GetType().Name, (object) typeof (TEntity).Name);
      return (TEntity) obj;
    }

    public Task<TEntity> FindAsync(
      CancellationToken cancellationToken,
      params object[] keyValues)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this.InternalContext.ObjectContext.AsyncMonitor.EnsureNotEntered();
      return this.FindInternalAsync(cancellationToken, keyValues);
    }

    private async Task<TEntity> FindInternalAsync(
      CancellationToken cancellationToken,
      params object[] keyValues)
    {
      this.InternalContext.DetectChanges(false);
      WrappedEntityKey key = new WrappedEntityKey(this.EntitySet, this.EntitySetName, keyValues, nameof (keyValues));
      object inStateManager = this.FindInStateManager(key);
      object obj;
      if (inStateManager == null)
        obj = await this.FindInStoreAsync(key, nameof (keyValues), cancellationToken).WithCurrentCulture<object>();
      else
        obj = inStateManager;
      object entity = obj;
      if (entity != null && !(entity is TEntity))
        throw Error.DbSet_WrongEntityTypeFound((object) entity.GetType().Name, (object) typeof (TEntity).Name);
      return (TEntity) entity;
    }

    private object FindInStateManager(WrappedEntityKey key)
    {
      ObjectStateEntry entry;
      if (!key.HasNullValues && this.InternalContext.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(key.EntityKey, out entry))
        return entry.Entity;
      object obj = (object) null;
      foreach (ObjectStateEntry objectStateEntry in this.InternalContext.ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added).Where<ObjectStateEntry>((Func<ObjectStateEntry, bool>) (e =>
      {
        if (!e.IsRelationship && e.Entity != null)
          return this.EntitySetBaseType.IsAssignableFrom(e.Entity.GetType());
        return false;
      })))
      {
        bool flag = true;
        foreach (KeyValuePair<string, object> keyValuePair in key.KeyValuePairs)
        {
          int ordinal = objectStateEntry.CurrentValues.GetOrdinal(keyValuePair.Key);
          if (!DbHelpers.KeyValuesEqual(keyValuePair.Value, objectStateEntry.CurrentValues.GetValue(ordinal)))
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          if (obj != null)
            throw Error.DbSet_MultipleAddedEntitiesFound();
          obj = objectStateEntry.Entity;
        }
      }
      return obj;
    }

    private object FindInStore(WrappedEntityKey key, string keyValuesParamName)
    {
      if (key.HasNullValues)
        return (object) null;
      try
      {
        return (object) this.BuildFindQuery(key).SingleOrDefault<TEntity>();
      }
      catch (EntitySqlException ex)
      {
        throw new ArgumentException(Strings.DbSet_WrongKeyValueType, keyValuesParamName, (Exception) ex);
      }
    }

    private async Task<object> FindInStoreAsync(
      WrappedEntityKey key,
      string keyValuesParamName,
      CancellationToken cancellationToken)
    {
      if (key.HasNullValues)
        return (object) null;
      try
      {
        return (object) await this.BuildFindQuery(key).SingleOrDefaultAsync<TEntity>(cancellationToken).WithCurrentCulture<TEntity>();
      }
      catch (EntitySqlException ex)
      {
        throw new ArgumentException(Strings.DbSet_WrongKeyValueType, keyValuesParamName, (Exception) ex);
      }
    }

    private System.Data.Entity.Core.Objects.ObjectQuery<TEntity> BuildFindQuery(
      WrappedEntityKey key)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("SELECT VALUE X FROM {0} AS X WHERE ", (object) this.QuotedEntitySetName);
      EntityKeyMember[] entityKeyValues = key.EntityKey.EntityKeyValues;
      ObjectParameter[] objectParameterArray = new ObjectParameter[entityKeyValues.Length];
      for (int index = 0; index < entityKeyValues.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(" AND ");
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "p{0}", (object) index.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.AppendFormat("X.{0} = @{1}", (object) DbHelpers.QuoteIdentifier(entityKeyValues[index].Key), (object) name);
        objectParameterArray[index] = new ObjectParameter(name, entityKeyValues[index].Value);
      }
      return this.InternalContext.ObjectContext.CreateQuery<TEntity>(stringBuilder.ToString(), objectParameterArray);
    }

    public ObservableCollection<TEntity> Local
    {
      get
      {
        this.InternalContext.DetectChanges(false);
        return (ObservableCollection<TEntity>) this._localView ?? (ObservableCollection<TEntity>) (this._localView = new DbLocalView<TEntity>(this.InternalContext));
      }
    }

    public virtual void Attach(object entity)
    {
      this.ActOnSet((Action) (() => this.InternalContext.ObjectContext.AttachTo(this.EntitySetName, entity)), EntityState.Unchanged, entity, nameof (Attach));
    }

    public virtual void Add(object entity)
    {
      this.ActOnSet((Action) (() => this.InternalContext.ObjectContext.AddObject(this.EntitySetName, entity)), EntityState.Added, entity, nameof (Add));
    }

    public virtual void AddRange(IEnumerable entities)
    {
      this.InternalContext.DetectChanges(false);
      this.ActOnSet((Action<object>) (entity => this.InternalContext.ObjectContext.AddObject(this.EntitySetName, entity)), EntityState.Added, entities, nameof (AddRange));
    }

    public virtual void Remove(object entity)
    {
      if (!(entity is TEntity))
        throw Error.DbSet_BadTypeForAddAttachRemove((object) nameof (Remove), (object) entity.GetType().Name, (object) typeof (TEntity).Name);
      this.InternalContext.DetectChanges(false);
      this.InternalContext.ObjectContext.DeleteObject(entity);
    }

    public virtual void RemoveRange(IEnumerable entities)
    {
      List<object> list = entities.Cast<object>().ToList<object>();
      this.InternalContext.DetectChanges(false);
      foreach (object entity in list)
      {
        Check.NotNull<object>(entity, "entity");
        if (!(entity is TEntity))
          throw Error.DbSet_BadTypeForAddAttachRemove((object) nameof (RemoveRange), (object) entity.GetType().Name, (object) typeof (TEntity).Name);
        this.InternalContext.ObjectContext.DeleteObject(entity);
      }
    }

    private void ActOnSet(Action action, EntityState newState, object entity, string methodName)
    {
      if (!(entity is TEntity))
        throw Error.DbSet_BadTypeForAddAttachRemove((object) methodName, (object) entity.GetType().Name, (object) typeof (TEntity).Name);
      this.InternalContext.DetectChanges(false);
      ObjectStateEntry entry;
      if (this.InternalContext.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
        entry.ChangeState(newState);
      else
        action();
    }

    private void ActOnSet(
      Action<object> action,
      EntityState newState,
      IEnumerable entities,
      string methodName)
    {
      foreach (object entity in entities)
      {
        Check.NotNull<object>(entity, "entity");
        if (!(entity is TEntity))
          throw Error.DbSet_BadTypeForAddAttachRemove((object) methodName, (object) entity.GetType().Name, (object) typeof (TEntity).Name);
        ObjectStateEntry entry;
        if (this.InternalContext.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
          entry.ChangeState(newState);
        else
          action(entity);
      }
    }

    public TEntity Create()
    {
      return this.InternalContext.CreateObject<TEntity>();
    }

    public TEntity Create(Type derivedEntityType)
    {
      if (!typeof (TEntity).IsAssignableFrom(derivedEntityType))
        throw Error.DbSet_BadTypeForCreate((object) derivedEntityType.Name, (object) typeof (TEntity).Name);
      return (TEntity) this.InternalContext.CreateObject(ObjectContextTypeCache.GetObjectType(derivedEntityType));
    }

    public override System.Data.Entity.Core.Objects.ObjectQuery<TEntity> ObjectQuery
    {
      get
      {
        this.Initialize();
        return base.ObjectQuery;
      }
    }

    public string EntitySetName
    {
      get
      {
        this.Initialize();
        return this._entitySetName;
      }
    }

    public string QuotedEntitySetName
    {
      get
      {
        this.Initialize();
        return this._quotedEntitySetName;
      }
    }

    public EntitySet EntitySet
    {
      get
      {
        this.Initialize();
        return this._entitySet;
      }
    }

    public Type EntitySetBaseType
    {
      get
      {
        this.Initialize();
        return this._baseType;
      }
    }

    public virtual void Initialize()
    {
      if (this._entitySet != null)
        return;
      EntitySetTypePair andBaseTypeForType = base.InternalContext.GetEntitySetAndBaseTypeForType(typeof (TEntity));
      if (this._entitySet != null)
        return;
      this.InitializeUnderlyingTypes(andBaseTypeForType);
    }

    public virtual void TryInitialize()
    {
      if (this._entitySet != null)
        return;
      EntitySetTypePair andBaseTypeForType = base.InternalContext.TryGetEntitySetAndBaseTypeForType(typeof (TEntity));
      if (andBaseTypeForType == null)
        return;
      this.InitializeUnderlyingTypes(andBaseTypeForType);
    }

    private void InitializeUnderlyingTypes(EntitySetTypePair pair)
    {
      this._entitySet = pair.EntitySet;
      this._baseType = pair.BaseType;
      this._entitySetName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) this._entitySet.EntityContainer.Name, (object) this._entitySet.Name);
      this._quotedEntitySetName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) DbHelpers.QuoteIdentifier(this._entitySet.EntityContainer.Name), (object) DbHelpers.QuoteIdentifier(this._entitySet.Name));
      this.InitializeQuery(this.CreateObjectQuery(false, new bool?(), (IDbExecutionStrategy) null));
    }

    private System.Data.Entity.Core.Objects.ObjectQuery<TEntity> CreateObjectQuery(
      bool asNoTracking,
      bool? streaming = null,
      IDbExecutionStrategy executionStrategy = null)
    {
      System.Data.Entity.Core.Objects.ObjectQuery<TEntity> objectQuery = this.InternalContext.ObjectContext.CreateQuery<TEntity>(this._quotedEntitySetName);
      if (this._baseType != typeof (TEntity))
        objectQuery = objectQuery.OfType<TEntity>();
      if (asNoTracking)
        objectQuery.MergeOption = MergeOption.NoTracking;
      if (streaming.HasValue)
        objectQuery.Streaming = streaming.Value;
      objectQuery.ExecutionStrategy = executionStrategy;
      return objectQuery;
    }

    public override string ToString()
    {
      this.Initialize();
      return base.ToString();
    }

    public override InternalContext InternalContext
    {
      get
      {
        this.Initialize();
        return base.InternalContext;
      }
    }

    public override IInternalQuery<TEntity> Include(string path)
    {
      this.Initialize();
      return base.Include(path);
    }

    public override IInternalQuery<TEntity> AsNoTracking()
    {
      this.Initialize();
      return (IInternalQuery<TEntity>) new InternalQuery<TEntity>(this.InternalContext, (System.Data.Entity.Core.Objects.ObjectQuery) this.CreateObjectQuery(true, new bool?(), (IDbExecutionStrategy) null));
    }

    public override IInternalQuery<TEntity> AsStreaming()
    {
      this.Initialize();
      return (IInternalQuery<TEntity>) new InternalQuery<TEntity>(this.InternalContext, (System.Data.Entity.Core.Objects.ObjectQuery) this.CreateObjectQuery(false, new bool?(true), (IDbExecutionStrategy) null));
    }

    public override IInternalQuery<TEntity> WithExecutionStrategy(
      IDbExecutionStrategy executionStrategy)
    {
      this.Initialize();
      return (IInternalQuery<TEntity>) new InternalQuery<TEntity>(this.InternalContext, (System.Data.Entity.Core.Objects.ObjectQuery) this.CreateObjectQuery(false, new bool?(false), executionStrategy));
    }

    public IEnumerator ExecuteSqlQuery(
      string sql,
      bool asNoTracking,
      bool? streaming,
      object[] parameters)
    {
      this.InternalContext.ObjectContext.AsyncMonitor.EnsureNotEntered();
      this.Initialize();
      MergeOption mergeOption = asNoTracking ? MergeOption.NoTracking : MergeOption.AppendOnly;
      return (IEnumerator) new LazyEnumerator<TEntity>((Func<ObjectResult<TEntity>>) (() => this.InternalContext.ObjectContext.ExecuteStoreQuery<TEntity>(sql, this.EntitySetName, new ExecutionOptions(mergeOption, streaming), parameters)));
    }

    public IDbAsyncEnumerator ExecuteSqlQueryAsync(
      string sql,
      bool asNoTracking,
      bool? streaming,
      object[] parameters)
    {
      this.InternalContext.ObjectContext.AsyncMonitor.EnsureNotEntered();
      this.Initialize();
      MergeOption mergeOption = asNoTracking ? MergeOption.NoTracking : MergeOption.AppendOnly;
      return (IDbAsyncEnumerator) new LazyAsyncEnumerator<TEntity>((Func<CancellationToken, Task<ObjectResult<TEntity>>>) (cancellationToken => this.InternalContext.ObjectContext.ExecuteStoreQueryAsync<TEntity>(sql, this.EntitySetName, new ExecutionOptions(mergeOption, streaming), cancellationToken, parameters)));
    }

    public override Expression Expression
    {
      get
      {
        this.Initialize();
        return base.Expression;
      }
    }

    public override ObjectQueryProvider ObjectQueryProvider
    {
      get
      {
        this.Initialize();
        return base.ObjectQueryProvider;
      }
    }

    public override IEnumerator<TEntity> GetEnumerator()
    {
      this.Initialize();
      return base.GetEnumerator();
    }

    public override IDbAsyncEnumerator<TEntity> GetAsyncEnumerator()
    {
      this.Initialize();
      return base.GetAsyncEnumerator();
    }
  }
}
