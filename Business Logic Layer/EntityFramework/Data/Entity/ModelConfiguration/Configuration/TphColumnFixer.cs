// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.TphColumnFixer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class TphColumnFixer
  {
    private readonly IList<ColumnMappingBuilder> _columnMappings;
    private readonly EntityType _table;
    private readonly EdmModel _storeModel;

    public TphColumnFixer(
      IEnumerable<ColumnMappingBuilder> columnMappings,
      EntityType table,
      EdmModel storeModel)
    {
      this._columnMappings = (IList<ColumnMappingBuilder>) columnMappings.OrderBy<ColumnMappingBuilder, string>((Func<ColumnMappingBuilder, string>) (m => m.ColumnProperty.Name)).ToList<ColumnMappingBuilder>();
      this._table = table;
      this._storeModel = storeModel;
    }

    public void RemoveDuplicateTphColumns()
    {
      int index1;
      for (int index2 = 0; index2 < this._columnMappings.Count - 1; index2 = index1)
      {
        StructuralType declaringType = this._columnMappings[index2].PropertyPath[0].DeclaringType;
        EdmProperty column = this._columnMappings[index2].ColumnProperty;
        index1 = index2 + 1;
        EdmType commonBaseType;
        while (index1 < this._columnMappings.Count && column.Name == this._columnMappings[index1].ColumnProperty.Name && (declaringType != this._columnMappings[index1].PropertyPath[0].DeclaringType && TypeSemantics.TryGetCommonBaseType((EdmType) declaringType, (EdmType) this._columnMappings[index1].PropertyPath[0].DeclaringType, out commonBaseType)))
          ++index1;
        System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration configuration1 = column.GetConfiguration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration;
        for (int index3 = index2 + 1; index3 < index1; ++index3)
        {
          ColumnMappingBuilder toFixup = this._columnMappings[index3];
          System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration configuration2 = toFixup.ColumnProperty.GetConfiguration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration;
          string errorMessage;
          if (configuration1 != null && !configuration1.IsCompatible(configuration2, false, out errorMessage))
            throw new MappingException(Strings.BadTphMappingToSharedColumn((object) string.Join(".", this._columnMappings[index2].PropertyPath.Select<EdmProperty, string>((Func<EdmProperty, string>) (p => p.Name))), (object) declaringType.Name, (object) string.Join(".", toFixup.PropertyPath.Select<EdmProperty, string>((Func<EdmProperty, string>) (p => p.Name))), (object) toFixup.PropertyPath[0].DeclaringType.Name, (object) column.Name, (object) column.DeclaringType.Name, (object) errorMessage));
          configuration2?.Configure(column, this._table, this._storeModel.ProviderManifest, false, false);
          column.Nullable = true;
          foreach (AssociationType associationType in this._storeModel.AssociationTypes.Where<AssociationType>((Func<AssociationType, bool>) (a => a.Constraint != null)).Select(a => new
          {
            a = a,
            p = a.Constraint.ToProperties
          }).Where(_param1 =>
          {
            if (!_param1.p.Contains(column))
              return _param1.p.Contains(toFixup.ColumnProperty);
            return true;
          }).Select(_param0 => _param0.a).ToArray<AssociationType>())
            this._storeModel.RemoveAssociationType(associationType);
          if (toFixup.ColumnProperty.DeclaringType.HasMember((EdmMember) toFixup.ColumnProperty))
            toFixup.ColumnProperty.DeclaringType.RemoveMember((EdmMember) toFixup.ColumnProperty);
          toFixup.ColumnProperty = column;
        }
      }
    }
  }
}
