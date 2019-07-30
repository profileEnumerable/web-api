// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.FunctionCommandText
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class FunctionCommandText : SchemaElement
  {
    private string _commandText;

    public FunctionCommandText(Function parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    public string CommandText
    {
      get
      {
        return this._commandText;
      }
    }

    protected override bool HandleText(XmlReader reader)
    {
      this._commandText = reader.Value;
      return true;
    }

    internal override void Validate()
    {
      base.Validate();
      if (!string.IsNullOrEmpty(this._commandText))
        return;
      this.AddError(ErrorCode.EmptyCommandText, EdmSchemaErrorSeverity.Error, (object) Strings.EmptyCommandText);
    }
  }
}
