using System;

namespace LinCms.Core.Aop
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class|AttributeTargets.Interface)]
    public class UnitOfWorkAttribute : Attribute
    {
        /// <summary>
        /// Is this UOW transactional?
        /// Uses default value if not supplied.
        /// </summary>
        public bool? IsTransactional { get; set; }
        public bool IsDisabled { get; set; }


        public UnitOfWorkAttribute()
        {

        }

        public UnitOfWorkAttribute(bool isTransactional)
        {
            IsTransactional = isTransactional;
        }
    }
}
