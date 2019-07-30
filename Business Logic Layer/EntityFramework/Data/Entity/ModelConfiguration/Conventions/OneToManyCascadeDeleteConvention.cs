// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to enable cascade delete for any required relationships.
  /// </summary>
  public class OneToManyCascadeDeleteConvention : IConceptualModelConvention<AssociationType>, IConvention
  {
    /// <inheritdoc />
    public virtual void Apply(AssociationType item, DbModel model)
    {
      Check.NotNull<AssociationType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      if (item.IsSelfReferencing())
        return;
      NavigationPropertyConfiguration configuration = item.GetConfiguration() as NavigationPropertyConfiguration;
      if (configuration != null && configuration.DeleteAction.HasValue)
        return;
      AssociationEndMember associationEndMember = (AssociationEndMember) null;
      if (item.IsRequiredToMany())
        associationEndMember = item.SourceEnd;
      else if (item.IsManyToRequired())
        associationEndMember = item.TargetEnd;
      if (associationEndMember == null)
        return;
      associationEndMember.DeleteBehavior = OperationAction.Cascade;
    }
  }
}
