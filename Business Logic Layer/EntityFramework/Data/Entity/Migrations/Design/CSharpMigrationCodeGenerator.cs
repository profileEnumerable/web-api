// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Design.CSharpMigrationCodeGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace System.Data.Entity.Migrations.Design
{
  /// <summary>Generates C# code for a code-based migration.</summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public class CSharpMigrationCodeGenerator : MigrationCodeGenerator
  {
    private IEnumerable<Tuple<CreateTableOperation, AddForeignKeyOperation>> _newTableForeignKeys;
    private IEnumerable<Tuple<CreateTableOperation, CreateIndexOperation>> _newTableIndexes;

    /// <inheritdoc />
    public override ScaffoldedMigration Generate(
      string migrationId,
      IEnumerable<MigrationOperation> operations,
      string sourceModel,
      string targetModel,
      string @namespace,
      string className)
    {
      Check.NotEmpty(migrationId, nameof (migrationId));
      Check.NotNull<IEnumerable<MigrationOperation>>(operations, nameof (operations));
      Check.NotEmpty(targetModel, nameof (targetModel));
      Check.NotEmpty(className, nameof (className));
      className = this.ScrubName(className);
      this._newTableForeignKeys = (IEnumerable<Tuple<CreateTableOperation, AddForeignKeyOperation>>) operations.OfType<CreateTableOperation>().SelectMany((Func<CreateTableOperation, IEnumerable<AddForeignKeyOperation>>) (ct => operations.OfType<AddForeignKeyOperation>()), (ct, cfk) => new
      {
        ct = ct,
        cfk = cfk
      }).Where(_param0 => _param0.ct.Name.EqualsIgnoreCase(_param0.cfk.DependentTable)).Select(_param0 => Tuple.Create<CreateTableOperation, AddForeignKeyOperation>(_param0.ct, _param0.cfk)).ToList<Tuple<CreateTableOperation, AddForeignKeyOperation>>();
      this._newTableIndexes = (IEnumerable<Tuple<CreateTableOperation, CreateIndexOperation>>) operations.OfType<CreateTableOperation>().SelectMany((Func<CreateTableOperation, IEnumerable<CreateIndexOperation>>) (ct => operations.OfType<CreateIndexOperation>()), (ct, ci) => new
      {
        ct = ct,
        ci = ci
      }).Where(_param0 => _param0.ct.Name.EqualsIgnoreCase(_param0.ci.Table)).Select(_param0 => Tuple.Create<CreateTableOperation, CreateIndexOperation>(_param0.ct, _param0.ci)).ToList<Tuple<CreateTableOperation, CreateIndexOperation>>();
      ScaffoldedMigration scaffoldedMigration = new ScaffoldedMigration()
      {
        MigrationId = migrationId,
        Language = "cs",
        UserCode = this.Generate(operations, @namespace, className),
        DesignerCode = this.Generate(migrationId, sourceModel, targetModel, @namespace, className)
      };
      if (!string.IsNullOrWhiteSpace(sourceModel))
        scaffoldedMigration.Resources.Add("Source", (object) sourceModel);
      scaffoldedMigration.Resources.Add("Target", (object) targetModel);
      return scaffoldedMigration;
    }

    /// <summary>
    /// Generates the primary code file that the user can view and edit.
    /// </summary>
    /// <param name="operations"> Operations to be performed by the migration. </param>
    /// <param name="namespace"> Namespace that code should be generated in. </param>
    /// <param name="className"> Name of the class that should be generated. </param>
    /// <returns> The generated code. </returns>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "namespace")]
    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    protected virtual string Generate(
      IEnumerable<MigrationOperation> operations,
      string @namespace,
      string className)
    {
      Check.NotNull<IEnumerable<MigrationOperation>>(operations, nameof (operations));
      Check.NotEmpty(className, nameof (className));
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) stringWriter))
        {
          this.WriteClassStart(@namespace, className, writer, "DbMigration", false, this.GetNamespaces(operations));
          writer.WriteLine("public override void Up()");
          writer.WriteLine("{");
          ++writer.Indent;
          ((IEnumerable<object>) operations.Except<MigrationOperation>((IEnumerable<MigrationOperation>) this._newTableForeignKeys.Select<Tuple<CreateTableOperation, AddForeignKeyOperation>, AddForeignKeyOperation>((Func<Tuple<CreateTableOperation, AddForeignKeyOperation>, AddForeignKeyOperation>) (t => t.Item2))).Except<MigrationOperation>((IEnumerable<MigrationOperation>) this._newTableIndexes.Select<Tuple<CreateTableOperation, CreateIndexOperation>, CreateIndexOperation>((Func<Tuple<CreateTableOperation, CreateIndexOperation>, CreateIndexOperation>) (t => t.Item2)))).Each<object>((Action<object>) (o =>
          {
            // ISSUE: reference to a compiler-generated field
            if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site23 == null)
            {
              // ISSUE: reference to a compiler-generated field
              CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site23 = CallSite<Action<CallSite, CSharpMigrationCodeGenerator, object, IndentedTextWriter>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, nameof (Generate), (IEnumerable<Type>) null, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site23.Target((CallSite) CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site23, this, o, writer);
          }));
          --writer.Indent;
          writer.WriteLine("}");
          writer.WriteLine();
          writer.WriteLine("public override void Down()");
          writer.WriteLine("{");
          ++writer.Indent;
          operations = operations.Select<MigrationOperation, MigrationOperation>((Func<MigrationOperation, MigrationOperation>) (o => o.Inverse)).Where<MigrationOperation>((Func<MigrationOperation, bool>) (o => o != null)).Reverse<MigrationOperation>();
          bool flag = operations.Any<MigrationOperation>((Func<MigrationOperation, bool>) (o => o is NotSupportedOperation));
          ((IEnumerable<object>) operations.Where<MigrationOperation>((Func<MigrationOperation, bool>) (o => !(o is NotSupportedOperation)))).Each<object>((Action<object>) (o =>
          {
            // ISSUE: reference to a compiler-generated field
            if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site24 == null)
            {
              // ISSUE: reference to a compiler-generated field
              CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site24 = CallSite<Action<CallSite, CSharpMigrationCodeGenerator, object, IndentedTextWriter>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, nameof (Generate), (IEnumerable<Type>) null, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site24.Target((CallSite) CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer22.\u003C\u003Ep__Site24, this, o, writer);
          }));
          if (flag)
          {
            writer.Write("throw new NotSupportedException(");
            writer.Write(this.Generate(Strings.ScaffoldSprocInDownNotSupported));
            writer.WriteLine(");");
          }
          --writer.Indent;
          writer.WriteLine("}");
          this.WriteClassEnd(@namespace, writer);
        }
        return stringWriter.ToString();
      }
    }

    /// <summary>
    /// Generates the code behind file with migration metadata.
    /// </summary>
    /// <param name="migrationId"> Unique identifier of the migration. </param>
    /// <param name="sourceModel"> Source model to be stored in the migration metadata. </param>
    /// <param name="targetModel"> Target model to be stored in the migration metadata. </param>
    /// <param name="namespace"> Namespace that code should be generated in. </param>
    /// <param name="className"> Name of the class that should be generated. </param>
    /// <returns> The generated code. </returns>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "namespace")]
    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    protected virtual string Generate(
      string migrationId,
      string sourceModel,
      string targetModel,
      string @namespace,
      string className)
    {
      Check.NotEmpty(migrationId, nameof (migrationId));
      Check.NotEmpty(targetModel, nameof (targetModel));
      Check.NotEmpty(className, nameof (className));
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) stringWriter))
        {
          writer.WriteLine("// <auto-generated />");
          this.WriteClassStart(@namespace, className, writer, "IMigrationMetadata", true, (IEnumerable<string>) null);
          writer.Write("private readonly ResourceManager Resources = new ResourceManager(typeof(");
          writer.Write(className);
          writer.WriteLine("));");
          writer.WriteLine();
          this.WriteProperty("Id", this.Quote(migrationId), writer);
          writer.WriteLine();
          this.WriteProperty("Source", sourceModel == null ? (string) null : "Resources.GetString(\"Source\")", writer);
          writer.WriteLine();
          this.WriteProperty("Target", "Resources.GetString(\"Target\")", writer);
          this.WriteClassEnd(@namespace, writer);
        }
        return stringWriter.ToString();
      }
    }

    /// <summary>
    /// Generates a property to return the source or target model in the code behind file.
    /// </summary>
    /// <param name="name"> Name of the property. </param>
    /// <param name="value"> Value to be returned. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void WriteProperty(string name, string value, IndentedTextWriter writer)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("string IMigrationMetadata.");
      writer.WriteLine(name);
      writer.WriteLine("{");
      ++writer.Indent;
      writer.Write("get { return ");
      writer.Write(value ?? "null");
      writer.WriteLine("; }");
      --writer.Indent;
      writer.WriteLine("}");
    }

    /// <summary>Generates class attributes.</summary>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    /// <param name="designer"> A value indicating if this class is being generated for a code-behind file. </param>
    protected virtual void WriteClassAttributes(IndentedTextWriter writer, bool designer)
    {
      if (!designer)
        return;
      writer.WriteLine("[GeneratedCode(\"EntityFramework.Migrations\", \"{0}\")]", (object) typeof (CSharpMigrationCodeGenerator).Assembly().GetInformationalVersion());
    }

    /// <summary>
    /// Generates a namespace, using statements and class definition.
    /// </summary>
    /// <param name="namespace"> Namespace that code should be generated in. </param>
    /// <param name="className"> Name of the class that should be generated. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    /// <param name="base"> Base class for the generated class. </param>
    /// <param name="designer"> A value indicating if this class is being generated for a code-behind file. </param>
    /// <param name="namespaces"> Namespaces for which using directives will be added. If null, then the namespaces returned from GetDefaultNamespaces will be used. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "base")]
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "namespace")]
    protected virtual void WriteClassStart(
      string @namespace,
      string className,
      IndentedTextWriter writer,
      string @base,
      bool designer = false,
      IEnumerable<string> namespaces = null)
    {
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      Check.NotEmpty(className, nameof (className));
      Check.NotEmpty(@base, nameof (@base));
      if (!string.IsNullOrWhiteSpace(@namespace))
      {
        writer.Write("namespace ");
        writer.WriteLine(@namespace);
        writer.WriteLine("{");
        ++writer.Indent;
      }
      (namespaces ?? this.GetDefaultNamespaces(designer)).Each<string>((Action<string>) (n => writer.WriteLine("using " + n + ";")));
      writer.WriteLine();
      this.WriteClassAttributes(writer, designer);
      writer.Write("public ");
      if (designer)
        writer.Write("sealed ");
      writer.Write("partial class ");
      writer.Write(className);
      writer.Write(" : ");
      writer.Write(@base);
      writer.WriteLine();
      writer.WriteLine("{");
      ++writer.Indent;
    }

    /// <summary>
    /// Generates the closing code for a class that was started with WriteClassStart.
    /// </summary>
    /// <param name="namespace"> Namespace that code should be generated in. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "namespace")]
    protected virtual void WriteClassEnd(string @namespace, IndentedTextWriter writer)
    {
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      --writer.Indent;
      writer.WriteLine("}");
      if (string.IsNullOrWhiteSpace(@namespace))
        return;
      --writer.Indent;
      writer.WriteLine("}");
    }

    /// <summary>
    /// Generates code to perform an <see cref="T:System.Data.Entity.Migrations.Model.AddColumnOperation" />.
    /// </summary>
    /// <param name="addColumnOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      AddColumnOperation addColumnOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<AddColumnOperation>(addColumnOperation, nameof (addColumnOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("AddColumn(");
      writer.Write(this.Quote(addColumnOperation.Table));
      writer.Write(", ");
      writer.Write(this.Quote(addColumnOperation.Column.Name));
      writer.Write(", c =>");
      this.Generate(addColumnOperation.Column, writer, false);
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.DropColumnOperation" />.
    /// </summary>
    /// <param name="dropColumnOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      DropColumnOperation dropColumnOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<DropColumnOperation>(dropColumnOperation, nameof (dropColumnOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("DropColumn(");
      writer.Write(this.Quote(dropColumnOperation.Table));
      writer.Write(", ");
      writer.Write(this.Quote(dropColumnOperation.Name));
      if (dropColumnOperation.RemovedAnnotations.Any<KeyValuePair<string, object>>())
      {
        ++writer.Indent;
        writer.WriteLine(",");
        writer.Write("removedAnnotations: ");
        this.GenerateAnnotations(dropColumnOperation.RemovedAnnotations, writer);
        --writer.Indent;
      }
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform an <see cref="T:System.Data.Entity.Migrations.Model.AlterColumnOperation" />.
    /// </summary>
    /// <param name="alterColumnOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      AlterColumnOperation alterColumnOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<AlterColumnOperation>(alterColumnOperation, nameof (alterColumnOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("AlterColumn(");
      writer.Write(this.Quote(alterColumnOperation.Table));
      writer.Write(", ");
      writer.Write(this.Quote(alterColumnOperation.Column.Name));
      writer.Write(", c =>");
      this.Generate(alterColumnOperation.Column, writer, false);
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code for to re-create the given dictionary of annotations for use when passing
    /// these annotations as a parameter of a <see cref="T:System.Data.Entity.Migrations.DbMigration" />. call.
    /// </summary>
    /// <param name="annotations">The annotations to generate.</param>
    /// <param name="writer">The writer to which generated code should be written.</param>
    protected internal virtual void GenerateAnnotations(
      IDictionary<string, object> annotations,
      IndentedTextWriter writer)
    {
      Check.NotNull<IDictionary<string, object>>(annotations, nameof (annotations));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine("new Dictionary<string, object>");
      writer.WriteLine("{");
      ++writer.Indent;
      foreach (string index in (IEnumerable<string>) annotations.Keys.OrderBy<string, string>((Func<string, string>) (k => k)))
      {
        writer.Write("{ ");
        writer.Write(this.Quote(index) + ", ");
        this.GenerateAnnotation(index, annotations[index], writer);
        writer.WriteLine(" },");
      }
      --writer.Indent;
      writer.Write("}");
    }

    /// <summary>
    /// Generates code for to re-create the given dictionary of annotations for use when passing
    /// these annotations as a parameter of a <see cref="T:System.Data.Entity.Migrations.DbMigration" />. call.
    /// </summary>
    /// <param name="annotations">The annotations to generate.</param>
    /// <param name="writer">The writer to which generated code should be written.</param>
    protected internal virtual void GenerateAnnotations(
      IDictionary<string, AnnotationValues> annotations,
      IndentedTextWriter writer)
    {
      Check.NotNull<IDictionary<string, AnnotationValues>>(annotations, nameof (annotations));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine("new Dictionary<string, AnnotationValues>");
      writer.WriteLine("{");
      ++writer.Indent;
      if (annotations != null)
      {
        foreach (string index in (IEnumerable<string>) annotations.Keys.OrderBy<string, string>((Func<string, string>) (k => k)))
        {
          writer.WriteLine("{ ");
          ++writer.Indent;
          writer.WriteLine(this.Quote(index) + ",");
          writer.Write("new AnnotationValues(oldValue: ");
          this.GenerateAnnotation(index, annotations[index].OldValue, writer);
          writer.Write(", newValue: ");
          this.GenerateAnnotation(index, annotations[index].NewValue, writer);
          writer.WriteLine(")");
          --writer.Indent;
          writer.WriteLine("},");
        }
      }
      --writer.Indent;
      writer.Write("}");
    }

    /// <summary>
    /// Generates code for the given annotation value, which may be null. The default behavior is to use an
    /// <see cref="T:System.Data.Entity.Infrastructure.Annotations.AnnotationCodeGenerator" /> if one is registered, otherwise call ToString on the annotation value.
    /// </summary>
    /// <remarks>
    /// Note that a <see cref="T:System.Data.Entity.Infrastructure.Annotations.AnnotationCodeGenerator" /> can be registered to generate code for custom annotations
    /// without the need to override the entire code generator.
    /// </remarks>
    /// <param name="name">The name of the annotation for which code is needed.</param>
    /// <param name="annotation">The annotation value to generate.</param>
    /// <param name="writer">The writer to which generated code should be written.</param>
    protected internal virtual void GenerateAnnotation(
      string name,
      object annotation,
      IndentedTextWriter writer)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      if (annotation == null)
      {
        writer.Write("null");
      }
      else
      {
        Func<AnnotationCodeGenerator> func;
        if (this.AnnotationGenerators.TryGetValue(name, out func) && func != null)
          func().Generate(name, annotation, writer);
        else
          writer.Write(this.Quote(annotation.ToString()));
      }
    }

    /// <summary>Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.CreateProcedureOperation" />.</summary>
    /// <param name="createProcedureOperation">The operation to generate code for.</param>
    /// <param name="writer">Text writer to add the generated code to.</param>
    protected virtual void Generate(
      CreateProcedureOperation createProcedureOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<CreateProcedureOperation>(createProcedureOperation, nameof (createProcedureOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      this.Generate((ProcedureOperation) createProcedureOperation, "CreateStoredProcedure", writer);
    }

    /// <summary>Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.AlterProcedureOperation" />.</summary>
    /// <param name="alterProcedureOperation">The operation to generate code for.</param>
    /// <param name="writer">Text writer to add the generated code to.</param>
    protected virtual void Generate(
      AlterProcedureOperation alterProcedureOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<AlterProcedureOperation>(alterProcedureOperation, nameof (alterProcedureOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      this.Generate((ProcedureOperation) alterProcedureOperation, "AlterStoredProcedure", writer);
    }

    private void Generate(
      ProcedureOperation procedureOperation,
      string methodName,
      IndentedTextWriter writer)
    {
      writer.Write(methodName);
      writer.WriteLine("(");
      ++writer.Indent;
      writer.Write(this.Quote(procedureOperation.Name));
      writer.WriteLine(",");
      if (procedureOperation.Parameters.Any<ParameterModel>())
      {
        writer.WriteLine("p => new");
        ++writer.Indent;
        writer.WriteLine("{");
        ++writer.Indent;
        procedureOperation.Parameters.Each<ParameterModel>((Action<ParameterModel>) (p =>
        {
          string b = this.ScrubName(p.Name);
          writer.Write(b);
          writer.Write(" =");
          this.Generate(p, writer, !string.Equals(p.Name, b, StringComparison.Ordinal));
          writer.WriteLine(",");
        }));
        --writer.Indent;
        writer.WriteLine("},");
        --writer.Indent;
      }
      writer.Write("body:");
      if (!string.IsNullOrWhiteSpace(procedureOperation.BodySql))
      {
        writer.WriteLine();
        ++writer.Indent;
        string newValue = writer.NewLine + writer.CurrentIndentation() + "  ";
        writer.Write("@");
        writer.WriteLine(this.Generate(procedureOperation.BodySql.Replace(Environment.NewLine, newValue)));
        --writer.Indent;
      }
      else
        writer.WriteLine(" \"\"");
      --writer.Indent;
      writer.WriteLine(");");
      writer.WriteLine();
    }

    /// <summary>Generates code to specify the definition for a <see cref="T:System.Data.Entity.Migrations.Model.ParameterModel" />.</summary>
    /// <param name="parameterModel">The parameter definition to generate code for.</param>
    /// <param name="writer">Text writer to add the generated code to.</param>
    /// <param name="emitName">A value indicating whether to include the column name in the definition.</param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    protected virtual void Generate(
      ParameterModel parameterModel,
      IndentedTextWriter writer,
      bool emitName = false)
    {
      Check.NotNull<ParameterModel>(parameterModel, nameof (parameterModel));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write(" p.");
      writer.Write(this.TranslateColumnType(parameterModel.Type));
      writer.Write("(");
      List<string> ts = new List<string>();
      if (emitName)
        ts.Add("name: " + this.Quote(parameterModel.Name));
      if (parameterModel.MaxLength.HasValue)
        ts.Add("maxLength: " + (object) parameterModel.MaxLength);
      byte? precision = parameterModel.Precision;
      if ((precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        ts.Add("precision: " + (object) parameterModel.Precision);
      byte? scale = parameterModel.Scale;
      if ((scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        ts.Add("scale: " + (object) parameterModel.Scale);
      if (parameterModel.IsFixedLength.HasValue)
        ts.Add("fixedLength: " + parameterModel.IsFixedLength.ToString().ToLowerInvariant());
      if (parameterModel.IsUnicode.HasValue)
        ts.Add("unicode: " + parameterModel.IsUnicode.ToString().ToLowerInvariant());
      if (parameterModel.DefaultValue != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site43 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site43 = CallSite<Action<CallSite, List<string>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, List<string>, object> target1 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site43.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, List<string>, object>> pSite43 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site43;
        List<string> stringList = ts;
        // ISSUE: reference to a compiler-generated field
        if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site44 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site44 = CallSite<Func<CallSite, string, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Add, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, string, object, object> target2 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site44.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, string, object, object>> pSite44 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site44;
        // ISSUE: reference to a compiler-generated field
        if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site45 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site45 = CallSite<Func<CallSite, CSharpMigrationCodeGenerator, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, nameof (Generate), (IEnumerable<Type>) null, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj1 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site45.Target((CallSite) CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer42.\u003C\u003Ep__Site45, this, parameterModel.DefaultValue);
        object obj2 = target2((CallSite) pSite44, "defaultValue: ", obj1);
        target1((CallSite) pSite43, stringList, obj2);
      }
      if (!string.IsNullOrWhiteSpace(parameterModel.DefaultValueSql))
        ts.Add("defaultValueSql: " + this.Quote(parameterModel.DefaultValueSql));
      if (!string.IsNullOrWhiteSpace(parameterModel.StoreType))
        ts.Add("storeType: " + this.Quote(parameterModel.StoreType));
      if (parameterModel.IsOutParameter)
        ts.Add("outParameter: true");
      writer.Write(ts.Join<string>((Func<string, string>) null, ", "));
      writer.Write(")");
    }

    /// <summary>Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.DropProcedureOperation" />.</summary>
    /// <param name="dropProcedureOperation">The operation to generate code for.</param>
    /// <param name="writer">Text writer to add the generated code to.</param>
    protected virtual void Generate(
      DropProcedureOperation dropProcedureOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<DropProcedureOperation>(dropProcedureOperation, nameof (dropProcedureOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("DropStoredProcedure(");
      writer.Write(this.Quote(dropProcedureOperation.Name));
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.CreateTableOperation" />.
    /// </summary>
    /// <param name="createTableOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      CreateTableOperation createTableOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<CreateTableOperation>(createTableOperation, nameof (createTableOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine("CreateTable(");
      ++writer.Indent;
      writer.Write(this.Quote(createTableOperation.Name));
      writer.WriteLine(",");
      writer.WriteLine("c => new");
      ++writer.Indent;
      writer.WriteLine("{");
      ++writer.Indent;
      createTableOperation.Columns.Each<ColumnModel>((Action<ColumnModel>) (c =>
      {
        string b = this.ScrubName(c.Name);
        writer.Write(b);
        writer.Write(" =");
        this.Generate(c, writer, !string.Equals(c.Name, b, StringComparison.Ordinal));
        writer.WriteLine(",");
      }));
      --writer.Indent;
      writer.Write("}");
      --writer.Indent;
      if (createTableOperation.Annotations.Any<KeyValuePair<string, object>>())
      {
        writer.WriteLine(",");
        writer.Write("annotations: ");
        this.GenerateAnnotations(createTableOperation.Annotations, writer);
      }
      writer.Write(")");
      this.GenerateInline(createTableOperation.PrimaryKey, writer);
      this._newTableForeignKeys.Where<Tuple<CreateTableOperation, AddForeignKeyOperation>>((Func<Tuple<CreateTableOperation, AddForeignKeyOperation>, bool>) (t => t.Item1 == createTableOperation)).Each<Tuple<CreateTableOperation, AddForeignKeyOperation>>((Action<Tuple<CreateTableOperation, AddForeignKeyOperation>>) (t => this.GenerateInline(t.Item2, writer)));
      this._newTableIndexes.Where<Tuple<CreateTableOperation, CreateIndexOperation>>((Func<Tuple<CreateTableOperation, CreateIndexOperation>, bool>) (t => t.Item1 == createTableOperation)).Each<Tuple<CreateTableOperation, CreateIndexOperation>>((Action<Tuple<CreateTableOperation, CreateIndexOperation>>) (t => this.GenerateInline(t.Item2, writer)));
      writer.WriteLine(";");
      --writer.Indent;
      writer.WriteLine();
    }

    /// <summary>
    /// Generates code for an <see cref="T:System.Data.Entity.Migrations.Model.AlterTableOperation" />.
    /// </summary>
    /// <param name="alterTableOperation">The operation for which code should be generated.</param>
    /// <param name="writer">The writer to which generated code should be written.</param>
    protected internal virtual void Generate(
      AlterTableOperation alterTableOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<AlterTableOperation>(alterTableOperation, nameof (alterTableOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine("AlterTableAnnotations(");
      ++writer.Indent;
      writer.Write(this.Quote(alterTableOperation.Name));
      writer.WriteLine(",");
      writer.WriteLine("c => new");
      ++writer.Indent;
      writer.WriteLine("{");
      ++writer.Indent;
      alterTableOperation.Columns.Each<ColumnModel>((Action<ColumnModel>) (c =>
      {
        string b = this.ScrubName(c.Name);
        writer.Write(b);
        writer.Write(" =");
        this.Generate(c, writer, !string.Equals(c.Name, b, StringComparison.Ordinal));
        writer.WriteLine(",");
      }));
      --writer.Indent;
      writer.Write("}");
      --writer.Indent;
      if (alterTableOperation.Annotations.Any<KeyValuePair<string, AnnotationValues>>())
      {
        writer.WriteLine(",");
        writer.Write("annotations: ");
        this.GenerateAnnotations(alterTableOperation.Annotations, writer);
      }
      writer.Write(")");
      writer.WriteLine(";");
      --writer.Indent;
      writer.WriteLine();
    }

    /// <summary>
    /// Generates code to perform an <see cref="T:System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation" /> as part of a <see cref="T:System.Data.Entity.Migrations.Model.CreateTableOperation" />.
    /// </summary>
    /// <param name="addPrimaryKeyOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void GenerateInline(
      AddPrimaryKeyOperation addPrimaryKeyOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      if (addPrimaryKeyOperation == null)
        return;
      writer.WriteLine();
      writer.Write(".PrimaryKey(");
      this.Generate((IEnumerable<string>) addPrimaryKeyOperation.Columns, writer);
      if (!addPrimaryKeyOperation.HasDefaultName)
      {
        writer.Write(", name: ");
        writer.Write(this.Quote(addPrimaryKeyOperation.Name));
      }
      if (!addPrimaryKeyOperation.IsClustered)
        writer.Write(", clustered: false");
      writer.Write(")");
    }

    /// <summary>
    /// Generates code to perform an <see cref="T:System.Data.Entity.Migrations.Model.AddForeignKeyOperation" /> as part of a <see cref="T:System.Data.Entity.Migrations.Model.CreateTableOperation" />.
    /// </summary>
    /// <param name="addForeignKeyOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void GenerateInline(
      AddForeignKeyOperation addForeignKeyOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<AddForeignKeyOperation>(addForeignKeyOperation, nameof (addForeignKeyOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine();
      writer.Write(".ForeignKey(" + this.Quote(addForeignKeyOperation.PrincipalTable) + ", ");
      this.Generate((IEnumerable<string>) addForeignKeyOperation.DependentColumns, writer);
      if (addForeignKeyOperation.CascadeDelete)
        writer.Write(", cascadeDelete: true");
      writer.Write(")");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.CreateIndexOperation" /> as part of a <see cref="T:System.Data.Entity.Migrations.Model.CreateTableOperation" />.
    /// </summary>
    /// <param name="createIndexOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void GenerateInline(
      CreateIndexOperation createIndexOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<CreateIndexOperation>(createIndexOperation, nameof (createIndexOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine();
      writer.Write(".Index(");
      this.Generate((IEnumerable<string>) createIndexOperation.Columns, writer);
      this.WriteIndexParameters(createIndexOperation, writer);
      writer.Write(")");
    }

    /// <summary>
    /// Generates code to specify a set of column names using a lambda expression.
    /// </summary>
    /// <param name="columns"> The columns to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(IEnumerable<string> columns, IndentedTextWriter writer)
    {
      Check.NotNull<IEnumerable<string>>(columns, nameof (columns));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("t => ");
      if (columns.Count<string>() == 1)
        writer.Write("t." + this.ScrubName(columns.Single<string>()));
      else
        writer.Write("new { " + columns.Join<string>((Func<string, string>) (c => "t." + this.ScrubName(c)), ", ") + " }");
    }

    /// <summary>
    /// Generates code to perform an <see cref="T:System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation" />.
    /// </summary>
    /// <param name="addPrimaryKeyOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      AddPrimaryKeyOperation addPrimaryKeyOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<AddPrimaryKeyOperation>(addPrimaryKeyOperation, nameof (addPrimaryKeyOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("AddPrimaryKey(");
      writer.Write(this.Quote(addPrimaryKeyOperation.Table));
      writer.Write(", ");
      bool flag = addPrimaryKeyOperation.Columns.Count<string>() > 1;
      if (flag)
        writer.Write("new[] { ");
      writer.Write(addPrimaryKeyOperation.Columns.Join<string>(new Func<string, string>(this.Quote), ", "));
      if (flag)
        writer.Write(" }");
      if (!addPrimaryKeyOperation.HasDefaultName)
      {
        writer.Write(", name: ");
        writer.Write(this.Quote(addPrimaryKeyOperation.Name));
      }
      if (!addPrimaryKeyOperation.IsClustered)
        writer.Write(", clustered: false");
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.DropPrimaryKeyOperation" />.
    /// </summary>
    /// <param name="dropPrimaryKeyOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      DropPrimaryKeyOperation dropPrimaryKeyOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<DropPrimaryKeyOperation>(dropPrimaryKeyOperation, nameof (dropPrimaryKeyOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("DropPrimaryKey(");
      writer.Write(this.Quote(dropPrimaryKeyOperation.Table));
      if (!dropPrimaryKeyOperation.HasDefaultName)
      {
        writer.Write(", name: ");
        writer.Write(this.Quote(dropPrimaryKeyOperation.Name));
      }
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform an <see cref="T:System.Data.Entity.Migrations.Model.AddForeignKeyOperation" />.
    /// </summary>
    /// <param name="addForeignKeyOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      AddForeignKeyOperation addForeignKeyOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<AddForeignKeyOperation>(addForeignKeyOperation, nameof (addForeignKeyOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("AddForeignKey(");
      writer.Write(this.Quote(addForeignKeyOperation.DependentTable));
      writer.Write(", ");
      bool flag = addForeignKeyOperation.DependentColumns.Count<string>() > 1;
      if (flag)
        writer.Write("new[] { ");
      writer.Write(addForeignKeyOperation.DependentColumns.Join<string>(new Func<string, string>(this.Quote), ", "));
      if (flag)
        writer.Write(" }");
      writer.Write(", ");
      writer.Write(this.Quote(addForeignKeyOperation.PrincipalTable));
      if (addForeignKeyOperation.PrincipalColumns.Any<string>())
      {
        writer.Write(", ");
        if (flag)
          writer.Write("new[] { ");
        writer.Write(addForeignKeyOperation.PrincipalColumns.Join<string>(new Func<string, string>(this.Quote), ", "));
        if (flag)
          writer.Write(" }");
      }
      if (addForeignKeyOperation.CascadeDelete)
        writer.Write(", cascadeDelete: true");
      if (!addForeignKeyOperation.HasDefaultName)
      {
        writer.Write(", name: ");
        writer.Write(this.Quote(addForeignKeyOperation.Name));
      }
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.DropForeignKeyOperation" />.
    /// </summary>
    /// <param name="dropForeignKeyOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      DropForeignKeyOperation dropForeignKeyOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<DropForeignKeyOperation>(dropForeignKeyOperation, nameof (dropForeignKeyOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("DropForeignKey(");
      writer.Write(this.Quote(dropForeignKeyOperation.DependentTable));
      writer.Write(", ");
      if (!dropForeignKeyOperation.HasDefaultName)
      {
        writer.Write(this.Quote(dropForeignKeyOperation.Name));
      }
      else
      {
        bool flag = dropForeignKeyOperation.DependentColumns.Count<string>() > 1;
        if (flag)
          writer.Write("new[] { ");
        writer.Write(dropForeignKeyOperation.DependentColumns.Join<string>(new Func<string, string>(this.Quote), ", "));
        if (flag)
          writer.Write(" }");
        writer.Write(", ");
        writer.Write(this.Quote(dropForeignKeyOperation.PrincipalTable));
      }
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.CreateIndexOperation" />.
    /// </summary>
    /// <param name="createIndexOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      CreateIndexOperation createIndexOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<CreateIndexOperation>(createIndexOperation, nameof (createIndexOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("CreateIndex(");
      writer.Write(this.Quote(createIndexOperation.Table));
      writer.Write(", ");
      bool flag = createIndexOperation.Columns.Count<string>() > 1;
      if (flag)
        writer.Write("new[] { ");
      writer.Write(createIndexOperation.Columns.Join<string>(new Func<string, string>(this.Quote), ", "));
      if (flag)
        writer.Write(" }");
      this.WriteIndexParameters(createIndexOperation, writer);
      writer.WriteLine(");");
    }

    private void WriteIndexParameters(
      CreateIndexOperation createIndexOperation,
      IndentedTextWriter writer)
    {
      if (createIndexOperation.IsUnique)
        writer.Write(", unique: true");
      if (createIndexOperation.IsClustered)
        writer.Write(", clustered: true");
      if (createIndexOperation.HasDefaultName)
        return;
      writer.Write(", name: ");
      writer.Write(this.Quote(createIndexOperation.Name));
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.DropIndexOperation" />.
    /// </summary>
    /// <param name="dropIndexOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      DropIndexOperation dropIndexOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<DropIndexOperation>(dropIndexOperation, nameof (dropIndexOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("DropIndex(");
      writer.Write(this.Quote(dropIndexOperation.Table));
      writer.Write(", ");
      if (!dropIndexOperation.HasDefaultName)
      {
        writer.Write(this.Quote(dropIndexOperation.Name));
      }
      else
      {
        writer.Write("new[] { ");
        writer.Write(dropIndexOperation.Columns.Join<string>(new Func<string, string>(this.Quote), ", "));
        writer.Write(" }");
      }
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to specify the definition for a <see cref="T:System.Data.Entity.Migrations.Model.ColumnModel" />.
    /// </summary>
    /// <param name="column"> The column definition to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    /// <param name="emitName"> A value indicating whether to include the column name in the definition. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    protected virtual void Generate(ColumnModel column, IndentedTextWriter writer, bool emitName = false)
    {
      Check.NotNull<ColumnModel>(column, nameof (column));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write(" c.");
      writer.Write(this.TranslateColumnType(column.Type));
      writer.Write("(");
      List<string> stringList1 = new List<string>();
      if (emitName)
        stringList1.Add("name: " + this.Quote(column.Name));
      bool? isNullable = column.IsNullable;
      if ((isNullable.GetValueOrDefault() ? 0 : (isNullable.HasValue ? 1 : 0)) != 0)
        stringList1.Add("nullable: false");
      if (column.MaxLength.HasValue)
        stringList1.Add("maxLength: " + (object) column.MaxLength);
      byte? precision = column.Precision;
      if ((precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        stringList1.Add("precision: " + (object) column.Precision);
      byte? scale = column.Scale;
      if ((scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        stringList1.Add("scale: " + (object) column.Scale);
      if (column.IsFixedLength.HasValue)
        stringList1.Add("fixedLength: " + column.IsFixedLength.ToString().ToLowerInvariant());
      if (column.IsUnicode.HasValue)
        stringList1.Add("unicode: " + column.IsUnicode.ToString().ToLowerInvariant());
      if (column.IsIdentity)
        stringList1.Add("identity: true");
      if (column.DefaultValue != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site53 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site53 = CallSite<Action<CallSite, List<string>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, List<string>, object> target1 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site53.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, List<string>, object>> pSite53 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site53;
        List<string> stringList2 = stringList1;
        // ISSUE: reference to a compiler-generated field
        if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site54 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site54 = CallSite<Func<CallSite, string, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Add, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, string, object, object> target2 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site54.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, string, object, object>> pSite54 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site54;
        // ISSUE: reference to a compiler-generated field
        if (CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site55 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site55 = CallSite<Func<CallSite, CSharpMigrationCodeGenerator, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, nameof (Generate), (IEnumerable<Type>) null, typeof (CSharpMigrationCodeGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj1 = CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site55.Target((CallSite) CSharpMigrationCodeGenerator.\u003CGenerate\u003Eo__SiteContainer52.\u003C\u003Ep__Site55, this, column.DefaultValue);
        object obj2 = target2((CallSite) pSite54, "defaultValue: ", obj1);
        target1((CallSite) pSite53, stringList2, obj2);
      }
      if (!string.IsNullOrWhiteSpace(column.DefaultValueSql))
        stringList1.Add("defaultValueSql: " + this.Quote(column.DefaultValueSql));
      if (column.IsTimestamp)
        stringList1.Add("timestamp: true");
      if (!string.IsNullOrWhiteSpace(column.StoreType))
        stringList1.Add("storeType: " + this.Quote(column.StoreType));
      writer.Write(stringList1.Join<string>((Func<string, string>) null, ", "));
      if (column.Annotations.Any<KeyValuePair<string, AnnotationValues>>())
      {
        ++writer.Indent;
        writer.WriteLine(stringList1.Any<string>() ? "," : "");
        writer.Write("annotations: ");
        this.GenerateAnnotations(column.Annotations, writer);
        --writer.Indent;
      }
      writer.Write(")");
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:byte[]" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(byte[] defaultValue)
    {
      return "new byte[] {" + ((IEnumerable<byte>) defaultValue).Join<byte>((Func<byte, string>) null, ", ") + "}";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.DateTime" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(DateTime defaultValue)
    {
      return "new DateTime(" + (object) defaultValue.Ticks + ", DateTimeKind." + Enum.GetName(typeof (DateTimeKind), (object) defaultValue.Kind) + ")";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.DateTimeOffset" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(DateTimeOffset defaultValue)
    {
      return "new DateTimeOffset(" + (object) defaultValue.Ticks + ", new TimeSpan(" + (object) defaultValue.Offset.Ticks + "))";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.Decimal" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(Decimal defaultValue)
    {
      return defaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "m";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.Guid" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(Guid defaultValue)
    {
      return "new Guid(\"" + (object) defaultValue + "\")";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.Int64" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(long defaultValue)
    {
      return defaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.Single" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(float defaultValue)
    {
      return defaultValue.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "f";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.String" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(string defaultValue)
    {
      return this.Quote(defaultValue);
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.TimeSpan" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(TimeSpan defaultValue)
    {
      return "new TimeSpan(" + (object) defaultValue.Ticks + ")";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.Data.Entity.Spatial.DbGeography" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(DbGeography defaultValue)
    {
      return "DbGeography.FromText(\"" + defaultValue.AsText() + "\", " + (object) defaultValue.CoordinateSystemId + ")";
    }

    /// <summary>
    /// Generates code to specify the default value for a <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> column.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    protected virtual string Generate(DbGeometry defaultValue)
    {
      return "DbGeometry.FromText(\"" + defaultValue.AsText() + "\", " + (object) defaultValue.CoordinateSystemId + ")";
    }

    /// <summary>
    /// Generates code to specify the default value for a column of unknown data type.
    /// </summary>
    /// <param name="defaultValue"> The value to be used as the default. </param>
    /// <returns> Code representing the default value. </returns>
    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    protected virtual string Generate(object defaultValue)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", defaultValue).ToLowerInvariant();
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.DropTableOperation" />.
    /// </summary>
    /// <param name="dropTableOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      DropTableOperation dropTableOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<DropTableOperation>(dropTableOperation, nameof (dropTableOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("DropTable(");
      writer.Write(this.Quote(dropTableOperation.Name));
      if (dropTableOperation.RemovedAnnotations.Any<KeyValuePair<string, object>>())
      {
        ++writer.Indent;
        writer.WriteLine(",");
        writer.Write("removedAnnotations: ");
        this.GenerateAnnotations(dropTableOperation.RemovedAnnotations, writer);
        --writer.Indent;
      }
      IDictionary<string, IDictionary<string, object>> columnAnnotations = dropTableOperation.RemovedColumnAnnotations;
      if (columnAnnotations.Any<KeyValuePair<string, IDictionary<string, object>>>())
      {
        ++writer.Indent;
        writer.WriteLine(",");
        writer.Write("removedColumnAnnotations: ");
        writer.WriteLine("new Dictionary<string, IDictionary<string, object>>");
        writer.WriteLine("{");
        ++writer.Indent;
        foreach (string identifier in (IEnumerable<string>) columnAnnotations.Keys.OrderBy<string, string>((Func<string, string>) (k => k)))
        {
          writer.WriteLine("{");
          ++writer.Indent;
          writer.WriteLine(this.Quote(identifier) + ",");
          this.GenerateAnnotations(columnAnnotations[identifier], writer);
          writer.WriteLine();
          --writer.Indent;
          writer.WriteLine("},");
        }
        --writer.Indent;
        writer.Write("}");
        --writer.Indent;
      }
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.MoveTableOperation" />.
    /// </summary>
    /// <param name="moveTableOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      MoveTableOperation moveTableOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<MoveTableOperation>(moveTableOperation, nameof (moveTableOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("MoveTable(name: ");
      writer.Write(this.Quote(moveTableOperation.Name));
      writer.Write(", newSchema: ");
      writer.Write(string.IsNullOrWhiteSpace(moveTableOperation.NewSchema) ? "null" : this.Quote(moveTableOperation.NewSchema));
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.MoveProcedureOperation" />.
    /// </summary>
    /// <param name="moveProcedureOperation">The operation to generate code for.</param>
    /// <param name="writer">Text writer to add the generated code to.</param>
    protected virtual void Generate(
      MoveProcedureOperation moveProcedureOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<MoveProcedureOperation>(moveProcedureOperation, nameof (moveProcedureOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("MoveStoredProcedure(name: ");
      writer.Write(this.Quote(moveProcedureOperation.Name));
      writer.Write(", newSchema: ");
      writer.Write(string.IsNullOrWhiteSpace(moveProcedureOperation.NewSchema) ? "null" : this.Quote(moveProcedureOperation.NewSchema));
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.RenameTableOperation" />.
    /// </summary>
    /// <param name="renameTableOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      RenameTableOperation renameTableOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<RenameTableOperation>(renameTableOperation, nameof (renameTableOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("RenameTable(name: ");
      writer.Write(this.Quote(renameTableOperation.Name));
      writer.Write(", newName: ");
      writer.Write(this.Quote(renameTableOperation.NewName));
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.RenameProcedureOperation" />.
    /// </summary>
    /// <param name="renameProcedureOperation">The operation to generate code for.</param>
    /// <param name="writer">Text writer to add the generated code to.</param>
    protected virtual void Generate(
      RenameProcedureOperation renameProcedureOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<RenameProcedureOperation>(renameProcedureOperation, nameof (renameProcedureOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("RenameStoredProcedure(name: ");
      writer.Write(this.Quote(renameProcedureOperation.Name));
      writer.Write(", newName: ");
      writer.Write(this.Quote(renameProcedureOperation.NewName));
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.RenameColumnOperation" />.
    /// </summary>
    /// <param name="renameColumnOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      RenameColumnOperation renameColumnOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<RenameColumnOperation>(renameColumnOperation, nameof (renameColumnOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("RenameColumn(table: ");
      writer.Write(this.Quote(renameColumnOperation.Table));
      writer.Write(", name: ");
      writer.Write(this.Quote(renameColumnOperation.Name));
      writer.Write(", newName: ");
      writer.Write(this.Quote(renameColumnOperation.NewName));
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.RenameIndexOperation" />.
    /// </summary>
    /// <param name="renameIndexOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(
      RenameIndexOperation renameIndexOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<RenameIndexOperation>(renameIndexOperation, nameof (renameIndexOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("RenameIndex(table: ");
      writer.Write(this.Quote(renameIndexOperation.Table));
      writer.Write(", name: ");
      writer.Write(this.Quote(renameIndexOperation.Name));
      writer.Write(", newName: ");
      writer.Write(this.Quote(renameIndexOperation.NewName));
      writer.WriteLine(");");
    }

    /// <summary>
    /// Generates code to perform a <see cref="T:System.Data.Entity.Migrations.Model.SqlOperation" />.
    /// </summary>
    /// <param name="sqlOperation"> The operation to generate code for. </param>
    /// <param name="writer"> Text writer to add the generated code to. </param>
    protected virtual void Generate(SqlOperation sqlOperation, IndentedTextWriter writer)
    {
      Check.NotNull<SqlOperation>(sqlOperation, nameof (sqlOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write("Sql(@");
      writer.Write(this.Quote(sqlOperation.Sql));
      if (sqlOperation.SuppressTransaction)
        writer.Write(", suppressTransaction: true");
      writer.WriteLine(");");
    }

    /// <summary>
    /// Removes any invalid characters from the name of an database artifact.
    /// </summary>
    /// <param name="name"> The name to be scrubbed. </param>
    /// <returns> The scrubbed name. </returns>
    [SuppressMessage("Microsoft.Security", "CA2141:TransparentMethodsMustNotSatisfyLinkDemandsFxCopRule")]
    protected virtual string ScrubName(string name)
    {
      Check.NotEmpty(name, nameof (name));
      name = new Regex("[^\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Nd}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Cf}\\p{Pc}\\p{Lm}]").Replace(name, string.Empty);
      using (CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider())
      {
        if (char.IsLetter(name[0]) || name[0] == '_')
        {
          if (csharpCodeProvider.IsValidIdentifier(name))
            goto label_7;
        }
        name = "_" + name;
      }
label_7:
      return name;
    }

    /// <summary>
    /// Gets the type name to use for a column of the given data type.
    /// </summary>
    /// <param name="primitiveTypeKind"> The data type to translate. </param>
    /// <returns> The type name to use in the generated migration. </returns>
    protected virtual string TranslateColumnType(PrimitiveTypeKind primitiveTypeKind)
    {
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Int16:
          return "Short";
        case PrimitiveTypeKind.Int32:
          return "Int";
        case PrimitiveTypeKind.Int64:
          return "Long";
        default:
          return Enum.GetName(typeof (PrimitiveTypeKind), (object) primitiveTypeKind);
      }
    }

    /// <summary>
    /// Quotes an identifier using appropriate escaping to allow it to be stored in a string.
    /// </summary>
    /// <param name="identifier"> The identifier to be quoted. </param>
    /// <returns> The quoted identifier. </returns>
    protected virtual string Quote(string identifier)
    {
      return "\"" + identifier + "\"";
    }
  }
}
