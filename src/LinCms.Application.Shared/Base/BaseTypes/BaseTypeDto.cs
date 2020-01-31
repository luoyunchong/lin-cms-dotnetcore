﻿using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Base.BaseTypes
{
    public class BaseTypeDto :Entity
    {
        public string TypeCode { get; set; }
        public string FullName { get; set; }
        public int? SortCode { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
