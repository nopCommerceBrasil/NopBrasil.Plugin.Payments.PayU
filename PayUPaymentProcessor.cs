using Nop.Core.Domain.Payments;
using Nop.Services.Logging;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Core.Plugins;
using NopBrasil.Plugin.Payments.PayU.Controllers;
using NopBrasil.Plugin.Payments.PayU.Services;

namespace NopBrasil.Plugin.Payments.PayU
{
    public class PayUPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IPaymentPayUService _payUService;

        public PayUPaymentProcessor(ILogger logger, ISettingService settingService, IPaymentPayUService payUService)
        {
            this._logger = logger;
            this._settingService = settingService;
            this._payUService = payUService;
        }

        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.EmailAdmin.PayU", "Email castrado no PayU");
            this.AddOrUpdatePluginLocaleResource("NopBrasil.Plugins.Payments.PayU.Fields.Redirection", "Você será redirecionado para a pagina do PayU.");
            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<PayUPaymentSettings>();
            this.DeletePluginLocaleResource("Plugins.Payments.EmailAdmin.PayU");
            this.DeletePluginLocaleResource("NopBrasil.Plugins.Payments.PayU.Fields.Redirection");
            base.Uninstall();
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.NewPaymentStatus = PaymentStatus.Pending;
            return processPaymentResult;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write(_payUService.GetStringPost(postProcessPaymentRequest));
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
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

        public void GetConfigurationRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayU";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "NopBrasil.Plugin.Payments.PayU.Controllers" },
                { "area", null }
            };
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPayU";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "NopBrasil.Plugin.Payments.PayU.Controllers" },
                { "area", null }
            };
        }

        public Type GetControllerType() => typeof(PaymentPayUController);

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool HidePaymentMethod(IList<Nop.Core.Domain.Orders.ShoppingCartItem> cart) => false;

        public bool SkipPaymentInfo => false;

        public string PaymentMethodDescription => "PayU";
    }
}
