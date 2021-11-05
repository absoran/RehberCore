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
        public List<Number> Model { get; set; }
        public List<Contact> Models { get; set; }
        public IEnumerable<Contact> Modelx { get; set; }
        public IEnumerable<Number> Modely { get; set; }

        private readonly RehberContext _dbcontext;
        public HomeController(ILogger<HomeController> logger,RehberContext dbcontext)
        {
            _logger = logger;
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }
        //public IActionResult ShowAll()
        //{
        //    //Model = new IEnumerable<Number>;
        //    Model = _dbcontext.Numbers.ToList();
        //    return View(Model);
        //}

        public IActionResult ShowAll2()
        {
            Models = _dbcontext.Contacts.ToList();
            foreach (var item in Models)
            {
                item.Numbers = _dbcontext.Numbers.Where(x => x.ContactId == item.Id).ToList();
                var x = item.Numbers[0];
                Console.WriteLine(item.Numbers);
            }
            return View(Models);
        }
        public IActionResult Create()
        {
            return View();
        }
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
            var newnumber = new Number
            {
                PhoneNumber = Model.PhoneNumber,
                ContactId = contacttoedit.Id,
            };
            var newnumber2 = new Number
            {
                PhoneNumber = Model.PhoneNumber2,
                ContactId = contacttoedit.Id,
            };
            var newnumber3 = new Number
            {
                PhoneNumber = Model.PhoneNumber3,
                ContactId = contacttoedit.Id,
            };
            _dbcontext.Numbers.Add(newnumber);
            _dbcontext.Numbers.Add(newnumber2);
            _dbcontext.Numbers.Add(newnumber3);
            var numberstoedit = _dbcontext.Numbers.Where(x => x.ContactId == contacttoedit.Id).ToList();
            if (numberstoedit == null)
            {
                return NotFound();
            }
            _dbcontext.SaveChanges();

            return RedirectToAction("ShowAll2");
        }
        public IActionResult Edit(int id)
        {
            var contact = _dbcontext.Contacts.Include(p => p.Numbers).Where(e => e.Id == id).ToList();
            var contactdto = new ContactDTO();
            if(contact[0].Numbers.Count == 1)
            {
                contactdto = new ContactDTO
                {
                    FirstName = contact[0].FirstName,
                    LastName = contact[0].LastName,
                    ContactId = contact[0].Id,
                    PhoneNumber = contact[0].Numbers[0].PhoneNumber
                };
            }
            else if (contact[0].Numbers.Count == 2)
            {
                contactdto = new ContactDTO
                {
                    FirstName = contact[0].FirstName,
                    LastName = contact[0].LastName,
                    ContactId = contact[0].Id,
                    PhoneNumber = contact[0].Numbers[0].PhoneNumber,
                    PhoneNumber2 = contact[0].Numbers[1].PhoneNumber
                };               
            }
            else if (contact[0].Numbers.Count == 3)
            {
                contactdto = new ContactDTO
                {
                    FirstName = contact[0].FirstName,
                    LastName = contact[0].LastName,
                    ContactId = contact[0].Id,
                    PhoneNumber = contact[0].Numbers[0].PhoneNumber,
                    PhoneNumber2 = contact[0].Numbers[1].PhoneNumber,
                    PhoneNumber3 = contact[0].Numbers[2].PhoneNumber
                };
            }
            return View(contactdto);
        }
        [HttpPost]
        public IActionResult Edit(ContactDTO Model)
        {
            var existingcontact = _dbcontext.Contacts.Include(p => p.Numbers).FirstOrDefault(x => x.FirstName == Model.FirstName);
            var existingnumber = _dbcontext.Numbers.Include(p => p.Contact).FirstOrDefault(x => x.ContactId == existingcontact.Id);
            _dbcontext.Entry(existingcontact).State = EntityState.Detached;
            _dbcontext.Entry(existingnumber).State = EntityState.Detached;
            var newcontact = new Contact
            {
                FirstName = Model.FirstName,
                LastName = Model.LastName,
                Numbers = existingcontact.Numbers,
                Id = existingcontact.Id
            };
            var newnumber = new Number
            {
                Contact = newcontact,
                Id = existingnumber.Id,
                ContactId = newcontact.Id,
                PhoneNumber = Model.PhoneNumber
            };
            newcontact.Numbers[0].PhoneNumber = Model.PhoneNumber;
            if (Model.PhoneNumber2 != null) { newcontact.Numbers[1].PhoneNumber = Model.PhoneNumber2; }
            if (Model.PhoneNumber3 != null) { newcontact.Numbers[2].PhoneNumber = Model.PhoneNumber2; }
            _dbcontext.Entry(newcontact).State = EntityState.Modified;
            _dbcontext.Entry(newnumber).State = EntityState.Modified;
            _dbcontext.SaveChanges();
            return RedirectToAction("ShowAll2");
        }


        public IActionResult Details(int id)
        {
            //var cont = _dbcontext.Contacts.Find(id);
            //var temp = _dbcontext.Numbers.Find(id);
            //int num = cont.Id;
            Modely = _dbcontext.Numbers.Where(x => x.ContactId == id).ToList();

            return View(Modely);
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
            return RedirectToAction("ShowAll2");
        }
        public IActionResult Privacy()
        {
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
