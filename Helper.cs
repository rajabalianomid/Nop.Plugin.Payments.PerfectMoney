using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.PerfectMoney
{
    public static class Helper
    {
        public static string GetActionName(this Controller controller)
        {
            return controller.ControllerContext.RouteData.Values["action"].ToString();
        }
        private static String PreparePOSTForm(string url, NameValueCollection data)
        {
            string formID = "PostForm";

            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
            foreach (string key in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\">");
            }
            strForm.Append("</form>");

            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            return strForm.ToString() + strScript.ToString();
        }
        public static string RedirectAndPOST(string destinationUrl, NameValueCollection data)
        {
            return PreparePOSTForm(destinationUrl, data);
        }
        private static String PrepareGetTForm(string url, NameValueCollection data)
        {
            string formID = "PostForm";

            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"GET\">");
            foreach (string key in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\">");
            }
            strForm.Append("</form>");

            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            return strForm.ToString() + strScript.ToString();
        }
        public static string RedirectAndGet(string destinationUrl, NameValueCollection data)
        {
            return PreparePOSTForm(destinationUrl, data);
        }
    }
}
