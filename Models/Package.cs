using Dapper;
using System;

namespace Models
{
    [Table("Packages")]
    public class Package
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public int UserId { get; set; }
    }
}
