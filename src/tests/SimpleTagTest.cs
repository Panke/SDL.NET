using SCG = System.Collections.Generic;
using NSubstitute;
using NUF = NUnit.Framework;
namespace SDL.Test {
   /**
     $make test TF='-run:SDL.Test.SimpleTagTest'
     */
   [NUF.TestFixture] public class SimpleTagTest {
      [NUF.Test]public void Int_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size 4");
         NUF.Assert.That(root.Name, NUF.Is.EqualTo("root"));
         NUF.Assert.That(root.GetChild("size").Value, NUF.Is.EqualTo(4));
         var s = root.GetChild("xx");
         NUF.Assert.That(s, NUF.Is.Null);
      }

      [NUF.Test]public void string_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("first_name \"Akiko\"");
         NUF.Assert.That(root.GetChild("first_name").Value, NUF.Is.EqualTo("Akiko"));
      }

      [NUF.Test]public void ticked_string_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("talk `I said \"something\"`");
         NUF.Assert.That(root.GetChild("talk").Value, NUF.Is.EqualTo("I said \"something\""));
      }

      [NUF.Test]public void simple_date() {
         SDL.Tag root = new SDL.Tag("root").ReadString("date 2005/12/05");
         NUF.Assert.That(root.GetChild("date").ValueAs<SDLDateTime>().DateTime
               , NUF.Is.EqualTo(new System.DateTime(2005, 12, 5)));
      }

      [NUF.Test]public void list_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("numbers 12 53 2");
         Tag num = root.GetChild("numbers");

         NUF.Assert.That(num.Values.Count, NUF.Is.EqualTo(3));
         NUF.Assert.That(num.Values[0], NUF.Is.EqualTo(12));
         NUF.Assert.That(num.Values[1], NUF.Is.EqualTo(53));
         NUF.Assert.That(num.Values[2], NUF.Is.EqualTo(2));

         SCG.IList<Tag> list = num.GetChildren(false);
         NUF.Assert.That(list.Count, NUF.Is.EqualTo(0));
      }
      [NUF.Test]public void attribute_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("person name=\"odv\" age=50");
         Tag p = root.GetChild("person");

         NUF.Assert.That(p["name"], NUF.Is.EqualTo("odv"));
         NUF.Assert.That(p["age"], NUF.Is.EqualTo(50));
      }

//

//# a date time literal without a timezone
//here 2005/12/05 14:12:23.345

//# a date time literal with a timezone
//in_japan 2005/12/05 14:12:23.345-JST

   }
}
