// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.ClrTypeAnnotationSerializer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.IO;
using System.Reflection;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class ClrTypeAnnotationSerializer : IMetadataAnnotationSerializer
  {
    public string Serialize(string name, object value)
    {
      return ((Type) value).AssemblyQualifiedName;
    }

    public object Deserialize(string name, string value)
    {
      try
      {
        return (object) Type.GetType(value, false);
      }
      catch (FileLoadException ex)
      {
      }
      catch (TargetInvocationException ex)
      {
      }
      catch (BadImageFormatException ex)
      {
      }
      return (object) null;
    }
  }
}
