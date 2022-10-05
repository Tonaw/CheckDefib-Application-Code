
using Xunit;
using CheckDefib.Core.Models;
using CheckDefib.Core.Services;


using CheckDefib.Data.Services;

namespace CheckDefib.Test
{
    public class ServiceTests
    {
        private IUserService service;

        public ServiceTests()
        {
            service = new UserServiceDb();
            service.Initialise();
        }

        [Fact]
        public void EmptyDbShouldReturnNoUsers()
        {
            // act
            var users = service.GetUsers();

            // assert
            Assert.Equal(0, users.Count);
        }
        
        [Fact]
        public void AddingUsersShouldWork()
        {
            // arrange
            service.AddUser("Admin", "Admin@mail.com", "Admin", Role.Admin);
            service.AddUser("Staff", "Staff@mail.com", "Staff", Role.Staff);

            // act
            var users = service.GetUsers();

            // assert
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void UpdatingUserShouldWork()
        {
            // arrange
            var user = service.AddUser("Admin", "Admin@mail.com", "Admin", Role.Admin );
            
            // act
            user.Username = "Administrator";
            user.Email = "Admin@mail.com";            
            var updatedUser = service.UpdateUser(user);

            // assert
            Assert.Equal("Administrator", user.Username);
            Assert.Equal("Admin@mail.com", user.Email);
        }

        [Fact]
        public void LoginWithValidCredentialsShouldWork()
        {
            // arrange
            service.AddUser("Admin", "Admin@mail.com", "Admin", Role.Admin );
            
            // act            
            var user = service.Authenticate("Admin@mail.com","Admin");

            // assert
            Assert.NotNull(user);
           
        }

        [Fact]
        public void LoginWithInvalidCredentialsShouldNotWork()
        {
            // arrange
            service.AddUser("Admin", "Admin@mail.com", "Admin", Role.Admin );

            // act      
            var user = service.Authenticate("Admin@mail.com","xxx");

            // assert
            Assert.Null(user);
        }

        [Fact]
        public void User_AddUser_WhenDuplicateEmail_ShouldReturnNull()
        {
            // act 
            var u1 = service.AddUser("XXX", "xxx@email.com", "XXX", 0);
            // this is a duplicate as the email address is same as previous student
            var u2 = service.AddUser("XXX", "xxx@email.com", "XXX", 0);
            
            // assert
            Assert.NotNull(u1); // this user should have been added correctly
            Assert.Null(u2); // this user shouldnt be added        
        }

         [Fact]
        public void User_DeleteUser_ThatDoesntExist_ShouldReturnFalse()
        {
            // act 	
            var deleted = service.DeleteUser(0);

            // assert
            Assert.False(deleted);
        } 


        //Defibrillator Test
        [Fact]
       
        public void Defibrillator_GetAllDefibrillators_WhenNone_ShouldReturn0()
        {
            // act 
            var defibrillator = service.GetDefibrillators();
            var count = defibrillator.Count;

            // assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void Defibrillator_GetAllDefibrillators_WhenNone_ShouldReturn()
        {

            //arrange
            var d1 = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, 0);
            var d2 = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, 0);
            var d3 = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, 0);
            var d4 = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, 0);
        

            // act 
            var defibrillator = service.GetDefibrillators();
            var count = defibrillator.Count;

            // assert
            Assert.Equal(4, count);
        }

        [Fact] 
        public void Defibrillator_GetDefibrillator_WhenNonExistent_ShouldReturnNull()
        {
            // act 
            var d = service.GetDefibrillator(1); // no defibrillator here

            // assert
            Assert.Null(d);
        }


        [Fact]
        
        public void Defibrillator_AddDefibrillator_WhenNone_ShouldSetAllProperties()
        {
            // act 
            var added = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, DefibType.Automatic);
            
            // retrieve Defibrillator just added by using the Id returned by EF
            var d = service.GetDefibrillator(added.Id);

            // assert - that Defibrillator is not null
            Assert.NotNull(d);
            
            // now assert that the properties were set properly
            Assert.Equal(d.Id, d.Id);
            Assert.Equal("xxx", d.DefibName);
            Assert.Equal("xxx", d.Postcode);
            Assert.Equal("xxx", d.Address);
            Assert.Equal(1.1, d.Latitude);
            Assert.Equal(1.1, d.Longitude);
            Assert.Equal(DefibType.Automatic, d.DefibType);
        }

        [Fact]
        public void Defibrillator_GetDefibrillator_ThatExists_ShouldReturnDeibrillator()
        {
            // act 
            var added = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, 0);

            var d = service.GetDefibrillator(added.Id);

            // assert
            Assert.NotNull(d);
            Assert.Equal(added.Id, d.Id);
        }


        [Fact]
        public void User_DeleteDefibrillator_ThatDoesntExist_ShouldReturnFalse()
        {
            // act 	
            var deleted = service.DeleteDefibrillator(1);

            // assert
            Assert.False(deleted);
        } 

        [Fact]
        public void User_DeleteDefibrillator_WhenExists_ShouldReturnTrue()
        {
            //arrange  _create test Defibrillator
            var added = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, 0);

            // act 	
            var deleted = service.DeleteDefibrillator(1);

            // assert
            Assert.True(deleted);
        }

        [Fact]
        public void eDefibrillator_UpdateeDefibrillator_ThatExists_ShouldSetAllProperties()
        {
            // arrange - create test Defibrillator
            var d = service.CreateDefibrillator("xxx", "xxx", "xxx", 1.1, 1.1, DefibType.Semi);
                        
            // act - create a copy and update any Defibrillator properties (except Id) 
            var u = new Defibrillator {
                Id = d.Id,
                DefibName = "yyy",
                Postcode = "yyy",
                Address = "yyy",
                DefibType = DefibType.Automatic
            };
            // save updated Defibrillator
            service.UpdateDefibrillator(u); 

            // reload updated Defibrillator from database into us
            var load = service.GetDefibrillator(u.Id);

            // assert
            Assert.NotNull(u);           

            // now assert that the properties were set properly           
            Assert.Equal(u.DefibName, load.DefibName);
            Assert.Equal(u.Postcode, load.Postcode);
            Assert.Equal(u.Address, load.Address);
            Assert.Equal(u.DefibType, load.DefibType);
            
        }

    }
}

