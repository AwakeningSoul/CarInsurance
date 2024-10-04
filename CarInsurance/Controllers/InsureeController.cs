using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // Calculate the insurance quote based on input data
        private decimal CalculateQuote(Insuree insuree)
        {
            decimal quote = 50m; // Base quote of $50 per month

            // Calculate age
            var today = DateTime.Today;
            var age = today.Year - insuree.DateOfBirth.Year;
            if (insuree.DateOfBirth.Date > today.AddYears(-age)) age--;

            // Add to the quote based on age
            if (age <= 18)
            {
                quote += 100;
            }
            else if (age >= 19 && age <= 25)
            {
                quote += 50;
            }
            else
            {
                quote += 25;
            }

            // Add to the quote based on car year
            if (insuree.CarYear < 2000)
            {
                quote += 25;
            }
            else if (insuree.CarYear > 2015)
            {
                quote += 25;
            }

            // Add to the quote based on car make and model
            if (insuree.CarMake.ToLower() == "porsche")
            {
                quote += 25;
                if (insuree.CarModel.ToLower() == "911 carrera")
                {
                    quote += 25;
                }
            }

            // Add to the quote for speeding tickets
            quote += insuree.SpeedingTickets * 10;

            // Add 25% to the total if the user has a DUI
            if (insuree.DUI)
            {
                quote *= 1.25m;
            }

            // Add 50% to the total if the user selects full coverage
            if (insuree.CoverageType)
            {
                quote *= 1.5m;
            }

            return quote;
        }

        // Updated Create POST method to calculate the quote before saving
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Calculate the quote
                insuree.Quote = CalculateQuote(insuree);

                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // Updated Edit POST method to recalculate the quote when editing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Recalculate the quote
                insuree.Quote = CalculateQuote(insuree);

                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }
       // Admin action to display the list of Insurees
        public ActionResult Admin()
        {
            return View(db.Insurees.ToList());
        }
    }
}