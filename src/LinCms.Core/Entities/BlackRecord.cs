using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Entities
{
    public class BlackRecord : Entity<Guid>, ICreateAduitEntity
    {
        [StringLength(-2)]
        public string Jti { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
