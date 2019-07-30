// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ClrEntityType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
  internal sealed class ClrEntityType : EntityType
  {
    private readonly Type _type;
    private Func<object> _constructor;
    private readonly string _cspaceTypeName;
    private readonly string _cspaceNamespaceName;
    private string _hash;

    internal ClrEntityType(Type type, string cspaceNamespaceName, string cspaceTypeName)
      : base(Check.NotNull<Type>(type, nameof (type)).Name, type.NestingNamespace() ?? string.Empty, DataSpace.OSpace)
    {
      this._type = type;
      this._cspaceNamespaceName = cspaceNamespaceName;
      this._cspaceTypeName = cspaceNamespaceName + "." + cspaceTypeName;
      this.Abstract = type.IsAbstract();
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal Func<object> Constructor
    {
      get
      {
        return this._constructor;
      }
      set
      {
        Interlocked.CompareExchange<Func<object>>(ref this._constructor, value, (Func<object>) null);
      }
    }

    internal override Type ClrType
    {
      get
      {
        return this._type;
      }
    }

    internal string CSpaceTypeName
    {
      get
      {
        return this._cspaceTypeName;
      }
    }

    internal string CSpaceNamespaceName
    {
      get
      {
        return this._cspaceNamespaceName;
      }
    }

    internal string HashedDescription
    {
      get
      {
        if (this._hash == null)
          Interlocked.CompareExchange<string>(ref this._hash, this.BuildEntityTypeHash(), (string) null);
        return this._hash;
      }
    }

    private string BuildEntityTypeHash()
    {
      using (SHA256 a256HashAlgorithm = MetadataHelper.CreateSHA256HashAlgorithm())
      {
        byte[] hash = a256HashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(this.BuildEntityTypeDescription()));
        StringBuilder stringBuilder = new StringBuilder(hash.Length * 2);
        foreach (byte num in hash)
          stringBuilder.Append(num.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
        return stringBuilder.ToString();
      }
    }

    private string BuildEntityTypeDescription()
    {
      StringBuilder stringBuilder = new StringBuilder(512);
      stringBuilder.Append("CLR:").Append(this.ClrType.FullName);
      stringBuilder.Append("Conceptual:").Append(this.CSpaceTypeName);
      SortedSet<string> sortedSet1 = new SortedSet<string>();
      foreach (NavigationProperty navigationProperty in this.NavigationProperties)
        sortedSet1.Add(navigationProperty.Name + "*" + navigationProperty.FromEndMember.Name + "*" + (object) navigationProperty.FromEndMember.RelationshipMultiplicity + "*" + navigationProperty.ToEndMember.Name + "*" + (object) navigationProperty.ToEndMember.RelationshipMultiplicity + "*");
      stringBuilder.Append("NavProps:");
      foreach (string str in sortedSet1)
        stringBuilder.Append(str);
      SortedSet<string> sortedSet2 = new SortedSet<string>();
      foreach (string keyMemberName in this.KeyMemberNames)
        sortedSet2.Add(keyMemberName);
      stringBuilder.Append("Keys:");
      foreach (string str in sortedSet2)
        stringBuilder.Append(str + "*");
      SortedSet<string> sortedSet3 = new SortedSet<string>();
      foreach (EdmMember member in this.Members)
      {
        if (!sortedSet2.Contains(member.Name))
          sortedSet3.Add(member.Name + "*");
      }
      stringBuilder.Append("Scalars:");
      foreach (string str in sortedSet3)
        stringBuilder.Append(str + "*");
      return stringBuilder.ToString();
    }
  }
}
