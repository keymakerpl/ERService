using System;

namespace ERService.Infrastructure.Helpers
{
    public static class EnumHelper
    {
        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            var type = enumValue.GetType();
            var memberInfo = type.GetMember(enumValue.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }
}
