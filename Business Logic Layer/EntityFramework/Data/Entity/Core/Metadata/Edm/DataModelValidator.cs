// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.DataModelValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class DataModelValidator
  {
    public event EventHandler<DataModelErrorEventArgs> OnError;

    public void Validate(EdmModel model, bool validateSyntax)
    {
      EdmModelValidationContext context = new EdmModelValidationContext(model, validateSyntax);
      context.OnError += this.OnError;
      new EdmModelValidationVisitor(context, EdmModelRuleSet.CreateEdmModelRuleSet(model.SchemaVersion, validateSyntax)).Visit(model);
    }
  }
}
