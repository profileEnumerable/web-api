// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaEnumType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class SchemaEnumType : SchemaType
  {
    private readonly IList<SchemaEnumMember> _enumMembers = (IList<SchemaEnumMember>) new List<SchemaEnumMember>();
    private bool _isFlags;
    private string _unresolvedUnderlyingTypeName;
    private SchemaType _underlyingType;

    public SchemaEnumType(Schema parentElement)
      : base(parentElement)
    {
      if (this.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
        return;
      this.OtherContent.Add(this.Schema.SchemaSource);
    }

    public bool IsFlags
    {
      get
      {
        return this._isFlags;
      }
    }

    public SchemaType UnderlyingType
    {
      get
      {
        return this._underlyingType;
      }
    }

    public IEnumerable<SchemaEnumMember> EnumMembers
    {
      get
      {
        return (IEnumerable<SchemaEnumMember>) this._enumMembers;
      }
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (!base.HandleElement(reader))
      {
        if (this.CanHandleElement(reader, "Member"))
        {
          this.HandleMemberElement(reader);
        }
        else
        {
          if (this.CanHandleElement(reader, "ValueAnnotation"))
          {
            this.SkipElement(reader);
            return true;
          }
          if (!this.CanHandleElement(reader, "TypeAnnotation"))
            return false;
          this.SkipElement(reader);
          return true;
        }
      }
      return true;
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (!base.HandleAttribute(reader))
      {
        if (SchemaElement.CanHandleAttribute(reader, "IsFlags"))
        {
          this.HandleBoolAttribute(reader, ref this._isFlags);
        }
        else
        {
          if (!SchemaElement.CanHandleAttribute(reader, "UnderlyingType"))
            return false;
          Utils.GetDottedName(this.Schema, reader, out this._unresolvedUnderlyingTypeName);
        }
      }
      return true;
    }

    private void HandleMemberElement(XmlReader reader)
    {
      SchemaEnumMember schemaEnumMember = new SchemaEnumMember((SchemaElement) this);
      schemaEnumMember.Parse(reader);
      if (!schemaEnumMember.Value.HasValue)
      {
        if (this._enumMembers.Count == 0)
        {
          schemaEnumMember.Value = new long?(0L);
        }
        else
        {
          long num = this._enumMembers[this._enumMembers.Count - 1].Value.Value;
          if (num < long.MaxValue)
          {
            schemaEnumMember.Value = new long?(num + 1L);
          }
          else
          {
            this.AddError(ErrorCode.CalculatedEnumValueOutOfRange, EdmSchemaErrorSeverity.Error, (object) Strings.CalculatedEnumValueOutOfRange);
            schemaEnumMember.Value = new long?(num);
          }
        }
      }
      this._enumMembers.Add(schemaEnumMember);
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    internal override void ResolveTopLevelNames()
    {
      if (this._unresolvedUnderlyingTypeName == null)
        this._underlyingType = this.Schema.SchemaManager.SchemaTypes.Single<SchemaType>((Func<SchemaType, bool>) (t =>
        {
          if (t is ScalarType)
            return ((ScalarType) t).TypeKind == PrimitiveTypeKind.Int32;
          return false;
        }));
      else
        this.Schema.ResolveTypeName((SchemaElement) this, this._unresolvedUnderlyingTypeName, out this._underlyingType);
    }

    internal override void Validate()
    {
      base.Validate();
      ScalarType enumUnderlyingType = this.UnderlyingType as ScalarType;
      if (enumUnderlyingType == null || !Helper.IsSupportedEnumUnderlyingType(enumUnderlyingType.TypeKind))
      {
        this.AddError(ErrorCode.InvalidEnumUnderlyingType, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidEnumUnderlyingType);
      }
      else
      {
        foreach (SchemaEnumMember schemaEnumMember in this._enumMembers.Where<SchemaEnumMember>((Func<SchemaEnumMember, bool>) (m => !Helper.IsEnumMemberValueInRange(enumUnderlyingType.TypeKind, m.Value.Value))))
          schemaEnumMember.AddError(ErrorCode.EnumMemberValueOutOfItsUnderylingTypeRange, EdmSchemaErrorSeverity.Error, (object) Strings.EnumMemberValueOutOfItsUnderylingTypeRange((object) schemaEnumMember.Value, (object) schemaEnumMember.Name, (object) this.UnderlyingType.Name));
      }
      if (!this._enumMembers.GroupBy<SchemaEnumMember, string>((Func<SchemaEnumMember, string>) (o => o.Name)).Where<IGrouping<string, SchemaEnumMember>>((Func<IGrouping<string, SchemaEnumMember>, bool>) (g => g.Count<SchemaEnumMember>() > 1)).Any<IGrouping<string, SchemaEnumMember>>())
        return;
      this.AddError(ErrorCode.DuplicateEnumMember, EdmSchemaErrorSeverity.Error, (object) Strings.DuplicateEnumMember);
    }
  }
}
