using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace NopBrasil.Plugin.Payments.PayU.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.EmailAdmin.PayU")]
        public string EmailPayU { get; set; }
    }
}
