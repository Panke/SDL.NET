using PetaTest;
using SCG = System.Collections.Generic;
namespace SDL.Test {
   /**
     $make test TF='-run:SDL.Test.SimpleTagTest'
     */
   [TestFixture] public class SimpleTagTest {
      [Test]public void Int_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size 4");
         Assert.AreEqual(root.Name, "root");
         Assert.AreEqual(root.GetChild("size").Value, 4);
         var s = root.GetChild("xx");
         Assert.IsNull(s);
      }

      [Test]public void string_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("first_name \"Akiko\"");
         Assert.AreEqual(root.GetChild("first_name").Value, "Akiko");
      }

      [Test]public void ticked_string_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("talk `I said \"something\"`");
         Assert.AreEqual(root.GetChild("talk").Value, "I said \"something\"");
      }

      [Test]public void simple_date() {
         SDL.Tag root = new SDL.Tag("root").ReadString("date 2005/12/05");
         Assert.AreEqual(root.GetChild("date").ValueAs<SDLDateTime>().DateTime
               , new System.DateTime(2005, 12, 5));
      }

      [Test]public void list_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("numbers 12 53 2");
         Tag num = root.GetChild("numbers");

         Assert.AreEqual(num.Values.Count, 3);
         Assert.AreEqual(num.Values[0], 12);
         Assert.AreEqual(num.Values[1], 53);
         Assert.AreEqual(num.Values[2], 2);

         SCG.IList<Tag> list = num.GetChildren(false);
         Assert.AreEqual(list.Count, 0);
      }
      [Test]public void attribute_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("person name=\"odv\" age=50");
         Tag p = root.GetChild("person");

         Assert.AreEqual(p["name"], "odv");
         Assert.AreEqual(p["age"], 50);
      }

//

//# a date time literal without a timezone
//here 2005/12/05 14:12:23.345

//# a date time literal with a timezone
//in_japan 2005/12/05 14:12:23.345-JST

   }
}
