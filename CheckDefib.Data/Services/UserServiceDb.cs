using System;
using System.Collections.Generic;
using System.Linq;

using CheckDefib.Core.Models;
using CheckDefib.Core.Services;
using CheckDefib.Core.Security;
using CheckDefib.Data.Repositories;

namespace CheckDefib.Data.Services
{
    public class UserServiceDb : IUserService
    {
        private readonly DatabaseContext  ctx;

        public UserServiceDb()
        {
            ctx = new DatabaseContext(); 
        }

        public void Initialise()
        {
           ctx.Initialise(); 
        }

        // ------------------ User Related Operations ------------------------

        // retrieve list of Users
        public IList<User> GetUsers()
        {
            return ctx.Users.ToList();
        }

        // Retrive User by Id 
        public User GetUser(int id)
        {
            return ctx.Users.FirstOrDefault(s => s.Id == id);
        }

    
        // Add a new User checking a User with same email does not exist
        public User AddUser(string username, string email, string password, Role role)
        {     
            var existing = GetUserByEmail(email);
            if (existing != null)
            {
                return null;
            } 

            var user = new User
            {            
                Username = username,
                Email = email,
                Password = Hasher.CalculateHash(password), // can hash if required 
                Role = role              
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return user; // return newly added User
        }

        // Delete the User identified by Id returning true if deleted and false if not found
        public bool DeleteUser(int id)
        {
            var s = GetUser(id);
            if (s == null)
            {
                return false;
            }
            ctx.Users.Remove(s);
            ctx.SaveChanges();
            return true;
        }

        // Update the User with the details in updated 
        public User UpdateUser(User updated)
        {
            // verify the User exists
            var User = GetUser(updated.Id);
            if (User == null)
            {
                return null;
            }
            // verify email address is registered or available to this user
            if (!IsEmailAvailable(updated.Email, updated.Id))
            {
                return null;
            }
            // update the details of the User retrieved and save
            User.Username = updated.Username;
            User.Email = updated.Email;
            User.Password = Hasher.CalculateHash(updated.Password);  
            User.Role = updated.Role; 

            ctx.SaveChanges();          
            return User;
        }

        // Find a user with specified email address
        public User GetUserByEmail(string email)
        {
            return ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        // Verify if email is available or registered to specified user
        public bool IsEmailAvailable(string email, int userId)
        {
            return ctx.Users.FirstOrDefault(u => u.Email == email && u.Id != userId) == null;
        }

        public IList<User> GetUsersQuery(Func<User,bool> q)
        {
            return ctx.Users.Where(q).ToList();
        }

        public User Authenticate(string email, string password)
        {
            // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
            var user = GetUserByEmail(email);

            // Verify the user exists and Hashed User password matches the password provided
            return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;
            //return (user != null && user.Password == password ) ? user: null;
        }


        // // ------------------ Defibrillator Related Operations ------------------------

        //Find all Defibrillators
        public IList<Defibrillator> GetDefibrillators(){
            return ctx.Defibrillators.ToList();
                                
        }

        //Find one Defibrillator
         public Defibrillator GetDefibrillator(int id)
        {
            // return Defibrillator
            return ctx.Defibrillators
                     .FirstOrDefault(t => t.Id == id);
        }

        //Get Defibrillator by Coordinates

        public Defibrillator GetDefibrillatorByCoordinates(double latitude, double longitude)
        {
            //return the Defibrillator
            return ctx.Defibrillators
                    .FirstOrDefault(t => t.Latitude == latitude && t.Longitude == longitude);
        }


        //Create Defibrillator
         public Defibrillator CreateDefibrillator(string defibName, string postcode, string address, double latitude, double longitude, DefibType defibType)
         {
            var defibrillator = new Defibrillator
            {     
                DefibName = defibName,
                Postcode = postcode,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                DefibType = defibType             
            };
            
            ctx.Defibrillators.Add(defibrillator);
            ctx.SaveChanges();
            return defibrillator; // return the new Defibrillator

         }


        //Delete one Defibrillator
         public bool DeleteDefibrillator(int id)
         {
            var d = GetDefibrillator(id);

            if (d == null)
            {
                return false;
            }

            ctx.Defibrillators.Remove(d);
            ctx.SaveChanges();
            return true;

         }

         public Defibrillator UpdateDefibrillator(Defibrillator updated)
         {
            var defibrillator = GetDefibrillator(updated.Id);
            if (defibrillator == null)
            {
                return null;
            }

            defibrillator.DefibName = updated.DefibName;
            defibrillator.Postcode = updated.Postcode;
            defibrillator.Address = updated.Address;
            defibrillator.DefibType = updated.DefibType;

            ctx.SaveChanges();
            return defibrillator;

         }

         //check for duplicate locations


        public bool IsDefibDuplicate(double latitude, double longitude)
        {
            var d = GetDefibrillatorByCoordinates(latitude, longitude);

            if (d != null)
            {
            return true;
            }

            return false;
        }

        //User Location

         //Current Location
        // public StaffLocation CurrentCoordinates(double lat, double longi)
        // {
        //     var s = 
            

        //     return staffLocation;
        // }


    
        
   
    }
}
