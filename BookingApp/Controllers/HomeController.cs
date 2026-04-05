using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Models;
using Microsoft.EntityFrameworkCore;
using BookingApp.Data;
using DNTCaptcha.Core;
using Microsoft.Extensions.Caching.Memory;
using Humanizer;

namespace BookingApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;



    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;

    }

    public IActionResult BookSlot()
    {
        try
        {
            var bookings = _context.BookingModel
                                    .Where(b => b.BookingStatus != 3 && b.BookingStatus != 4)
                                    .ToList();

            var events = bookings.Select(b => new
            {
                title = b.name,
                start = b.FromDate.ToString("yyyy-MM-dd") + "T" + DateTime.Parse(b.startTime).ToString("HH:mm:ss"),
                end = b.EndDate.ToString("yyyy-MM-dd") + "T" + DateTime.Parse(b.endTime).ToString("HH:mm:ss"),
                description = "Event: " + b.name
            }).ToList();


            return Json(events);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }


    

    [HttpGet]
    public IActionResult Default()
    {
        HttpContext.Session.Clear();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateDNTCaptcha(ErrorMessage = "Enter valid Captcha!!!")]
    public async Task<IActionResult> Default(Login model)
    {


        if (ModelState.IsValid)
        {



            var user = await _context.Login
                       .FirstOrDefaultAsync(u => u.Username.Trim() == model.Username.Trim() && u.Password.Trim() == model.Password.Trim());

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);

                return RedirectToAction("Index", "Home");
            }
            else
            {

                ModelState.AddModelError("", "Invalid username or password.");
            }
        }

        ViewData["ModalOpenFlag"] = true;

        return View(model);

    }


    [HttpPost]
    public async Task<IActionResult> CreateBooking(BookingModel booking, List<string> selectedFacilities)
    {

        DateTime startDate = booking.FromDate;
        DateTime endDate = booking.EndDate;


        var overlappingBookings = await _context.BookingModel
            .Where(b => (b.FromDate <= endDate && b.EndDate >= startDate && b.BookingStatus != 3 && b.BookingStatus != 4))
            .ToListAsync();

        if (overlappingBookings.Any())
        {
            ViewBag.FailedMessage = "There is already an event booked during this time range.";

            return View("Index", booking);
        }


        TimeSpan startTime = TimeSpan.Parse(booking.startTime); 
        TimeSpan endTime = TimeSpan.Parse(booking.endTime);

        DateTime FullStartDateTime = startDate.Date + startTime;
        DateTime FullEndDateTime = endDate.Date + endTime;

        TimeSpan totalDuration = FullEndDateTime - FullStartDateTime;
        double totalHours = totalDuration.TotalHours;

        if (totalHours < 6)
        {
            ViewBag.FailedMessage = "Total booking duration should be of minimum 6 hours.";

            return View("Index", booking);
        }


        string username1 = HttpContext.Session.GetString("Username");



        var booking1 = new BookingModel
        {

            name = booking.name,
            BookingDate = DateTime.Now.Date,
            startTime = booking.startTime,
            endTime = booking.endTime,
            BookingStatus = 1,
            IsBooked = true,
            FromDate = booking.FromDate,
            EndDate = booking.EndDate,
            PDFFileNAME = "NA",
            PDFFileDATA = new byte[] { },
            username = username1
        };

        _context.BookingModel.Add(booking1);
        _context.SaveChanges();


        if (selectedFacilities != null && selectedFacilities.Any())
        {
            foreach (var facility in selectedFacilities)
            {
                var bookingFacility = new BookingFacilities
                {
                    BookingId = booking1.Id,
                    FacilityName = facility,
                    Username = username1
                };

                _context.BookingFacilities.Add(bookingFacility);
            }

            await _context.SaveChangesAsync();
        }


        ViewBag.SuccessMessage = "Booking Successful.";

        return View("Index", booking);


    }


    [HttpGet]
    public async Task<IActionResult> GetBookingsByStatus(int status)
    {
        string username1 = HttpContext.Session.GetString("Username");


        var bookings = await _context.BookingModel
            .Where(b => b.username == username1 && b.BookingStatus == status)
            .ToListAsync();


        return PartialView("_BookingTable", bookings);
    }


    [HttpPost]
    [Route("Home/CancelBooking/{bookingId}")]
    public IActionResult CancelBooking(int bookingId)
    {
        var booking = _context.BookingModel.FirstOrDefault(b => b.Id == bookingId);
        if (booking == null)
        {
            return Json(new { success = false, message = "Booking not found" });
        }

        booking.BookingStatus = 3;
        _context.SaveChanges();

        return Json(new { success = true });
    }



    public async Task<IActionResult> CheckStat()
    {
        string username1 = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username1))
        {

            return RedirectToAction("Default", "Home");
        }

        var bookings = await _context.BookingModel
            .Where(b => b.username == username1)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.Id)
            .ToListAsync();

        return View(bookings);
    }



    public IActionResult Register()
    {
        return View();
    }
    public IActionResult Index()
    {
        string username1 = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username1))
        {

            return RedirectToAction("Default", "Home");
        }

        return View();
    }


    [HttpGet]
    public async Task<IActionResult> AdminPanel()
    {

        string username2 = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username2) && username2 != "admin")
        {
            return RedirectToAction("Default", "Home");
        }

        var bookings = await _context.BookingModel
            .Where(b => b.BookingStatus == 1)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.Id)
            .ToListAsync();

        if (bookings == null)
        {
            bookings = new List<BookingModel>();
        }

        return View(bookings);
    }


    [HttpGet]
    public JsonResult GetBookingDetails(int id)
    {
        var booking = _context.BookingModel
            .Where(b => b.Id == id)
            .FirstOrDefault();

        if (booking != null)
        {
            return Json(new { success = true, booking });
        }
        return Json(new { success = false });
    }

    [HttpPost]
    public IActionResult UpdateBookingStatus(int bookingId, int bookingStatus)
    {
        var booking = _context.BookingModel.FirstOrDefault(b => b.Id == bookingId);
        if (booking == null)
        {
            return Json(new { success = false, message = "Booking not found" });
        }


        booking.BookingStatus = bookingStatus;
        _context.SaveChanges();

        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Approved()
    {

        string username2 = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username2) && username2 != "admin")
        {
            return RedirectToAction("Default", "Home");
        }

        var bookings = await _context.BookingModel
            .Where(b => b.BookingStatus == 2)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.Id)
            .ToListAsync();

        if (bookings == null)
        {
            bookings = new List<BookingModel>();
        }

        return View(bookings);
    }

    [HttpGet]
    public async Task<IActionResult> Rejected()
    {

        string username2 = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username2) && username2 != "admin")
        {
            return RedirectToAction("Default", "Home");
        }

        var bookings = await _context.BookingModel
            .Where(b => b.BookingStatus == 4)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.Id)
            .ToListAsync();

        if (bookings == null)
        {
            bookings = new List<BookingModel>();
        }

        return View(bookings);
    }

    private readonly Dictionary<string, decimal> _fixedPrices = new Dictionary<string, decimal>
{
    { "Auditorium", 30000 },
    { "Cafeteria", 6000 },
    { "Parking", 7500 },
    { "ArtGallery", 1500 }
};

//    private readonly Dictionary<string, decimal> _HourlyPrices = new Dictionary<string, decimal>
//{
//    { "Auditorium", 5000 },
//    { "Cafeteria", 1000 },
//    { "Parking", 1300 },
//    { "ArtGallery", 300 }
//};

    [HttpGet]
    [Route("Home/GetFacilities/{bookingId}")]
    public async Task<IActionResult> GetFacilities(int bookingId, DateTime bookingStartTime, DateTime bookingEndTime, string bookingStartDate, string bookingEndDate)
    {
        var selectedFacilities = await _context.BookingFacilities
            .Where(bf => bf.BookingId == bookingId)
            .Select(bf => bf.FacilityName)
            .ToListAsync();

        Console.WriteLine($"BookingId: {bookingId}, Facilities: {string.Join(", ", selectedFacilities)}");

        if (selectedFacilities == null || !selectedFacilities.Any())
        {
            return NotFound($"No facilities found for bookingId: {bookingId}.");
        }

        var pricing = new Dictionary<string, decimal>();
        decimal rentCharge = 0;
        //decimal totalHourlyCharge = 0;

        DateTime parsedBookingStartDate = DateTime.ParseExact(bookingStartDate, "dd-MM-yyyy", null);
        DateTime parsedBookingEndDate = DateTime.ParseExact(bookingEndDate, "dd-MM-yyyy", null);


        DateTime fullBookingStartTime = new DateTime(parsedBookingStartDate.Year, parsedBookingStartDate.Month, parsedBookingStartDate.Day, bookingStartTime.Hour, bookingStartTime.Minute, bookingStartTime.Second);
        DateTime fullBookingEndTime = new DateTime(parsedBookingEndDate.Year, parsedBookingEndDate.Month, parsedBookingEndDate.Day, bookingEndTime.Hour, bookingEndTime.Minute, bookingEndTime.Second);

        double totalBookingDuration = (fullBookingEndTime - fullBookingStartTime).TotalHours;

       
        int extraHours = 0;
        if (totalBookingDuration > 6)
        {
            extraHours = (int)(totalBookingDuration - 6);
        }

        

        foreach (var facility in selectedFacilities)
        {
            if (_fixedPrices.ContainsKey(facility))
            {
               
                pricing[facility] = _fixedPrices[facility];
                rentCharge += _fixedPrices[facility];
            }

            //if (_HourlyPrices.ContainsKey(facility) && extraHours > 0)
            //{

            //    var hourlyCharge = _HourlyPrices[facility] * extraHours;
            //    pricing[facility] += hourlyCharge;
            //    totalHourlyCharge += hourlyCharge;
            //}
        }



        //decimal totalRentCharge = rentCharge + totalHourlyCharge;
        var ExtraHourCharge = extraHours * 4500;

        
            Console.WriteLine($"ExtraHourCharge: {ExtraHourCharge}");
        
        int generatorcharge = 3000;
        var serviceCharge = (rentCharge + ExtraHourCharge) * 0.10m;
        var securityDeposit = (rentCharge + ExtraHourCharge) * 0.20m;
        var cgst = (rentCharge + ExtraHourCharge + serviceCharge + generatorcharge) * 0.09m;
        var sgst = (rentCharge + ExtraHourCharge + serviceCharge + generatorcharge) * 0.09m;

        var totalAmount = rentCharge + ExtraHourCharge + generatorcharge + serviceCharge + securityDeposit + cgst + sgst;

        return Ok(new
        {
            selectedFacilities = pricing,
            totalBookingDuration,
            //totalRentCharge,
            ExtraHourCharge,
            rentCharge,
            //totalHourlyCharge,
            generatorcharge,
            serviceCharge,
            securityDeposit,
            cgst,
            sgst,
            totalAmount
        });
    }

    [HttpGet]
    public IActionResult Approvement_Letter(int id)
    {
        string username2 = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username2) && username2 != "admin")
        {
            return RedirectToAction("Default", "Home");
        }

        var booking = _context.BookingModel.FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        var result = (from b in _context.BookingModel
                      join u in _context.Register on b.username equals u.Username
                      join p in _context.BookingFacilities on b.username equals p.Username
                      where b.Id == id
                      select new ApprovementLetterViewModel
                      {
                          BookingDetails = b,
                          UserDetails = u,
                          FacilitiesDetails = p
                      }).FirstOrDefault();

        if (result == null)
        {
            return NotFound();
        }

        return View(result);
    }

}
