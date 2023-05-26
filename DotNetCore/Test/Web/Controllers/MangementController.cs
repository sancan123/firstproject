using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class MangementController : Controller
    {
        // GET: MangementController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MangementController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MangementController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MangementController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MangementController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MangementController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MangementController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MangementController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
