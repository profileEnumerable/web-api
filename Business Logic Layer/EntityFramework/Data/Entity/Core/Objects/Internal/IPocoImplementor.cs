// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.IPocoImplementor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class IPocoImplementor
  {
    internal static readonly MethodInfo EntityMemberChangingMethod = typeof (IEntityChangeTracker).GetDeclaredMethod("EntityMemberChanging", typeof (string));
    internal static readonly MethodInfo EntityMemberChangedMethod = typeof (IEntityChangeTracker).GetDeclaredMethod("EntityMemberChanged", typeof (string));
    internal static readonly MethodInfo CreateRelationshipManagerMethod = typeof (RelationshipManager).GetDeclaredMethod("Create", typeof (IEntityWithRelationships));
    internal static readonly MethodInfo GetRelationshipManagerMethod = typeof (IEntityWithRelationships).GetDeclaredProperty("RelationshipManager").Getter();
    internal static readonly MethodInfo GetRelatedReferenceMethod = typeof (RelationshipManager).GetDeclaredMethod("GetRelatedReference", typeof (string), typeof (string));
    internal static readonly MethodInfo GetRelatedCollectionMethod = typeof (RelationshipManager).GetDeclaredMethod("GetRelatedCollection", typeof (string), typeof (string));
    internal static readonly MethodInfo GetRelatedEndMethod = typeof (RelationshipManager).GetDeclaredMethod("GetRelatedEnd", typeof (string), typeof (string));
    internal static readonly MethodInfo ObjectEqualsMethod = typeof (object).GetDeclaredMethod("Equals", typeof (object), typeof (object));
    private static readonly ConstructorInfo _invalidOperationConstructorMethod = typeof (InvalidOperationException).GetDeclaredConstructor(typeof (string));
    internal static readonly MethodInfo GetEntityMethod = typeof (IEntityWrapper).GetDeclaredProperty("Entity").Getter();
    internal static readonly MethodInfo InvokeMethod = typeof (Action<object>).GetDeclaredMethod("Invoke", typeof (object));
    internal static readonly MethodInfo FuncInvokeMethod = typeof (Func<object, object, bool>).GetDeclaredMethod("Invoke", typeof (object), typeof (object));
    internal static readonly MethodInfo SetChangeTrackerMethod = typeof (IEntityWithChangeTracker).GetOnlyDeclaredMethod("SetChangeTracker");
    private readonly EntityType _ospaceEntityType;
    private FieldBuilder _changeTrackerField;
    private FieldBuilder _relationshipManagerField;
    private FieldBuilder _resetFKSetterFlagField;
    private FieldBuilder _compareByteArraysField;
    private MethodBuilder _entityMemberChanging;
    private MethodBuilder _entityMemberChanged;
    private MethodBuilder _getRelationshipManager;
    private readonly List<KeyValuePair<NavigationProperty, PropertyInfo>> _referenceProperties;
    private readonly List<KeyValuePair<NavigationProperty, PropertyInfo>> _collectionProperties;
    private bool _implementIEntityWithChangeTracker;
    private bool _implementIEntityWithRelationships;
    private HashSet<EdmMember> _scalarMembers;
    private HashSet<EdmMember> _relationshipMembers;

    public IPocoImplementor(EntityType ospaceEntityType)
    {
      Type clrType = ospaceEntityType.ClrType;
      this._referenceProperties = new List<KeyValuePair<NavigationProperty, PropertyInfo>>();
      this._collectionProperties = new List<KeyValuePair<NavigationProperty, PropertyInfo>>();
      this._implementIEntityWithChangeTracker = (Type) null == clrType.GetInterface(typeof (IEntityWithChangeTracker).Name);
      this._implementIEntityWithRelationships = (Type) null == clrType.GetInterface(typeof (IEntityWithRelationships).Name);
      this.CheckType(ospaceEntityType);
      this._ospaceEntityType = ospaceEntityType;
    }

    private void CheckType(EntityType ospaceEntityType)
    {
      this._scalarMembers = new HashSet<EdmMember>();
      this._relationshipMembers = new HashSet<EdmMember>();
      foreach (EdmMember member in ospaceEntityType.Members)
      {
        PropertyInfo topProperty = ospaceEntityType.ClrType.GetTopProperty(member.Name);
        if (topProperty != (PropertyInfo) null && EntityProxyFactory.CanProxySetter(topProperty))
        {
          if (member.BuiltInTypeKind == BuiltInTypeKind.EdmProperty)
          {
            if (this._implementIEntityWithChangeTracker)
              this._scalarMembers.Add(member);
          }
          else if (member.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty && this._implementIEntityWithRelationships)
          {
            if (((NavigationProperty) member).ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
            {
              if (topProperty.PropertyType.IsGenericType() && topProperty.PropertyType.GetGenericTypeDefinition() == typeof (ICollection<>))
                this._relationshipMembers.Add(member);
            }
            else
              this._relationshipMembers.Add(member);
          }
        }
      }
      if (ospaceEntityType.Members.Count == this._scalarMembers.Count + this._relationshipMembers.Count)
        return;
      this._scalarMembers.Clear();
      this._relationshipMembers.Clear();
      this._implementIEntityWithChangeTracker = false;
      this._implementIEntityWithRelationships = false;
    }

    public void Implement(TypeBuilder typeBuilder, Action<FieldBuilder, bool> registerField)
    {
      if (this._implementIEntityWithChangeTracker)
        this.ImplementIEntityWithChangeTracker(typeBuilder, registerField);
      if (this._implementIEntityWithRelationships)
        this.ImplementIEntityWithRelationships(typeBuilder, registerField);
      this._resetFKSetterFlagField = typeBuilder.DefineField("_resetFKSetterFlag", typeof (Action<object>), FieldAttributes.Private | FieldAttributes.Static);
      this._compareByteArraysField = typeBuilder.DefineField("_compareByteArrays", typeof (Func<object, object, bool>), FieldAttributes.Private | FieldAttributes.Static);
    }

    public Type[] Interfaces
    {
      get
      {
        List<Type> typeList = new List<Type>();
        if (this._implementIEntityWithChangeTracker)
          typeList.Add(typeof (IEntityWithChangeTracker));
        if (this._implementIEntityWithRelationships)
          typeList.Add(typeof (IEntityWithRelationships));
        return typeList.ToArray();
      }
    }

    private static DynamicMethod CreateDynamicMethod(
      string name,
      Type returnType,
      Type[] parameterTypes)
    {
      return new DynamicMethod(name, returnType, parameterTypes, true);
    }

    public DynamicMethod CreateInitalizeCollectionMethod(Type proxyType)
    {
      if (this._collectionProperties.Count <= 0)
        return (DynamicMethod) null;
      DynamicMethod dynamicMethod = IPocoImplementor.CreateDynamicMethod(proxyType.Name + "_InitializeEntityCollections", typeof (IEntityWrapper), new Type[1]
      {
        typeof (IEntityWrapper)
      });
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      ilGenerator.DeclareLocal(proxyType);
      ilGenerator.DeclareLocal(typeof (RelationshipManager));
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Callvirt, IPocoImplementor.GetEntityMethod);
      ilGenerator.Emit(OpCodes.Castclass, proxyType);
      ilGenerator.Emit(OpCodes.Stloc_0);
      ilGenerator.Emit(OpCodes.Ldloc_0);
      ilGenerator.Emit(OpCodes.Callvirt, IPocoImplementor.GetRelationshipManagerMethod);
      ilGenerator.Emit(OpCodes.Stloc_1);
      foreach (KeyValuePair<NavigationProperty, PropertyInfo> collectionProperty in this._collectionProperties)
      {
        MethodInfo meth = IPocoImplementor.GetRelatedCollectionMethod.MakeGenericMethod(EntityUtil.GetCollectionElementType(collectionProperty.Value.PropertyType));
        ilGenerator.Emit(OpCodes.Ldloc_0);
        ilGenerator.Emit(OpCodes.Ldloc_1);
        ilGenerator.Emit(OpCodes.Ldstr, collectionProperty.Key.RelationshipType.FullName);
        ilGenerator.Emit(OpCodes.Ldstr, collectionProperty.Key.ToEndMember.Name);
        ilGenerator.Emit(OpCodes.Callvirt, meth);
        ilGenerator.Emit(OpCodes.Callvirt, collectionProperty.Value.Setter());
      }
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ret);
      return dynamicMethod;
    }

    public bool CanProxyMember(EdmMember member)
    {
      if (!this._scalarMembers.Contains(member))
        return this._relationshipMembers.Contains(member);
      return true;
    }

    public bool EmitMember(
      TypeBuilder typeBuilder,
      EdmMember member,
      PropertyBuilder propertyBuilder,
      PropertyInfo baseProperty,
      BaseProxyImplementor baseImplementor)
    {
      if (this._scalarMembers.Contains(member))
      {
        bool isKeyMember = this._ospaceEntityType.KeyMembers.Contains(member.Identity);
        this.EmitScalarSetter(typeBuilder, propertyBuilder, baseProperty, isKeyMember);
        return true;
      }
      if (!this._relationshipMembers.Contains(member))
        return false;
      NavigationProperty navProperty = member as NavigationProperty;
      if (navProperty.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
        this.EmitCollectionProperty(typeBuilder, propertyBuilder, baseProperty, navProperty);
      else
        this.EmitReferenceProperty(typeBuilder, propertyBuilder, baseProperty, navProperty);
      baseImplementor.AddBasePropertySetter(baseProperty);
      return true;
    }

    private void EmitScalarSetter(
      TypeBuilder typeBuilder,
      PropertyBuilder propertyBuilder,
      PropertyInfo baseProperty,
      bool isKeyMember)
    {
      MethodInfo meth1 = baseProperty.Setter();
      MethodAttributes methodAttributes = meth1.Attributes & MethodAttributes.MemberAccessMask;
      MethodBuilder mdBuilder = typeBuilder.DefineMethod("set_" + baseProperty.Name, methodAttributes | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName, (Type) null, new Type[1]
      {
        baseProperty.PropertyType
      });
      ILGenerator ilGenerator = mdBuilder.GetILGenerator();
      Label label = ilGenerator.DefineLabel();
      if (isKeyMember)
      {
        MethodInfo meth2 = baseProperty.Getter();
        if (meth2 != (MethodInfo) null)
        {
          Type propertyType = baseProperty.PropertyType;
          if (propertyType == typeof (int) || propertyType == typeof (short) || (propertyType == typeof (long) || propertyType == typeof (bool)) || (propertyType == typeof (byte) || propertyType == typeof (uint) || (propertyType == typeof (ulong) || propertyType == typeof (float))) || (propertyType == typeof (double) || propertyType.IsEnum()))
          {
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, meth2);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Beq_S, label);
          }
          else if (propertyType == typeof (byte[]))
          {
            ilGenerator.Emit(OpCodes.Ldsfld, (FieldInfo) this._compareByteArraysField);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, meth2);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Callvirt, IPocoImplementor.FuncInvokeMethod);
            ilGenerator.Emit(OpCodes.Brtrue_S, label);
          }
          else
          {
            MethodInfo declaredMethod = propertyType.GetDeclaredMethod("op_Inequality", propertyType, propertyType);
            if (declaredMethod != (MethodInfo) null)
            {
              ilGenerator.Emit(OpCodes.Ldarg_0);
              ilGenerator.Emit(OpCodes.Call, meth2);
              ilGenerator.Emit(OpCodes.Ldarg_1);
              ilGenerator.Emit(OpCodes.Call, declaredMethod);
              ilGenerator.Emit(OpCodes.Brfalse_S, label);
            }
            else
            {
              ilGenerator.Emit(OpCodes.Ldarg_0);
              ilGenerator.Emit(OpCodes.Call, meth2);
              if (propertyType.IsValueType())
                ilGenerator.Emit(OpCodes.Box, propertyType);
              ilGenerator.Emit(OpCodes.Ldarg_1);
              if (propertyType.IsValueType())
                ilGenerator.Emit(OpCodes.Box, propertyType);
              ilGenerator.Emit(OpCodes.Call, IPocoImplementor.ObjectEqualsMethod);
              ilGenerator.Emit(OpCodes.Brtrue_S, label);
            }
          }
        }
      }
      ilGenerator.BeginExceptionBlock();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldstr, baseProperty.Name);
      ilGenerator.Emit(OpCodes.Call, (MethodInfo) this._entityMemberChanging);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Call, meth1);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldstr, baseProperty.Name);
      ilGenerator.Emit(OpCodes.Call, (MethodInfo) this._entityMemberChanged);
      ilGenerator.BeginFinallyBlock();
      ilGenerator.Emit(OpCodes.Ldsfld, (FieldInfo) this._resetFKSetterFlagField);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Callvirt, IPocoImplementor.InvokeMethod);
      ilGenerator.EndExceptionBlock();
      ilGenerator.MarkLabel(label);
      ilGenerator.Emit(OpCodes.Ret);
      propertyBuilder.SetSetMethod(mdBuilder);
    }

    private void EmitReferenceProperty(
      TypeBuilder typeBuilder,
      PropertyBuilder propertyBuilder,
      PropertyInfo baseProperty,
      NavigationProperty navProperty)
    {
      MethodAttributes methodAttributes = baseProperty.Setter().Attributes & MethodAttributes.MemberAccessMask;
      MethodInfo meth = IPocoImplementor.GetRelatedReferenceMethod.MakeGenericMethod(baseProperty.PropertyType);
      MethodInfo onlyDeclaredMethod = typeof (EntityReference<>).MakeGenericType(baseProperty.PropertyType).GetOnlyDeclaredMethod("set_Value");
      MethodBuilder mdBuilder = typeBuilder.DefineMethod("set_" + baseProperty.Name, methodAttributes | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName, (Type) null, new Type[1]
      {
        baseProperty.PropertyType
      });
      ILGenerator ilGenerator = mdBuilder.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Callvirt, (MethodInfo) this._getRelationshipManager);
      ilGenerator.Emit(OpCodes.Ldstr, navProperty.RelationshipType.FullName);
      ilGenerator.Emit(OpCodes.Ldstr, navProperty.ToEndMember.Name);
      ilGenerator.Emit(OpCodes.Callvirt, meth);
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Callvirt, onlyDeclaredMethod);
      ilGenerator.Emit(OpCodes.Ret);
      propertyBuilder.SetSetMethod(mdBuilder);
      this._referenceProperties.Add(new KeyValuePair<NavigationProperty, PropertyInfo>(navProperty, baseProperty));
    }

    private void EmitCollectionProperty(
      TypeBuilder typeBuilder,
      PropertyBuilder propertyBuilder,
      PropertyInfo baseProperty,
      NavigationProperty navProperty)
    {
      MethodAttributes methodAttributes = baseProperty.Setter().Attributes & MethodAttributes.MemberAccessMask;
      string str = Strings.EntityProxyTypeInfo_CannotSetEntityCollectionProperty((object) propertyBuilder.Name, (object) typeBuilder.Name);
      MethodBuilder mdBuilder = typeBuilder.DefineMethod("set_" + baseProperty.Name, methodAttributes | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName, (Type) null, new Type[1]
      {
        baseProperty.PropertyType
      });
      ILGenerator ilGenerator = mdBuilder.GetILGenerator();
      Label label = ilGenerator.DefineLabel();
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, (MethodInfo) this._getRelationshipManager);
      ilGenerator.Emit(OpCodes.Ldstr, navProperty.RelationshipType.FullName);
      ilGenerator.Emit(OpCodes.Ldstr, navProperty.ToEndMember.Name);
      ilGenerator.Emit(OpCodes.Callvirt, IPocoImplementor.GetRelatedEndMethod);
      ilGenerator.Emit(OpCodes.Beq_S, label);
      ilGenerator.Emit(OpCodes.Ldstr, str);
      ilGenerator.Emit(OpCodes.Newobj, IPocoImplementor._invalidOperationConstructorMethod);
      ilGenerator.Emit(OpCodes.Throw);
      ilGenerator.MarkLabel(label);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Call, baseProperty.Setter());
      ilGenerator.Emit(OpCodes.Ret);
      propertyBuilder.SetSetMethod(mdBuilder);
      this._collectionProperties.Add(new KeyValuePair<NavigationProperty, PropertyInfo>(navProperty, baseProperty));
    }

    private void ImplementIEntityWithChangeTracker(
      TypeBuilder typeBuilder,
      Action<FieldBuilder, bool> registerField)
    {
      this._changeTrackerField = typeBuilder.DefineField("_changeTracker", typeof (IEntityChangeTracker), FieldAttributes.Private);
      registerField(this._changeTrackerField, false);
      this._entityMemberChanging = typeBuilder.DefineMethod("EntityMemberChanging", MethodAttributes.Private | MethodAttributes.HideBySig, typeof (void), new Type[1]
      {
        typeof (string)
      });
      ILGenerator ilGenerator1 = this._entityMemberChanging.GetILGenerator();
      Label label1 = ilGenerator1.DefineLabel();
      ilGenerator1.Emit(OpCodes.Ldarg_0);
      ilGenerator1.Emit(OpCodes.Ldfld, (FieldInfo) this._changeTrackerField);
      ilGenerator1.Emit(OpCodes.Brfalse_S, label1);
      ilGenerator1.Emit(OpCodes.Ldarg_0);
      ilGenerator1.Emit(OpCodes.Ldfld, (FieldInfo) this._changeTrackerField);
      ilGenerator1.Emit(OpCodes.Ldarg_1);
      ilGenerator1.Emit(OpCodes.Callvirt, IPocoImplementor.EntityMemberChangingMethod);
      ilGenerator1.MarkLabel(label1);
      ilGenerator1.Emit(OpCodes.Ret);
      this._entityMemberChanged = typeBuilder.DefineMethod("EntityMemberChanged", MethodAttributes.Private | MethodAttributes.HideBySig, typeof (void), new Type[1]
      {
        typeof (string)
      });
      ILGenerator ilGenerator2 = this._entityMemberChanged.GetILGenerator();
      Label label2 = ilGenerator2.DefineLabel();
      ilGenerator2.Emit(OpCodes.Ldarg_0);
      ilGenerator2.Emit(OpCodes.Ldfld, (FieldInfo) this._changeTrackerField);
      ilGenerator2.Emit(OpCodes.Brfalse_S, label2);
      ilGenerator2.Emit(OpCodes.Ldarg_0);
      ilGenerator2.Emit(OpCodes.Ldfld, (FieldInfo) this._changeTrackerField);
      ilGenerator2.Emit(OpCodes.Ldarg_1);
      ilGenerator2.Emit(OpCodes.Callvirt, IPocoImplementor.EntityMemberChangedMethod);
      ilGenerator2.MarkLabel(label2);
      ilGenerator2.Emit(OpCodes.Ret);
      MethodBuilder methodBuilder = typeBuilder.DefineMethod("IEntityWithChangeTracker.SetChangeTracker", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, typeof (void), new Type[1]
      {
        typeof (IEntityChangeTracker)
      });
      ILGenerator ilGenerator3 = methodBuilder.GetILGenerator();
      ilGenerator3.Emit(OpCodes.Ldarg_0);
      ilGenerator3.Emit(OpCodes.Ldarg_1);
      ilGenerator3.Emit(OpCodes.Stfld, (FieldInfo) this._changeTrackerField);
      ilGenerator3.Emit(OpCodes.Ret);
      typeBuilder.DefineMethodOverride((MethodInfo) methodBuilder, IPocoImplementor.SetChangeTrackerMethod);
    }

    private void ImplementIEntityWithRelationships(
      TypeBuilder typeBuilder,
      Action<FieldBuilder, bool> registerField)
    {
      this._relationshipManagerField = typeBuilder.DefineField("_relationshipManager", typeof (RelationshipManager), FieldAttributes.Private);
      registerField(this._relationshipManagerField, true);
      PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("RelationshipManager", System.Reflection.PropertyAttributes.None, typeof (RelationshipManager), Type.EmptyTypes);
      this._getRelationshipManager = typeBuilder.DefineMethod("IEntityWithRelationships.get_RelationshipManager", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.SpecialName, typeof (RelationshipManager), Type.EmptyTypes);
      ILGenerator ilGenerator = this._getRelationshipManager.GetILGenerator();
      Label label = ilGenerator.DefineLabel();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldfld, (FieldInfo) this._relationshipManagerField);
      ilGenerator.Emit(OpCodes.Brtrue_S, label);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, IPocoImplementor.CreateRelationshipManagerMethod);
      ilGenerator.Emit(OpCodes.Stfld, (FieldInfo) this._relationshipManagerField);
      ilGenerator.MarkLabel(label);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldfld, (FieldInfo) this._relationshipManagerField);
      ilGenerator.Emit(OpCodes.Ret);
      propertyBuilder.SetGetMethod(this._getRelationshipManager);
      typeBuilder.DefineMethodOverride((MethodInfo) this._getRelationshipManager, IPocoImplementor.GetRelationshipManagerMethod);
    }
  }
}
