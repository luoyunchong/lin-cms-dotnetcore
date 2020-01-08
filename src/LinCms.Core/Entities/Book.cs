﻿using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(Name = "book")]
    public class Book : FullAduitEntity
    {
        [Column(DbType = "varchar(30)")]
        public string Author { get; set; } = string.Empty;

        [Column(DbType = "varchar(50)")]
        public string Image { get; set; } = string.Empty;

        [Column(DbType = "varchar(1000)")]
        public string Summary { get; set; } = string.Empty;

        [Column(DbType = "varchar(50)")]
        public string Title { get; set; } = string.Empty;

    }

}
