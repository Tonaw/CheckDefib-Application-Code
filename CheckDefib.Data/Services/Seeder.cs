
using CheckDefib.Core.Models;
using CheckDefib.Core.Services;

namespace CheckDefib.Data.Services
{
    public static class Seeder
    {
        // use this class to seed the database with dummy 
        // test data using an IUserService 
         public static void Seed(IUserService svc)
        {
            svc.Initialise();

            // add users
            svc.AddUser("Administrator", "admin@mail.com", "Admin", Role.Admin);
            svc.AddUser("Manager", "manager@mail.com", "Manager", Role.Manager);
            svc.AddUser("Staff", "staff@mail.com", "Staff", Role.Staff);   

            //add defibrillators
            svc.CreateDefibrillator("PBNI", "BT48 6QP", "Richmond Chambers, The Diamond, Londonderry", 54.99511, -7.32182, 0);
            svc.CreateDefibrillator("Roe Park Resort", "BT49 9LB", "40 Drumrane Rd, Limavady", 55.027677, -7.03307, 0);
            svc.CreateDefibrillator("REACH ACROSS", "BT48 6PW", "10-14 BISHOP STREET WITHIN, LONDONDERRY", 54.9946025, -7.3235005, 0);
            svc.CreateDefibrillator("Apprentice Boys Memorial Hall", "BT48 6PJ", "Apprentice Boys Memorial Hall, Ground & 2nd Floor, 13 Society Street, Londonderry", 54.9952483, -7.3239316, 0);
            svc.CreateDefibrillator("Derry Central Library", "BT48 6PW", "35 Foyle Street, Londonderry", 54.9965964, -7.3185444, 0);
            svc.CreateDefibrillator("Guildhall", "BT48 6DH", "Guildhall Guildhall Street, Londonderry", 54.9976026, -7.3193957, 0);
            svc.CreateDefibrillator("THE PENSION CENTRE CARLISLE HOUSE", "BT48 6JN", "20A CARLISLE ROAD, LONDONDERRY", 54.993624, -7.3197898, 0);
            svc.CreateDefibrillator("PureGym",	"BT48 7PW", "UNIT 1 LESLEY RETAIL PARK, LONDONDERRY", 55.0087148, -7.317696, 0);
            svc.CreateDefibrillator("St. Marys College", "BT48 0AN", "35 Northland Road, Londonderry", 55.0109478,	-7.3291317, 0);
            svc.CreateDefibrillator("JP CORRY", "BT48 6JN", "177 STRAND ROAD, PENNYBURN LONDONDERRY", 55.0125422, -7.3161837, 0);
            svc.CreateDefibrillator("Eurospar", "BT48 0RX", "155 CREGGAN ROAD, ROSEMOUNT, LONDONDERRY", 55.0088132, -7.3449421, 0);
            
            
        }
    }
}











