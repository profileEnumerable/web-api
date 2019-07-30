// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.ReferentialConstraint
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class ReferentialConstraint : SchemaElement
  {
    private const char KEY_DELIMITER = ' ';
    private ReferentialConstraintRoleElement _principalRole;
    private ReferentialConstraintRoleElement _dependentRole;

    public ReferentialConstraint(Relationship relationship)
      : base((SchemaElement) relationship, (IDbDependencyResolver) null)
    {
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal override void Validate()
    {
      base.Validate();
      this._principalRole.Validate();
      this._dependentRole.Validate();
      if (!ReferentialConstraint.ReadyForFurtherValidation(this._principalRole) || !ReferentialConstraint.ReadyForFurtherValidation(this._dependentRole))
        return;
      IRelationshipEnd end1 = this._principalRole.End;
      IRelationshipEnd end2 = this._dependentRole.End;
      if (this._principalRole.Name == this._dependentRole.Name)
        this.AddError(ErrorCode.SameRoleReferredInReferentialConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.SameRoleReferredInReferentialConstraint((object) this.ParentElement.Name));
      bool isKeyProperty1;
      bool areAllPropertiesNullable1;
      bool isAnyPropertyNullable1;
      bool isSubsetOfKeyProperties1;
      ReferentialConstraint.IsKeyProperty(this._dependentRole, end2.Type, out isKeyProperty1, out areAllPropertiesNullable1, out isAnyPropertyNullable1, out isSubsetOfKeyProperties1);
      bool isKeyProperty2;
      bool areAllPropertiesNullable2;
      bool isAnyPropertyNullable2;
      bool isSubsetOfKeyProperties2;
      ReferentialConstraint.IsKeyProperty(this._principalRole, end1.Type, out isKeyProperty2, out areAllPropertiesNullable2, out isAnyPropertyNullable2, out isSubsetOfKeyProperties2);
      if (!isKeyProperty2)
      {
        this.AddError(ErrorCode.InvalidPropertyInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidFromPropertyInRelationshipConstraint((object) this.PrincipalRole.Name, (object) end1.Type.FQName, (object) this.ParentElement.FQName));
      }
      else
      {
        bool flag = this.Schema.SchemaVersion <= 1.1;
        RelationshipMultiplicity relationshipMultiplicity1 = (flag ? (areAllPropertiesNullable2 ? 1 : 0) : (isAnyPropertyNullable2 ? 1 : 0)) != 0 ? RelationshipMultiplicity.ZeroOrOne : RelationshipMultiplicity.One;
        RelationshipMultiplicity relationshipMultiplicity2 = (flag ? (areAllPropertiesNullable1 ? 1 : 0) : (isAnyPropertyNullable1 ? 1 : 0)) != 0 ? RelationshipMultiplicity.ZeroOrOne : RelationshipMultiplicity.Many;
        IRelationshipEnd relationshipEnd1 = end1;
        RelationshipMultiplicity? multiplicity1 = end1.Multiplicity;
        RelationshipMultiplicity? nullable1 = new RelationshipMultiplicity?(multiplicity1.HasValue ? multiplicity1.GetValueOrDefault() : relationshipMultiplicity1);
        relationshipEnd1.Multiplicity = nullable1;
        IRelationshipEnd relationshipEnd2 = end2;
        RelationshipMultiplicity? multiplicity2 = end2.Multiplicity;
        RelationshipMultiplicity? nullable2 = new RelationshipMultiplicity?(multiplicity2.HasValue ? multiplicity2.GetValueOrDefault() : relationshipMultiplicity2);
        relationshipEnd2.Multiplicity = nullable2;
        RelationshipMultiplicity? multiplicity3 = end1.Multiplicity;
        if ((multiplicity3.GetValueOrDefault() != RelationshipMultiplicity.Many ? 0 : (multiplicity3.HasValue ? 1 : 0)) != 0)
        {
          this.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidMultiplicityFromRoleUpperBoundMustBeOne((object) this._principalRole.Name, (object) this.ParentElement.Name));
        }
        else
        {
          if (areAllPropertiesNullable1)
          {
            RelationshipMultiplicity? multiplicity4 = end1.Multiplicity;
            if ((multiplicity4.GetValueOrDefault() != RelationshipMultiplicity.One ? 0 : (multiplicity4.HasValue ? 1 : 0)) != 0)
            {
              this.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidMultiplicityFromRoleToPropertyNullableV1((object) this._principalRole.Name, (object) this.ParentElement.Name));
              goto label_13;
            }
          }
          if (flag && !areAllPropertiesNullable1 || !flag && !isAnyPropertyNullable1)
          {
            RelationshipMultiplicity? multiplicity4 = end1.Multiplicity;
            if ((multiplicity4.GetValueOrDefault() != RelationshipMultiplicity.One ? 1 : (!multiplicity4.HasValue ? 1 : 0)) != 0)
              this.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, !flag ? (object) Strings.InvalidMultiplicityFromRoleToPropertyNonNullableV2((object) this._principalRole.Name, (object) this.ParentElement.Name) : (object) Strings.InvalidMultiplicityFromRoleToPropertyNonNullableV1((object) this._principalRole.Name, (object) this.ParentElement.Name));
          }
        }
label_13:
        RelationshipMultiplicity? multiplicity5 = end2.Multiplicity;
        if ((multiplicity5.GetValueOrDefault() != RelationshipMultiplicity.One ? 0 : (multiplicity5.HasValue ? 1 : 0)) != 0 && this.Schema.DataModel == SchemaDataModelOption.ProviderDataModel)
          this.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidMultiplicityToRoleLowerBoundMustBeZero((object) this._dependentRole.Name, (object) this.ParentElement.Name));
        if (!isSubsetOfKeyProperties1 && !this.ParentElement.IsForeignKey && this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
          this.AddError(ErrorCode.InvalidPropertyInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidToPropertyInRelationshipConstraint((object) this.DependentRole.Name, (object) end2.Type.FQName, (object) this.ParentElement.FQName));
        if (isKeyProperty1)
        {
          RelationshipMultiplicity? multiplicity4 = end2.Multiplicity;
          if ((multiplicity4.GetValueOrDefault() != RelationshipMultiplicity.Many ? 0 : (multiplicity4.HasValue ? 1 : 0)) != 0)
            this.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidMultiplicityToRoleUpperBoundMustBeOne((object) end2.Name, (object) this.ParentElement.Name));
        }
        else
        {
          RelationshipMultiplicity? multiplicity4 = end2.Multiplicity;
          if ((multiplicity4.GetValueOrDefault() != RelationshipMultiplicity.Many ? 1 : (!multiplicity4.HasValue ? 1 : 0)) != 0)
            this.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidMultiplicityToRoleUpperBoundMustBeMany((object) end2.Name, (object) this.ParentElement.Name));
        }
        if (this._dependentRole.RoleProperties.Count != this._principalRole.RoleProperties.Count)
        {
          this.AddError(ErrorCode.MismatchNumberOfPropertiesInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.MismatchNumberOfPropertiesinRelationshipConstraint);
        }
        else
        {
          for (int index = 0; index < this._dependentRole.RoleProperties.Count; ++index)
          {
            if (this._dependentRole.RoleProperties[index].Property.Type != this._principalRole.RoleProperties[index].Property.Type)
              this.AddError(ErrorCode.TypeMismatchRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.TypeMismatchRelationshipConstraint((object) this._dependentRole.RoleProperties[index].Name, (object) this._dependentRole.End.Type.Identity, (object) this._principalRole.RoleProperties[index].Name, (object) this._principalRole.End.Type.Identity, (object) this.ParentElement.Name));
          }
        }
      }
    }

    private static bool ReadyForFurtherValidation(ReferentialConstraintRoleElement role)
    {
      if (role == null || role.End == null || role.RoleProperties.Count == 0)
        return false;
      foreach (PropertyRefElement roleProperty in (IEnumerable<PropertyRefElement>) role.RoleProperties)
      {
        if (roleProperty.Property == null)
          return false;
      }
      return true;
    }

    private static void IsKeyProperty(
      ReferentialConstraintRoleElement roleElement,
      SchemaEntityType itemType,
      out bool isKeyProperty,
      out bool areAllPropertiesNullable,
      out bool isAnyPropertyNullable,
      out bool isSubsetOfKeyProperties)
    {
      isKeyProperty = true;
      areAllPropertiesNullable = true;
      isAnyPropertyNullable = false;
      isSubsetOfKeyProperties = true;
      if (itemType.KeyProperties.Count != roleElement.RoleProperties.Count)
        isKeyProperty = false;
      for (int index1 = 0; index1 < roleElement.RoleProperties.Count; ++index1)
      {
        if (isSubsetOfKeyProperties)
        {
          bool flag = false;
          for (int index2 = 0; index2 < itemType.KeyProperties.Count; ++index2)
          {
            if (itemType.KeyProperties[index2].Property == roleElement.RoleProperties[index1].Property)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            isKeyProperty = false;
            isSubsetOfKeyProperties = false;
          }
        }
        areAllPropertiesNullable &= roleElement.RoleProperties[index1].Property.Nullable;
        isAnyPropertyNullable |= roleElement.RoleProperties[index1].Property.Nullable;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      return false;
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "Principal"))
      {
        this.HandleReferentialConstraintPrincipalRoleElement(reader);
        return true;
      }
      if (!this.CanHandleElement(reader, "Dependent"))
        return false;
      this.HandleReferentialConstraintDependentRoleElement(reader);
      return true;
    }

    internal void HandleReferentialConstraintPrincipalRoleElement(XmlReader reader)
    {
      this._principalRole = new ReferentialConstraintRoleElement(this);
      this._principalRole.Parse(reader);
    }

    internal void HandleReferentialConstraintDependentRoleElement(XmlReader reader)
    {
      this._dependentRole = new ReferentialConstraintRoleElement(this);
      this._dependentRole.Parse(reader);
    }

    internal override void ResolveTopLevelNames()
    {
      this._dependentRole.ResolveTopLevelNames();
      this._principalRole.ResolveTopLevelNames();
    }

    internal IRelationship ParentElement
    {
      get
      {
        return (IRelationship) base.ParentElement;
      }
    }

    internal ReferentialConstraintRoleElement PrincipalRole
    {
      get
      {
        return this._principalRole;
      }
    }

    internal ReferentialConstraintRoleElement DependentRole
    {
      get
      {
        return this._dependentRole;
      }
    }
  }
}
