using Nop.Core.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Payments.PerfectMoney
{
    public class PerfectMoneyPaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether shippable products are required in order to display this payment method during checkout
        /// </summary>
        public bool ShippableProductRequired { get; set; }
        public string PayeeAccount { get; set; }
        public string PayeeName { get; set; }

        public Dictionary<string, string> PayeeAccounts
        {
            get
            {
                var accounts = new Dictionary<string, string>();
                PayeeAccount.Split(';').ToList().ForEach(f =>
                {
                    accounts.Add(f.Split(':')[0], f.Split(':')[1]);
                });
                return accounts;
            }
        }
    }
}
