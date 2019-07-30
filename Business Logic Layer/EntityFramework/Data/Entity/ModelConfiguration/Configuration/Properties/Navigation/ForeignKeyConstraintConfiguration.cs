// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation.ForeignKeyConstraintConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation
{
  internal class ForeignKeyConstraintConfiguration : ConstraintConfiguration
  {
    private readonly List<PropertyInfo> _dependentProperties = new List<PropertyInfo>();
    private readonly bool _isFullySpecified;

    public ForeignKeyConstraintConfiguration()
    {
    }

    internal ForeignKeyConstraintConfiguration(IEnumerable<PropertyInfo> dependentProperties)
    {
      this._dependentProperties.AddRange(dependentProperties);
      this._isFullySpecified = true;
    }

    private ForeignKeyConstraintConfiguration(ForeignKeyConstraintConfiguration source)
    {
      this._dependentProperties.AddRange((IEnumerable<PropertyInfo>) source._dependentProperties);
      this._isFullySpecified = source._isFullySpecified;
    }

    internal override ConstraintConfiguration Clone()
    {
      return (ConstraintConfiguration) new ForeignKeyConstraintConfiguration(this);
    }

    public override bool IsFullySpecified
    {
      get
      {
        return this._isFullySpecified;
      }
    }

    internal IEnumerable<PropertyInfo> ToProperties
    {
      get
      {
        return (IEnumerable<PropertyInfo>) this._dependentProperties;
      }
    }

    public void AddColumn(PropertyInfo propertyInfo)
    {
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      if (this._dependentProperties.ContainsSame(propertyInfo))
        return;
      this._dependentProperties.Add(propertyInfo);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    internal override void Configure(
      AssociationType associationType,
      AssociationEndMember dependentEnd,
      EntityTypeConfiguration entityTypeConfiguration)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ForeignKeyConstraintConfiguration.\u003C\u003Ec__DisplayClasse cDisplayClasse1 = new ForeignKeyConstraintConfiguration.\u003C\u003Ec__DisplayClasse();
      // ISSUE: reference to a compiler-generated field
      cDisplayClasse1.entityTypeConfiguration = entityTypeConfiguration;
      if (!this._dependentProperties.Any<PropertyInfo>())
        return;
      IEnumerable<PropertyInfo> propertyInfos = this._dependentProperties.AsEnumerable<PropertyInfo>();
      if (!this.IsFullySpecified)
      {
        // ISSUE: reference to a compiler-generated field
        if (EntityTypeExtensions.GetClrType(dependentEnd.GetEntityType()) != cDisplayClasse1.entityTypeConfiguration.ClrType)
          return;
        // ISSUE: reference to a compiler-generated method
        IEnumerable<\u003C\u003Ef__AnonymousType41<PropertyInfo, int?>> source = this._dependentProperties.Select(new Func<PropertyInfo, \u003C\u003Ef__AnonymousType41<PropertyInfo, int?>>(cDisplayClasse1.\u003CConfigure\u003Eb__0));
        if (this._dependentProperties.Count > 1 && source.Any(p => !p.ColumnOrder.HasValue))
        {
          ReadOnlyMetadataCollection<EdmProperty> dependentKeys = dependentEnd.GetEntityType().KeyProperties;
          if (dependentKeys.Count != this._dependentProperties.Count || !source.All(fk =>
          {
            // ISSUE: variable of a compiler-generated type
            ForeignKeyConstraintConfiguration.\u003C\u003Ec__DisplayClasse cDisplayClasse = cDisplayClasse1;
            var fk1 = fk;
            return dependentKeys.Any<EdmProperty>((Func<EdmProperty, bool>) (p => p.GetClrPropertyInfo().IsSameAs(fk1.PropertyInfo)));
          }))
          {
            // ISSUE: reference to a compiler-generated field
            throw Error.ForeignKeyAttributeConvention_OrderRequired((object) cDisplayClasse1.entityTypeConfiguration.ClrType);
          }
          propertyInfos = dependentKeys.Select<EdmProperty, PropertyInfo>((Func<EdmProperty, PropertyInfo>) (p => p.GetClrPropertyInfo()));
        }
        else
          propertyInfos = source.OrderBy(p => p.ColumnOrder).Select(p => p.PropertyInfo);
      }
      List<EdmProperty> edmPropertyList = new List<EdmProperty>();
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        EdmProperty primitiveProperty = dependentEnd.GetEntityType().GetDeclaredPrimitiveProperty(propertyInfo);
        if (primitiveProperty == null)
          throw Error.ForeignKeyPropertyNotFound((object) propertyInfo.Name, (object) dependentEnd.GetEntityType().Name);
        edmPropertyList.Add(primitiveProperty);
      }
      AssociationEndMember otherEnd = associationType.GetOtherEnd(dependentEnd);
      ReferentialConstraint referentialConstraint = new ReferentialConstraint((RelationshipEndMember) otherEnd, (RelationshipEndMember) dependentEnd, (IEnumerable<EdmProperty>) otherEnd.GetEntityType().KeyProperties, (IEnumerable<EdmProperty>) edmPropertyList);
      if (otherEnd.IsRequired())
        referentialConstraint.ToProperties.Each<EdmProperty, bool>((Func<EdmProperty, bool>) (p => p.Nullable = false));
      associationType.Constraint = referentialConstraint;
    }

    public bool Equals(ForeignKeyConstraintConfiguration other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      return other.ToProperties.SequenceEqual<PropertyInfo>(this.ToProperties, (IEqualityComparer<PropertyInfo>) new DynamicEqualityComparer<PropertyInfo>((Func<PropertyInfo, PropertyInfo, bool>) ((p1, p2) => p1.IsSameAs(p2))));
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != typeof (ForeignKeyConstraintConfiguration))
        return false;
      return this.Equals((ForeignKeyConstraintConfiguration) obj);
    }

    public override int GetHashCode()
    {
      return this.ToProperties.Aggregate<PropertyInfo, int>(0, (Func<int, PropertyInfo, int>) ((t, p) => t + p.GetHashCode()));
    }
  }
}
