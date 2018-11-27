using Dapper;

namespace Models
{
    [Table("Files")]
    public class File
    {
        [Key]
        public int Id { get; set; }
        public string Filename { get; set; }
        public string GeneratedFilename { get; set; }

        public int PackageId { get; set; }
        
    }
}
