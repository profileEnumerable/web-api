// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.Function
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class Function : SchemaType
  {
    private static readonly Regex _typeParser = new Regex("^(?<modifier>((Collection)|(Ref)))\\s*\\(\\s*(?<typeName>\\S*)\\s*\\)$", RegexOptions.Compiled);
    protected bool _isComposable = true;
    private bool _isAggregate;
    private bool _isBuiltIn;
    private bool _isNiladicFunction;
    protected FunctionCommandText _commandText;
    private string _storeFunctionName;
    protected SchemaType _type;
    private string _unresolvedType;
    protected bool _isRefType;
    protected SchemaElementLookUpTable<Parameter> _parameters;
    protected List<ReturnType> _returnTypeList;
    private CollectionKind _returnTypeCollectionKind;
    private ParameterTypeSemantics _parameterTypeSemantics;
    private string _schema;
    private string _functionStrongName;

    internal static void RemoveTypeModifier(
      ref string type,
      out TypeModifier typeModifier,
      out bool isRefType)
    {
      isRefType = false;
      typeModifier = TypeModifier.None;
      Match match = Function._typeParser.Match(type);
      if (!match.Success)
        return;
      type = match.Groups["typeName"].Value;
      switch (match.Groups["modifier"].Value)
      {
        case "Collection":
          typeModifier = TypeModifier.Array;
          break;
        case "Ref":
          isRefType = true;
          break;
      }
    }

    internal static string GetTypeNameForErrorMessage(
      SchemaType type,
      CollectionKind colKind,
      bool isRef)
    {
      string str = type.FQName;
      if (isRef)
        str = "Ref(" + str + ")";
      if (colKind == CollectionKind.Bag)
        str = "Collection(" + str + ")";
      return str;
    }

    public Function(Schema parentElement)
      : base(parentElement)
    {
    }

    public bool IsAggregate
    {
      get
      {
        return this._isAggregate;
      }
      internal set
      {
        this._isAggregate = value;
      }
    }

    public bool IsBuiltIn
    {
      get
      {
        return this._isBuiltIn;
      }
      internal set
      {
        this._isBuiltIn = value;
      }
    }

    public bool IsNiladicFunction
    {
      get
      {
        return this._isNiladicFunction;
      }
      internal set
      {
        this._isNiladicFunction = value;
      }
    }

    public bool IsComposable
    {
      get
      {
        return this._isComposable;
      }
      internal set
      {
        this._isComposable = value;
      }
    }

    public string CommandText
    {
      get
      {
        if (this._commandText != null)
          return this._commandText.CommandText;
        return (string) null;
      }
    }

    public ParameterTypeSemantics ParameterTypeSemantics
    {
      get
      {
        return this._parameterTypeSemantics;
      }
      internal set
      {
        this._parameterTypeSemantics = value;
      }
    }

    public string StoreFunctionName
    {
      get
      {
        return this._storeFunctionName;
      }
      internal set
      {
        this._storeFunctionName = value;
      }
    }

    public virtual SchemaType Type
    {
      get
      {
        if (this._returnTypeList != null)
          return this._returnTypeList[0].Type;
        return this._type;
      }
    }

    public IList<ReturnType> ReturnTypeList
    {
      get
      {
        if (this._returnTypeList == null)
          return (IList<ReturnType>) null;
        return (IList<ReturnType>) new ReadOnlyCollection<ReturnType>((IList<ReturnType>) this._returnTypeList);
      }
    }

    public SchemaElementLookUpTable<Parameter> Parameters
    {
      get
      {
        if (this._parameters == null)
          this._parameters = new SchemaElementLookUpTable<Parameter>();
        return this._parameters;
      }
    }

    public CollectionKind CollectionKind
    {
      get
      {
        return this._returnTypeCollectionKind;
      }
      internal set
      {
        this._returnTypeCollectionKind = value;
      }
    }

    public override string Identity
    {
      get
      {
        if (string.IsNullOrEmpty(this._functionStrongName))
        {
          StringBuilder builder = new StringBuilder(this.FQName);
          bool flag = true;
          builder.Append('(');
          foreach (Parameter parameter in this.Parameters)
          {
            if (!flag)
              builder.Append(',');
            else
              flag = false;
            builder.Append(Helper.ToString(parameter.ParameterDirection));
            builder.Append(' ');
            parameter.WriteIdentity(builder);
          }
          builder.Append(')');
          this._functionStrongName = builder.ToString();
        }
        return this._functionStrongName;
      }
    }

    public bool IsReturnAttributeReftype
    {
      get
      {
        return this._isRefType;
      }
    }

    public virtual bool IsFunctionImport
    {
      get
      {
        return false;
      }
    }

    public string DbSchema
    {
      get
      {
        return this._schema;
      }
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "CommandText"))
      {
        this.HandleCommandTextFunctionElment(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "Parameter"))
      {
        this.HandleParameterElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "ReturnType"))
      {
        this.HandleReturnTypeElement(reader);
        return true;
      }
      if (this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        if (this.CanHandleElement(reader, "ValueAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "TypeAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
      }
      return false;
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (SchemaElement.CanHandleAttribute(reader, "ReturnType"))
      {
        this.HandleReturnTypeAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Aggregate"))
      {
        this.HandleAggregateAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "BuiltIn"))
      {
        this.HandleBuiltInAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "StoreFunctionName"))
      {
        this.HandleStoreFunctionNameAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "NiladicFunction"))
      {
        this.HandleNiladicFunctionAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "IsComposable"))
      {
        this.HandleIsComposableAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "ParameterTypeSemantics"))
      {
        this.HandleParameterTypeSemanticsAttribute(reader);
        return true;
      }
      if (!SchemaElement.CanHandleAttribute(reader, "Schema"))
        return false;
      this.HandleDbSchemaAttribute(reader);
      return true;
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      if (this._unresolvedType != null)
        this.Schema.ResolveTypeName((SchemaElement) this, this.UnresolvedReturnType, out this._type);
      if (this._returnTypeList != null)
      {
        foreach (SchemaElement returnType in this._returnTypeList)
          returnType.ResolveTopLevelNames();
      }
      foreach (SchemaElement parameter in this.Parameters)
        parameter.ResolveTopLevelNames();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal override void Validate()
    {
      base.Validate();
      if (this._type != null && this._returnTypeList != null)
        this.AddError(ErrorCode.ReturnTypeDeclaredAsAttributeAndElement, EdmSchemaErrorSeverity.Error, (object) Strings.TypeDeclaredAsAttributeAndElement);
      if (this._returnTypeList == null && this.Type == null)
      {
        if (this.IsComposable)
          this.AddError(ErrorCode.ComposableFunctionOrFunctionImportWithoutReturnType, EdmSchemaErrorSeverity.Error, (object) Strings.ComposableFunctionOrFunctionImportMustDeclareReturnType);
      }
      else if (!this.IsComposable && !this.IsFunctionImport)
        this.AddError(ErrorCode.NonComposableFunctionWithReturnType, EdmSchemaErrorSeverity.Error, (object) Strings.NonComposableFunctionMustNotDeclareReturnType);
      if (this.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
      {
        if (this.IsAggregate)
        {
          if (this.Parameters.Count != 1)
            this.AddError(ErrorCode.InvalidNumberOfParametersForAggregateFunction, EdmSchemaErrorSeverity.Error, (SchemaElement) this, (object) Strings.InvalidNumberOfParametersForAggregateFunction((object) this.FQName));
          else if (this.Parameters.GetElementAt(0).CollectionKind == CollectionKind.None)
            this.AddError(ErrorCode.InvalidParameterTypeForAggregateFunction, EdmSchemaErrorSeverity.Error, (SchemaElement) this, (object) Strings.InvalidParameterTypeForAggregateFunction((object) this.Parameters.GetElementAt(0).Name, (object) this.FQName));
        }
        if (!this.IsComposable && (this.IsAggregate || this.IsNiladicFunction || this.IsBuiltIn))
          this.AddError(ErrorCode.NonComposableFunctionAttributesNotValid, EdmSchemaErrorSeverity.Error, (object) Strings.NonComposableFunctionHasDisallowedAttribute);
        if (this.CommandText != null)
        {
          if (this.IsComposable)
            this.AddError(ErrorCode.ComposableFunctionWithCommandText, EdmSchemaErrorSeverity.Error, (object) Strings.CommandTextFunctionsNotComposable);
          if (this.StoreFunctionName != null)
            this.AddError(ErrorCode.FunctionDeclaresCommandTextAndStoreFunctionName, EdmSchemaErrorSeverity.Error, (object) Strings.CommandTextFunctionsCannotDeclareStoreFunctionName);
        }
      }
      if (this.Schema.DataModel == SchemaDataModelOption.ProviderDataModel && this._type != null && (!(this._type is ScalarType) || this._returnTypeCollectionKind != CollectionKind.None))
        this.AddError(ErrorCode.FunctionWithNonPrimitiveTypeNotSupported, EdmSchemaErrorSeverity.Error, (SchemaElement) this, (object) Strings.FunctionWithNonPrimitiveTypeNotSupported((object) Function.GetTypeNameForErrorMessage(this._type, this._returnTypeCollectionKind, this._isRefType), (object) this.FQName));
      if (this._returnTypeList != null)
      {
        foreach (SchemaElement returnType in this._returnTypeList)
          returnType.Validate();
      }
      if (this._parameters != null)
      {
        foreach (SchemaElement parameter in this._parameters)
          parameter.Validate();
      }
      if (this._commandText == null)
        return;
      this._commandText.Validate();
    }

    internal override void ResolveSecondLevelNames()
    {
      foreach (SchemaElement parameter in this._parameters)
        parameter.ResolveSecondLevelNames();
    }

    internal override SchemaElement Clone(SchemaElement parentElement)
    {
      throw Error.NotImplemented();
    }

    protected void CloneSetFunctionFields(Function clone)
    {
      clone._isAggregate = this._isAggregate;
      clone._isBuiltIn = this._isBuiltIn;
      clone._isNiladicFunction = this._isNiladicFunction;
      clone._isComposable = this._isComposable;
      clone._commandText = this._commandText;
      clone._storeFunctionName = this._storeFunctionName;
      clone._type = this._type;
      clone._returnTypeList = this._returnTypeList;
      clone._returnTypeCollectionKind = this._returnTypeCollectionKind;
      clone._parameterTypeSemantics = this._parameterTypeSemantics;
      clone._schema = this._schema;
      clone.Name = this.Name;
      foreach (Parameter parameter in this.Parameters)
      {
        int num = (int) clone.Parameters.TryAdd((Parameter) parameter.Clone((SchemaElement) clone));
      }
    }

    internal string UnresolvedReturnType
    {
      get
      {
        return this._unresolvedType;
      }
      set
      {
        this._unresolvedType = value;
      }
    }

    private void HandleDbSchemaAttribute(XmlReader reader)
    {
      this._schema = reader.Value;
    }

    private void HandleAggregateAttribute(XmlReader reader)
    {
      bool field = false;
      this.HandleBoolAttribute(reader, ref field);
      this.IsAggregate = field;
    }

    private void HandleBuiltInAttribute(XmlReader reader)
    {
      bool field = false;
      this.HandleBoolAttribute(reader, ref field);
      this.IsBuiltIn = field;
    }

    private void HandleStoreFunctionNameAttribute(XmlReader reader)
    {
      string str = reader.Value;
      if (string.IsNullOrEmpty(str))
        return;
      this.StoreFunctionName = str.Trim();
    }

    private void HandleNiladicFunctionAttribute(XmlReader reader)
    {
      bool field = false;
      this.HandleBoolAttribute(reader, ref field);
      this.IsNiladicFunction = field;
    }

    private void HandleIsComposableAttribute(XmlReader reader)
    {
      bool field = true;
      this.HandleBoolAttribute(reader, ref field);
      this.IsComposable = field;
    }

    private void HandleCommandTextFunctionElment(XmlReader reader)
    {
      FunctionCommandText functionCommandText = new FunctionCommandText(this);
      functionCommandText.Parse(reader);
      this._commandText = functionCommandText;
    }

    protected virtual void HandleReturnTypeAttribute(XmlReader reader)
    {
      string type;
      if (!Utils.GetString(this.Schema, reader, out type))
        return;
      TypeModifier typeModifier;
      Function.RemoveTypeModifier(ref type, out typeModifier, out this._isRefType);
      switch (typeModifier)
      {
        case TypeModifier.Array:
          this.CollectionKind = CollectionKind.Bag;
          break;
      }
      if (!Utils.ValidateDottedName(this.Schema, reader, type))
        return;
      this.UnresolvedReturnType = type;
    }

    protected void HandleParameterElement(XmlReader reader)
    {
      Parameter type = new Parameter(this);
      type.Parse(reader);
      this.Parameters.Add(type, true, new Func<object, string>(Strings.ParameterNameAlreadyDefinedDuplicate));
    }

    protected void HandleReturnTypeElement(XmlReader reader)
    {
      ReturnType returnType = new ReturnType(this);
      returnType.Parse(reader);
      if (this._returnTypeList == null)
        this._returnTypeList = new List<ReturnType>();
      this._returnTypeList.Add(returnType);
    }

    private void HandleParameterTypeSemanticsAttribute(XmlReader reader)
    {
      string str1 = reader.Value;
      if (string.IsNullOrEmpty(str1))
        return;
      string str2 = str1.Trim();
      if (string.IsNullOrEmpty(str2))
        return;
      switch (str2)
      {
        case "ExactMatchOnly":
          this.ParameterTypeSemantics = ParameterTypeSemantics.ExactMatchOnly;
          break;
        case "AllowImplicitPromotion":
          this.ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitPromotion;
          break;
        case "AllowImplicitConversion":
          this.ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion;
          break;
        default:
          this.AddError(ErrorCode.InvalidValueForParameterTypeSemantics, EdmSchemaErrorSeverity.Error, reader, (object) Strings.InvalidValueForParameterTypeSemanticsAttribute((object) str2));
          break;
      }
    }
  }
}
