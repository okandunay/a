using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Port.Entities.Entities
{
    [Table("Port_UserDetail")]
    // ReSharper disable once InconsistentNaming
    public class Port_UserDetail
    {

        [Key, ForeignKey("Port_User")]
        [Column(Order = 0)]
        // ReSharper disable once InconsistentNaming
        public Int64 pud_Id { get; set; }
        // ReSharper disable once InconsistentNaming
        public virtual Port_User Port_User { get; set; }
    }
}
