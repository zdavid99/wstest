using System;
using System.Collections.Generic;
using System.Text;

namespace OwensCorning.Locator.Data
{
    [Serializable]
    public class Builder : Contractor
    {
        public Builder(LocatorBusinessTypes businessType)
            : base(LocatorResultTypes.Builder, businessType)
        {
            BuilderPrograms = BuilderPrograms.None;
        }

        public BuilderPrograms BuilderPrograms { get; set; }
    }

    [Flags]
    public enum BuilderPrograms
    { 
        //VALUES MUST BE POWERS OF 2
        None = 0,
        IsSystemThinkingBuilder = 1,
        IsQuietZoneBuilder = 2,
        All = IsQuietZoneBuilder | IsSystemThinkingBuilder
    }
}
