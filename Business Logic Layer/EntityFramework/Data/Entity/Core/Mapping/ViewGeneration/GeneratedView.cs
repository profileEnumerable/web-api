// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.GeneratedView
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Core.Query.PlanCompiler;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal sealed class GeneratedView : InternalBase
  {
    private readonly EntitySetBase m_extent;
    private readonly EdmType m_type;
    private DbQueryCommandTree m_commandTree;
    private readonly string m_eSQL;
    private System.Data.Entity.Core.Query.InternalTrees.Node m_internalTreeNode;
    private DiscriminatorMap m_discriminatorMap;
    private readonly StorageMappingItemCollection m_mappingItemCollection;
    private readonly ConfigViewGenerator m_config;

    internal static GeneratedView CreateGeneratedView(
      EntitySetBase extent,
      EdmType type,
      DbQueryCommandTree commandTree,
      string eSQL,
      StorageMappingItemCollection mappingItemCollection,
      ConfigViewGenerator config)
    {
      DiscriminatorMap discriminatorMap = (DiscriminatorMap) null;
      if (commandTree != null)
      {
        commandTree = ViewSimplifier.SimplifyView(extent, commandTree);
        if (extent.BuiltInTypeKind == BuiltInTypeKind.EntitySet)
          DiscriminatorMap.TryCreateDiscriminatorMap((EntitySet) extent, commandTree.Query, out discriminatorMap);
      }
      return new GeneratedView(extent, type, commandTree, eSQL, discriminatorMap, mappingItemCollection, config);
    }

    internal static GeneratedView CreateGeneratedViewForFKAssociationSet(
      EntitySetBase extent,
      EdmType type,
      DbQueryCommandTree commandTree,
      StorageMappingItemCollection mappingItemCollection,
      ConfigViewGenerator config)
    {
      return new GeneratedView(extent, type, commandTree, (string) null, (DiscriminatorMap) null, mappingItemCollection, config);
    }

    internal static bool TryParseUserSpecifiedView(
      EntitySetBaseMapping setMapping,
      EntityTypeBase type,
      string eSQL,
      bool includeSubtypes,
      StorageMappingItemCollection mappingItemCollection,
      ConfigViewGenerator config,
      IList<EdmSchemaError> errors,
      out GeneratedView generatedView)
    {
      bool flag = false;
      DbQueryCommandTree commandTree;
      DiscriminatorMap discriminatorMap;
      Exception parserException;
      if (!GeneratedView.TryParseView(eSQL, true, setMapping.Set, mappingItemCollection, config, out commandTree, out discriminatorMap, out parserException))
      {
        EdmSchemaError edmSchemaError = new EdmSchemaError(Strings.Mapping_Invalid_QueryView2((object) setMapping.Set.Name, (object) parserException.Message), 2068, EdmSchemaErrorSeverity.Error, setMapping.EntityContainerMapping.SourceLocation, setMapping.StartLineNumber, setMapping.StartLinePosition, parserException);
        errors.Add(edmSchemaError);
        flag = true;
      }
      else
      {
        foreach (EdmSchemaError edmSchemaError in ViewValidator.ValidateQueryView(commandTree, setMapping, type, includeSubtypes))
        {
          errors.Add(edmSchemaError);
          flag = true;
        }
        CollectionType edmType = commandTree.Query.ResultType.EdmType as CollectionType;
        if (edmType == null || !setMapping.Set.ElementType.IsAssignableFrom(edmType.TypeUsage.EdmType))
        {
          EdmSchemaError edmSchemaError = new EdmSchemaError(Strings.Mapping_Invalid_QueryView_Type((object) setMapping.Set.Name), 2069, EdmSchemaErrorSeverity.Error, setMapping.EntityContainerMapping.SourceLocation, setMapping.StartLineNumber, setMapping.StartLinePosition);
          errors.Add(edmSchemaError);
          flag = true;
        }
      }
      if (!flag)
      {
        generatedView = new GeneratedView(setMapping.Set, (EdmType) type, commandTree, eSQL, discriminatorMap, mappingItemCollection, config);
        return true;
      }
      generatedView = (GeneratedView) null;
      return false;
    }

    private GeneratedView(
      EntitySetBase extent,
      EdmType type,
      DbQueryCommandTree commandTree,
      string eSQL,
      DiscriminatorMap discriminatorMap,
      StorageMappingItemCollection mappingItemCollection,
      ConfigViewGenerator config)
    {
      this.m_extent = extent;
      this.m_type = type;
      this.m_commandTree = commandTree;
      this.m_eSQL = eSQL;
      this.m_discriminatorMap = discriminatorMap;
      this.m_mappingItemCollection = mappingItemCollection;
      this.m_config = config;
      if (!this.m_config.IsViewTracing)
        return;
      StringBuilder builder = new StringBuilder(1024);
      this.ToCompactString(builder);
      Helpers.FormatTraceLine("CQL view for {0}", (object) builder.ToString());
    }

    internal string eSQL
    {
      get
      {
        return this.m_eSQL;
      }
    }

    internal DbQueryCommandTree GetCommandTree()
    {
      Exception parserException;
      if (this.m_commandTree == null && !GeneratedView.TryParseView(this.m_eSQL, false, this.m_extent, this.m_mappingItemCollection, this.m_config, out this.m_commandTree, out this.m_discriminatorMap, out parserException))
        throw new MappingException(Strings.Mapping_Invalid_QueryView((object) this.m_extent.Name, (object) parserException.Message));
      return this.m_commandTree;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "projectOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal System.Data.Entity.Core.Query.InternalTrees.Node GetInternalTree(
      Command targetIqtCommand)
    {
      if (this.m_internalTreeNode == null)
      {
        Command command = ITreeGenerator.Generate(this.GetCommandTree(), this.m_discriminatorMap);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(command.Root.Op.OpType == OpType.PhysicalProject, "Expected a physical projectOp at the root of the tree - found " + (object) command.Root.Op.OpType);
        command.DisableVarVecEnumCaching();
        this.m_internalTreeNode = command.Root.Child0;
      }
      return OpCopier.Copy(targetIqtCommand, this.m_internalTreeNode);
    }

    private static bool TryParseView(
      string eSQL,
      bool isUserSpecified,
      EntitySetBase extent,
      StorageMappingItemCollection mappingItemCollection,
      ConfigViewGenerator config,
      out DbQueryCommandTree commandTree,
      out DiscriminatorMap discriminatorMap,
      out Exception parserException)
    {
      commandTree = (DbQueryCommandTree) null;
      discriminatorMap = (DiscriminatorMap) null;
      parserException = (Exception) null;
      config.StartSingleWatch(PerfType.ViewParsing);
      try
      {
        ParserOptions.CompilationMode compilationMode = ParserOptions.CompilationMode.RestrictedViewGenerationMode;
        if (isUserSpecified)
          compilationMode = ParserOptions.CompilationMode.UserViewGenerationMode;
        commandTree = (DbQueryCommandTree) ExternalCalls.CompileView(eSQL, mappingItemCollection, compilationMode);
        commandTree = ViewSimplifier.SimplifyView(extent, commandTree);
        if (extent.BuiltInTypeKind == BuiltInTypeKind.EntitySet)
          DiscriminatorMap.TryCreateDiscriminatorMap((EntitySet) extent, commandTree.Query, out discriminatorMap);
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
          parserException = ex;
        else
          throw;
      }
      finally
      {
        config.StopSingleWatch(PerfType.ViewParsing);
      }
      return parserException == null;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      bool flag = this.m_type != this.m_extent.ElementType;
      if (flag)
        builder.Append("OFTYPE(");
      builder.AppendFormat("{0}.{1}", (object) this.m_extent.EntityContainer.Name, (object) this.m_extent.Name);
      if (flag)
        builder.Append(", ").Append(this.m_type.Name).Append(')');
      builder.AppendLine(" = ");
      if (!string.IsNullOrEmpty(this.m_eSQL))
        builder.Append(this.m_eSQL);
      else
        builder.Append(this.m_commandTree.Print());
    }
  }
}
