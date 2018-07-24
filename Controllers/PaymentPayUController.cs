using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using NopBrasil.Plugin.Payments.PayU.Models;
using Nop.Web.Framework;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.Filters;

namespace NopBrasil.Plugin.Payments.PayU.Controllers
{
    [Area(AreaNames.Admin)]
    public class PaymentPayUController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly PayUPaymentSettings _payUPaymentSettings;
        private readonly IPermissionService _permissionService;

        public PaymentPayUController(ISettingService settingService, PayUPaymentSettings payUPaymentSettings, IPermissionService permissionService)
        {
            this._settingService = settingService;
            this._payUPaymentSettings = payUPaymentSettings;
            this._permissionService = permissionService;
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel()
            {
                EmailPayU = _payUPaymentSettings.EmailPayU,
                PaymentMethodDescription = _payUPaymentSettings.PaymentMethodDescription
            };
            return View(@"~/Plugins/Payments.PayU/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            _payUPaymentSettings.EmailPayU = model.EmailPayU;
            _payUPaymentSettings.PaymentMethodDescription = model.PaymentMethodDescription;
            _settingService.SaveSetting(_payUPaymentSettings);

            return View(@"~/Plugins/Payments.PayU/Views/Configure.cshtml", model);
        }
    }
}
