// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Annotations.IndexAnnotationSerializer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Entity.Infrastructure.Annotations
{
  /// <summary>
  ///     This class is used to serialize and deserialize <see cref="T:System.Data.Entity.Infrastructure.Annotations.IndexAnnotation" /> objects so that they
  ///     can be stored in the EDMX form of the Entity Framework model.
  /// </summary>
  /// <remarks>
  ///     An example of the serialized format is:
  ///     { Name: 'MyIndex', Order: 7, IsClustered: True, IsUnique: False } { } { Name: 'MyOtherIndex' }.
  ///     Note that properties that have not been explicitly set in an index attribute will be excluded from
  ///     the serialized output. So, in the example above, the first index has all properties specified,
  ///     the second has none, and the third has just the name set.
  /// </remarks>
  public class IndexAnnotationSerializer : IMetadataAnnotationSerializer
  {
    private static readonly Regex _indexesSplitter = new Regex("(?<!\\\\)}\\s*{", RegexOptions.Compiled);
    private static readonly Regex _indexPartsSplitter = new Regex("(?<!\\\\),", RegexOptions.Compiled);
    internal const string FormatExample = "{ Name: MyIndex, Order: 7, IsClustered: True, IsUnique: False } { } { Name: MyOtherIndex }";

    /// <summary>
    ///     Serializes the given <see cref="T:System.Data.Entity.Infrastructure.Annotations.IndexAnnotation" /> into a string for storage in the EDMX XML.
    /// </summary>
    /// <param name="name">The name of the annotation that is being serialized.</param>
    /// <param name="value">The value to serialize which must be an IndexAnnotation object.</param>
    /// <returns>The serialized value.</returns>
    public virtual string Serialize(string name, object value)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<object>(value, nameof (value));
      IndexAnnotation indexAnnotation = value as IndexAnnotation;
      if (indexAnnotation == null)
        throw new ArgumentException(Strings.AnnotationSerializeWrongType((object) value.GetType().Name, (object) typeof (IndexAnnotationSerializer).Name, (object) typeof (IndexAnnotation).Name));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (IndexAttribute index in indexAnnotation.Indexes)
        stringBuilder.Append(IndexAnnotationSerializer.SerializeIndexAttribute(index));
      return stringBuilder.ToString();
    }

    internal static string SerializeIndexAttribute(IndexAttribute indexAttribute)
    {
      StringBuilder stringBuilder = new StringBuilder("{ ");
      if (!string.IsNullOrWhiteSpace(indexAttribute.Name))
        stringBuilder.Append("Name: ").Append(indexAttribute.Name.Replace(",", "\\,").Replace("{", "\\{"));
      if (indexAttribute.Order != -1)
      {
        if (stringBuilder.Length > 2)
          stringBuilder.Append(", ");
        stringBuilder.Append("Order: ").Append(indexAttribute.Order);
      }
      if (indexAttribute.IsClusteredConfigured)
      {
        if (stringBuilder.Length > 2)
          stringBuilder.Append(", ");
        stringBuilder.Append("IsClustered: ").Append(indexAttribute.IsClustered);
      }
      if (indexAttribute.IsUniqueConfigured)
      {
        if (stringBuilder.Length > 2)
          stringBuilder.Append(", ");
        stringBuilder.Append("IsUnique: ").Append(indexAttribute.IsUnique);
      }
      if (stringBuilder.Length > 2)
        stringBuilder.Append(" ");
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    /// <summary>
    ///     Deserializes the given string back into an <see cref="T:System.Data.Entity.Infrastructure.Annotations.IndexAnnotation" /> object.
    /// </summary>
    /// <param name="name">The name of the annotation that is being deserialized.</param>
    /// <param name="value">The string to deserialize.</param>
    /// <returns>The deserialized annotation value.</returns>
    /// <exception cref="T:System.FormatException">If there is an error reading the serialized value.</exception>
    public virtual object Deserialize(string name, string value)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(value, nameof (value));
      value = value.Trim();
      if (!value.StartsWith("{", StringComparison.Ordinal) || !value.EndsWith("}", StringComparison.Ordinal))
        throw IndexAnnotationSerializer.BuildFormatException(value);
      List<IndexAttribute> indexAttributeList = new List<IndexAttribute>();
      List<string> list = ((IEnumerable<string>) IndexAnnotationSerializer._indexesSplitter.Split(value)).Select<string, string>((Func<string, string>) (s => s.Trim())).ToList<string>();
      list[0] = list[0].Substring(1);
      int index = list.Count - 1;
      list[index] = list[index].Substring(0, list[index].Length - 1);
      foreach (string input in list)
      {
        IndexAttribute indexAttribute = new IndexAttribute();
        if (!string.IsNullOrWhiteSpace(input))
        {
          foreach (string str1 in ((IEnumerable<string>) IndexAnnotationSerializer._indexPartsSplitter.Split(input)).Select<string, string>((Func<string, string>) (s => s.Trim())))
          {
            if (str1.StartsWith("Name:", StringComparison.Ordinal))
            {
              string str2 = str1.Substring(5).Trim();
              if (string.IsNullOrWhiteSpace(str2) || !string.IsNullOrWhiteSpace(indexAttribute.Name))
                throw IndexAnnotationSerializer.BuildFormatException(value);
              indexAttribute.Name = str2.Replace("\\,", ",").Replace("\\{", "{");
            }
            else if (str1.StartsWith("Order:", StringComparison.Ordinal))
            {
              int result;
              if (!int.TryParse(str1.Substring(6).Trim(), out result) || indexAttribute.Order != -1)
                throw IndexAnnotationSerializer.BuildFormatException(value);
              indexAttribute.Order = result;
            }
            else if (str1.StartsWith("IsClustered:", StringComparison.Ordinal))
            {
              bool result;
              if (!bool.TryParse(str1.Substring(12).Trim(), out result) || indexAttribute.IsClusteredConfigured)
                throw IndexAnnotationSerializer.BuildFormatException(value);
              indexAttribute.IsClustered = result;
            }
            else
            {
              if (!str1.StartsWith("IsUnique:", StringComparison.Ordinal))
                throw IndexAnnotationSerializer.BuildFormatException(value);
              bool result;
              if (!bool.TryParse(str1.Substring(9).Trim(), out result) || indexAttribute.IsUniqueConfigured)
                throw IndexAnnotationSerializer.BuildFormatException(value);
              indexAttribute.IsUnique = result;
            }
          }
        }
        indexAttributeList.Add(indexAttribute);
      }
      return (object) new IndexAnnotation((IEnumerable<IndexAttribute>) indexAttributeList);
    }

    private static FormatException BuildFormatException(string value)
    {
      return new FormatException(Strings.AnnotationSerializeBadFormat((object) value, (object) typeof (IndexAnnotationSerializer).Name, (object) "{ Name: MyIndex, Order: 7, IsClustered: True, IsUnique: False } { } { Name: MyOtherIndex }"));
    }
  }
}
