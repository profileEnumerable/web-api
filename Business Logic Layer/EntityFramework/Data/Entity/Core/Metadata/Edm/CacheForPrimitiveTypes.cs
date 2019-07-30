// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.CacheForPrimitiveTypes
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm.Provider;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class CacheForPrimitiveTypes
  {
    private readonly List<PrimitiveType>[] _primitiveTypeMap = new List<PrimitiveType>[31];

    internal void Add(PrimitiveType type)
    {
      List<PrimitiveType> primitiveTypeList = EntityUtil.CheckArgumentOutOfRange<List<PrimitiveType>>(this._primitiveTypeMap, (int) type.PrimitiveTypeKind, "primitiveTypeKind");
      if (primitiveTypeList == null)
        this._primitiveTypeMap[(int) type.PrimitiveTypeKind] = new List<PrimitiveType>()
        {
          type
        };
      else
        primitiveTypeList.Add(type);
    }

    internal bool TryGetType(
      PrimitiveTypeKind primitiveTypeKind,
      IEnumerable<Facet> facets,
      out PrimitiveType type)
    {
      type = (PrimitiveType) null;
      List<PrimitiveType> primitiveTypeList = EntityUtil.CheckArgumentOutOfRange<List<PrimitiveType>>(this._primitiveTypeMap, (int) primitiveTypeKind, nameof (primitiveTypeKind));
      if (primitiveTypeList == null || 0 >= primitiveTypeList.Count)
        return false;
      if (primitiveTypeList.Count == 1)
      {
        type = primitiveTypeList[0];
        return true;
      }
      if (facets == null)
      {
        FacetDescription[] facetDescriptions = EdmProviderManifest.GetInitialFacetDescriptions(primitiveTypeKind);
        if (facetDescriptions == null)
        {
          type = primitiveTypeList[0];
          return true;
        }
        facets = (IEnumerable<Facet>) CacheForPrimitiveTypes.CreateInitialFacets(facetDescriptions);
      }
      bool flag = false;
      foreach (Facet facet in facets)
      {
        if ((primitiveTypeKind == PrimitiveTypeKind.String || primitiveTypeKind == PrimitiveTypeKind.Binary) && (facet.Value != null && facet.Name == "MaxLength") && Helper.IsUnboundedFacetValue(facet))
          flag = true;
      }
      int num1 = 0;
      foreach (PrimitiveType primitiveType in primitiveTypeList)
      {
        if (flag)
        {
          if (type == null)
          {
            type = primitiveType;
            num1 = Helper.GetFacet((IEnumerable<FacetDescription>) primitiveType.FacetDescriptions, "MaxLength").MaxValue.Value;
          }
          else
          {
            int num2 = Helper.GetFacet((IEnumerable<FacetDescription>) primitiveType.FacetDescriptions, "MaxLength").MaxValue.Value;
            if (num2 > num1)
            {
              type = primitiveType;
              num1 = num2;
            }
          }
        }
        else
        {
          type = primitiveType;
          break;
        }
      }
      return true;
    }

    private static Facet[] CreateInitialFacets(FacetDescription[] facetDescriptions)
    {
      Facet[] facetArray = new Facet[facetDescriptions.Length];
      for (int index = 0; index < facetDescriptions.Length; ++index)
      {
        switch (facetDescriptions[index].FacetName)
        {
          case "MaxLength":
            facetArray[index] = Facet.Create(facetDescriptions[index], (object) TypeUsage.DefaultMaxLengthFacetValue);
            break;
          case "Unicode":
            facetArray[index] = Facet.Create(facetDescriptions[index], (object) true);
            break;
          case "FixedLength":
            facetArray[index] = Facet.Create(facetDescriptions[index], (object) false);
            break;
          case "Precision":
            facetArray[index] = Facet.Create(facetDescriptions[index], (object) TypeUsage.DefaultPrecisionFacetValue);
            break;
          case "Scale":
            facetArray[index] = Facet.Create(facetDescriptions[index], (object) TypeUsage.DefaultScaleFacetValue);
            break;
        }
      }
      return facetArray;
    }

    internal ReadOnlyCollection<PrimitiveType> GetTypes()
    {
      List<PrimitiveType> primitiveTypeList = new List<PrimitiveType>();
      foreach (List<PrimitiveType> primitiveType in this._primitiveTypeMap)
      {
        if (primitiveType != null)
          primitiveTypeList.AddRange((IEnumerable<PrimitiveType>) primitiveType);
      }
      return new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) primitiveTypeList);
    }
  }
}
