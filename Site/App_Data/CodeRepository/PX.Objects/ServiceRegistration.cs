using Autofac;
using PX.Data;
using PX.Data.RelativeDates;
using PX.Objects.SM;

namespace PX.Objects
{
    public class ServiceRegistration: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
            .RegisterType<FinancialPeriodManager>()
            .As<IFinancialPeriodManager>();

            builder
                .RegisterType<TodayBusinessDate>()
                .As<ITodayUtc>();
        }
    }
}
