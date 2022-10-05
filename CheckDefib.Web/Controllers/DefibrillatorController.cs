using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Device.Location;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


using CheckDefib.Core.Models;
using CheckDefib.Core.Services;
using CheckDefib.Data.Services;
using CheckDefib.Web;

namespace CheckDefib.Web.Controllers
{
    [Authorize]
    public class DefibrillatorController : BaseController
    {
        private IUserService svc;

        public DefibrillatorController()
        {
            svc = new UserServiceDb();
        }

        //Load All Defibrillators

        public IActionResult Index()
        {
            var defibrillators = svc.GetDefibrillators();

            return View(defibrillators);
        }



        public IActionResult Details(int id)
        {
            var d = svc.GetDefibrillator(id);

            //Check if Defibrillator is not found in dB
            if(d == null)
            {
                //Alert of "Not Found" to View
                Alert($"Defibrillator{id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(d);
        }

        //Creating Defibs(GET)
        [Authorize(Roles="Admin,Manager")]
        
        public IActionResult Create()
        {
            return View();
        }

        
        //Creating Defibs(POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin,Manager")]
        public IActionResult Create([Bind("DefibName, Postcode, Address, Latitude, Longitude, DefibType")] Defibrillator d)
        {
                if (svc.IsDefibDuplicate(d.Latitude, d.Longitude))
            {
                // add manual validation error
                ModelState.AddModelError(nameof(d.DefibName),"A defibrillator has already been attached to this location");
            }

            // complete POST action to add defibrillator
            if (ModelState.IsValid)
            {
                // pass data to service to save 
                d = svc.CreateDefibrillator(d.DefibName, d.Postcode, d.Address, d.Latitude, d.Longitude, d.DefibType);
                Alert($"Defibrillator created successfully", AlertType.success);

                return RedirectToAction(nameof(Details), new {Id = d.Id});
            }
            
            
            return View(d);
            
        }


        // GET /Defibrillator/edit/{id}
        [Authorize(Roles="Admin,Manager")]
        public IActionResult Edit(int id)
        {        
            // load the Defibrillator using the service
            var d = svc.GetDefibrillator(id);

            // check if d is null, and if so, alert
            if (d == null)
            {
                Alert($"Defibrillator {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }   

            // pass Defibrillator to view for editing
            return View(d);
        }

        // POST /Defibrillator/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin,Manager")]
        public IActionResult Edit(int id, [Bind("Id, DefibName, Postcode, Address, Latitude, Longitude, DefibType")] Defibrillator d)
        {
            

            if (ModelState.IsValid)
            {
                // pass data to service to update
                svc.UpdateDefibrillator(d);
                Alert("Defibrillator updated successfully", AlertType.info);

                return RedirectToAction(nameof(Details), new {Id = d.Id });
            }

            return View(d);
        }


        // GET / Defibrillator/delete/{id}
        [Authorize(Roles="Admin,Manager")]      
        public IActionResult Delete(int id)
        {       
            // load the defibrillator using the service
            var d = svc.GetDefibrillator(id);
            // check the returned defibrillator is not null and if so return NotFound()
            if (d == null)
            {
                // TBC - Display suitable warning alert and redirect
                Alert($"Defibrillator {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }     
            
            // pass defibrillator to view for deletion confirmation
            return View(d);
        }

        // POST /Defibrillator/delete/{id}
        [HttpPost]
        [Authorize(Roles="Admin,Manager")]
        [ValidateAntiForgeryToken]              
        public IActionResult DeleteConfirm(int id)
        {
            // TBC delete Defibrillator via service
            svc.DeleteDefibrillator(id);

            Alert("Defibrillator deleted successfully", AlertType.danger);
            // redirect to the index view
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CheckDefib()
        {
            var defibrillators = svc.GetDefibrillators();

            String message = "";

           Dictionary<double, string> distances = new Dictionary<double, string>();


            for (int i = 0; i < defibrillators.Count; i++)
            {
                for (int j = 1; j < defibrillators.Count - i; j++)
                {

                    if (defibrillators[i].Id != defibrillators[j].Id)
                    {
                        double distance = CordinatesToDistance.DistanceTo(defibrillators[i].Latitude, defibrillators[i].Longitude, defibrillators[j].Latitude, defibrillators[j].Longitude);
                        distance = Math.Round(distance, 2);
                        if((!distances.ContainsKey(distance)) && (distance > 1.25))
                        {
                        distances.Add(distance, defibrillators[i].DefibName + " & " + defibrillators[j].DefibName);
                        } 
                                          
                    }

               }
                        if(distances.Count > 0)
                        {
                        var d = distances.Min(x => x.Key);
                        string e = distances[d];

                        
                        message += $"Defibrillators {e} are far apart by {d} miles {Environment.NewLine}";
                        distances.Clear();
                        } 
   
                    
            }

            MessageBox.Show(message);

            return RedirectToAction("Index");
        }

        
        //FInd Nearest Defib
        [HttpPost]
        public IActionResult FindNear(string lat, string longi)
        {
            var defibrillators = svc.GetDefibrillators();
            
            //Look for the nearest defibrillator

            double dist = 0;
            double defibLat = 0;
            double defibLong = 0;
            string closedefibName = "";
            string closedefibAddress = "";

            Dictionary<double, string> closestDefibSet = new Dictionary<double, string>();

            for(int i = 0; i < defibrillators.Count; i++)
            {
                double lowest = CordinatesToDistance.DistanceTo(double.Parse(lat), double.Parse(longi), defibrillators[i].Latitude, defibrillators[i].Longitude);

                if (lowest < dist)
                {
                    dist = lowest;
                    closedefibName = defibrillators[i].DefibName;
                    closedefibAddress = defibrillators[i].Address;
                    defibLat = defibrillators[i].Latitude;
                    defibLong = defibrillators[i].Longitude;
                }
            }

            //closestDefibSet.Add(dist, $"{closedefibName} {closedefibAddress}");

            return View();
         }

         public IActionResult Find()
         {      

            
            return View();

         }


         public ActionResult ShowSearchForm()
         {
            return svc.GetDefibrillator != null ?
                    View():
                    Problem("Entity set 'ApplicationDbContex.Defibrillator' is null");
         }
        

        //Search Defibrillator
        [HttpPost]
         public IActionResult ShowSearchResults(String SearchPhrase)
         {
            
            var defibrillator = svc.GetDefibrillators().Where(x => x.Postcode.Contains(SearchPhrase, StringComparison.OrdinalIgnoreCase) || 
                                                                   x.DefibName.Contains(SearchPhrase, StringComparison.OrdinalIgnoreCase));
                                                                   return View ("Index",defibrillator.ToList());
         }

     
    }
    

 

}


