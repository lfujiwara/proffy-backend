using ProffyBackend.Models.PropertyValidators;
using NUnit.Framework;
using System.Globalization;

namespace ProffyBackendTest.Models.PropertyValidators
{
    public class LocaleAttributeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValidLocalesReturnsTrue()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            var localeAttribute = new LocaleAttribute();
            foreach (var cultureInfo in cultures)
            {
                Assert.IsTrue(localeAttribute.IsValid(cultureInfo));
            }
        }
        
        [Test]
        public void InvalidLocalesReturnsFalse()
        {
            var localeAttribute = new LocaleAttribute();
            Assert.IsFalse(localeAttribute.IsValid("This is an invalid locale"));   
        }
    }
}