// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.DynamicToFunctionModificationCommandConverter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Data.Entity.Migrations.Infrastructure
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class DynamicToFunctionModificationCommandConverter : DefaultExpressionVisitor
  {
    private readonly EntityTypeModificationFunctionMapping _entityTypeModificationFunctionMapping;
    private readonly AssociationSetModificationFunctionMapping _associationSetModificationFunctionMapping;
    private readonly EntityContainerMapping _entityContainerMapping;
    private ModificationFunctionMapping _currentFunctionMapping;
    private EdmProperty _currentProperty;
    private List<EdmProperty> _storeGeneratedKeys;
    private int _nextStoreGeneratedKey;
    private bool _useOriginalValues;

    public DynamicToFunctionModificationCommandConverter(
      EntityTypeModificationFunctionMapping entityTypeModificationFunctionMapping,
      EntityContainerMapping entityContainerMapping)
    {
      this._entityTypeModificationFunctionMapping = entityTypeModificationFunctionMapping;
      this._entityContainerMapping = entityContainerMapping;
    }

    public DynamicToFunctionModificationCommandConverter(
      AssociationSetModificationFunctionMapping associationSetModificationFunctionMapping,
      EntityContainerMapping entityContainerMapping)
    {
      this._associationSetModificationFunctionMapping = associationSetModificationFunctionMapping;
      this._entityContainerMapping = entityContainerMapping;
    }

    public IEnumerable<TCommandTree> Convert<TCommandTree>(
      IEnumerable<TCommandTree> modificationCommandTrees)
      where TCommandTree : DbModificationCommandTree
    {
      this._currentFunctionMapping = (ModificationFunctionMapping) null;
      this._currentProperty = (EdmProperty) null;
      this._storeGeneratedKeys = (List<EdmProperty>) null;
      this._nextStoreGeneratedKey = 0;
      return modificationCommandTrees.Select<TCommandTree, object>((Func<TCommandTree, object>) (modificationCommandTree =>
      {
        // ISSUE: reference to a compiler-generated field
        if (DynamicToFunctionModificationCommandConverter.\u003CConvert\u003Eo__SiteContainer0<TCommandTree>.\u003C\u003Ep__Site1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          DynamicToFunctionModificationCommandConverter.\u003CConvert\u003Eo__SiteContainer0<TCommandTree>.\u003C\u003Ep__Site1 = CallSite<Func<CallSite, DynamicToFunctionModificationCommandConverter, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ConvertInternal", (IEnumerable<Type>) null, typeof (DynamicToFunctionModificationCommandConverter), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return DynamicToFunctionModificationCommandConverter.\u003CConvert\u003Eo__SiteContainer0<TCommandTree>.\u003C\u003Ep__Site1.Target((CallSite) DynamicToFunctionModificationCommandConverter.\u003CConvert\u003Eo__SiteContainer0<TCommandTree>.\u003C\u003Ep__Site1, this, (object) modificationCommandTree);
      })).Cast<TCommandTree>();
    }

    private DbModificationCommandTree ConvertInternal(
      DbInsertCommandTree commandTree)
    {
      if (this._currentFunctionMapping == null)
      {
        this._currentFunctionMapping = this._entityTypeModificationFunctionMapping != null ? this._entityTypeModificationFunctionMapping.InsertFunctionMapping : this._associationSetModificationFunctionMapping.InsertFunctionMapping;
        this._storeGeneratedKeys = ((DbScanExpression) commandTree.Target.Expression).Target.ElementType.KeyProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => p.IsStoreGeneratedIdentity)).ToList<EdmProperty>();
      }
      this._nextStoreGeneratedKey = 0;
      return (DbModificationCommandTree) new DbInsertCommandTree(commandTree.MetadataWorkspace, commandTree.DataSpace, commandTree.Target, this.VisitSetClauses(commandTree.SetClauses), commandTree.Returning != null ? commandTree.Returning.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this) : (DbExpression) null);
    }

    private DbModificationCommandTree ConvertInternal(
      DbUpdateCommandTree commandTree)
    {
      this._currentFunctionMapping = this._entityTypeModificationFunctionMapping.UpdateFunctionMapping;
      this._useOriginalValues = true;
      DbExpression predicate = commandTree.Predicate.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this);
      this._useOriginalValues = false;
      return (DbModificationCommandTree) new DbUpdateCommandTree(commandTree.MetadataWorkspace, commandTree.DataSpace, commandTree.Target, predicate, this.VisitSetClauses(commandTree.SetClauses), commandTree.Returning != null ? commandTree.Returning.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this) : (DbExpression) null);
    }

    private DbModificationCommandTree ConvertInternal(
      DbDeleteCommandTree commandTree)
    {
      this._currentFunctionMapping = this._entityTypeModificationFunctionMapping != null ? this._entityTypeModificationFunctionMapping.DeleteFunctionMapping : this._associationSetModificationFunctionMapping.DeleteFunctionMapping;
      return (DbModificationCommandTree) new DbDeleteCommandTree(commandTree.MetadataWorkspace, commandTree.DataSpace, commandTree.Target, commandTree.Predicate.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this));
    }

    private ReadOnlyCollection<DbModificationClause> VisitSetClauses(
      IList<DbModificationClause> setClauses)
    {
      return new ReadOnlyCollection<DbModificationClause>((IList<DbModificationClause>) setClauses.Cast<DbSetClause>().Select<DbSetClause, DbSetClause>((Func<DbSetClause, DbSetClause>) (s => new DbSetClause(s.Property.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this), s.Value.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this)))).Cast<DbModificationClause>().ToList<DbModificationClause>());
    }

    public override DbExpression Visit(DbComparisonExpression expression)
    {
      DbComparisonExpression left1 = (DbComparisonExpression) base.Visit(expression);
      DbPropertyExpression left2 = (DbPropertyExpression) left1.Left;
      if (!((EdmProperty) left2.Property).Nullable)
        return (DbExpression) left1;
      DbAndExpression dbAndExpression = left2.IsNull().And((DbExpression) left1.Right.IsNull());
      return (DbExpression) left1.Or((DbExpression) dbAndExpression);
    }

    public override DbExpression Visit(DbPropertyExpression expression)
    {
      this._currentProperty = (EdmProperty) expression.Property;
      return base.Visit(expression);
    }

    public override DbExpression Visit(DbConstantExpression expression)
    {
      if (this._currentProperty != null)
      {
        Tuple<FunctionParameter, bool> parameter = this.GetParameter(this._currentProperty, this._useOriginalValues);
        if (parameter != null)
          return (DbExpression) new DbParameterReferenceExpression(parameter.Item1.TypeUsage, parameter.Item1.Name);
      }
      return base.Visit(expression);
    }

    public override DbExpression Visit(DbAndExpression expression)
    {
      DbExpression left = this.VisitExpression(expression.Left);
      DbExpression right = this.VisitExpression(expression.Right);
      if (left != null && right != null)
        return (DbExpression) left.And(right);
      return left ?? right;
    }

    public override DbExpression Visit(DbIsNullExpression expression)
    {
      DbPropertyExpression left = expression.Argument as DbPropertyExpression;
      if (left != null)
      {
        Tuple<FunctionParameter, bool> parameter = this.GetParameter((EdmProperty) left.Property, true);
        if (parameter != null)
        {
          if (parameter.Item2)
            return (DbExpression) null;
          DbParameterReferenceExpression referenceExpression = new DbParameterReferenceExpression(parameter.Item1.TypeUsage, parameter.Item1.Name);
          return (DbExpression) left.Equal((DbExpression) referenceExpression).Or((DbExpression) left.IsNull().And((DbExpression) referenceExpression.IsNull()));
        }
      }
      return base.Visit(expression);
    }

    public override DbExpression Visit(DbNullExpression expression)
    {
      if (this._currentProperty != null)
      {
        Tuple<FunctionParameter, bool> parameter = this.GetParameter(this._currentProperty, false);
        if (parameter != null)
          return (DbExpression) new DbParameterReferenceExpression(parameter.Item1.TypeUsage, parameter.Item1.Name);
      }
      return base.Visit(expression);
    }

    public override DbExpression Visit(DbNewInstanceExpression expression)
    {
      return (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) expression.Arguments.Cast<DbPropertyExpression>().Select(propertyExpression => new
      {
        propertyExpression = propertyExpression,
        resultBinding = this._currentFunctionMapping.ResultBindings.Single<ModificationFunctionResultBinding>((Func<ModificationFunctionResultBinding, bool>) (rb => this._entityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings), (esm, etm) => new
        {
          esm = esm,
          etm = etm
        }).SelectMany(_param0 => (IEnumerable<MappingFragment>) _param0.etm.MappingFragments, (_param0, mf) => new
        {
          \u003C\u003Eh__TransparentIdentifier7 = _param0,
          mf = mf
        }).SelectMany(_param0 => _param0.mf.PropertyMappings.OfType<ScalarPropertyMapping>(), (_param0, pm) => new
        {
          \u003C\u003Eh__TransparentIdentifier8 = _param0,
          pm = pm
        }).Where(_param1 =>
        {
          if (_param1.pm.Column.EdmEquals((MetadataItem) propertyExpression.Property))
            return _param1.pm.Column.DeclaringType.EdmEquals((MetadataItem) propertyExpression.Property.DeclaringType);
          return false;
        }).Select(_param0 => _param0.pm.Property).Contains<EdmProperty>(rb.Property)))
      }).Select(_param0 => new KeyValuePair<string, DbExpression>(_param0.resultBinding.ColumnName, (DbExpression) _param0.propertyExpression)).ToList<KeyValuePair<string, DbExpression>>());
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private Tuple<FunctionParameter, bool> GetParameter(
      EdmProperty column,
      bool originalValue = false)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      DynamicToFunctionModificationCommandConverter.\u003C\u003Ec__DisplayClass54 cDisplayClass54_1 = new DynamicToFunctionModificationCommandConverter.\u003C\u003Ec__DisplayClass54()
      {
        column = column
      };
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated method
      cDisplayClass54_1.columnMappings = this._entityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings), (esm, etm) => new
      {
        esm = esm,
        etm = etm
      }).SelectMany(_param0 => (IEnumerable<MappingFragment>) _param0.etm.MappingFragments, (_param0, mf) => new
      {
        \u003C\u003Eh__TransparentIdentifier22 = _param0,
        mf = mf
      }).SelectMany(_param0 => _param0.mf.FlattenedProperties, (_param0, cm) => new
      {
        \u003C\u003Eh__TransparentIdentifier23 = _param0,
        cm = cm
      }).Where(new Func<\u003C\u003Ef__AnonymousType8<\u003C\u003Ef__AnonymousType7<\u003C\u003Ef__AnonymousType1<EntitySetMapping, EntityTypeMapping>, MappingFragment>, ColumnMappingBuilder>, bool>(cDisplayClass54_1.\u003CGetParameter\u003Eb__2f)).Select(_param0 => _param0.cm).ToList<ColumnMappingBuilder>();
      // ISSUE: reference to a compiler-generated method
      List<ModificationFunctionParameterBinding> list = this._currentFunctionMapping.ParameterBindings.Where<ModificationFunctionParameterBinding>(new Func<ModificationFunctionParameterBinding, bool>(cDisplayClass54_1.\u003CGetParameter\u003Eb__31)).ToList<ModificationFunctionParameterBinding>();
      if (!list.Any<ModificationFunctionParameterBinding>())
      {
        // ISSUE: reference to a compiler-generated method
        List<EdmMember[]> iaColumnMappings = this._entityContainerMapping.AssociationSetMappings.SelectMany((Func<AssociationSetMapping, IEnumerable<TypeMapping>>) (asm => asm.TypeMappings), (asm, tm) => new
        {
          asm = asm,
          tm = tm
        }).SelectMany(_param0 => (IEnumerable<MappingFragment>) _param0.tm.MappingFragments, (_param0, mf) => new
        {
          \u003C\u003Eh__TransparentIdentifier25 = _param0,
          mf = mf
        }).SelectMany(_param0 => _param0.mf.PropertyMappings.OfType<EndPropertyMapping>(), (_param0, epm) => new
        {
          \u003C\u003Eh__TransparentIdentifier26 = _param0,
          epm = epm
        }).SelectMany(_param0 => (IEnumerable<ScalarPropertyMapping>) _param0.epm.PropertyMappings, (_param0, pm) => new
        {
          \u003C\u003Eh__TransparentIdentifier27 = _param0,
          pm = pm
        }).Where(new Func<\u003C\u003Ef__AnonymousTypec<\u003C\u003Ef__AnonymousTypeb<\u003C\u003Ef__AnonymousTypea<\u003C\u003Ef__AnonymousType9<AssociationSetMapping, TypeMapping>, MappingFragment>, EndPropertyMapping>, ScalarPropertyMapping>, bool>(cDisplayClass54_1.\u003CGetParameter\u003Eb__3b)).Select(_param0 => new EdmMember[2]
        {
          (EdmMember) _param0.pm.Property,
          (EdmMember) _param0.\u003C\u003Eh__TransparentIdentifier27.epm.AssociationEnd
        }).ToList<EdmMember[]>();
        list = this._currentFunctionMapping.ParameterBindings.Where<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb =>
        {
          // ISSUE: variable of a compiler-generated type
          DynamicToFunctionModificationCommandConverter.\u003C\u003Ec__DisplayClass54 cDisplayClass54 = cDisplayClass54_1;
          ModificationFunctionParameterBinding pb1 = pb;
          return iaColumnMappings.Any<EdmMember[]>((Func<EdmMember[], bool>) (epm => pb1.MemberPath.Members.SequenceEqual<EdmMember>((IEnumerable<EdmMember>) epm)));
        })).ToList<ModificationFunctionParameterBinding>();
      }
      // ISSUE: reference to a compiler-generated field
      if (list.Count == 0 && cDisplayClass54_1.column.IsPrimaryKeyColumn)
      {
        // ISSUE: reference to a compiler-generated field
        return Tuple.Create<FunctionParameter, bool>(new FunctionParameter(this._storeGeneratedKeys[this._nextStoreGeneratedKey++].Name, cDisplayClass54_1.column.TypeUsage, ParameterMode.In), true);
      }
      if (list.Count == 1)
        return Tuple.Create<FunctionParameter, bool>(list[0].Parameter, list[0].IsCurrent);
      if (list.Count == 0)
        return (Tuple<FunctionParameter, bool>) null;
      ModificationFunctionParameterBinding parameterBinding = originalValue ? list.Single<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb => !pb.IsCurrent)) : list.Single<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb => pb.IsCurrent));
      return Tuple.Create<FunctionParameter, bool>(parameterBinding.Parameter, parameterBinding.IsCurrent);
    }
  }
}
