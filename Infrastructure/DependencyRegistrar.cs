using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using NopBrasil.Plugin.Payments.PayU.Controllers;
using NopBrasil.Plugin.Payments.PayU.Services;

namespace NopBrasil.Plugin.Payments.PayU.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {
            builder.RegisterType<PaymentPayUController>().AsSelf();
            builder.RegisterType<PaymentPayUService>().As<IPaymentPayUService>().InstancePerDependency();
        }

        public int Order => 2;
    }
}
