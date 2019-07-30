// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ExpensiveOSpaceLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class ExpensiveOSpaceLoader
  {
    public virtual Dictionary<string, EdmType> LoadTypesExpensiveWay(
      Assembly assembly)
    {
      KnownAssembliesSet knownAssemblies = new KnownAssembliesSet();
      Dictionary<string, EdmType> typesInLoading;
      List<EdmItemError> errors;
      AssemblyCache.LoadAssembly(assembly, false, knownAssemblies, out typesInLoading, out errors);
      if (errors.Count != 0)
        throw EntityUtil.InvalidSchemaEncountered(Helper.CombineErrorMessage((IEnumerable<EdmItemError>) errors));
      return typesInLoading;
    }

    public virtual AssociationType GetRelationshipTypeExpensiveWay(
      Type entityClrType,
      string relationshipName)
    {
      Dictionary<string, EdmType> dictionary = this.LoadTypesExpensiveWay(entityClrType.Assembly());
      EdmType type;
      if (dictionary != null && dictionary.TryGetValue(relationshipName, out type) && Helper.IsRelationshipType(type))
        return (AssociationType) type;
      return (AssociationType) null;
    }

    public virtual IEnumerable<AssociationType> GetAllRelationshipTypesExpensiveWay(
      Assembly assembly)
    {
      Dictionary<string, EdmType> typesInLoading = this.LoadTypesExpensiveWay(assembly);
      if (typesInLoading != null)
      {
        foreach (EdmType type in typesInLoading.Values)
        {
          if (Helper.IsAssociationType(type))
            yield return (AssociationType) type;
        }
      }
    }
  }
}
