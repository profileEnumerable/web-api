// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmFunction
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class for representing a function</summary>
  public class EdmFunction : EdmType
  {
    private readonly EdmFunction.FunctionAttributes _functionAttributes = EdmFunction.FunctionAttributes.IsComposable;
    private readonly ReadOnlyMetadataCollection<FunctionParameter> _returnParameters;
    private readonly ReadOnlyMetadataCollection<FunctionParameter> _parameters;
    private string _storeFunctionNameAttribute;
    private readonly ParameterTypeSemantics _parameterTypeSemantics;
    private readonly string _commandTextAttribute;
    private string _schemaName;
    private readonly ReadOnlyCollection<EntitySet> _entitySets;

    internal EdmFunction(string name, string namespaceName, DataSpace dataSpace)
      : this(name, namespaceName, dataSpace, new EdmFunctionPayload())
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal EdmFunction(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      EdmFunctionPayload payload)
      : base(name, namespaceName, dataSpace)
    {
      this._schemaName = payload.Schema;
      IList<FunctionParameter> source = payload.ReturnParameters ?? (IList<FunctionParameter>) new FunctionParameter[0];
      foreach (FunctionParameter functionParameter in (IEnumerable<FunctionParameter>) source)
      {
        if (functionParameter == null)
          throw new ArgumentException(Strings.ADP_CollectionParameterElementIsNull((object) nameof (ReturnParameters)));
        if (functionParameter.Mode != ParameterMode.ReturnValue)
          throw new ArgumentException(Strings.NonReturnParameterInReturnParameterCollection);
      }
      this._returnParameters = new ReadOnlyMetadataCollection<FunctionParameter>(source.Select<FunctionParameter, FunctionParameter>((Func<FunctionParameter, FunctionParameter>) (returnParameter => SafeLink<EdmFunction>.BindChild<FunctionParameter>(this, FunctionParameter.DeclaringFunctionLinker, returnParameter))).ToList<FunctionParameter>());
      if (payload.IsAggregate.HasValue)
        EdmFunction.SetFunctionAttribute(ref this._functionAttributes, EdmFunction.FunctionAttributes.Aggregate, payload.IsAggregate.Value);
      if (payload.IsBuiltIn.HasValue)
        EdmFunction.SetFunctionAttribute(ref this._functionAttributes, EdmFunction.FunctionAttributes.BuiltIn, payload.IsBuiltIn.Value);
      if (payload.IsNiladic.HasValue)
        EdmFunction.SetFunctionAttribute(ref this._functionAttributes, EdmFunction.FunctionAttributes.NiladicFunction, payload.IsNiladic.Value);
      if (payload.IsComposable.HasValue)
        EdmFunction.SetFunctionAttribute(ref this._functionAttributes, EdmFunction.FunctionAttributes.IsComposable, payload.IsComposable.Value);
      if (payload.IsFromProviderManifest.HasValue)
        EdmFunction.SetFunctionAttribute(ref this._functionAttributes, EdmFunction.FunctionAttributes.IsFromProviderManifest, payload.IsFromProviderManifest.Value);
      if (payload.IsCachedStoreFunction.HasValue)
        EdmFunction.SetFunctionAttribute(ref this._functionAttributes, EdmFunction.FunctionAttributes.IsCachedStoreFunction, payload.IsCachedStoreFunction.Value);
      if (payload.IsFunctionImport.HasValue)
        EdmFunction.SetFunctionAttribute(ref this._functionAttributes, EdmFunction.FunctionAttributes.IsFunctionImport, payload.IsFunctionImport.Value);
      if (payload.ParameterTypeSemantics.HasValue)
        this._parameterTypeSemantics = payload.ParameterTypeSemantics.Value;
      if (payload.StoreFunctionName != null)
        this._storeFunctionNameAttribute = payload.StoreFunctionName;
      if (payload.EntitySets != null)
      {
        if (payload.EntitySets.Count != source.Count)
          throw new ArgumentException(Strings.NumberOfEntitySetsDoesNotMatchNumberOfReturnParameters);
        this._entitySets = new ReadOnlyCollection<EntitySet>(payload.EntitySets);
      }
      else
      {
        if (this._returnParameters.Count > 1)
          throw new ArgumentException(Strings.NullEntitySetsForFunctionReturningMultipleResultSets);
        this._entitySets = new ReadOnlyCollection<EntitySet>((IList<EntitySet>) this._returnParameters.Select<FunctionParameter, EntitySet>((Func<FunctionParameter, EntitySet>) (p => (EntitySet) null)).ToList<EntitySet>());
      }
      if (payload.CommandText != null)
        this._commandTextAttribute = payload.CommandText;
      if (payload.Parameters != null)
      {
        foreach (FunctionParameter parameter in (IEnumerable<FunctionParameter>) payload.Parameters)
        {
          if (parameter == null)
            throw new ArgumentException(Strings.ADP_CollectionParameterElementIsNull((object) "parameters"));
          if (parameter.Mode == ParameterMode.ReturnValue)
            throw new ArgumentException(Strings.ReturnParameterInInputParameterCollection);
        }
        this._parameters = (ReadOnlyMetadataCollection<FunctionParameter>) new SafeLinkCollection<EdmFunction, FunctionParameter>(this, FunctionParameter.DeclaringFunctionLinker, new MetadataCollection<FunctionParameter>((IEnumerable<FunctionParameter>) payload.Parameters));
      }
      else
        this._parameters = new ReadOnlyMetadataCollection<FunctionParameter>(new MetadataCollection<FunctionParameter>());
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />.
    /// </summary>
    /// <returns>
    /// One of the enumeration values of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> enumeration.
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.EdmFunction;
      }
    }

    /// <summary>Returns the full name (namespace plus name) of this type. </summary>
    /// <returns>The full name of the type.</returns>
    public override string FullName
    {
      get
      {
        return this.NamespaceName + "." + this.Name;
      }
    }

    /// <summary>
    /// Gets the parameters of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the parameters of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />
    /// .
    /// </returns>
    public ReadOnlyMetadataCollection<FunctionParameter> Parameters
    {
      get
      {
        return this._parameters;
      }
    }

    /// <summary>Adds a parameter to this function.</summary>
    /// <param name="functionParameter">The parameter to be added.</param>
    public void AddParameter(FunctionParameter functionParameter)
    {
      Check.NotNull<FunctionParameter>(functionParameter, nameof (functionParameter));
      Util.ThrowIfReadOnly((MetadataItem) this);
      if (functionParameter.Mode == ParameterMode.ReturnValue)
        throw new ArgumentException(Strings.ReturnParameterInInputParameterCollection);
      this._parameters.Source.Add(functionParameter);
    }

    internal bool HasUserDefinedBody
    {
      get
      {
        if (this.IsModelDefinedFunction)
          return !string.IsNullOrEmpty(this.CommandTextAttribute);
        return false;
      }
    }

    [MetadataProperty(BuiltInTypeKind.EntitySet, false)]
    internal EntitySet EntitySet
    {
      get
      {
        if (this._entitySets.Count == 0)
          return (EntitySet) null;
        return this._entitySets[0];
      }
    }

    [MetadataProperty(BuiltInTypeKind.EntitySet, true)]
    internal ReadOnlyCollection<EntitySet> EntitySets
    {
      get
      {
        return this._entitySets;
      }
    }

    /// <summary>
    /// Gets the return parameter of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" /> object that represents the return parameter of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.FunctionParameter, false)]
    public FunctionParameter ReturnParameter
    {
      get
      {
        return this._returnParameters.FirstOrDefault<FunctionParameter>();
      }
    }

    /// <summary>
    /// Gets the return parameters of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that represents the return parameters of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.FunctionParameter, true)]
    public ReadOnlyMetadataCollection<FunctionParameter> ReturnParameters
    {
      get
      {
        return this._returnParameters;
      }
    }

    /// <summary>Gets the store function name attribute of this function.</summary>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string StoreFunctionNameAttribute
    {
      get
      {
        return this._storeFunctionNameAttribute;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._storeFunctionNameAttribute = value;
      }
    }

    internal string FunctionName
    {
      get
      {
        return this.StoreFunctionNameAttribute ?? this.Name;
      }
    }

    /// <summary>Gets the parameter type semantics attribute of this function.</summary>
    [MetadataProperty(typeof (ParameterTypeSemantics), false)]
    public ParameterTypeSemantics ParameterTypeSemanticsAttribute
    {
      get
      {
        return this._parameterTypeSemantics;
      }
    }

    /// <summary>Gets the aggregate attribute of this function.</summary>
    [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
    public bool AggregateAttribute
    {
      get
      {
        return this.GetFunctionAttribute(EdmFunction.FunctionAttributes.Aggregate);
      }
    }

    /// <summary>
    /// Gets a value indicating whether built in attribute is present on this function.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the attribute is present; otherwise, <c>false</c>.
    /// </value>
    [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
    public virtual bool BuiltInAttribute
    {
      get
      {
        return this.GetFunctionAttribute(EdmFunction.FunctionAttributes.BuiltIn);
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is from the provider manifest.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is from the provider manifest; otherwise, <c>false</c>.
    /// </value>
    [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
    public bool IsFromProviderManifest
    {
      get
      {
        return this.GetFunctionAttribute(EdmFunction.FunctionAttributes.IsFromProviderManifest);
      }
    }

    /// <summary>
    /// Gets a value indicating whether the is a niladic function (a function that accepts no arguments).
    /// </summary>
    /// <value>
    /// <c>true</c> if the function is niladic; otherwise, <c>false</c>.
    /// </value>
    [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
    public bool NiladicFunctionAttribute
    {
      get
      {
        return this.GetFunctionAttribute(EdmFunction.FunctionAttributes.NiladicFunction);
      }
    }

    /// <summary>Gets whether this instance is mapped to a function or to a stored procedure.</summary>
    /// <returns>true if this instance is mapped to a function; false if this instance is mapped to a stored procedure.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Composable")]
    [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
    public bool IsComposableAttribute
    {
      get
      {
        return this.GetFunctionAttribute(EdmFunction.FunctionAttributes.IsComposable);
      }
    }

    /// <summary>Gets a query in the language that is used by the database management system or storage model. </summary>
    /// <returns>
    /// A string value in the syntax used by the database management system or storage model that contains the query or update statement of the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />
    /// .
    /// </returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string CommandTextAttribute
    {
      get
      {
        return this._commandTextAttribute;
      }
    }

    internal bool IsCachedStoreFunction
    {
      get
      {
        return this.GetFunctionAttribute(EdmFunction.FunctionAttributes.IsCachedStoreFunction);
      }
    }

    internal bool IsModelDefinedFunction
    {
      get
      {
        if (this.DataSpace == DataSpace.CSpace && !this.IsCachedStoreFunction && !this.IsFromProviderManifest)
          return !this.IsFunctionImport;
        return false;
      }
    }

    internal bool IsFunctionImport
    {
      get
      {
        return this.GetFunctionAttribute(EdmFunction.FunctionAttributes.IsFunctionImport);
      }
    }

    /// <summary>Gets or sets the schema associated with the function.</summary>
    /// <returns>The schema associated with the function.</returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string Schema
    {
      get
      {
        return this._schemaName;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._schemaName = value;
      }
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
      this.Parameters.Source.SetReadOnly();
      foreach (MetadataItem returnParameter in this.ReturnParameters)
        returnParameter.SetReadOnly();
    }

    internal override void BuildIdentity(StringBuilder builder)
    {
      if (this.CacheIdentity != null)
        builder.Append(this.CacheIdentity);
      else
        EdmFunction.BuildIdentity<FunctionParameter>(builder, this.FullName, (IEnumerable<FunctionParameter>) this.Parameters, (Func<FunctionParameter, TypeUsage>) (param => param.TypeUsage), (Func<FunctionParameter, ParameterMode>) (param => param.Mode));
    }

    internal static string BuildIdentity(
      string functionName,
      IEnumerable<TypeUsage> functionParameters)
    {
      StringBuilder builder = new StringBuilder();
      EdmFunction.BuildIdentity<TypeUsage>(builder, functionName, functionParameters, (Func<TypeUsage, TypeUsage>) (param => param), (Func<TypeUsage, ParameterMode>) (param => ParameterMode.In));
      return builder.ToString();
    }

    internal static void BuildIdentity<TParameterMetadata>(
      StringBuilder builder,
      string functionName,
      IEnumerable<TParameterMetadata> functionParameters,
      Func<TParameterMetadata, TypeUsage> getParameterTypeUsage,
      Func<TParameterMetadata, ParameterMode> getParameterMode)
    {
      builder.Append(functionName);
      builder.Append('(');
      bool flag = true;
      foreach (TParameterMetadata functionParameter in functionParameters)
      {
        if (flag)
          flag = false;
        else
          builder.Append(",");
        builder.Append(Helper.ToString(getParameterMode(functionParameter)));
        builder.Append(' ');
        getParameterTypeUsage(functionParameter).BuildIdentity(builder);
      }
      builder.Append(')');
    }

    private bool GetFunctionAttribute(EdmFunction.FunctionAttributes attribute)
    {
      return attribute == (attribute & this._functionAttributes);
    }

    private static void SetFunctionAttribute(
      ref EdmFunction.FunctionAttributes field,
      EdmFunction.FunctionAttributes attribute,
      bool isSet)
    {
      if (isSet)
        field |= attribute;
      else
        field ^= field & attribute;
    }

    /// <summary>
    /// The factory method for constructing the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> object.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="namespaceName">The namespace of the function.</param>
    /// <param name="dataSpace">The namespace the function belongs to.</param>
    /// <param name="payload">Additional function attributes and properties.</param>
    /// <param name="metadataProperties">Metadata properties that will be added to the function. Can be null.</param>
    /// <returns>
    /// A new, read-only instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> type.
    /// </returns>
    public static EdmFunction Create(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      EdmFunctionPayload payload,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      EdmFunction edmFunction = new EdmFunction(name, namespaceName, dataSpace, payload);
      if (metadataProperties != null)
        edmFunction.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      edmFunction.SetReadOnly();
      return edmFunction;
    }

    [Flags]
    private enum FunctionAttributes : byte
    {
      Aggregate = 1,
      BuiltIn = 2,
      NiladicFunction = 4,
      IsComposable = 8,
      IsFromProviderManifest = 16, // 0x10
      IsCachedStoreFunction = 32, // 0x20
      IsFunctionImport = 64, // 0x40
      Default = IsComposable, // 0x08
    }
  }
}
