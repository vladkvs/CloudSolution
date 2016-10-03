using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cloud.Core;
using Cloud.Core.Factory;
using Cloud.Web.Models;

namespace Cloud.Web.Controllers
{
    public class MailRuController : Controller
    {
        // GET: MailRu
        public async Task<ActionResult> Index()
        {
            CloudViewModel model = null;

            Creator creator = new MailRuCreator();
            ICloud mailRuCloud = creator.CreateCloud();

            if (mailRuCloud != null)
            {
                string token = await mailRuCloud.GetServiceToken();
                model = new CloudViewModel
                {
                    Folders = mailRuCloud.GetFolderList(""),
                    Files = mailRuCloud.GetFileList("")
                };
                ViewBag.Token = token;
            }

            return View(model);
        }

        // GET: MailRu/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MailRu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MailRu/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MailRu/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MailRu/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MailRu/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MailRu/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
