// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.StructuredType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal abstract class StructuredType : SchemaType
  {
    private bool? _baseTypeResolveResult;
    private string _unresolvedBaseType;
    private bool _isAbstract;
    private SchemaElementLookUpTable<SchemaElement> _namedMembers;
    private ISchemaElementLookUpTable<StructuredProperty> _properties;

    public StructuredType BaseType { get; private set; }

    public ISchemaElementLookUpTable<StructuredProperty> Properties
    {
      get
      {
        if (this._properties == null)
          this._properties = (ISchemaElementLookUpTable<StructuredProperty>) new FilteredSchemaElementLookUpTable<StructuredProperty, SchemaElement>(this.NamedMembers);
        return this._properties;
      }
    }

    protected SchemaElementLookUpTable<SchemaElement> NamedMembers
    {
      get
      {
        if (this._namedMembers == null)
          this._namedMembers = new SchemaElementLookUpTable<SchemaElement>();
        return this._namedMembers;
      }
    }

    public virtual bool IsTypeHierarchyRoot
    {
      get
      {
        return this.BaseType == null;
      }
    }

    public bool IsAbstract
    {
      get
      {
        return this._isAbstract;
      }
    }

    public StructuredProperty FindProperty(string name)
    {
      StructuredProperty structuredProperty = this.Properties.LookUpEquivalentKey(name);
      if (structuredProperty != null)
        return structuredProperty;
      if (this.IsTypeHierarchyRoot)
        return (StructuredProperty) null;
      return this.BaseType.FindProperty(name);
    }

    public bool IsOfType(StructuredType baseType)
    {
      StructuredType structuredType = this;
      while (structuredType != null && structuredType != baseType)
        structuredType = structuredType.BaseType;
      return structuredType == baseType;
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      this.TryResolveBaseType();
      foreach (SchemaElement namedMember in this.NamedMembers)
        namedMember.ResolveTopLevelNames();
    }

    internal override void Validate()
    {
      base.Validate();
      foreach (SchemaElement namedMember in this.NamedMembers)
      {
        if (this.BaseType != null)
        {
          string str = (string) null;
          StructuredType definingType;
          SchemaElement definingMember;
          if (StructuredType.HowDefined.AsMember == this.BaseType.DefinesMemberName(namedMember.Name, out definingType, out definingMember))
            str = Strings.DuplicateMemberName((object) namedMember.Name, (object) this.FQName, (object) definingType.FQName);
          if (str != null)
            namedMember.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) str);
        }
        namedMember.Validate();
      }
    }

    protected StructuredType(Schema parentElement)
      : base(parentElement)
    {
    }

    protected void AddMember(SchemaElement newMember)
    {
      if (string.IsNullOrEmpty(newMember.Name))
        return;
      if (this.Schema.DataModel != SchemaDataModelOption.ProviderDataModel && Utils.CompareNames(newMember.Name, this.Name) == 0)
        newMember.AddError(ErrorCode.BadProperty, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidMemberNameMatchesTypeName((object) newMember.Name, (object) this.FQName));
      this.NamedMembers.Add(newMember, true, new Func<object, string>(Strings.PropertyNameAlreadyDefinedDuplicate));
    }

    private StructuredType.HowDefined DefinesMemberName(
      string name,
      out StructuredType definingType,
      out SchemaElement definingMember)
    {
      if (this.NamedMembers.ContainsKey(name))
      {
        definingType = this;
        definingMember = this.NamedMembers[name];
        return StructuredType.HowDefined.AsMember;
      }
      definingMember = this.NamedMembers.LookUpEquivalentKey(name);
      if (!this.IsTypeHierarchyRoot)
        return this.BaseType.DefinesMemberName(name, out definingType, out definingMember);
      definingType = (StructuredType) null;
      definingMember = (SchemaElement) null;
      return StructuredType.HowDefined.NotDefined;
    }

    protected string UnresolvedBaseType
    {
      get
      {
        return this._unresolvedBaseType;
      }
      set
      {
        this._unresolvedBaseType = value;
      }
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (!this.CanHandleElement(reader, "Property"))
        return false;
      this.HandlePropertyElement(reader);
      return true;
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (SchemaElement.CanHandleAttribute(reader, "BaseType"))
      {
        this.HandleBaseTypeAttribute(reader);
        return true;
      }
      if (!SchemaElement.CanHandleAttribute(reader, "Abstract"))
        return false;
      this.HandleAbstractAttribute(reader);
      return true;
    }

    private bool TryResolveBaseType()
    {
      if (this._baseTypeResolveResult.HasValue)
        return this._baseTypeResolveResult.Value;
      if (this.BaseType != null)
      {
        this._baseTypeResolveResult = new bool?(true);
        return this._baseTypeResolveResult.Value;
      }
      if (this.UnresolvedBaseType == null)
      {
        this._baseTypeResolveResult = new bool?(true);
        return this._baseTypeResolveResult.Value;
      }
      SchemaType type;
      if (!this.Schema.ResolveTypeName((SchemaElement) this, this.UnresolvedBaseType, out type))
      {
        this._baseTypeResolveResult = new bool?(false);
        return this._baseTypeResolveResult.Value;
      }
      this.BaseType = type as StructuredType;
      if (this.BaseType == null)
      {
        this.AddError(ErrorCode.InvalidBaseType, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidBaseTypeForStructuredType((object) this.UnresolvedBaseType, (object) this.FQName));
        this._baseTypeResolveResult = new bool?(false);
        return this._baseTypeResolveResult.Value;
      }
      if (this.CheckForInheritanceCycle())
      {
        this.BaseType = (StructuredType) null;
        this.AddError(ErrorCode.CycleInTypeHierarchy, EdmSchemaErrorSeverity.Error, (object) Strings.CycleInTypeHierarchy((object) this.FQName));
        this._baseTypeResolveResult = new bool?(false);
        return this._baseTypeResolveResult.Value;
      }
      this._baseTypeResolveResult = new bool?(true);
      return true;
    }

    private void HandleBaseTypeAttribute(XmlReader reader)
    {
      string name;
      if (!Utils.GetDottedName(this.Schema, reader, out name))
        return;
      this.UnresolvedBaseType = name;
    }

    private void HandleAbstractAttribute(XmlReader reader)
    {
      this.HandleBoolAttribute(reader, ref this._isAbstract);
    }

    private void HandlePropertyElement(XmlReader reader)
    {
      StructuredProperty structuredProperty = new StructuredProperty(this);
      structuredProperty.Parse(reader);
      this.AddMember((SchemaElement) structuredProperty);
    }

    private bool CheckForInheritanceCycle()
    {
      StructuredType baseType = this.BaseType;
      StructuredType structuredType1 = baseType;
      StructuredType structuredType2 = baseType;
      do
      {
        structuredType2 = structuredType2.BaseType;
        if (object.ReferenceEquals((object) structuredType1, (object) structuredType2))
          return true;
        if (structuredType1 == null)
          return false;
        structuredType1 = structuredType1.BaseType;
        if (structuredType2 != null)
          structuredType2 = structuredType2.BaseType;
      }
      while (structuredType2 != null);
      return false;
    }

    private enum HowDefined
    {
      NotDefined,
      AsMember,
    }
  }
}
