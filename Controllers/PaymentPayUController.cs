using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using NopBrasil.Plugin.Payments.PayU.Models;
using Nop.Web.Framework;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace NopBrasil.Plugin.Payments.PayU.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class PaymentPayUController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly PayUPaymentSettings _payUPaymentSettings;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        public PaymentPayUController(ISettingService settingService, PayUPaymentSettings payUPaymentSettings, IPermissionService permissionService, INotificationService notificationService, ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._payUPaymentSettings = payUPaymentSettings;
            this._permissionService = permissionService;
            this._notificationService = notificationService;
            this._localizationService = localizationService;
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

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
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return View(@"~/Plugins/Payments.PayU/Views/Configure.cshtml", model);
        }
    }
}
