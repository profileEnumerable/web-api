// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.ValidationContextExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Internal;

namespace System.Data.Entity.Utilities
{
  internal static class ValidationContextExtensions
  {
    public static void SetDisplayName(
      this ValidationContext validationContext,
      InternalMemberEntry property,
      DisplayAttribute displayAttribute)
    {
      string str = displayAttribute == null ? (string) null : displayAttribute.GetName();
      if (property == null)
      {
        Type objectType = ObjectContextTypeCache.GetObjectType(validationContext.ObjectType);
        validationContext.DisplayName = str ?? objectType.Name;
        validationContext.MemberName = (string) null;
      }
      else
      {
        validationContext.DisplayName = str ?? DbHelpers.GetPropertyPath(property);
        validationContext.MemberName = DbHelpers.GetPropertyPath(property);
      }
    }
  }
}
