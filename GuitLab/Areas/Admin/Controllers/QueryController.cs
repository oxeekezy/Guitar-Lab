using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitLab.Models.Data;

namespace GuitLab.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QueryController : Controller
    {
        // GET: Admin/Query
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SqlQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("SqlError");
            }

            using (var ctx = new Db())
            using (var cmd = ctx.Database.Connection.CreateCommand())
            {
                ctx.Database.Connection.Open();
                cmd.CommandText = query;
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var model = Read(reader).ToList();
                        return View(model);
                    }
                }
                catch (Exception e)
                {
                    TempData["msg"] = e.Message;
                    return View("SqlError");
                }
            }
        }
        private static IEnumerable<object[]> Read(DbDataReader reader)
        {
            while (reader.Read())
            {
                var values = new List<object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(reader.GetValue(i));
                }
                yield return values.ToArray();
            }
        }
    }
}