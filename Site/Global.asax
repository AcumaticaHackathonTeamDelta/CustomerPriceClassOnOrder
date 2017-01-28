<%@ Application Language="C#" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.Runtime.InteropServices" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="PX.Common" %>
<%@ Import Namespace="PX.Data" %>
<%@ Import Namespace="PX.Data.Maintenance" %>
<%@ Import Namespace="PX.Data.Update" %>
<%@ Import Namespace="PX.SM" %>
<%@ Import Namespace="PX.Web.UI" %>

<script RunAt="server">
    void Application_Start(object sender, EventArgs e)
    {
        PXFirstChanceExceptionLogger.Initialise();
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        AzureConfigurationManager.PatchWebConfig();

        if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Bin/App_Subcode_Caches.dll")))
            throw new ApplicationException("Remove App_Subcode_Caches.dll from Website/Bin directory.");

        Customization.WebsiteEntryPoints.InitCustomizationResourceProvider();
        AssemblyResourceProvider.MergeAssemblyResourcesIntoWebsite<PX.Web.Controls.PXResPanelEditor>();
        AssemblyResourceProvider.MergeAssemblyResourcesIntoWebsite<PX.Web.UI.PXResPanelBase>();

        CompositionRoot.CreateContainer();

        try {
            PXCheckDBResult checkResult;
            if (PXUpdateHelper.NeedsUpdate(out checkResult)) {
                PXUpdateHelper.Update(false, checkResult);
                return;
            } } catch { }

        Initialization.ProcessApplication();
        ScheduleProcessor.Initialize();

        PXPage.CompileAllPagesAsync();

        PXDBSlotsCleanup.Instance.Start();
        PXPerformanceMonitor.Init();
    }

    void Application_BeginRequest(object sender, EventArgs e)
    {
        if (_requestLatency > 0)
            Thread.Sleep(_requestLatency);

        if (!Request.IsSecureConnection && PX.Common.AzureConfigurationManager.IsAzure && Request.Headers["X-Host"] == null)
        {
            string url = String.Format("https://{0}{1}", Request.Headers["Host"], VirtualPathUtility.ToAbsolute("~/main.aspx"));
            Response.Redirect(url, true);
            return;
        }

        Initialization.ProcessRequest();
        Customization.WebsiteEntryPoints.ApplicationBeginRequest();

        PXPerformanceMonitor.BeginRequest(Request);

    }

    protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
    {
        if (Request.IsAuthenticated && HttpContext.Current != null)
        {
            String cpny = HttpContext.Current.Request.QueryString["cpid"];
            // executes request on behalf of a different company
            Int32 cpid;
            if (!String.IsNullOrEmpty(cpny) && Int32.TryParse(cpny, out cpid))
                PX.Data.PXDatabase.InitializeRequest(cpid);
        }
        if (!Request.IsAuthenticated && Request.HttpMethod == "POST" && Request.Form["__CALLBACKID"] != null)
            throw new PX.Data.PXNotLoggedInException();
    }

    void clearAccessProvider()
    {
        PXAccess.Clear();
    }

    protected void Application_PostAuthorizeRequest()
    {
    }

    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        PXSessionContextFactory.BeginRequest();
        if (WebConfig.IsClusterEnabled)
        {
            bool retry = false;
            try
            {
                PXDatabase.Subscribe<PXDatabaseProviderBase.ResetAllTables>(clearAccessProvider);
                PXDatabase.SelectTimeStamp();
            }
            catch (PXNotLoggedInException)
            {
                retry = true;
            }
            catch (PXUndefinedCompanyException)
            {
                retry = true;
            }
            finally
            {
                PXDatabase.UnSubscribe<PXDatabaseProviderBase.ResetAllTables>(clearAccessProvider);
            }
            if (retry)
            {
                try
                {
                    PXDatabase.Subscribe<PXDatabaseProviderBase.ResetAllTables>(clearAccessProvider);
                    using (new PXLoginScope(PXDatabase.Companies.Length > 0 ? "admin@" + PXDatabase.Companies[0] : "admin", PXAccess.GetAdministratorRoles()))
                    {
                        PXDatabase.SelectTimeStamp();
                    }
                }
                catch
                {
                }
                finally
                {
                    PXDatabase.UnSubscribe<PXDatabaseProviderBase.ResetAllTables>(clearAccessProvider);
                }
            }

        }
        PX.Data.PXSessionStateStore.InitExtensions();

        LocaleInfo.SetAllCulture();
        Initialization.ProcessHandler();

        String url = HttpContext.Current.Request.Url.ToString();
        if (!url.Contains("Frames/Maintenance.aspx") && PXContext.PXIdentity.Authenticated
            && !String.IsNullOrEmpty(PXContext.PXIdentity.IdentityName)
            && !String.Equals("anonymous", PXContext.PXIdentity.IdentityName, StringComparison.OrdinalIgnoreCase)
            && !PXContext.PXIdentity.User.IsInRole(PXAccess.GetAdministratorRoles().First())
            && PX.Data.Maintenance.PXSiteLockout.GetStatus() == PXSiteLockout.Status.Locked)
        {
            Redirector.RedirectPage(HttpContext.Current, "~/Frames/Maintenance.aspx");
        }

        HttpApplication app = sender as HttpApplication;
        if (app == null) return;
        Page p = app.Context.Handler as Page;
        if (p == null) return;
        if (p.AppRelativeVirtualPath == PX.Data.PXSiteMap.DefaultFrame) return;

        if (app.Context.User != null &&
            !string.IsNullOrEmpty(PXContext.PXIdentity.IdentityName) &&
            !"anonymous".Equals(PXContext.PXIdentity.IdentityName, StringComparison.OrdinalIgnoreCase))
        {
            Customization.WebsiteEntryPoints.InitPageDesignMode(p);
        }

        //Checking file size
        //if (Request.RequestType == "POST" && Request.Form["FileUniqueKey"] != null)
        //{
        //	if (Request.ContentLength > PX.Data.Process.RequestSizeValidator.MaxUploadSize * 1024)
        //	{
        //		PX.Data.Process.RequestSizeValidator.SetIsSizeValid(Request.Form["FileUniqueKey"], false);
        //		System.Threading.Thread.Sleep(2000);
        //	}
        //	else
        //		PX.Data.Process.RequestSizeValidator.SetIsSizeValid(Request.Form["FileUniqueKey"], true);
        //}
    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        if (User == null)
        {
            var username = WebConfig.GetString("autoLoginUserName", null);
            var password = WebConfig.GetString("autoLoginPassword", null);
            var locale = WebConfig.GetString("autoLoginLocale", null);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(locale))
                {
                    PXLocale[] locales = PXLocalesProvider.GetLocales(username);
                    if (locales != null && locales.Any())
                    {
                        locale = locales[0].Name;
                    }
                    else
                    {
                        locale = Thread.CurrentThread.CurrentCulture.Name; // set system locale for api
                    }
                }

                if (PXLogin.LoginUser(ref username, password))
                {
                    PXLogin.InitUserEnvironment(username, locale);
                }
            }
        }
    }

    void Application_PostRequestHandlerExecute(object sender, EventArgs e)
    {
        try
        {
            if (this.Context.CurrentHandler is PXPage) PXPageCache.SetCookie(this.Context);
        }
        catch (HttpException)
        {
        }
        PXSessionContextFactory.EndRequest();
        PXConnectionList.CheckConnections();
    }

    void Application_EndRequest(object sender, EventArgs e)
    {
        PXLongOperation.FlushState();
        PXPerformanceMonitor.EndRequest(Request);

        if (PXContext.PXIdentity.Authenticated)
        {
            PXTimeZoneInfo timeZone = LocaleInfo.GetTimeZone();
            HttpCookie localeCookie = Response.Cookies["Locale"];
            if (timeZone != null) localeCookie["TimeZone"] = timeZone.Id;
            if (string.IsNullOrEmpty(localeCookie["Culture"]))
                localeCookie["Culture"] = LocaleInfo.GetCulture().Name;

            int? branchId = PXContext.GetBranchID();
            HttpCookie branchCooky = Response.Cookies["UserBranch"];
            string branchObj = branchId == null ? null : branchId.ToString();
            if (branchCooky == null)
            {
                Response.Cookies.Add(new HttpCookie("UserBranch", branchObj));
            }
            else
            {
                branchCooky.Value = branchObj;
            }
        }
    }

    void Application_PreSendRequestHeaders(object sender, EventArgs e)
    {
        PX.Web.UI.PXImages.WriteCacheHeaders(this.Context);
        if (!PX.Translation.ResourceCollectingManager.IsStringCollecting &&
            Request.RawUrl.Contains("QR.axd"))
        {
            AssemblyResourceLoader.WriteResponseHeaders(Context);
        }

        var performanceInfo = PXContext.GetSlot<PXPerformanceInfo>();
        if (performanceInfo != null)
        {
            var path = Request.AppRelativeCurrentExecutionFilePath ?? "";
            if (!path.StartsWith("~/rest/")) // see PerfomanceMonitorFilter for ASP.NET Web API based apis
            {
                PXPerformanceMonitor.MarkRequest(performanceInfo, HttpContext.Current);
            }
        }
    }

    void Application_Error(Object sender, EventArgs e)
    {
        //if we have special header or query string parameter than we should skip this handler. (Debugging purpose)
        if (Request.Headers.Get("ShowError") != null || Request.QueryString["ShowError"] != null) return;

        HttpContext ctx = HttpContext.Current;
        if (string.Compare(ctx.Request.HttpMethod, "GET", true) != 0 &&
            string.Compare(ctx.Request.HttpMethod, "POST", true) != 0)
        {
            return;
        }
        if (Request.IsAuthenticated && PX.Data.PXDatabase.RequiresLogOut)
        {
            ctx.ClearError();
            PX.Export.Authentication.AuthenticationManagerModule.Instance.SignOut();
            PXLogin.RequestInvalidating();
            Session.Abandon();
            Response.Clear();

            if (Request.Form["__CALLBACKID"] != null)
            {
                if (Request.QueryString["PopupPanel"] != null) Response.Write("eClosePopup");
                else PX.Data.Redirector.Refresh(ctx);
                Response.StatusCode = (int)PX.Export.HttpStatusCodes.Forbidden;
                Response.End();
            }
            else
            {
                string retUrl = HttpUtility.UrlEncode(PX.Common.PXUrl.ToAbsoluteUrl("~/Main.aspx"));
                string logUrl = PX.Export.Authentication.FormsAuthenticationModule.LoginUrl;
                string NavigateUrl = string.Format("{0}?ReturnUrl={1}", logUrl, retUrl);
                PX.Data.Redirector.RedirectPage(ctx, NavigateUrl);
            }
            return;
        }

        Exception exception = ctx.Server.GetLastError();
        HttpException serverError = exception as HttpException;
        if (serverError != null && serverError.GetHttpCode() == 404)
        {
            ctx.ClearError();
            if (string.Compare(ctx.Request.HttpMethod, "GET", true) == 0)
                Response.Redirect("~/Frames/Default.aspx");
        }

        // if database locked for update
        if (FindException(typeof(PX.Data.PXUnderMaintenanceException), exception))
        {
            ctx.ClearError();
            PX.Data.Redirector.RedirectPage(ctx, "~/Frames/Maintenance.aspx");
            return;
        }
        // if logged out due to license or disabled user account
        if (FindException(typeof(PX.Data.PXForceLogOutException), exception))
        {
            ctx.ClearError();
            return;
        }

        PX.Data.PXSetupNotEnteredException setPropException = exception.InnerException as PX.Data.PXSetupNotEnteredException;
        if (setPropException == null &&
                exception is HttpUnhandledException &&
                exception.InnerException is System.Reflection.TargetInvocationException &&
                exception.InnerException.InnerException is PX.Data.PXSetupNotEnteredException)
        {
            setPropException = exception.InnerException.InnerException as PX.Data.PXSetupNotEnteredException;
        }

        if ((Request.IsLocal && !PX.Common.AzureConfigurationManager.IsAzure && (setPropException == null))
            || (Request.RawUrl != null && Request.RawUrl.EndsWith("/Frames/Error.aspx")))
            return;

        Guid? exceptionId = null;
        try
        {
            if (exception is HttpUnhandledException)
            {
                exception = exception.InnerException;
                if (exception is System.Reflection.TargetInvocationException)
                {
                    exception = exception.InnerException;
                }
            }
            PX.Data.PXTrace.WriteError(exception);
            PX.Data.PXException pe = exception as PX.Data.PXException;
            if (setPropException != null)
            {
                exceptionId = Guid.NewGuid();
                PXContext.Session.Exception[exceptionId.ToString()] = setPropException;
            }
            else if (pe != null)
            {
                exceptionId = Guid.NewGuid();
                PXContext.Session.Exception[exceptionId.ToString()] = pe;
            }
        }
        catch { }

        if (Request.HttpMethod != "POST" || Request.Form["__CALLBACKID"] == null)
        {
            try
            {
                PX.Data.PXEventLog.WriteGeneralError(exception);
            }
            catch { }

            //we need to clear error on IntegratedPipeline. overwise redirect will not work correctly.
            if (System.Web.HttpRuntime.UsingIntegratedPipeline) ctx.ClearError();

            if (exceptionId != null)
                PX.Data.Redirector.Redirect(ctx, string.Format("~/Frames/Error.aspx?exceptionID={0}", exceptionId.ToString()));
            else
                PX.Data.Redirector.Redirect(ctx, "~/Frames/Error.aspx");
        }
    }


    void Session_End(object sender, EventArgs e)
    {
        string login = PXContext.Session["UserLogin"] as string;
        if (string.IsNullOrEmpty(login))
            return;
        try
        {
            PX.Data.PXLogin.SessionExpired(login, PXContext.Session["IPAddress"] as string, Session.SessionID);
        }
        catch { }
    }

    private static readonly bool _logAppEnd = WebConfig.GetBool("EnableApplicationShutdownLogging", false);

    void Application_End(object sender, EventArgs e)
    {
        if (_logAppEnd)
        {
            HttpRuntime runtime = (HttpRuntime) typeof(System.Web.HttpRuntime).InvokeMember("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
            if (runtime == null)
                return;

            string shutDownMessage = (string) runtime.GetType().InvokeMember("_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);
            string shutDownStack = (string) runtime.GetType().InvokeMember("_shutDownStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);
            ApplicationShutdownReason shutDownReason = (ApplicationShutdownReason)runtime.GetType().InvokeMember("_shutdownReason", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

            if (!EventLog.SourceExists(".NET Runtime"))
                EventLog.CreateEventSource(".NET Runtime", "Application");

            EventLog log = new EventLog {Source = ".NET Runtime"};
            string message = String.Format("\r\n\r\nShutDownMessage='{0}'\r\n\r\nShutDownStack='{1}'\r\n\r\nShutDownReason='{2}'", shutDownMessage, shutDownStack, shutDownReason);
            PXFirstChanceExceptionLogger.LogMessage(message);
            log.WriteEntry(message, EventLogEntryType.Error);
        }

        PXDBSlotsCleanup.Instance.Dispose();
    }

    private static readonly int _requestLatency = RequestLatency;
    private static int RequestLatency
    {
        get
        {
            string config = WebConfigurationManager.AppSettings["RequestLatency"];
            int result;
            int.TryParse(config, out result);
            return result;
        }
    }
    //private System.Reflection.Assembly App_SubCode_Caches(object sender, ResolveEventArgs args)
    //{
    //	if (args.Name != null && args.Name.StartsWith("App_SubCode_Caches."))
    //	{
    //		return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(_ => _.FullName != null && (_.FullName.StartsWith("App_SubCode_Caches,") || _.FullName.StartsWith("App_SubCode_Caches.")));
    //	}
    //	return null;
    //}
    private bool FindException(Type exType, Exception ex)
    {
        if (ex != null)
        {
            if (ex.GetType() == exType) return true;
            if (ex.InnerException != null) return FindException(exType, ex.InnerException);
        }
        return false;
    }
</script>

