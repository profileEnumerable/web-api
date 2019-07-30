// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.InputForComputingCellGroups
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping.ViewGeneration;

namespace System.Data.Entity.Core.Mapping
{
  internal struct InputForComputingCellGroups : IEquatable<InputForComputingCellGroups>, IEqualityComparer<InputForComputingCellGroups>
  {
    internal readonly EntityContainerMapping ContainerMapping;
    internal readonly ConfigViewGenerator Config;

    internal InputForComputingCellGroups(
      EntityContainerMapping containerMapping,
      ConfigViewGenerator config)
    {
      this.ContainerMapping = containerMapping;
      this.Config = config;
    }

    public bool Equals(InputForComputingCellGroups other)
    {
      if (this.ContainerMapping.Equals((object) other.ContainerMapping))
        return this.Config.Equals((object) other.Config);
      return false;
    }

    public bool Equals(InputForComputingCellGroups one, InputForComputingCellGroups two)
    {
      if (object.ReferenceEquals((object) one, (object) two))
        return true;
      if (object.ReferenceEquals((object) one, (object) null) || object.ReferenceEquals((object) two, (object) null))
        return false;
      return one.Equals(two);
    }

    public int GetHashCode(InputForComputingCellGroups value)
    {
      return value.GetHashCode();
    }

    public override int GetHashCode()
    {
      return this.ContainerMapping.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      if (obj is InputForComputingCellGroups)
        return this.Equals((InputForComputingCellGroups) obj);
      return false;
    }

    public static bool operator ==(
      InputForComputingCellGroups input1,
      InputForComputingCellGroups input2)
    {
      if (object.ReferenceEquals((object) input1, (object) input2))
        return true;
      return input1.Equals(input2);
    }

    public static bool operator !=(
      InputForComputingCellGroups input1,
      InputForComputingCellGroups input2)
    {
      return !(input1 == input2);
    }
  }
}
