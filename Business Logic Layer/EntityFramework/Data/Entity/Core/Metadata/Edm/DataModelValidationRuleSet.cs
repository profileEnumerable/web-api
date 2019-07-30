// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.DataModelValidationRuleSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal abstract class DataModelValidationRuleSet
  {
    private readonly List<DataModelValidationRule> _rules = new List<DataModelValidationRule>();

    protected void AddRule(DataModelValidationRule rule)
    {
      this._rules.Add(rule);
    }

    protected void RemoveRule(DataModelValidationRule rule)
    {
      this._rules.Remove(rule);
    }

    internal IEnumerable<DataModelValidationRule> GetRules(
      MetadataItem itemToValidate)
    {
      return this._rules.Where<DataModelValidationRule>((Func<DataModelValidationRule, bool>) (r => r.ValidatedType.IsInstanceOfType((object) itemToValidate)));
    }
  }
}
