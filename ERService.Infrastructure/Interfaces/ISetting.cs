using System;

namespace ERService.Infrastructure.Interfaces
{
    public interface ISetting
    {
        string Category { get; set; }
        string Description { get; set; }
        Guid Id { get; set; }
        string Key { get; set; }
        string Value { get; set; }
        string ValueType { get; set; }
    }
}