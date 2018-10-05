using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PerfectMoney.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
        public string PayeeAccount { get; set; }
        public string PayeeName { get; set; }
    }
}