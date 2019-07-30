// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.BaseProxyImplementor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class BaseProxyImplementor
  {
    internal static readonly MethodInfo StringEquals = typeof (string).GetDeclaredMethod("op_Equality", typeof (string), typeof (string));
    private static readonly ConstructorInfo _invalidOperationConstructor = typeof (InvalidOperationException).GetDeclaredConstructor();
    private readonly List<PropertyInfo> _baseGetters;
    private readonly List<PropertyInfo> _baseSetters;

    public BaseProxyImplementor()
    {
      this._baseGetters = new List<PropertyInfo>();
      this._baseSetters = new List<PropertyInfo>();
    }

    public List<PropertyInfo> BaseGetters
    {
      get
      {
        return this._baseGetters;
      }
    }

    public List<PropertyInfo> BaseSetters
    {
      get
      {
        return this._baseSetters;
      }
    }

    public void AddBasePropertyGetter(PropertyInfo baseProperty)
    {
      this._baseGetters.Add(baseProperty);
    }

    public void AddBasePropertySetter(PropertyInfo baseProperty)
    {
      this._baseSetters.Add(baseProperty);
    }

    public void Implement(TypeBuilder typeBuilder)
    {
      if (this._baseGetters.Count > 0)
        this.ImplementBaseGetter(typeBuilder);
      if (this._baseSetters.Count <= 0)
        return;
      this.ImplementBaseSetter(typeBuilder);
    }

    private void ImplementBaseGetter(TypeBuilder typeBuilder)
    {
      ILGenerator ilGenerator = typeBuilder.DefineMethod("GetBasePropertyValue", MethodAttributes.Public | MethodAttributes.HideBySig, typeof (object), new Type[1]
      {
        typeof (string)
      }).GetILGenerator();
      Label[] labelArray = new Label[this._baseGetters.Count];
      for (int index = 0; index < this._baseGetters.Count; ++index)
      {
        labelArray[index] = ilGenerator.DefineLabel();
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Ldstr, this._baseGetters[index].Name);
        ilGenerator.Emit(OpCodes.Call, BaseProxyImplementor.StringEquals);
        ilGenerator.Emit(OpCodes.Brfalse_S, labelArray[index]);
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Call, this._baseGetters[index].Getter());
        ilGenerator.Emit(OpCodes.Ret);
        ilGenerator.MarkLabel(labelArray[index]);
      }
      ilGenerator.Emit(OpCodes.Newobj, BaseProxyImplementor._invalidOperationConstructor);
      ilGenerator.Emit(OpCodes.Throw);
    }

    private void ImplementBaseSetter(TypeBuilder typeBuilder)
    {
      ILGenerator ilGenerator = typeBuilder.DefineMethod("SetBasePropertyValue", MethodAttributes.Public | MethodAttributes.HideBySig, typeof (void), new Type[2]
      {
        typeof (string),
        typeof (object)
      }).GetILGenerator();
      Label[] labelArray = new Label[this._baseSetters.Count];
      for (int index = 0; index < this._baseSetters.Count; ++index)
      {
        labelArray[index] = ilGenerator.DefineLabel();
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Ldstr, this._baseSetters[index].Name);
        ilGenerator.Emit(OpCodes.Call, BaseProxyImplementor.StringEquals);
        ilGenerator.Emit(OpCodes.Brfalse_S, labelArray[index]);
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldarg_2);
        ilGenerator.Emit(OpCodes.Castclass, this._baseSetters[index].PropertyType);
        ilGenerator.Emit(OpCodes.Call, this._baseSetters[index].Setter());
        ilGenerator.Emit(OpCodes.Ret);
        ilGenerator.MarkLabel(labelArray[index]);
      }
      ilGenerator.Emit(OpCodes.Newobj, BaseProxyImplementor._invalidOperationConstructor);
      ilGenerator.Emit(OpCodes.Throw);
    }
  }
}
