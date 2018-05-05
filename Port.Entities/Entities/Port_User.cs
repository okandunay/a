using Port.Entities.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Port.Entities.Entities
{
    [Table("Port_User")]
    // ReSharper disable once InconsistentNaming
    public class Port_User
    {
        public Port_User()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Port_UserDetail = new Port_UserDetail();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        [Display(Name = "User Name")]
        // ReSharper disable once InconsistentNaming
        public Int64 pu_Id { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        // ReSharper disable once InconsistentNaming
        public string pu_FirstName { get; set; }


        [StringLength(50, ErrorMessage = "Last Name name cannot be longer than 50 characters.")]
        // ReSharper disable once InconsistentNaming
        public string pu_LastName { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        // ReSharper disable once InconsistentNaming
        public string pu_Email { get; set; }

        // ReSharper disable once InconsistentNaming
        public string pu_Pasaword { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText="Doğum Tarihini Boş Bıraktınız")]
        // ReSharper disable once InconsistentNaming
        public DateTime? pu_BirthDate { get; set; }

        [Required]
        // ReSharper disable once InconsistentNaming
        public virtual Port_UserDetail Port_UserDetail { get; set; }

        public bool pu_RememberMe { get; set; }

        public Guid responseGuidId { get; set; }

        public ResponseResultEnums status { get; set; }


        public override string ToString()
        {
            return pu_FirstName +  " "  + pu_LastName;
        }    
    }
}
