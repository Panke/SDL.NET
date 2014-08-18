using PetaTest;
namespace SDL.Test {
   [TestFixture] public class IkayzoTest {
      [Test]public void attributes_should_consistently_ordered() {
         Tag t1 = new Tag("test");
         t1["foo"]="bar";
         t1["john"]="doe";

         Tag t2 = new Tag("test");
         t2["john"]="doe";
         t2["foo"]="bar";
         Assert.AreEqual(t1, t2);
      }
   }
}

