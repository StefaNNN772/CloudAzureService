using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DatabaseRepository.Models;
using DatabaseRepository.Repositories; 

namespace HealthStatusService.Controllers
{
    public class HealthManagerController : Controller
    {
        private HealthCheckRepository _healthCheckRepository;

        public HealthManagerController()
        {
            _healthCheckRepository = new HealthCheckRepository();
        }

        // GET: HealthManager
        public ActionResult Index()
        {
            try
            {
                // Retrieve all health checks from the last 3 hours
                var healthChecks = _healthCheckRepository.RetrieveAllHealthChecks().OrderBy(h => h.Date).ToList();

                // Optional: Log the count for debugging
                System.Diagnostics.Debug.WriteLine($"Retrieved {healthChecks.Count} health checks");

                return View("HealthManager", healthChecks);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use your preferred logging framework)
                System.Diagnostics.Debug.WriteLine($"Error retrieving health checks: {ex.Message}");

                // Return empty list in case of error to prevent page crash
                ViewBag.ErrorMessage = "Unable to retrieve health check data. Please try again later.";
                return View("HealthManager", new List<HealthCheck>());
            }
        }

        // Optional: Action to refresh data via AJAX
        [HttpGet]
        public JsonResult GetHealthCheckData()
        {
            try
            {
                var healthChecks = _healthCheckRepository.RetrieveAllHealthChecks().OrderBy(h => h.Date).ToList();

                var result = new
                {
                    success = true,
                    data = healthChecks.Select(h => new
                    {
                        serviceName = h.ServiceName,
                        status = h.Status,
                        date = h.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                        
                    }).ToList()
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Optional: Action to manually add a health check (for testing)
        [HttpPost]
        public JsonResult AddHealthCheck(string serviceName, string status)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(status))
                {
                    return Json(new { success = false, message = "ServiceName and Status are required" });
                }

                if (status != "ok" && status != "not_ok")
                {
                    return Json(new { success = false, message = "Status must be 'ok' or 'not_ok'" });
                }

                var healthCheck = new HealthCheck(Guid.NewGuid().ToString())
                {
                    ServiceName = serviceName,
                    Status = status,
                    Date = DateTime.UtcNow
                };

                _healthCheckRepository.AddHealthCheck(healthCheck);

                return Json(new { success = true, message = "Health check added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose repository if it implements IDisposable
                if (_healthCheckRepository is IDisposable disposableRepo)
                {
                    disposableRepo.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}