using Invoice.API.Model.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice.API.Model
{
    [Table("invoice")]
    public class InvoiceEntity : BaseEntity
    {
        [Column("value")]
        [Required]
        public double Value { get; set; }

        [Column("customer_id")]
        [Required]
        public long CustomerId { get; set; }

        [Column("initial_date")]
        [Required]
        public DateTime InitialDate { get; set; }

        [Column("final_date")]
        [Required]
        public DateTime FinalDate { get; set; }

    }
}
