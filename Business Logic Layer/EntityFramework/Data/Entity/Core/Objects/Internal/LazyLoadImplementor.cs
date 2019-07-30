// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.LazyLoadImplementor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class LazyLoadImplementor
  {
    private HashSet<EdmMember> _members;

    public LazyLoadImplementor(EntityType ospaceEntityType)
    {
      this.CheckType(ospaceEntityType);
    }

    public IEnumerable<EdmMember> Members
    {
      get
      {
        return (IEnumerable<EdmMember>) this._members;
      }
    }

    private void CheckType(EntityType ospaceEntityType)
    {
      this._members = new HashSet<EdmMember>();
      foreach (EdmMember member in ospaceEntityType.Members)
      {
        PropertyInfo topProperty = ospaceEntityType.ClrType.GetTopProperty(member.Name);
        if (topProperty != (PropertyInfo) null && EntityProxyFactory.CanProxyGetter(topProperty) && LazyLoadBehavior.IsLazyLoadCandidate(ospaceEntityType, member))
          this._members.Add(member);
      }
    }

    public bool CanProxyMember(EdmMember member)
    {
      return this._members.Contains(member);
    }

    public virtual void Implement(TypeBuilder typeBuilder, Action<FieldBuilder, bool> registerField)
    {
      FieldBuilder fieldBuilder = typeBuilder.DefineField("_entityWrapper", typeof (object), FieldAttributes.Public);
      registerField(fieldBuilder, false);
    }

    public bool EmitMember(
      TypeBuilder typeBuilder,
      EdmMember member,
      PropertyBuilder propertyBuilder,
      PropertyInfo baseProperty,
      BaseProxyImplementor baseImplementor)
    {
      if (!this._members.Contains(member))
        return false;
      MethodInfo meth = baseProperty.Getter();
      MethodAttributes methodAttributes = meth.Attributes & MethodAttributes.MemberAccessMask;
      Type type = typeof (Func<,,>).MakeGenericType((Type) typeBuilder, baseProperty.PropertyType, typeof (bool));
      MethodInfo method = TypeBuilder.GetMethod(type, typeof (Func<,,>).GetOnlyDeclaredMethod("Invoke"));
      FieldBuilder fieldBuilder = typeBuilder.DefineField(LazyLoadImplementor.GetInterceptorFieldName(baseProperty.Name), type, FieldAttributes.Private | FieldAttributes.Static);
      MethodBuilder mdBuilder = typeBuilder.DefineMethod("get_" + baseProperty.Name, methodAttributes | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName, baseProperty.PropertyType, Type.EmptyTypes);
      ILGenerator ilGenerator = mdBuilder.GetILGenerator();
      Label label = ilGenerator.DefineLabel();
      ilGenerator.DeclareLocal(baseProperty.PropertyType);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, meth);
      ilGenerator.Emit(OpCodes.Stloc_0);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldfld, (FieldInfo) fieldBuilder);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldloc_0);
      ilGenerator.Emit(OpCodes.Callvirt, method);
      ilGenerator.Emit(OpCodes.Brtrue_S, label);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, meth);
      ilGenerator.Emit(OpCodes.Ret);
      ilGenerator.MarkLabel(label);
      ilGenerator.Emit(OpCodes.Ldloc_0);
      ilGenerator.Emit(OpCodes.Ret);
      propertyBuilder.SetGetMethod(mdBuilder);
      baseImplementor.AddBasePropertyGetter(baseProperty);
      return true;
    }

    internal static string GetInterceptorFieldName(string memberName)
    {
      return "ef_proxy_interceptorFor" + memberName;
    }
  }
}
