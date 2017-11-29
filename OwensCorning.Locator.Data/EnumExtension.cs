using System;
using System.Data;

namespace OwensCorning.Locator.Data
{
    public static class EnumExtension
    {
        public static bool Includes(this LocatorBusinessTypes type, LocatorBusinessTypes value)
        {
            return (type & value) == value;
        }

        public static bool Includes(this ContractorPrograms type, ContractorPrograms value)
        {
            return (type & value) == value;
        }
    }
}