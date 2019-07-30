// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ProxyDataContractResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Runtime.Serialization;
using System.Xml;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// A DataContractResolver that knows how to resolve proxy types created for persistent
  /// ignorant classes to their base types. This is used with the DataContractSerializer.
  /// </summary>
  public class ProxyDataContractResolver : DataContractResolver
  {
    private readonly XsdDataContractExporter _exporter = new XsdDataContractExporter();

    /// <summary>During deserialization, maps any xsi:type information to the actual type of the persistence-ignorant object. </summary>
    /// <returns>Returns the type that the xsi:type is mapped to. Returns null if no known type was found that matches the xsi:type.</returns>
    /// <param name="typeName">The xsi:type information to map.</param>
    /// <param name="typeNamespace">The namespace of the xsi:type.</param>
    /// <param name="declaredType">The declared type.</param>
    /// <param name="knownTypeResolver">
    /// An instance of <see cref="T:System.Data.Entity.Core.Objects.ProxyDataContractResolver" />.
    /// </param>
    public override Type ResolveName(
      string typeName,
      string typeNamespace,
      Type declaredType,
      DataContractResolver knownTypeResolver)
    {
      Check.NotEmpty(typeName, nameof (typeName));
      Check.NotEmpty(typeNamespace, nameof (typeNamespace));
      Check.NotNull<Type>(declaredType, nameof (declaredType));
      Check.NotNull<DataContractResolver>(knownTypeResolver, nameof (knownTypeResolver));
      return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, (DataContractResolver) null);
    }

    /// <summary>During serialization, maps actual types to xsi:type information.</summary>
    /// <returns>true if the type was resolved; otherwise, false.  </returns>
    /// <param name="type">The actual type of the persistence-ignorant object.</param>
    /// <param name="declaredType">The declared type.</param>
    /// <param name="knownTypeResolver">
    /// An instance of <see cref="T:System.Data.Entity.Core.Objects.ProxyDataContractResolver" />.
    /// </param>
    /// <param name="typeName">When this method returns, contains a list of xsi:type declarations.</param>
    /// <param name="typeNamespace">When this method returns, contains a list of namespaces used.</param>
    public override bool TryResolveType(
      Type type,
      Type declaredType,
      DataContractResolver knownTypeResolver,
      out XmlDictionaryString typeName,
      out XmlDictionaryString typeNamespace)
    {
      Check.NotNull<Type>(type, nameof (type));
      Check.NotNull<Type>(declaredType, nameof (declaredType));
      Check.NotNull<DataContractResolver>(knownTypeResolver, nameof (knownTypeResolver));
      Type objectType = ObjectContext.GetObjectType(type);
      if (!(objectType != type))
        return knownTypeResolver.TryResolveType(type, declaredType, (DataContractResolver) null, out typeName, out typeNamespace);
      XmlQualifiedName schemaTypeName = this._exporter.GetSchemaTypeName(objectType);
      XmlDictionary xmlDictionary = new XmlDictionary(2);
      typeName = new XmlDictionaryString((IXmlDictionary) xmlDictionary, schemaTypeName.Name, 0);
      typeNamespace = new XmlDictionaryString((IXmlDictionary) xmlDictionary, schemaTypeName.Namespace, 1);
      return true;
    }
  }
}
