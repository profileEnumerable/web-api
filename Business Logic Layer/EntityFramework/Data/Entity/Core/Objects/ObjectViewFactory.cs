// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectViewFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Core.Objects
{
  internal static class ObjectViewFactory
  {
    private static readonly Type _genericObjectViewType = typeof (ObjectView<>);
    private static readonly Type _genericObjectViewDataInterfaceType = typeof (IObjectViewData<>);
    private static readonly Type _genericObjectViewQueryResultDataType = typeof (ObjectViewQueryResultData<>);
    private static readonly Type _genericObjectViewEntityCollectionDataType = typeof (ObjectViewEntityCollectionData<,>);

    internal static IBindingList CreateViewForQuery<TElement>(
      TypeUsage elementEdmTypeUsage,
      IEnumerable<TElement> queryResults,
      ObjectContext objectContext,
      bool forceReadOnly,
      EntitySet singleEntitySet)
    {
      Type type1 = (Type) null;
      TypeUsage ospaceTypeUsage = ObjectViewFactory.GetOSpaceTypeUsage(elementEdmTypeUsage, objectContext);
      if (ospaceTypeUsage == null)
        type1 = typeof (TElement);
      Type clrType = ObjectViewFactory.GetClrType<TElement>(ospaceTypeUsage.EdmType);
      object objectStateManager = (object) objectContext.ObjectStateManager;
      IBindingList bindingList;
      if (clrType == typeof (TElement))
        bindingList = (IBindingList) new ObjectView<TElement>((IObjectViewData<TElement>) new ObjectViewQueryResultData<TElement>((IEnumerable) queryResults, objectContext, forceReadOnly, singleEntitySet), objectStateManager);
      else if (clrType == (Type) null)
      {
        bindingList = (IBindingList) new DataRecordObjectView((IObjectViewData<DbDataRecord>) new ObjectViewQueryResultData<DbDataRecord>((IEnumerable) queryResults, objectContext, true, (EntitySet) null), objectStateManager, (RowType) ospaceTypeUsage.EdmType, typeof (TElement));
      }
      else
      {
        if (!typeof (TElement).IsAssignableFrom(clrType))
          throw EntityUtil.ValueInvalidCast(clrType, typeof (TElement));
        Type type2 = ObjectViewFactory._genericObjectViewQueryResultDataType.MakeGenericType(clrType);
        object viewData = type2.GetDeclaredConstructor(typeof (IEnumerable), typeof (ObjectContext), typeof (bool), typeof (EntitySet)).Invoke(new object[4]
        {
          (object) queryResults,
          (object) objectContext,
          (object) forceReadOnly,
          (object) singleEntitySet
        });
        bindingList = ObjectViewFactory.CreateObjectView(clrType, type2, viewData, objectStateManager);
      }
      return bindingList;
    }

    internal static IBindingList CreateViewForEntityCollection<TElement>(
      EntityType entityType,
      EntityCollection<TElement> entityCollection)
      where TElement : class
    {
      TypeUsage ospaceTypeUsage = ObjectViewFactory.GetOSpaceTypeUsage(entityType == null ? (TypeUsage) null : TypeUsage.Create((EdmType) entityType), entityCollection.ObjectContext);
      Type type1;
      if (ospaceTypeUsage == null)
      {
        type1 = typeof (TElement);
      }
      else
      {
        type1 = ObjectViewFactory.GetClrType<TElement>(ospaceTypeUsage.EdmType);
        if (type1 == (Type) null)
          type1 = typeof (TElement);
      }
      IBindingList bindingList;
      if (type1 == typeof (TElement))
      {
        bindingList = (IBindingList) new ObjectView<TElement>((IObjectViewData<TElement>) new ObjectViewEntityCollectionData<TElement, TElement>(entityCollection), (object) entityCollection);
      }
      else
      {
        if (!typeof (TElement).IsAssignableFrom(type1))
          throw EntityUtil.ValueInvalidCast(type1, typeof (TElement));
        Type type2 = ObjectViewFactory._genericObjectViewEntityCollectionDataType.MakeGenericType(type1, typeof (TElement));
        object viewData = type2.GetDeclaredConstructor(typeof (EntityCollection<TElement>)).Invoke(new object[1]
        {
          (object) entityCollection
        });
        bindingList = ObjectViewFactory.CreateObjectView(type1, type2, viewData, (object) entityCollection);
      }
      return bindingList;
    }

    private static IBindingList CreateObjectView(
      Type clrElementType,
      Type objectViewDataType,
      object viewData,
      object eventDataSource)
    {
      return (IBindingList) ObjectViewFactory._genericObjectViewType.MakeGenericType(clrElementType).GetDeclaredConstructor(objectViewDataType.FindInterfaces((TypeFilter) ((type, unusedFilter) => type.Name == ObjectViewFactory._genericObjectViewDataInterfaceType.Name), (object) null)[0], typeof (object)).Invoke(new object[2]
      {
        viewData,
        eventDataSource
      });
    }

    private static TypeUsage GetOSpaceTypeUsage(
      TypeUsage typeUsage,
      ObjectContext objectContext)
    {
      return typeUsage == null || typeUsage.EdmType == null ? (TypeUsage) null : (typeUsage.EdmType.DataSpace != DataSpace.OSpace ? objectContext?.Perspective.MetadataWorkspace.GetOSpaceTypeUsage(typeUsage) : typeUsage);
    }

    private static Type GetClrType<TElement>(EdmType ospaceEdmType)
    {
      Type type;
      if (ospaceEdmType.BuiltInTypeKind == BuiltInTypeKind.RowType)
      {
        RowType rowType = (RowType) ospaceEdmType;
        if (rowType.InitializerMetadata != null && rowType.InitializerMetadata.ClrType != (Type) null)
        {
          type = rowType.InitializerMetadata.ClrType;
        }
        else
        {
          Type c = typeof (TElement);
          type = typeof (IDataRecord).IsAssignableFrom(c) || c == typeof (object) ? (Type) null : typeof (TElement);
        }
      }
      else
      {
        type = ospaceEdmType.ClrType;
        if (type == (Type) null)
          type = typeof (TElement);
      }
      return type;
    }
  }
}
