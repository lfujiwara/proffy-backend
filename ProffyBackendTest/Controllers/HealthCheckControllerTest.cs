using NUnit.Framework;
using ProffyBackend.Controllers.HealthCheck;

namespace ProffyBackendTest.Controllers
{
    public class HealthCheckControllerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var controller = new HealthCheckController();
            var result = controller.Get();
            Assert.AreEqual(result.Message, "System is running");
        }
    }
}