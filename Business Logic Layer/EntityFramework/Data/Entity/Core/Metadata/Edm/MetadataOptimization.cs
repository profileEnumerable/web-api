// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataOptimization
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataOptimization
  {
    private readonly IDictionary<Type, EntitySetTypePair> _entitySetMappingsCache = (IDictionary<Type, EntitySetTypePair>) new Dictionary<Type, EntitySetTypePair>();
    private object _entitySetMappingsUpdateLock = new object();
    private readonly MetadataWorkspace _workspace;
    private volatile AssociationType[] _csAssociationTypes;
    private volatile AssociationType[] _osAssociationTypes;
    private volatile object[] _csAssociationTypeToSets;

    internal MetadataOptimization(MetadataWorkspace workspace)
    {
      this._workspace = workspace;
    }

    internal IDictionary<Type, EntitySetTypePair> EntitySetMappingCache
    {
      get
      {
        return this._entitySetMappingsCache;
      }
    }

    private void UpdateEntitySetMappings()
    {
      ObjectItemCollection itemCollection = (ObjectItemCollection) this._workspace.GetItemCollection(DataSpace.OSpace);
      ReadOnlyCollection<EntityType> items = this._workspace.GetItems<EntityType>(DataSpace.OSpace);
      Stack<EntityType> entityTypeStack = new Stack<EntityType>();
      foreach (EntityType entityType in items)
      {
        entityTypeStack.Clear();
        EntityType cspaceType = (EntityType) this._workspace.GetEdmSpaceType((StructuralType) entityType);
        do
        {
          entityTypeStack.Push(cspaceType);
          cspaceType = (EntityType) cspaceType.BaseType;
        }
        while (cspaceType != null);
        EntitySet entitySet = (EntitySet) null;
        while (entitySet == null && entityTypeStack.Count > 0)
        {
          cspaceType = entityTypeStack.Pop();
          foreach (EntityContainer entityContainer in this._workspace.GetItems<EntityContainer>(DataSpace.CSpace))
          {
            List<EntitySetBase> list = entityContainer.BaseEntitySets.Where<EntitySetBase>((Func<EntitySetBase, bool>) (s => s.ElementType == cspaceType)).ToList<EntitySetBase>();
            int count = list.Count;
            if (count > 1 || count == 1 && entitySet != null)
              throw Error.DbContext_MESTNotSupported();
            if (count == 1)
              entitySet = (EntitySet) list[0];
          }
        }
        if (entitySet != null)
        {
          EntityType objectSpaceType = (EntityType) this._workspace.GetObjectSpaceType((StructuralType) cspaceType);
          Type clrType1 = itemCollection.GetClrType((StructuralType) entityType);
          Type clrType2 = itemCollection.GetClrType((StructuralType) objectSpaceType);
          this._entitySetMappingsCache[clrType1] = new EntitySetTypePair(entitySet, clrType2);
        }
      }
    }

    internal bool TryUpdateEntitySetMappingsForType(Type entityType)
    {
      if (this._entitySetMappingsCache.ContainsKey(entityType))
        return true;
      Type type = entityType;
      do
      {
        this._workspace.LoadFromAssembly(type.Assembly());
        type = type.BaseType();
      }
      while (type != (Type) null && type != typeof (object));
      lock (this._entitySetMappingsUpdateLock)
      {
        if (this._entitySetMappingsCache.ContainsKey(entityType))
          return true;
        this.UpdateEntitySetMappings();
      }
      return this._entitySetMappingsCache.ContainsKey(entityType);
    }

    internal AssociationType GetCSpaceAssociationType(
      AssociationType osAssociationType)
    {
      return this._csAssociationTypes[osAssociationType.Index];
    }

    internal AssociationSet FindCSpaceAssociationSet(
      AssociationType associationType,
      string endName,
      EntitySet endEntitySet)
    {
      object associationTypeToSets = this.GetCSpaceAssociationTypeToSetsMap()[associationType.Index];
      if (associationTypeToSets == null)
        return (AssociationSet) null;
      AssociationSet associationSet1 = associationTypeToSets as AssociationSet;
      if (associationSet1 != null)
      {
        if (associationSet1.AssociationSetEnds[endName].EntitySet != endEntitySet)
          return (AssociationSet) null;
        return associationSet1;
      }
      foreach (AssociationSet associationSet2 in (AssociationSet[]) associationTypeToSets)
      {
        if (associationSet2.AssociationSetEnds[endName].EntitySet == endEntitySet)
          return associationSet2;
      }
      return (AssociationSet) null;
    }

    internal AssociationSet FindCSpaceAssociationSet(
      AssociationType associationType,
      string endName,
      string entitySetName,
      string entityContainerName,
      out EntitySet endEntitySet)
    {
      object associationTypeToSets = this.GetCSpaceAssociationTypeToSetsMap()[associationType.Index];
      if (associationTypeToSets == null)
      {
        endEntitySet = (EntitySet) null;
        return (AssociationSet) null;
      }
      AssociationSet associationSet1 = associationTypeToSets as AssociationSet;
      if (associationSet1 != null)
      {
        EntitySet entitySet = associationSet1.AssociationSetEnds[endName].EntitySet;
        if (entitySet.Name == entitySetName && entitySet.EntityContainer.Name == entityContainerName)
        {
          endEntitySet = entitySet;
          return associationSet1;
        }
        endEntitySet = (EntitySet) null;
        return (AssociationSet) null;
      }
      foreach (AssociationSet associationSet2 in (AssociationSet[]) associationTypeToSets)
      {
        EntitySet entitySet = associationSet2.AssociationSetEnds[endName].EntitySet;
        if (entitySet.Name == entitySetName && entitySet.EntityContainer.Name == entityContainerName)
        {
          endEntitySet = entitySet;
          return associationSet2;
        }
      }
      endEntitySet = (EntitySet) null;
      return (AssociationSet) null;
    }

    internal AssociationType[] GetCSpaceAssociationTypes()
    {
      if (this._csAssociationTypes == null)
        this._csAssociationTypes = MetadataOptimization.IndexCSpaceAssociationTypes(this._workspace.GetItemCollection(DataSpace.CSpace));
      return this._csAssociationTypes;
    }

    private static AssociationType[] IndexCSpaceAssociationTypes(
      ItemCollection itemCollection)
    {
      List<AssociationType> associationTypeList = new List<AssociationType>();
      int num = 0;
      foreach (AssociationType associationType in itemCollection.GetItems<AssociationType>())
      {
        associationTypeList.Add(associationType);
        associationType.Index = num++;
      }
      return associationTypeList.ToArray();
    }

    internal object[] GetCSpaceAssociationTypeToSetsMap()
    {
      if (this._csAssociationTypeToSets == null)
        this._csAssociationTypeToSets = MetadataOptimization.MapCSpaceAssociationTypeToSets(this._workspace.GetItemCollection(DataSpace.CSpace), this.GetCSpaceAssociationTypes().Length);
      return this._csAssociationTypeToSets;
    }

    private static object[] MapCSpaceAssociationTypeToSets(
      ItemCollection itemCollection,
      int associationTypeCount)
    {
      object[] array = new object[associationTypeCount];
      foreach (EntityContainer entityContainer in itemCollection.GetItems<EntityContainer>())
      {
        foreach (EntitySetBase baseEntitySet in entityContainer.BaseEntitySets)
        {
          AssociationSet newItem = baseEntitySet as AssociationSet;
          if (newItem != null)
          {
            int index = newItem.ElementType.Index;
            MetadataOptimization.AddItemAtIndex<AssociationSet>(array, index, newItem);
          }
        }
      }
      return array;
    }

    internal AssociationType GetOSpaceAssociationType(
      AssociationType cSpaceAssociationType,
      Func<AssociationType> initializer)
    {
      AssociationType[] associationTypes = this.GetOSpaceAssociationTypes();
      int index = cSpaceAssociationType.Index;
      Thread.MemoryBarrier();
      AssociationType associationType = associationTypes[index];
      if (associationType == null)
      {
        associationType = initializer();
        associationType.Index = index;
        associationTypes[index] = associationType;
        Thread.MemoryBarrier();
      }
      return associationType;
    }

    internal AssociationType[] GetOSpaceAssociationTypes()
    {
      if (this._osAssociationTypes == null)
        this._osAssociationTypes = new AssociationType[this.GetCSpaceAssociationTypes().Length];
      return this._osAssociationTypes;
    }

    private static void AddItemAtIndex<T>(object[] array, int index, T newItem) where T : class
    {
      object obj1 = array[index];
      if (obj1 == null)
      {
        array[index] = (object) newItem;
      }
      else
      {
        T obj2 = obj1 as T;
        if ((object) obj2 != null)
        {
          array[index] = (object) new T[2]{ obj2, newItem };
        }
        else
        {
          T[] array1 = (T[]) obj1;
          int length = array1.Length;
          Array.Resize<T>(ref array1, length + 1);
          array1[length] = newItem;
          array[index] = (object) array1;
        }
      }
    }
  }
}
