using EmployeeApp.Db;
using EmployeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EmployeeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        /* Create Method*/
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Create(EmployeesData employeesData)
        {
            if (!ModelState.IsValid) 
            {
                return View(employeesData);
            }
            if(employeesData.Photo!=null && employeesData.Photo.Length > 0) 
            {
                try 
                {
                    var filename = Path.GetFileName(employeesData.Photo.FileName);
                    var newfilename = $"{Path.GetFileNameWithoutExtension(filename)}_{Guid.NewGuid()}{Path.GetExtension(filename)}";
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images",newfilename);

                    Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                    using (var filestream= new FileStream(filepath,FileMode.Create)) 
                    {
                        await employeesData.Photo.CopyToAsync(filestream);
                    }
                    employeesData.PhotoPath = $"/images/{newfilename}";
                }
                catch(Exception ex) 
                {
                    ModelState.AddModelError("Error", "Error in saving file : " + ex.Message);
                    return View(employeesData);
                }
            }
            else 
            {
                ModelState.AddModelError("Photo Required", "Please upload photo");
                return View(employeesData);
            }
            _applicationDbContext.Employees.Add(employeesData);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var employeedata = await _applicationDbContext.Employees.ToListAsync(); 
            return View(employeedata);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsView(int id)
        {
            var detailsView = await _applicationDbContext.Employees.FindAsync(id);
            if (detailsView == null)
            {
                return NotFound();
            }
            return View(detailsView);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var editview = await _applicationDbContext.Employees.FindAsync(id);
            if(editview == null) 
            {
                return NotFound();
            }
            return View(editview);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,EmployeesData employeesData) 
        {
            if (!ModelState.IsValid) 
            {
                return View(employeesData);
            }
            if (employeesData.Photo != null && employeesData.Photo.Length > 0)
            {
                try
                {
                    var filename = Path.GetFileName(employeesData.Photo.FileName);
                    var newfilename = $"{Path.GetFileNameWithoutExtension(filename)}_{Guid.NewGuid()}{Path.GetExtension(filename)}";
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images",newfilename);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                    using(var filestream=new FileStream(filepath,FileMode.Create)) 
                    {
                        await employeesData.Photo.CopyToAsync(filestream);
                    }
                    if (!string.IsNullOrEmpty(employeesData.PhotoPath))
                    {
                        var oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", employeesData.PhotoPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Delete(oldpath);
                        }
                    }
                    employeesData.PhotoPath = $"/images/{newfilename}";               
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error in saving file" + ex.Message);
                    return View(employeesData);
                }
            }
            _applicationDbContext.Employees.Update(employeesData);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("GetAll");

        }
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var employees = await _applicationDbContext.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }
            return View(employees);  // Show confirmation view with student data
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var student = await _applicationDbContext.Employees.FindAsync(id);
                if (student != null)
                {
                    // If there is an associated photo, delete it
                    if (!string.IsNullOrEmpty(student.PhotoPath))
                    {
                        var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", student.PhotoPath.TrimStart('/'));
                        if (System.IO.File.Exists(photoPath))
                        {
                            System.IO.File.Delete(photoPath);
                        }
                    }

                    // Remove the student record
                    _applicationDbContext.Employees.Remove(student);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., logging)
            }

            // Redirect to list of all students
            return RedirectToAction("GetAll");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
