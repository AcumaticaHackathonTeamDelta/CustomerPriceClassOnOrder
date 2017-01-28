using System;
using System.Web.UI;
using Autofac;
using PX.Common.Services;
using PX.Data.Services;
using PX.Data;
using PX.Data.RelativeDates;
using PX.Data.Wiki.Parser;
using PX.Objects.SM;
using PX.Reports.Services;

public class ServiceRegistration : Module
{
	protected override void Load(ContainerBuilder builder)
	{
        //Filter variables
        builder
            .RegisterType<FinancialPeriodManager>()
            .As<IFinancialPeriodManager>();

        builder
            .RegisterType<TodayBusinessDate>()
            .As<ITodayUtc>();

        //Services
        builder
            .RegisterType<LicenseServiceStub>()
            .As<ILicenseService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<ReportServiceStub>()
            .As<IReportService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<SessionContextService>()
            .As<ISessionContextService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<FeatureService>()
            .As<IFeatureService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<SnapshotServiceStub>()
            .As<ISnapshotService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<StorageServiceStub>()
            .As<IStorageService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<UserService>()
            .As<IUserService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<LicenseWarningService>()
            .As<ILicenseWarningService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<VersionService>()
            .As<IVersionService>()
            .PreserveExistingDefaults();

        builder
            .RegisterType<CompanyService>()
            .As<ICompanyService>()
            .PreserveExistingDefaults();

        builder
            .RegisterInstance<Func<Page, ISettings>>(PXWikiSettingsRelative.GetSettings)
            .Named<Func<Page, ISettings>>("Relative");
	}
}
