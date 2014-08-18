using PetaTest;
using SCG = System.Collections.Generic;
namespace SDL.Test {
   /**
     $make test TF='-run:SDL.Test.SimpleTagTest'
     */
   [TestFixture] public class ValueAsTest {
      [Test]public void get_int_child_as_T_sould_return_valid_value() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size 4");
         Assert.AreEqual(root.GetChildAs<int>("size", 0), 4);
         Assert.AreEqual(root.GetChildAs<double>("size", 0), 4D);
         Assert.AreEqual(root.GetChildAs<string>("size", "x"), "4");
         Assert.IsTrue(root.GetChildAs<bool>("size", false));

         Assert.AreEqual(root.GetChildAs<int>("no_value", 1), 1);
         Assert.AreEqual(root.GetChildAs<double>("no_value", 0), 0);
         Assert.AreEqual(root.GetChildAs<string>("no_value", "x"), "x");
         Assert.IsFalse(root.GetChildAs<bool>("no_value", false));
      }

      [Test]public void get_string_child_as_T_sould_return_valid_value() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size `a`");
         Assert.AreEqual(root.GetChildAs<int>("size", 0), 0);
         Assert.AreEqual(root.GetChildAs<double>("size", 0), 0);
         Assert.AreEqual(root.GetChildAs<string>("size", "x"), "a");
         Assert.IsFalse(root.GetChildAs<bool>("size", false));

         root = new SDL.Tag("root").ReadString("size `8`");
         Assert.AreEqual(root.GetChildAs<int>("size", 0), 8);
         Assert.AreEqual(root.GetChildAs<double>("size", 0), 8);
         Assert.AreEqual(root.GetChildAs<string>("size", "x"), "8");
         Assert.IsFalse(root.GetChildAs<bool>("size", false));
      }
   }
}
