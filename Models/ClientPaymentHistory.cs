using System.ComponentModel.DataAnnotations.Schema;

namespace TMCC.Models
{
    public class ClientPaymentHistory
    {
        [Column("PaymentId")]
        public Guid PaymentId { get; set; }

        [Column("ClientId")]
        public Guid ClientId { get; set; }

        [Column("ClientName")]
        public string ClientName { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("PaymentDate")]
        public DateTime PaymentDate { get; set; }

        [Column("PaymentMode")]
        public string PaymentMode { get; set; }

        [Column("Remarks")]
        public string Remarks { get; set; }

        [Column("CreatedBy")]
        public string CreatedBy { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}