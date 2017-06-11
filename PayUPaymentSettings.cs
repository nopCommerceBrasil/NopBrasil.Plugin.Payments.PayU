using Nop.Core.Configuration;

namespace NopBrasil.Plugin.Payments.PayU
{
    public class PayUPaymentSettings : ISettings
    {
        public string EmailPayU { get; set; }
        public string PaymentMethodDescription { get; set; }
    }
}
