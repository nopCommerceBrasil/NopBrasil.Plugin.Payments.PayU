using Nop.Core.Domain.Customers;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Payments;
using System;
using System.Text;
using Nop.Services.Common;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;

namespace NopBrasil.Plugin.Payments.PayU.Services
{
    public class PaymentPayUService : IPaymentPayUService
    {
        //colocar a moeda utilizadas como configuração
        private const string CURRENCY_CODE = "BRL";

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly CustomerSettings _customerSettings;
        private readonly PayUPaymentSettings _payUPaymentSettings;

        private StringBuilder _strBuilderPost { get; set; }

        public PaymentPayUService(ISettingService settingService, IWebHelper webHelper, CustomerSettings customerSettings, PayUPaymentSettings payUPaymentSettings,
            ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._customerSettings = customerSettings;
            this._payUPaymentSettings = payUPaymentSettings;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._strBuilderPost = new StringBuilder();
        }

        private void InsertParams(string nameParam, object value, bool quotedValue, string inputType = "<input type=\"hidden\"")
        {
            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                if (quotedValue)
                    _strBuilderPost.AppendFormat($"{inputType} name={nameParam} value=\"{value.ToString()}\" >");
                else
                    _strBuilderPost.AppendFormat($@"{inputType} name={nameParam} value={value.ToString()} >");
            }
        }

        private void InsertParam(string param) => _strBuilderPost.Append(param);

        private string GetUrlPayU => @"https://www.bcash.com.br/checkout/pay/";

        private void InitializeStringPost()
        {
            _strBuilderPost.Clear();
            InsertParam("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            InsertParam("<head />");
            InsertParam("<body>");
            InsertParam($"<form name=\"bcash\" action=\"{GetUrlPayU}\" method=\"POST\">");
        }

        public string GetStringPost(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            InitializeStringPost();
            InsertParams(@"email_loja", _payUPaymentSettings.EmailPayU, true);

            LoadItems(postProcessPaymentRequest);

            InsertParams(@"tipo_integracao", "PAD", true);
            InsertParams(@"frete", Math.Round(GetConvertedRate(postProcessPaymentRequest.Order.OrderShippingInclTax), 2).ToString().Replace(@",", @"."), true);

            LoadCustomerData(postProcessPaymentRequest);

            InsertParams(@"url_retorno", @_webHelper.GetStoreLocation(), true);
            InsertParams("redirect", "true", true);
            InsertParams("redirect_time", "3", true);
            InsertParams("id_pedido", postProcessPaymentRequest.Order.Id, true);
            InsertParams("email", @postProcessPaymentRequest.Order.Customer.Email, true);
            InsertParams("nome", @postProcessPaymentRequest.Order.Customer.GetFullName(), true);
            InsertJavaScriptPost();

            return _strBuilderPost.ToString();
        }

        private void InsertJavaScriptPost()
        {
            string formID = @"bcash";
            InsertParam("<script language=\"javascript\">");
            InsertParam($"var v{formID} = document.{formID};v{formID}.submit();");
            InsertParam("</script>");
            InsertParam("</body>");
            InsertParam("</html>");
        }

        private void LoadCustomerData(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            if (_customerSettings.GenderEnabled)
                InsertParams(@"sexo", postProcessPaymentRequest.Order.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender), true);

            InsertParams(@"data_nascimento", postProcessPaymentRequest.Order.Customer.GetAttribute<string>(SystemCustomerAttributeNames.DateOfBirth), true);
            InsertParams(@"telefone", postProcessPaymentRequest.Order.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone), true);

            if (postProcessPaymentRequest.Order.ShippingAddress != null)
            {
                InsertParams(@"cep", postProcessPaymentRequest.Order.ShippingAddress.ZipPostalCode, true);
                InsertParams(@"endereco", postProcessPaymentRequest.Order.ShippingAddress.Address1, true);
                InsertParams(@"cidade", postProcessPaymentRequest.Order.ShippingAddress.City, true);
                if (!string.IsNullOrEmpty(postProcessPaymentRequest.Order.ShippingAddress.StateProvince?.Country?.Name))
                    InsertParams(@"estado", postProcessPaymentRequest.Order.ShippingAddress.StateProvince.Country.Name, true);
            }
        }

        private void LoadItems(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            int iCount = 1;
            foreach (var product in postProcessPaymentRequest.Order.OrderItems)
            {
                InsertParams($@"produto_codigo_{iCount}", product.Product.Id, true);
                InsertParams($@"produto_descricao_{iCount}", @product.Product.Name, true);
                InsertParams($@"produto_qtde_{iCount}", product.Quantity, true);
                InsertParams($@"produto_valor_{iCount}", Math.Round(GetConvertedRate(product.UnitPriceInclTax), 2).ToString().Replace(",", "."), true);
                iCount++;
            }
        }

        private decimal GetConvertedRate(decimal rate)
        {
            var usedCurrency = _currencyService.GetCurrencyByCode(CURRENCY_CODE);
            if (usedCurrency == null)
                throw new NopException($"PayU payment service. Could not load \"{CURRENCY_CODE}\" currency");

            if (usedCurrency.CurrencyCode == _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode)
                return rate;
            else
                return _currencyService.ConvertFromPrimaryStoreCurrency(rate, usedCurrency);
        }
    }
}