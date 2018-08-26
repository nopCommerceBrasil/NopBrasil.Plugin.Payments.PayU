using Nop.Core.Domain.Payments;
using Nop.Services.Logging;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Core.Plugins;
using NopBrasil.Plugin.Payments.PayU.Controllers;
using NopBrasil.Plugin.Payments.PayU.Services;
using Microsoft.AspNetCore.Http;
using Nop.Core;

namespace NopBrasil.Plugin.Payments.PayU
{
    public class PayUPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IPaymentPayUService _payUService;
        private readonly PayUPaymentSettings _payUPaymentSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public PayUPaymentProcessor(ILogger logger, ISettingService settingService, IPaymentPayUService payUService, PayUPaymentSettings payUPaymentSettings, 
            IHttpContextAccessor httpContextAccessor, IWebHelper webHelper, ILocalizationService localizationService)
        {
            this._logger = logger;
            this._settingService = settingService;
            this._payUService = payUService;
            this._payUPaymentSettings = payUPaymentSettings;
            this._httpContextAccessor = httpContextAccessor;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
        }

        public override void Install()
        {
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.EmailAdmin.PayU", "Email castrado no PayU");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.MethodDescription.PayU", "Descrição que será exibida no checkout");
            _localizationService.AddOrUpdatePluginLocaleResource("NopBrasil.Plugins.Payments.PayU.Fields.Redirection", "Você será redirecionado para a pagina do PayU.");
            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<PayUPaymentSettings>();
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.EmailAdmin.PayU");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.MethodDescription.PayU");
            _localizationService.DeletePluginLocaleResource("NopBrasil.Plugins.Payments.PayU.Fields.Redirection");
            base.Uninstall();
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var processPaymentResult = new ProcessPaymentResult()
            {
                NewPaymentStatus = PaymentStatus.Pending
            };
            return processPaymentResult;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                _httpContextAccessor.HttpContext.Response.Clear();
                var write = _httpContextAccessor.HttpContext.Response.WriteAsync(_payUService.GetStringPost(postProcessPaymentRequest));
                _httpContextAccessor.HttpContext.Response.Body.FlushAsync();
                _httpContextAccessor.HttpContext.Response.Body.EndWrite(write);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }

        public decimal GetAdditionalHandlingFee(IList<Nop.Core.Domain.Orders.ShoppingCartItem> cart) => 0;

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest) => new CapturePaymentResult();

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest) => new RefundPaymentResult();

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest) => new VoidPaymentResult();

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest) => new ProcessPaymentResult();

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest) => new CancelRecurringPaymentResult();

        public bool CanRePostProcessPayment(Nop.Core.Domain.Orders.Order order) => false;

        public Type GetControllerType() => typeof(PaymentPayUController);

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool HidePaymentMethod(IList<Nop.Core.Domain.Orders.ShoppingCartItem> cart) => false;

        public bool SkipPaymentInfo => false;

        public IList<string> ValidatePaymentForm(IFormCollection form) => new List<string>();

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form) => new ProcessPaymentRequest();

        public string PaymentMethodDescription => _payUPaymentSettings.PaymentMethodDescription;

        public override string GetConfigurationPageUrl() => $"{_webHelper.GetStoreLocation()}Admin/PaymentPayU/Configure";

        public string GetPublicViewComponentName() => "PaymentPayU";
    }
}
