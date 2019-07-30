// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Annotations.IndexAttributeExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Infrastructure.Annotations
{
  internal static class IndexAttributeExtensions
  {
    internal static CompatibilityResult IsCompatibleWith(
      this IndexAttribute me,
      IndexAttribute other,
      bool ignoreOrder = false)
    {
      if (object.ReferenceEquals((object) me, (object) other) || other == null)
        return new CompatibilityResult(true, (string) null);
      string errorMessage = (string) null;
      if (me.Name != other.Name)
        errorMessage = Strings.ConflictingIndexAttributeProperty((object) "Name", (object) me.Name, (object) other.Name);
      if (!ignoreOrder && me.Order != -1 && (other.Order != -1 && me.Order != other.Order))
        errorMessage = (errorMessage == null ? "" : errorMessage + Environment.NewLine + "\t") + Strings.ConflictingIndexAttributeProperty((object) "Order", (object) me.Order, (object) other.Order);
      if (me.IsClusteredConfigured && other.IsClusteredConfigured && me.IsClustered != other.IsClustered)
        errorMessage = (errorMessage == null ? "" : errorMessage + Environment.NewLine + "\t") + Strings.ConflictingIndexAttributeProperty((object) "IsClustered", (object) me.IsClustered, (object) other.IsClustered);
      if (me.IsUniqueConfigured && other.IsUniqueConfigured && me.IsUnique != other.IsUnique)
        errorMessage = (errorMessage == null ? "" : errorMessage + Environment.NewLine + "\t") + Strings.ConflictingIndexAttributeProperty((object) "IsUnique", (object) me.IsUnique, (object) other.IsUnique);
      return new CompatibilityResult(errorMessage == null, errorMessage);
    }

    internal static IndexAttribute MergeWith(
      this IndexAttribute me,
      IndexAttribute other,
      bool ignoreOrder = false)
    {
      if (object.ReferenceEquals((object) me, (object) other) || other == null)
        return me;
      CompatibilityResult compatibilityResult = me.IsCompatibleWith(other, ignoreOrder);
      if (!(bool) compatibilityResult)
        throw new InvalidOperationException(Strings.ConflictingIndexAttribute((object) me.Name, (object) (Environment.NewLine + "\t" + compatibilityResult.ErrorMessage)));
      IndexAttribute indexAttribute = me.Name != null ? new IndexAttribute(me.Name) : (other.Name != null ? new IndexAttribute(other.Name) : new IndexAttribute());
      if (!ignoreOrder)
      {
        if (me.Order != -1)
          indexAttribute.Order = me.Order;
        else if (other.Order != -1)
          indexAttribute.Order = other.Order;
      }
      if (me.IsClusteredConfigured)
        indexAttribute.IsClustered = me.IsClustered;
      else if (other.IsClusteredConfigured)
        indexAttribute.IsClustered = other.IsClustered;
      if (me.IsUniqueConfigured)
        indexAttribute.IsUnique = me.IsUnique;
      else if (other.IsUniqueConfigured)
        indexAttribute.IsUnique = other.IsUnique;
      return indexAttribute;
    }
  }
}
