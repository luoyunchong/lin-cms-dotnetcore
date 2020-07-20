using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace IGeekfan.Localization.FreeSql.Models
{
    public class LocalCulture
    {
        public LocalCulture()
        {
        }

        public LocalCulture(string name, string displayName, ICollection<LocalResource> resources) : this(name, displayName)
        {
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public LocalCulture(string name, string displayName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        [Column(StringLength = 10)]
        public string Name { get; set; }
        
        [Column(StringLength =64)]
        public string DisplayName { get; set; }
        public virtual ICollection<LocalResource> Resources { get; set; }
    }
}
