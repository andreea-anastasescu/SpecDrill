using SpecDrill.Secondary.Ports.AutomationFramework;
using System;

namespace SpecDrill
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FindTargetAttribute : Attribute
    {
        public By? SelectorType { get; }
        public string? SelectorValue { get; }

        public string? PropertyName { get; }
        public FindTargetAttribute(string PropertyName) => (this.PropertyName) = (PropertyName);
        public FindTargetAttribute(By SelectorType, string SelectorValue) => (this.SelectorType, this.SelectorValue) = (SelectorType, SelectorValue);
        
    }
}
