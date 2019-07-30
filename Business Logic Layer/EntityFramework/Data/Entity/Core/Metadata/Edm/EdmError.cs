// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmError
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// This class encapsulates the error information for a generic EDM error.
  /// </summary>
  [Serializable]
  public abstract class EdmError
  {
    private readonly string _message;

    internal EdmError(string message)
    {
      Check.NotEmpty(message, nameof (message));
      this._message = message;
    }

    /// <summary>Gets the error message.</summary>
    /// <returns>The error message.</returns>
    public string Message
    {
      get
      {
        return this._message;
      }
    }
  }
}
