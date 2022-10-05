
using CheckDefib.Core.Models;

namespace CheckDefib.Core.Services
{
    // This interface describes the operations that a UserService class implementation should provide
    public interface IUserService
    {
        // Initialise the repository - only to be used during development 
        void Initialise();

        // ---------------- User Management --------------
        IList<User> GetUsers();
        User GetUser(int id);

        User GetUserByEmail(string email);
        bool IsEmailAvailable(string email, int userId);
        User AddUser(string username, string email, string password, Role role);
        User UpdateUser(User user);
        bool DeleteUser(int id);
        User Authenticate(string email, string password);


        // ---------------- Defibrillator Management --------------
        IList<Defibrillator> GetDefibrillators();

        Defibrillator GetDefibrillator(int id);

        Defibrillator CreateDefibrillator(string defibName, string postcode, string address, double latitude, double longitude, DefibType defibType);

        bool DeleteDefibrillator(int id);

        Defibrillator UpdateDefibrillator(Defibrillator defibrillator);

        bool IsDefibDuplicate(double latitude, double longitude);

        //Current Location
        // StaffLocation CurrentCoordinates(double latitude, double longitude);

        
       
    }
    
}
