using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using NopBrasil.Plugin.Payments.PayU.Services;
using NopBrasil.Plugin.Payments.PayU.Models;

namespace NopBrasil.Plugin.Payments.PayU.Controllers
{
    public class PaymentPayUController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentPayUService _payUService;
        private readonly PayUPaymentSettings _payUPaymentSettings;

        public PaymentPayUController(ISettingService settingService, IWebHelper webHelper, IPaymentPayUService payUService, PayUPaymentSettings payUPaymentSettings)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._payUService = payUService;
            this._payUPaymentSettings = payUPaymentSettings;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel() { EmailPayU = _payUPaymentSettings.EmailPayU };
            return View(@"~/Plugins/Payments.PayU/Views/PaymentPayU/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            _payUPaymentSettings.EmailPayU = model.EmailPayU;
            _settingService.SaveSetting(_payUPaymentSettings);

            return View(@"~/Plugins/Payments.PayU/Views/PaymentPayU/Configure.cshtml", model);
        }


        [ChildActionOnly]
        public ActionResult PaymentInfo() => View("~/Plugins/Payments.PayU/Views/PaymentPayU/PaymentInfo.cshtml");

        public override IList<string> ValidatePaymentForm(System.Web.Mvc.FormCollection form) => new List<string>();

        public override ProcessPaymentRequest GetPaymentInfo(System.Web.Mvc.FormCollection form) => new ProcessPaymentRequest();
    }
}
