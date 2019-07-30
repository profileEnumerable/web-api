// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmModelValidationVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Edm;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class EdmModelValidationVisitor : EdmModelVisitor
  {
    private readonly HashSet<MetadataItem> _visitedItems = new HashSet<MetadataItem>();
    private readonly EdmModelValidationContext _context;
    private readonly EdmModelRuleSet _ruleSet;

    internal EdmModelValidationVisitor(EdmModelValidationContext context, EdmModelRuleSet ruleSet)
    {
      this._context = context;
      this._ruleSet = ruleSet;
    }

    protected internal override void VisitMetadataItem(MetadataItem item)
    {
      if (!this._visitedItems.Add(item))
        return;
      this.EvaluateItem(item);
    }

    private void EvaluateItem(MetadataItem item)
    {
      foreach (DataModelValidationRule rule in this._ruleSet.GetRules(item))
        rule.Evaluate(this._context, item);
    }

    internal void Visit(EdmModel model)
    {
      this.EvaluateItem((MetadataItem) model);
      this.VisitEdmModel(model);
    }
  }
}
