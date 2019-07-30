// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.Utils
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal static class Utils
  {
    internal static void ExtractNamespaceAndName(
      string qualifiedTypeName,
      out string namespaceName,
      out string name)
    {
      Utils.GetBeforeAndAfterLastPeriod(qualifiedTypeName, out namespaceName, out name);
    }

    internal static string ExtractTypeName(string qualifiedTypeName)
    {
      return Utils.GetEverythingAfterLastPeriod(qualifiedTypeName);
    }

    private static void GetBeforeAndAfterLastPeriod(
      string qualifiedTypeName,
      out string before,
      out string after)
    {
      int length = qualifiedTypeName.LastIndexOf('.');
      if (length < 0)
      {
        before = (string) null;
        after = qualifiedTypeName;
      }
      else
      {
        before = qualifiedTypeName.Substring(0, length);
        after = qualifiedTypeName.Substring(length + 1);
      }
    }

    internal static string GetEverythingBeforeLastPeriod(string qualifiedTypeName)
    {
      int length = qualifiedTypeName.LastIndexOf('.');
      if (length < 0)
        return (string) null;
      return qualifiedTypeName.Substring(0, length);
    }

    private static string GetEverythingAfterLastPeriod(string qualifiedTypeName)
    {
      int num = qualifiedTypeName.LastIndexOf('.');
      if (num < 0)
        return qualifiedTypeName;
      return qualifiedTypeName.Substring(num + 1);
    }

    public static bool GetString(System.Data.Entity.Core.SchemaObjectModel.Schema schema, XmlReader reader, out string value)
    {
      if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
      {
        value = (string) null;
        return false;
      }
      value = reader.Value;
      if (!string.IsNullOrEmpty(value))
        return true;
      schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, (object) Strings.InvalidName((object) value, (object) reader.Name));
      return false;
    }

    public static bool GetDottedName(System.Data.Entity.Core.SchemaObjectModel.Schema schema, XmlReader reader, out string name)
    {
      if (!Utils.GetString(schema, reader, out name))
        return false;
      return Utils.ValidateDottedName(schema, reader, name);
    }

    internal static bool ValidateDottedName(System.Data.Entity.Core.SchemaObjectModel.Schema schema, XmlReader reader, string name)
    {
      if (schema.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        string str = name;
        char[] chArray = new char[1]{ '.' };
        foreach (string name1 in str.Split(chArray))
        {
          if (!name1.IsValidUndottedName())
          {
            schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, (object) Strings.InvalidName((object) name, (object) reader.Name));
            return false;
          }
        }
      }
      return true;
    }

    public static bool GetUndottedName(System.Data.Entity.Core.SchemaObjectModel.Schema schema, XmlReader reader, out string name)
    {
      if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
      {
        name = (string) null;
        return false;
      }
      name = reader.Value;
      if (string.IsNullOrEmpty(name))
      {
        schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, (object) Strings.EmptyName((object) reader.Name));
        return false;
      }
      if (schema.DataModel != SchemaDataModelOption.EntityDataModel || name.IsValidUndottedName())
        return true;
      schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, (object) Strings.InvalidName((object) name, (object) reader.Name));
      return false;
    }

    public static bool GetBool(System.Data.Entity.Core.SchemaObjectModel.Schema schema, XmlReader reader, out bool value)
    {
      if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
      {
        value = true;
        return false;
      }
      try
      {
        value = reader.ReadContentAsBoolean();
        return true;
      }
      catch (XmlException ex)
      {
        schema.AddError(ErrorCode.BoolValueExpected, EdmSchemaErrorSeverity.Error, reader, (object) Strings.ValueNotUnderstood((object) reader.Value, (object) reader.Name));
      }
      value = true;
      return false;
    }

    public static bool GetInt(System.Data.Entity.Core.SchemaObjectModel.Schema schema, XmlReader reader, out int value)
    {
      if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
      {
        value = 0;
        return false;
      }
      string s = reader.Value;
      value = int.MinValue;
      if (int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out value))
        return true;
      schema.AddError(ErrorCode.IntegerExpected, EdmSchemaErrorSeverity.Error, reader, (object) Strings.ValueNotUnderstood((object) reader.Value, (object) reader.Name));
      return false;
    }

    public static bool GetByte(System.Data.Entity.Core.SchemaObjectModel.Schema schema, XmlReader reader, out byte value)
    {
      if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
      {
        value = (byte) 0;
        return false;
      }
      string s = reader.Value;
      value = (byte) 0;
      if (byte.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out value))
        return true;
      schema.AddError(ErrorCode.ByteValueExpected, EdmSchemaErrorSeverity.Error, reader, (object) Strings.ValueNotUnderstood((object) reader.Value, (object) reader.Name));
      return false;
    }

    public static int CompareNames(string lhsName, string rhsName)
    {
      return string.Compare(lhsName, rhsName, StringComparison.Ordinal);
    }
  }
}
