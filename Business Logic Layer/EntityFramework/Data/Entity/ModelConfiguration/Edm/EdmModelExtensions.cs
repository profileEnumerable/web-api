// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.EdmModelExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Edm.Services;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal static class EdmModelExtensions
  {
    public const string DefaultSchema = "dbo";
    public const string DefaultModelNamespace = "CodeFirstNamespace";
    public const string DefaultStoreNamespace = "CodeFirstDatabaseSchema";

    public static System.Data.Entity.Core.Metadata.Edm.EntityType AddTable(
      this EdmModel database,
      string name)
    {
      string str = ((IEnumerable<INamedDataModelItem>) database.EntityTypes).UniquifyName(name);
      System.Data.Entity.Core.Metadata.Edm.EntityType elementType = new System.Data.Entity.Core.Metadata.Edm.EntityType(str, "CodeFirstDatabaseSchema", DataSpace.SSpace);
      database.AddItem(elementType);
      database.AddEntitySet(elementType.Name, elementType, str);
      return elementType;
    }

    public static System.Data.Entity.Core.Metadata.Edm.EntityType AddTable(
      this EdmModel database,
      string name,
      System.Data.Entity.Core.Metadata.Edm.EntityType pkSource)
    {
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType = database.AddTable(name);
      foreach (EdmProperty keyProperty in pkSource.KeyProperties)
        entityType.AddKeyMember((EdmMember) keyProperty.Clone());
      return entityType;
    }

    public static EdmFunction AddFunction(
      this EdmModel database,
      string name,
      EdmFunctionPayload functionPayload)
    {
      EdmFunction edmFunction = new EdmFunction(((IEnumerable<INamedDataModelItem>) database.Functions).UniquifyName(name), "CodeFirstDatabaseSchema", DataSpace.SSpace, functionPayload);
      database.AddItem(edmFunction);
      return edmFunction;
    }

    public static System.Data.Entity.Core.Metadata.Edm.EntityType FindTableByName(
      this EdmModel database,
      DatabaseName tableName)
    {
      IList<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypeList = database.EntityTypes as IList<System.Data.Entity.Core.Metadata.Edm.EntityType> ?? (IList<System.Data.Entity.Core.Metadata.Edm.EntityType>) database.EntityTypes.ToList<System.Data.Entity.Core.Metadata.Edm.EntityType>();
      for (int index = 0; index < entityTypeList.Count; ++index)
      {
        System.Data.Entity.Core.Metadata.Edm.EntityType table = entityTypeList[index];
        DatabaseName tableName1 = table.GetTableName();
        if ((tableName1 != null ? (tableName1.Equals(tableName) ? 1 : 0) : (!string.Equals(table.Name, tableName.Name, StringComparison.Ordinal) ? 0 : (tableName.Schema == null ? 1 : 0))) != 0)
          return table;
      }
      return (System.Data.Entity.Core.Metadata.Edm.EntityType) null;
    }

    public static bool HasCascadeDeletePath(
      this EdmModel model,
      System.Data.Entity.Core.Metadata.Edm.EntityType sourceEntityType,
      System.Data.Entity.Core.Metadata.Edm.EntityType targetEntityType)
    {
      return model.AssociationTypes.SelectMany((Func<AssociationType, IEnumerable<AssociationEndMember>>) (a => a.Members.Cast<AssociationEndMember>()), (a, ae) => new
      {
        a = a,
        ae = ae
      }).Where(_param1 =>
      {
        if (_param1.ae.GetEntityType() == sourceEntityType)
          return _param1.ae.DeleteBehavior == OperationAction.Cascade;
        return false;
      }).Select(_param0 => _param0.a.GetOtherEnd(_param0.ae).GetEntityType()).Any<System.Data.Entity.Core.Metadata.Edm.EntityType>((Func<System.Data.Entity.Core.Metadata.Edm.EntityType, bool>) (et =>
      {
        if (et != targetEntityType)
          return model.HasCascadeDeletePath(et, targetEntityType);
        return true;
      }));
    }

    public static IEnumerable<Type> GetClrTypes(this EdmModel model)
    {
      return model.EntityTypes.Select<System.Data.Entity.Core.Metadata.Edm.EntityType, Type>((Func<System.Data.Entity.Core.Metadata.Edm.EntityType, Type>) (e => EntityTypeExtensions.GetClrType(e))).Union<Type>(model.ComplexTypes.Select<ComplexType, Type>((Func<ComplexType, Type>) (ct => ComplexTypeExtensions.GetClrType(ct))));
    }

    public static NavigationProperty GetNavigationProperty(
      this EdmModel model,
      PropertyInfo propertyInfo)
    {
      IList<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypeList = model.EntityTypes as IList<System.Data.Entity.Core.Metadata.Edm.EntityType> ?? (IList<System.Data.Entity.Core.Metadata.Edm.EntityType>) model.EntityTypes.ToList<System.Data.Entity.Core.Metadata.Edm.EntityType>();
      for (int index = 0; index < entityTypeList.Count; ++index)
      {
        NavigationProperty navigationProperty = entityTypeList[index].GetNavigationProperty(propertyInfo);
        if (navigationProperty != null)
          return navigationProperty;
      }
      return (NavigationProperty) null;
    }

    public static void ValidateAndSerializeCsdl(this EdmModel model, XmlWriter writer)
    {
      List<DataModelErrorEventArgs> csdlErrors = model.SerializeAndGetCsdlErrors(writer);
      if (csdlErrors.Count > 0)
        throw new ModelValidationException((IEnumerable<DataModelErrorEventArgs>) csdlErrors);
    }

    private static List<DataModelErrorEventArgs> SerializeAndGetCsdlErrors(
      this EdmModel model,
      XmlWriter writer)
    {
      List<DataModelErrorEventArgs> validationErrors = new List<DataModelErrorEventArgs>();
      CsdlSerializer csdlSerializer = new CsdlSerializer();
      csdlSerializer.OnError += (EventHandler<DataModelErrorEventArgs>) ((s, e) => validationErrors.Add(e));
      csdlSerializer.Serialize(model, writer, (string) null);
      return validationErrors;
    }

    public static DbDatabaseMapping GenerateDatabaseMapping(
      this EdmModel model,
      DbProviderInfo providerInfo,
      DbProviderManifest providerManifest)
    {
      return new DatabaseMappingGenerator(providerInfo, providerManifest).Generate(model);
    }

    public static EdmType GetStructuralOrEnumType(this EdmModel model, string name)
    {
      return model.GetStructuralType(name) ?? (EdmType) model.GetEnumType(name);
    }

    public static EdmType GetStructuralType(this EdmModel model, string name)
    {
      return (EdmType) model.GetEntityType(name) ?? (EdmType) model.GetComplexType(name);
    }

    public static System.Data.Entity.Core.Metadata.Edm.EntityType GetEntityType(
      this EdmModel model,
      string name)
    {
      return model.EntityTypes.SingleOrDefault<System.Data.Entity.Core.Metadata.Edm.EntityType>((Func<System.Data.Entity.Core.Metadata.Edm.EntityType, bool>) (e => e.Name == name));
    }

    public static System.Data.Entity.Core.Metadata.Edm.EntityType GetEntityType(
      this EdmModel model,
      Type clrType)
    {
      IList<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypeList = model.EntityTypes as IList<System.Data.Entity.Core.Metadata.Edm.EntityType> ?? (IList<System.Data.Entity.Core.Metadata.Edm.EntityType>) model.EntityTypes.ToList<System.Data.Entity.Core.Metadata.Edm.EntityType>();
      for (int index = 0; index < entityTypeList.Count; ++index)
      {
        System.Data.Entity.Core.Metadata.Edm.EntityType entityType = entityTypeList[index];
        if (EntityTypeExtensions.GetClrType(entityType) == clrType)
          return entityType;
      }
      return (System.Data.Entity.Core.Metadata.Edm.EntityType) null;
    }

    public static ComplexType GetComplexType(this EdmModel model, string name)
    {
      return model.ComplexTypes.SingleOrDefault<ComplexType>((Func<ComplexType, bool>) (e => e.Name == name));
    }

    public static ComplexType GetComplexType(this EdmModel model, Type clrType)
    {
      return model.ComplexTypes.SingleOrDefault<ComplexType>((Func<ComplexType, bool>) (e => ComplexTypeExtensions.GetClrType(e) == clrType));
    }

    public static EnumType GetEnumType(this EdmModel model, string name)
    {
      return model.EnumTypes.SingleOrDefault<EnumType>((Func<EnumType, bool>) (e => e.Name == name));
    }

    public static System.Data.Entity.Core.Metadata.Edm.EntityType AddEntityType(
      this EdmModel model,
      string name,
      string modelNamespace = null)
    {
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType = new System.Data.Entity.Core.Metadata.Edm.EntityType(name, modelNamespace ?? "CodeFirstNamespace", DataSpace.CSpace);
      model.AddItem(entityType);
      return entityType;
    }

    public static EntitySet GetEntitySet(this EdmModel model, System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
    {
      return model.GetEntitySets().SingleOrDefault<EntitySet>((Func<EntitySet, bool>) (e => e.ElementType == entityType.GetRootType()));
    }

    public static AssociationSet GetAssociationSet(
      this EdmModel model,
      AssociationType associationType)
    {
      return model.Containers.Single<EntityContainer>().AssociationSets.SingleOrDefault<AssociationSet>((Func<AssociationSet, bool>) (a => a.ElementType == associationType));
    }

    public static IEnumerable<EntitySet> GetEntitySets(this EdmModel model)
    {
      return (IEnumerable<EntitySet>) model.Containers.Single<EntityContainer>().EntitySets;
    }

    public static EntitySet AddEntitySet(
      this EdmModel model,
      string name,
      System.Data.Entity.Core.Metadata.Edm.EntityType elementType,
      string table = null)
    {
      EntitySet entitySet = new EntitySet(name, (string) null, table, (string) null, elementType);
      model.Containers.Single<EntityContainer>().AddEntitySetBase((EntitySetBase) entitySet);
      return entitySet;
    }

    public static ComplexType AddComplexType(
      this EdmModel model,
      string name,
      string modelNamespace = null)
    {
      ComplexType complexType = new ComplexType(name, modelNamespace ?? "CodeFirstNamespace", DataSpace.CSpace);
      model.AddItem(complexType);
      return complexType;
    }

    public static EnumType AddEnumType(
      this EdmModel model,
      string name,
      string modelNamespace = null)
    {
      EnumType enumType = new EnumType(name, modelNamespace ?? "CodeFirstNamespace", PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32), false, DataSpace.CSpace);
      model.AddItem(enumType);
      return enumType;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public static AssociationType GetAssociationType(
      this EdmModel model,
      string name)
    {
      return model.AssociationTypes.SingleOrDefault<AssociationType>((Func<AssociationType, bool>) (a => a.Name == name));
    }

    public static IEnumerable<AssociationType> GetAssociationTypesBetween(
      this EdmModel model,
      System.Data.Entity.Core.Metadata.Edm.EntityType first,
      System.Data.Entity.Core.Metadata.Edm.EntityType second)
    {
      return model.AssociationTypes.Where<AssociationType>((Func<AssociationType, bool>) (a =>
      {
        if (a.SourceEnd.GetEntityType() == first && a.TargetEnd.GetEntityType() == second)
          return true;
        if (a.SourceEnd.GetEntityType() == second)
          return a.TargetEnd.GetEntityType() == first;
        return false;
      }));
    }

    public static AssociationType AddAssociationType(
      this EdmModel model,
      string name,
      System.Data.Entity.Core.Metadata.Edm.EntityType sourceEntityType,
      RelationshipMultiplicity sourceAssociationEndKind,
      System.Data.Entity.Core.Metadata.Edm.EntityType targetEntityType,
      RelationshipMultiplicity targetAssociationEndKind,
      string modelNamespace = null)
    {
      AssociationType associationType = new AssociationType(name, modelNamespace ?? "CodeFirstNamespace", false, DataSpace.CSpace)
      {
        SourceEnd = new AssociationEndMember(name + "_Source", sourceEntityType.GetReferenceType(), sourceAssociationEndKind),
        TargetEnd = new AssociationEndMember(name + "_Target", targetEntityType.GetReferenceType(), targetAssociationEndKind)
      };
      model.AddAssociationType(associationType);
      return associationType;
    }

    public static void AddAssociationType(this EdmModel model, AssociationType associationType)
    {
      model.AddItem(associationType);
    }

    public static void AddAssociationSet(this EdmModel model, AssociationSet associationSet)
    {
      model.Containers.Single<EntityContainer>().AddEntitySetBase((EntitySetBase) associationSet);
    }

    public static void RemoveEntityType(this EdmModel model, System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
    {
      model.RemoveItem(entityType);
      EntityContainer entityContainer = model.Containers.Single<EntityContainer>();
      EntitySet entitySet = entityContainer.EntitySets.SingleOrDefault<EntitySet>((Func<EntitySet, bool>) (a => a.ElementType == entityType));
      if (entitySet == null)
        return;
      entityContainer.RemoveEntitySetBase((EntitySetBase) entitySet);
    }

    public static void ReplaceEntitySet(
      this EdmModel model,
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType,
      EntitySet newSet)
    {
      EntityContainer entityContainer = model.Containers.Single<EntityContainer>();
      EntitySet entitySet = entityContainer.EntitySets.SingleOrDefault<EntitySet>((Func<EntitySet, bool>) (a => a.ElementType == entityType));
      if (entitySet == null)
        return;
      entityContainer.RemoveEntitySetBase((EntitySetBase) entitySet);
      if (newSet == null)
        return;
      foreach (AssociationSet associationSet in model.Containers.Single<EntityContainer>().AssociationSets)
      {
        if (associationSet.SourceSet == entitySet)
          associationSet.SourceSet = newSet;
        if (associationSet.TargetSet == entitySet)
          associationSet.TargetSet = newSet;
      }
    }

    public static void RemoveAssociationType(this EdmModel model, AssociationType associationType)
    {
      model.RemoveItem(associationType);
      EntityContainer entityContainer = model.Containers.Single<EntityContainer>();
      AssociationSet associationSet = entityContainer.AssociationSets.SingleOrDefault<AssociationSet>((Func<AssociationSet, bool>) (a => a.ElementType == associationType));
      if (associationSet == null)
        return;
      entityContainer.RemoveEntitySetBase((EntitySetBase) associationSet);
    }

    public static AssociationSet AddAssociationSet(
      this EdmModel model,
      string name,
      AssociationType associationType)
    {
      AssociationSet associationSet = new AssociationSet(name, associationType)
      {
        SourceSet = model.GetEntitySet(associationType.SourceEnd.GetEntityType()),
        TargetSet = model.GetEntitySet(associationType.TargetEnd.GetEntityType())
      };
      model.Containers.Single<EntityContainer>().AddEntitySetBase((EntitySetBase) associationSet);
      return associationSet;
    }

    public static IEnumerable<System.Data.Entity.Core.Metadata.Edm.EntityType> GetDerivedTypes(
      this EdmModel model,
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
    {
      return model.EntityTypes.Where<System.Data.Entity.Core.Metadata.Edm.EntityType>((Func<System.Data.Entity.Core.Metadata.Edm.EntityType, bool>) (et => et.BaseType == entityType));
    }

    public static IEnumerable<System.Data.Entity.Core.Metadata.Edm.EntityType> GetSelfAndAllDerivedTypes(
      this EdmModel model,
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
    {
      List<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypes = new List<System.Data.Entity.Core.Metadata.Edm.EntityType>();
      EdmModelExtensions.AddSelfAndAllDerivedTypes(model, entityType, entityTypes);
      return (IEnumerable<System.Data.Entity.Core.Metadata.Edm.EntityType>) entityTypes;
    }

    private static void AddSelfAndAllDerivedTypes(
      EdmModel model,
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType,
      List<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypes)
    {
      entityTypes.Add(entityType);
      foreach (System.Data.Entity.Core.Metadata.Edm.EntityType entityType1 in model.EntityTypes.Where<System.Data.Entity.Core.Metadata.Edm.EntityType>((Func<System.Data.Entity.Core.Metadata.Edm.EntityType, bool>) (et => et.BaseType == entityType)))
        EdmModelExtensions.AddSelfAndAllDerivedTypes(model, entityType1, entityTypes);
    }
  }
}
