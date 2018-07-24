using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace NopBrasil.Plugin.Payments.PayU.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.EmailAdmin.PayU")]
        public string EmailPayU { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MethodDescription.PayU")]
        public string PaymentMethodDescription { get; set; }
    }
}
