using Nop.Services.Payments;

namespace NopBrasil.Plugin.Payments.PayU.Services
{
    public interface IPaymentPayUService
    {
        string GetStringPost(PostProcessPaymentRequest postProcessPaymentRequest);
    }
}
