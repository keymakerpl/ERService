using System;

namespace ERService.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InterpreterAttribute : Attribute 
    {
        public string Name { get; set; }
        public string Pattern { get; set; }
    }
}
