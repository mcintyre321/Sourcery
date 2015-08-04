using System;
using Sourcery.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Sourcery.Web
{
    public class EventStoreController<T> : Controller
    {
        protected readonly ISourcerer<T> Sourcerer;

        public EventStoreController(ISourcerer<T> sourcerer)
        {
            Sourcerer = sourcerer;
        }

        public ActionResult MigrationError()
        {
            return View("Sourcery/MigrationError", EnsureAllowed());
        }

        [Authorize]
        public ActionResult History(int page = 1)
        {
            EnsureAdmin();
            Func<string, CommandBase> deserialize = s => JsonConvert.DeserializeObject<CommandBase>(s, new CustomSerializerSettings());
            using (var session = Sourcerer.EventStore.OpenSession())
            {
                var q = from e in session.Events
                        orderby e.Name descending
                        select Tuple.Create(deserialize(e.Content), e.Content);


                return View("Sourcery/History",
                            Tuple.Create(session.Events.Count(), q.Skip(10*(page - 1)).Take(10)));
            }
        }
 
        [HttpPost]
        public ActionResult Delete(string name)
        {
            EnsureAllowed();
            Sourcerer.EventStore.Delete(name);
            return Redirect("/");
        }

        private UpdateException EnsureAllowed()
        {
            try
            {
                var read = Sourcerer.ReadModel;
                throw new Exception("Can only function when readmodel is null");
            }
            catch (UpdateException ex)
            {
                EnsureAdmin();

                return ex;
            }
        }

        private void EnsureAdmin()
        {
            if(Request.IsLocal) return;
            if (Request.Cookies[adminCookieValue] != null) return;
            throw new SecurityException("You must be running locally or be an EventStore admin to view this page");
        }

        const string adminCookieValue = "ac4372bd-9e42-41aa-93a2-9bee5a1a5aa9";

        public static void WriteAdminCookie(bool persist)
        {
            var httpCookie = new HttpCookie(adminCookieValue, "true"){ };
            if (persist) httpCookie.Expires = DateTime.UtcNow.AddYears(1);
            System.Web.HttpContext.Current.Response.SetCookie(httpCookie);
        }
    }
}
