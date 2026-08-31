using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Payment___Reviews;
using Osta.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Customer
{
    [Index(nameof(Code), IsUnique = true)]
    [Table("Coupons", Schema = "Payment")]
    public class Coupons
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Code { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();

        [Required]
        public DiscountTypeEnum DiscountType { get; set; } = DiscountTypeEnum.Percentage;

        [Required]
        [Range(0.01, 100000)]
        [Precision(18, 2)]
        public decimal DiscountValue { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UsageLimit { get; set; }

        [Range(0, int.MaxValue)]
        public int UsedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();

    }
}