// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DbModelBuilderVersionAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity
{
  /// <summary>
  /// This attribute can be applied to a class derived from <see cref="T:System.Data.Entity.DbContext" /> to set which
  /// version of the DbContext and <see cref="T:System.Data.Entity.DbModelBuilder" /> conventions should be used when building
  /// a model from code--also known as "Code First". See the <see cref="T:System.Data.Entity.DbModelBuilderVersion" />
  /// enumeration for details about DbModelBuilder versions.
  /// </summary>
  /// <remarks>
  /// If the attribute is missing from DbContextthen DbContext will always use the latest
  /// version of the conventions.  This is equivalent to using DbModelBuilderVersion.Latest.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class DbModelBuilderVersionAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.DbModelBuilderVersionAttribute" /> class.
    /// </summary>
    /// <param name="version">
    /// The <see cref="T:System.Data.Entity.DbModelBuilder" /> conventions version to use.
    /// </param>
    public DbModelBuilderVersionAttribute(DbModelBuilderVersion version)
    {
      if (!Enum.IsDefined(typeof (DbModelBuilderVersion), (object) version))
        throw new ArgumentOutOfRangeException(nameof (version));
      this.Version = version;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.DbModelBuilder" /> conventions version.
    /// </summary>
    /// <value>
    /// The <see cref="T:System.Data.Entity.DbModelBuilder" /> conventions version.
    /// </value>
    public DbModelBuilderVersion Version { get; private set; }
  }
}
