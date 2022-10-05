using System;
using System.Windows.Forms;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using CheckDefib.Core.Models;
using CheckDefib.Core.Services;
using CheckDefib.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using CheckDefib.Core.Security;

/**
 *  User Management Controller providing registration and login functionality
 */
namespace CheckDefib.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IUserService _svc;

        public UserController(IUserService svc, IConfiguration config)
        {        
            _config = config;    
            _svc = svc;
        }

        public IActionResult Index()
        {
            var users = _svc.GetUsers();
            return View(users);
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] UserLoginViewModel m)
        {
            var user = _svc.Authenticate(m.Email, m.Password);
            // check if login was unsuccessful and add validation errors
            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }

            // Login Successful, so sign user in using cookie authentication
            await SignInCookie(user);

            Alert("Successfully Logged in", AlertType.info);

            return Redirect("/");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
         [Authorize(Roles="Admin,Manager")]
        public IActionResult Register([Bind("Username,Email,Password,PasswordConfirm,Role")] UserRegisterViewModel m)       
        {
            if (!ModelState.IsValid)
            {
                return View(m);
            }
            // add user via service
            var user = _svc.AddUser(m.Username, m.Email,m.Password, m.Role);
            // check if error adding user and display warning
            if (user == null) {
                Alert("There was a problem Registering. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Registered.", AlertType.success);

            return View();
        //return RedirectToAction(nameof(Login));
        }

        [Authorize]
        public IActionResult UpdateProfile()
        {
           // use BaseClass helper method to retrieve Id of signed in user
            var user = _svc.GetUser(User.GetSignedInUserId());
            var userViewModel = new UserProfileViewModel { 
                Id = user.Id, 
                Username = user.Username, 
                Email = user.Email,                 
                Role = user.Role
            };
            return View(userViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind("Id,Username,Email,Role")] UserProfileViewModel m)       
        {
            var user = _svc.GetUser(m.Id);
            // check if form is invalid and redisplay
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            } 

            // update user details and call service
            user.Username = m.Username;
            user.Email = m.Email;
            user.Role = m.Role;        
            var updated = _svc.UpdateUser(user);

            // check if error updating service
            if (updated == null) {
                Alert("There was a problem Updating. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Account Details", AlertType.info);
            
            // sign the user in with updated details)
            await SignInCookie(user);

            return RedirectToAction("Index","Home");
        }

        // Change Password
        [Authorize]
        public IActionResult UpdatePassword()
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var user = _svc.GetUser(User.GetSignedInUserId());
            var passwordViewModel = new UserPasswordViewModel { 
                Id = user.Id, 
                Password = user.Password, 
                PasswordConfirm = user.Password, 
            };
            return View(passwordViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([Bind("Id,OldPassword,Password,PasswordConfirm")] UserPasswordViewModel m)       
        {
            var user = _svc.GetUser(m.Id);
            if (!ModelState.IsValid || user == null)
            {
                return View(m);
            }  
            // update the password
            user.Password = m.Password; 
            // save changes      
            var updated = _svc.UpdateUser(user);
            if (updated == null) {
                Alert("There was a problem Updating the password. Please try again", AlertType.warning);
                return View(m);
            }

            Alert("Successfully Updated Password", AlertType.info);
            // sign the user in with updated details
            await SignInCookie(user);

            return RedirectToAction("Index","Home");
        }

         public IActionResult Details(int id)
        {
            // load the user using the service
            var user = _svc.GetUser(id);

            if(user == null)
            {
                //Alert of "Not Found" to View
                Alert($"User{id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET / User/delete/{id}
        [Authorize(Roles="Admin,Manager")]      
        public IActionResult Delete(int id)
        {       
            // load the user using the service
            var user = _svc.GetUser(id);
            // check the returned user is not null and if so return NotFound()
            if (user == null)
            {
                // TBC - Display suitable warning alert and redirect
                Alert($"User {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }     
            
            // pass defibrillator to view for deletion confirmation
            return View(user);
        }

        // POST /User/delete/{id}
        [HttpPost]
        [Authorize(Roles="Admin,Manager")]
        [ValidateAntiForgeryToken]              
        public IActionResult DeleteConfirm(int id)
        {
            // TBC delete Defibrillator via service
            _svc.DeleteUser(id);

            Alert("User deleted successfully", AlertType.danger);
            // redirect to the index view
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // Return not authorised and not authenticated views
        public IActionResult ErrorNotAuthorised() => View();
        public IActionResult ErrorNotAuthenticated() => View();

        // -------------------------- Helper Methods ------------------------------

        // Called by Remote Validation attribute on RegisterViewModel to verify email address is available
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmailAvailable(string email, int id)
        {
            // check if email is available, or owned by user with id 
            if (!_svc.IsEmailAvailable(email,id))
            {
                return Json($"A user with this email address {email} already exists.");
            }
            return Json(true);                  
        }

        // Called by Remote Validation attribute on ChangePassword to verify old password
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPassword(string oldPassword)
        {
            // use BaseClass helper method to retrieve Id of signed in user 
            var id = User.GetSignedInUserId();            
            // check if email is available, unless already owned by user with id
            var user = _svc.GetUser(id);
            if (user == null || !Hasher.ValidateHash(user.Password, oldPassword))
            {
                return Json($"Please enter current password.");
            }
            return Json(true);                  
        }

        // Sign user in using Cookie authentication scheme
        private async Task SignInCookie(User user)
        {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                AuthBuilder.BuildClaimsPrincipal(user)
            );
        }
    }
}