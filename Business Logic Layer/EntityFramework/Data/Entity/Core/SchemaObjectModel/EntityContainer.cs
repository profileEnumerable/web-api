// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.EntityContainer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  [DebuggerDisplay("Name={Name}")]
  internal sealed class EntityContainer : SchemaType
  {
    private SchemaElementLookUpTable<SchemaElement> _members;
    private ISchemaElementLookUpTable<EntityContainerEntitySet> _entitySets;
    private ISchemaElementLookUpTable<EntityContainerRelationshipSet> _relationshipSets;
    private ISchemaElementLookUpTable<Function> _functionImports;
    private string _unresolvedExtendedEntityContainerName;
    private EntityContainer _entityContainerGettingExtended;
    private bool _isAlreadyValidated;
    private bool _isAlreadyResolved;

    public EntityContainer(Schema parentElement)
      : base(parentElement)
    {
      if (this.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
        return;
      this.OtherContent.Add(this.Schema.SchemaSource);
    }

    private SchemaElementLookUpTable<SchemaElement> Members
    {
      get
      {
        if (this._members == null)
          this._members = new SchemaElementLookUpTable<SchemaElement>();
        return this._members;
      }
    }

    public ISchemaElementLookUpTable<EntityContainerEntitySet> EntitySets
    {
      get
      {
        if (this._entitySets == null)
          this._entitySets = (ISchemaElementLookUpTable<EntityContainerEntitySet>) new FilteredSchemaElementLookUpTable<EntityContainerEntitySet, SchemaElement>(this.Members);
        return this._entitySets;
      }
    }

    public ISchemaElementLookUpTable<EntityContainerRelationshipSet> RelationshipSets
    {
      get
      {
        if (this._relationshipSets == null)
          this._relationshipSets = (ISchemaElementLookUpTable<EntityContainerRelationshipSet>) new FilteredSchemaElementLookUpTable<EntityContainerRelationshipSet, SchemaElement>(this.Members);
        return this._relationshipSets;
      }
    }

    public ISchemaElementLookUpTable<Function> FunctionImports
    {
      get
      {
        if (this._functionImports == null)
          this._functionImports = (ISchemaElementLookUpTable<Function>) new FilteredSchemaElementLookUpTable<Function, SchemaElement>(this.Members);
        return this._functionImports;
      }
    }

    public EntityContainer ExtendingEntityContainer
    {
      get
      {
        return this._entityContainerGettingExtended;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (!SchemaElement.CanHandleAttribute(reader, "Extends"))
        return false;
      this.HandleExtendsAttribute(reader);
      return true;
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "EntitySet"))
      {
        this.HandleEntitySetElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "AssociationSet"))
      {
        this.HandleAssociationSetElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "FunctionImport"))
      {
        this.HandleFunctionImport(reader);
        return true;
      }
      if (this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        if (this.CanHandleElement(reader, "ValueAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "TypeAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
      }
      return false;
    }

    private void HandleEntitySetElement(XmlReader reader)
    {
      EntityContainerEntitySet containerEntitySet = new EntityContainerEntitySet(this);
      containerEntitySet.Parse(reader);
      this.Members.Add((SchemaElement) containerEntitySet, true, new Func<object, string>(Strings.DuplicateEntityContainerMemberName));
    }

    private void HandleAssociationSetElement(XmlReader reader)
    {
      EntityContainerAssociationSet containerAssociationSet = new EntityContainerAssociationSet(this);
      containerAssociationSet.Parse(reader);
      this.Members.Add((SchemaElement) containerAssociationSet, true, new Func<object, string>(Strings.DuplicateEntityContainerMemberName));
    }

    private void HandleFunctionImport(XmlReader reader)
    {
      FunctionImportElement functionImportElement = new FunctionImportElement(this);
      functionImportElement.Parse(reader);
      this.Members.Add((SchemaElement) functionImportElement, true, new Func<object, string>(Strings.DuplicateEntityContainerMemberName));
    }

    private void HandleExtendsAttribute(XmlReader reader)
    {
      this._unresolvedExtendedEntityContainerName = this.HandleUndottedNameAttribute(reader, this._unresolvedExtendedEntityContainerName);
    }

    internal override void ResolveTopLevelNames()
    {
      if (this._isAlreadyResolved)
        return;
      base.ResolveTopLevelNames();
      if (!string.IsNullOrEmpty(this._unresolvedExtendedEntityContainerName))
      {
        if (this._unresolvedExtendedEntityContainerName == this.Name)
        {
          this.AddError(ErrorCode.EntityContainerCannotExtendItself, EdmSchemaErrorSeverity.Error, (object) Strings.EntityContainerCannotExtendItself((object) this.Name));
        }
        else
        {
          SchemaType schemaType;
          if (!this.Schema.SchemaManager.TryResolveType((string) null, this._unresolvedExtendedEntityContainerName, out schemaType))
          {
            this.AddError(ErrorCode.InvalidEntityContainerNameInExtends, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidEntityContainerNameInExtends((object) this._unresolvedExtendedEntityContainerName));
          }
          else
          {
            this._entityContainerGettingExtended = (EntityContainer) schemaType;
            this._entityContainerGettingExtended.ResolveTopLevelNames();
          }
        }
      }
      foreach (SchemaElement member in this.Members)
        member.ResolveTopLevelNames();
      this._isAlreadyResolved = true;
    }

    internal override void ResolveSecondLevelNames()
    {
      base.ResolveSecondLevelNames();
      foreach (SchemaElement member in this.Members)
        member.ResolveSecondLevelNames();
    }

    internal override void Validate()
    {
      if (this._isAlreadyValidated)
        return;
      base.Validate();
      if (this.ExtendingEntityContainer != null)
      {
        this.ExtendingEntityContainer.Validate();
        foreach (SchemaElement member in this.ExtendingEntityContainer.Members)
        {
          AddErrorKind error = this.Members.TryAdd(member.Clone((SchemaElement) this));
          this.DuplicateOrEquivalentMemberNameWhileExtendingEntityContainer(member, error);
        }
      }
      HashSet<string> tableKeys = new HashSet<string>();
      foreach (SchemaElement member in this.Members)
      {
        EntityContainerEntitySet entitySet = member as EntityContainerEntitySet;
        if (entitySet != null && this.Schema.DataModel == SchemaDataModelOption.ProviderDataModel)
          this.CheckForDuplicateTableMapping(tableKeys, entitySet);
        member.Validate();
      }
      this.ValidateRelationshipSetHaveUniqueEnds();
      this.ValidateOnlyBaseEntitySetTypeDefinesConcurrency();
      this._isAlreadyValidated = true;
    }

    internal EntityContainerEntitySet FindEntitySet(string name)
    {
      for (EntityContainer entityContainer = this; entityContainer != null; entityContainer = entityContainer.ExtendingEntityContainer)
      {
        foreach (EntityContainerEntitySet entitySet in entityContainer.EntitySets)
        {
          if (System.Data.Entity.Core.SchemaObjectModel.Utils.CompareNames(entitySet.Name, name) == 0)
            return entitySet;
        }
      }
      return (EntityContainerEntitySet) null;
    }

    private void DuplicateOrEquivalentMemberNameWhileExtendingEntityContainer(
      SchemaElement schemaElement,
      AddErrorKind error)
    {
      if (error == AddErrorKind.Succeeded)
        return;
      schemaElement.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) Strings.DuplicateMemberNameInExtendedEntityContainer((object) schemaElement.Name, (object) this.ExtendingEntityContainer.Name, (object) this.Name));
    }

    private void ValidateOnlyBaseEntitySetTypeDefinesConcurrency()
    {
      Dictionary<SchemaEntityType, EntityContainerEntitySet> baseEntitySetTypes = new Dictionary<SchemaEntityType, EntityContainerEntitySet>();
      foreach (SchemaElement member in this.Members)
      {
        EntityContainerEntitySet containerEntitySet = member as EntityContainerEntitySet;
        if (containerEntitySet != null && !baseEntitySetTypes.ContainsKey(containerEntitySet.EntityType))
          baseEntitySetTypes.Add(containerEntitySet.EntityType, containerEntitySet);
      }
      foreach (SchemaType schemaType in this.Schema.SchemaTypes)
      {
        SchemaEntityType itemType = schemaType as SchemaEntityType;
        EntityContainerEntitySet set;
        if (itemType != null && EntityContainer.TypeIsSubTypeOf(itemType, baseEntitySetTypes, out set) && EntityContainer.TypeDefinesNewConcurrencyProperties(itemType))
          this.AddError(ErrorCode.ConcurrencyRedefinedOnSubTypeOfEntitySetType, EdmSchemaErrorSeverity.Error, (object) Strings.ConcurrencyRedefinedOnSubTypeOfEntitySetType((object) itemType.FQName, (object) set.EntityType.FQName, (object) set.FQName));
      }
    }

    private void ValidateRelationshipSetHaveUniqueEnds()
    {
      List<EntityContainerRelationshipSetEnd> relationshipSetEndList = new List<EntityContainerRelationshipSetEnd>();
      foreach (EntityContainerRelationshipSet relationshipSet in this.RelationshipSets)
      {
        foreach (EntityContainerRelationshipSetEnd end in relationshipSet.Ends)
        {
          bool flag = false;
          foreach (EntityContainerRelationshipSetEnd left in relationshipSetEndList)
          {
            if (EntityContainer.AreRelationshipEndsEqual(left, end))
            {
              this.AddError(ErrorCode.SimilarRelationshipEnd, EdmSchemaErrorSeverity.Error, (object) Strings.SimilarRelationshipEnd((object) left.Name, (object) left.ParentElement.Name, (object) end.ParentElement.Name, (object) left.EntitySet.Name, (object) this.FQName));
              flag = true;
              break;
            }
          }
          if (!flag)
            relationshipSetEndList.Add(end);
        }
      }
    }

    private static bool TypeIsSubTypeOf(
      SchemaEntityType itemType,
      Dictionary<SchemaEntityType, EntityContainerEntitySet> baseEntitySetTypes,
      out EntityContainerEntitySet set)
    {
      if (itemType.IsTypeHierarchyRoot)
      {
        set = (EntityContainerEntitySet) null;
        return false;
      }
      for (SchemaEntityType baseType = itemType.BaseType as SchemaEntityType; baseType != null; baseType = baseType.BaseType as SchemaEntityType)
      {
        if (baseEntitySetTypes.ContainsKey(baseType))
        {
          set = baseEntitySetTypes[baseType];
          return true;
        }
      }
      set = (EntityContainerEntitySet) null;
      return false;
    }

    private static bool TypeDefinesNewConcurrencyProperties(SchemaEntityType itemType)
    {
      foreach (StructuredProperty property in itemType.Properties)
      {
        if (property.Type is ScalarType && MetadataHelper.GetConcurrencyMode(property.TypeUsage) != ConcurrencyMode.None)
          return true;
      }
      return false;
    }

    public override string FQName
    {
      get
      {
        return this.Name;
      }
    }

    public override string Identity
    {
      get
      {
        return this.Name;
      }
    }

    private void CheckForDuplicateTableMapping(
      HashSet<string> tableKeys,
      EntityContainerEntitySet entitySet)
    {
      string str1 = !string.IsNullOrEmpty(entitySet.DbSchema) ? entitySet.DbSchema : this.Name;
      string str2 = !string.IsNullOrEmpty(entitySet.Table) ? entitySet.Table : entitySet.Name;
      string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) str1, (object) str2);
      if (entitySet.DefiningQuery != null)
        str3 = entitySet.Name;
      if (tableKeys.Add(str3))
        return;
      entitySet.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) Strings.DuplicateEntitySetTable((object) entitySet.Name, (object) str1, (object) str2));
    }

    private static bool AreRelationshipEndsEqual(
      EntityContainerRelationshipSetEnd left,
      EntityContainerRelationshipSetEnd right)
    {
      return object.ReferenceEquals((object) left.EntitySet, (object) right.EntitySet) && object.ReferenceEquals((object) left.ParentElement.Relationship, (object) right.ParentElement.Relationship) && left.Name == right.Name;
    }
  }
}
