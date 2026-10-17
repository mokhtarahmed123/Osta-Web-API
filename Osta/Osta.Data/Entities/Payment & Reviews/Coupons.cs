using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Payment___Reviews;
using Osta.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Customer
{
    [Index(nameof(Code), IsUnique = true)]
    [Table("Coupons", Schema = "Payment")]
    public class Coupons
    {
        public int Id { get; set; }


        public string Code { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();


        public DiscountTypeEnum DiscountType { get; set; } = DiscountTypeEnum.Percentage;
        public decimal DiscountValue { get; set; }


        public DateOnly StartDate { get; set; }


        public DateOnly EndDate { get; set; }

        public int UsageLimit { get; set; }


        public int UsedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();

    }
}