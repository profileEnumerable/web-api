// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Validation.DbValidationError
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Validation
{
  /// <summary>
  /// Validation error. Can be either entity or property level validation error.
  /// </summary>
  [Serializable]
  public class DbValidationError
  {
    private readonly string _propertyName;
    private readonly string _errorMessage;

    /// <summary>
    /// Creates an instance of <see cref="T:System.Data.Entity.Validation.DbValidationError" />.
    /// </summary>
    /// <param name="propertyName"> Name of the invalid property. Can be null. </param>
    /// <param name="errorMessage"> Validation error message. Can be null. </param>
    public DbValidationError(string propertyName, string errorMessage)
    {
      this._propertyName = propertyName;
      this._errorMessage = errorMessage;
    }

    /// <summary>Gets name of the invalid property.</summary>
    public string PropertyName
    {
      get
      {
        return this._propertyName;
      }
    }

    /// <summary>Gets validation error message.</summary>
    public string ErrorMessage
    {
      get
      {
        return this._errorMessage;
      }
    }
  }
}
