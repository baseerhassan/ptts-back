using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemUsersAPI.Models
{
    [Table("Lookup_ExVision_Shades")]
    public class LookupExVisionShade
    {
        [Key]
        public int Id { get; set; }
        
        [Column("Shade")]
        [StringLength(50)]
        public string Shade { get; set; }
        
        [Column("Description")]
        [StringLength(500)]
        public string Description { get; set; }
        
        [Column("IsActive")]
        public int? IsActive { get; set; }
    }
}