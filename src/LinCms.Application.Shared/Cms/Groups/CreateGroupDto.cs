﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Cms.Groups
{
    public class CreateGroupDto : UpdateGroupDto, IValidatableObject
    {
        public List<string> Permissions { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Permissions.Count == 0)
            {
                yield return new ValidationResult("请输入auths字段", new List<string> { "Auths" });
            }
        }
    }
}
