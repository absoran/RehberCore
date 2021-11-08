using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RehberCore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace RehberCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public List<Contact> ContactList { get; set; } //contact list for display all contacts
        public IEnumerable<Number> NumberList { get; set; } //number list for display all contact numbers 
        public static int ContactIdForNewNumber { get; set; } // global contact id variable for add new nubmer to existing contact

        private readonly RehberContext _dbcontext;
        public HomeController(ILogger<HomeController> logger,RehberContext dbcontext) //dbcontext instance added with dependency injection for database operations
        {
            _logger = logger;
            _dbcontext = dbcontext;
        }

        
        public IActionResult ListContacts()
        {
            ContactList = _dbcontext.Contacts.ToList();
            foreach (var item in ContactList)
            {
                item.Numbers = _dbcontext.Numbers.Where(x => x.ContactId == item.Id).ToList();
                var x = item.Numbers[0];
                Console.WriteLine(item.Numbers);
            }
            return View(ContactList);
        }
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// User can add 3 number on creating new contact. If user need to add more number to contact; user can use add new number in details screen
        /// </summary>
        /// <param name="Model"> used data transfer object for add multiple number on create new contact. </param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(ContactDTO Model)
        {
            var newcontact = new Contact {
            FirstName = Model.FirstName,
            LastName = Model.LastName,
            };
            _dbcontext.Contacts.Add(newcontact);
            _dbcontext.SaveChanges();

            var contacttoedit = _dbcontext.Contacts.FirstOrDefault(x => x.FirstName == Model.FirstName);
            if (contacttoedit == null)
            {
                return NotFound();
            }
            _dbcontext.Entry(contacttoedit).State = EntityState.Detached;
            if (Model.PhoneNumber != null)
            {
                var newnumber = new Number
                {
                    PhoneNumber = Model.PhoneNumber,
                    ContactId = contacttoedit.Id,
                };
                _dbcontext.Numbers.Add(newnumber);
            }
            if (Model.PhoneNumber2 != null)
            {
                var newnumber2 = new Number
                {
                    PhoneNumber = Model.PhoneNumber2,
                    ContactId = contacttoedit.Id,
                };
                _dbcontext.Numbers.Add(newnumber2);
            }
            if (Model.PhoneNumber3 != null)
            {
                var newnumber3 = new Number
                {
                    PhoneNumber = Model.PhoneNumber3,
                    ContactId = contacttoedit.Id,
                };
                _dbcontext.Numbers.Add(newnumber3);
            }
            _dbcontext.SaveChanges();
            return RedirectToAction("ListContacts");
        }

        public IActionResult AddNumber(int Id)
        {
            ContactIdForNewNumber = Id; // set global id variable for later use
            return View();
        }

        [HttpPost]
        public IActionResult AddNumber(Number Model)
        {
            Model.ContactId = ContactIdForNewNumber;
            _dbcontext.Numbers.Add(Model);
            _dbcontext.SaveChanges();
            return RedirectToAction("ListContacts");
        }

        public IActionResult Edit(int id)
        {
            var contactToEdit = _dbcontext.Contacts.Include(e => e.Numbers).FirstOrDefault(x => x.Id == id);
            return View(contactToEdit);
        }   

        [HttpPost]
        public IActionResult Edit(Contact Model)
        {
            var numberList = _dbcontext.Numbers.ToList().Where(x => x.ContactId == Model.Id);
            int counter = 0;
            foreach(var item in numberList)
            {
                var numberToEdit = _dbcontext.Numbers.Find(item.Id);
                numberToEdit.PhoneNumber = Model.Numbers[counter].PhoneNumber;
                _dbcontext.Entry(numberToEdit).State = EntityState.Modified;
                counter++;
            }
            _dbcontext.Entry(Model).State = EntityState.Modified;
            _dbcontext.SaveChanges();
            return RedirectToAction("ListContacts");
        }

        public IActionResult Details(int id)
        {
            NumberList = _dbcontext.Numbers.Where(x => x.ContactId == id).Include(x => x.Contact).ToList();
            return View(NumberList);
        }

        public IActionResult Delete(int id)
        {
            var obj = _dbcontext.Contacts.Find(id);
            if(obj != null && ModelState.IsValid)
            {
                _dbcontext.Contacts.Remove(obj);
                var num = _dbcontext.Numbers.Find(obj.Id);
                if(num != null)
                {
                    _dbcontext.Numbers.Remove(num);
                }
                _dbcontext.SaveChanges();
            }
            return RedirectToAction("ListContacts");
        }
        public IActionResult DeleteNumber(int Id)
        {
            var numberToDelete = _dbcontext.Numbers.Find(Id);
            _dbcontext.Numbers.Remove(numberToDelete);
            _dbcontext.SaveChanges();
            return RedirectToAction("ListContacts");
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public IActionResult EditNumber(int Id)
        {
            var numberToEdit = _dbcontext.Numbers.Include(x => x.Contact).FirstOrDefault(e => e.Id == Id);
            return View(numberToEdit);
        }
        [HttpPost]
        public IActionResult EditNumber(Number number)
        {
            var numberToEdit = _dbcontext.Numbers.Find(number.Id);
            numberToEdit.PhoneNumber = number.PhoneNumber;
            _dbcontext.Entry(numberToEdit).State = EntityState.Modified;
            _dbcontext.SaveChanges();
            return RedirectToAction("ListContacts");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {

            return View();
        }
    }  
}
