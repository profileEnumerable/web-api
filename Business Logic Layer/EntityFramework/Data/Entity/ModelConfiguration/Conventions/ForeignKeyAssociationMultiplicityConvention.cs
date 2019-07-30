// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ForeignKeyAssociationMultiplicityConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to distinguish between optional and required relationships based on CLR nullability of the foreign key property.
  /// </summary>
  public class ForeignKeyAssociationMultiplicityConvention : IConceptualModelConvention<AssociationType>, IConvention
  {
    /// <inheritdoc />
    public virtual void Apply(AssociationType item, DbModel model)
    {
      Check.NotNull<AssociationType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      ReferentialConstraint constraint = item.Constraint;
      if (constraint == null)
        return;
      NavigationPropertyConfiguration configuration = item.Annotations.GetConfiguration() as NavigationPropertyConfiguration;
      if (!constraint.ToProperties.All<EdmProperty>((Func<EdmProperty, bool>) (p => !p.Nullable)))
        return;
      AssociationEndMember principalEnd = item.GetOtherEnd(constraint.DependentEnd);
      NavigationProperty navigationProperty = model.ConceptualModel.EntityTypes.SelectMany<EntityType, NavigationProperty>((Func<EntityType, IEnumerable<NavigationProperty>>) (et => (IEnumerable<NavigationProperty>) et.DeclaredNavigationProperties)).SingleOrDefault<NavigationProperty>((Func<NavigationProperty, bool>) (np => np.ResultEnd == principalEnd));
      PropertyInfo clrPropertyInfo;
      if (configuration != null && navigationProperty != null && (clrPropertyInfo = navigationProperty.Annotations.GetClrPropertyInfo()) != (PropertyInfo) null && (clrPropertyInfo == configuration.NavigationProperty && configuration.RelationshipMultiplicity.HasValue || clrPropertyInfo == configuration.InverseNavigationProperty && configuration.InverseEndKind.HasValue))
        return;
      principalEnd.RelationshipMultiplicity = RelationshipMultiplicity.One;
    }
  }
}
