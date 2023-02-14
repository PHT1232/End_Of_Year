using System.Threading.Tasks;
using Nguyen_Tan_Phat_Project.Models.TokenAuth;
using Nguyen_Tan_Phat_Project.Web.Controllers;
using Shouldly;
using Xunit;

namespace Nguyen_Tan_Phat_Project.Web.Tests.Controllers
{
    public class HomeController_Tests: Nguyen_Tan_Phat_ProjectWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}