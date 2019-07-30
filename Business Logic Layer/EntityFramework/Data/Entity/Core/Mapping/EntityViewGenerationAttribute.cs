// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.EntityViewGenerationAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Attribute to mark the assemblies that contain the generated views type.
  /// </summary>
  [Obsolete("The mechanism to provide pre-generated views has changed. Implement a class that derives from System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCache and has a parameterless constructor, then associate it with a type that derives from DbContext or ObjectContext by using System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCacheTypeAttribute.", true)]
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public sealed class EntityViewGenerationAttribute : Attribute
  {
    private readonly Type m_viewGenType;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Mapping.EntityViewGenerationAttribute" /> class.
    /// </summary>
    /// <param name="viewGenerationType">The view type.</param>
    public EntityViewGenerationAttribute(Type viewGenerationType)
    {
      Check.NotNull<Type>(viewGenerationType, nameof (viewGenerationType));
      this.m_viewGenType = viewGenerationType;
    }

    /// <summary>Gets the T:System.Type of the view.</summary>
    /// <returns>The T:System.Type of the view.</returns>
    public Type ViewGenerationType
    {
      get
      {
        return this.m_viewGenType;
      }
    }
  }
}
