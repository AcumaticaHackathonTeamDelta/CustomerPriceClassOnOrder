using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using PX.Data;

namespace PX.Objects.CR.Handlers
{
    public class FileOutlookGetter : IHttpHandler, IRequiresSessionState
    {
        private const string _resourceAddinManifest = "PX.Objects.CR.Handlers.AddinManifest.xml";
        private const string _outputFileName = "OutlookAddinManifest.xml";

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if(!HttpContext.Current.Request.IsSecureConnection)
                 throw new PXSetPropertyException(Messages.OutlookPluginHttps);
            context.Response.Clear();
            context.Response.Cache.SetCacheability(HttpCacheability.Private);
            context.Response.Cache.SetValidUntilExpires(true);
            context.Response.AddHeader("Connection", "Keep-Alive");
            context.Response.BufferOutput = false;
            foreach (string ck in context.Request.Cookies.AllKeys)
                context.Response.Cookies.Add(context.Request.Cookies[ck]);
            context.Response.AddHeader("content-type", "application/octet-stream");
            context.Response.AddHeader("Accept-Ranges", "bytes");

            string text = null;
            using (var fs = Assembly.GetExecutingAssembly().GetManifestResourceStream(_resourceAddinManifest))
            {
                using (var sR = new StreamReader(fs))
                {
                    string address = context.Request.Url.ToString().Replace(context.Request.Url.Segments[context.Request.Url.Segments.Length - 1], "");
                    text = sR.ReadToEnd().Replace("{domain}", address);
                }

            }
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + _outputFileName);
            context.Response.ContentType = "application/xml";
            context.Response.BinaryWrite(buffer);
        }
    }
}
