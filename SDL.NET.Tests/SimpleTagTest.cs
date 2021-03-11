using Xunit;
using SCG = System.Collections.Generic;
namespace SDL.Test {

   public class SimpleTagTest {
      [Fact]public void Int_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size 4");
         Assert.Equal(root.Name, "root");
         Assert.Equal(root.GetChild("size").Value, 4);
         var s = root.GetChild("xx");
         Assert.Null(s);
      }

      [Fact]public void string_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("first_name \"Akiko\"");
         Assert.Equal(root.GetChild("first_name").Value, "Akiko");
      }

      [Fact]public void ticked_string_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("talk `I said \"something\"`");
         Assert.Equal(root.GetChild("talk").Value, "I said \"something\"");
      }

      [Fact]public void simple_date() {
         SDL.Tag root = new SDL.Tag("root").ReadString("date 2005/12/05");
         Assert.Equal(root.GetChild("date").ValueAs<SDLDateTime>().DateTime
               , new System.DateTime(2005, 12, 5));
      }

      [Fact]public void list_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("numbers 12 53 2");
         Tag num = root.GetChild("numbers");

         Assert.Equal(num.Values.Count, 3);
         Assert.Equal(num.Values[0], 12);
         Assert.Equal(num.Values[1], 53);
         Assert.Equal(num.Values[2], 2);

         SCG.IList<Tag> list = num.GetChildren(false);
         Assert.Equal(list.Count, 0);
      }
      [Fact]public void attribute_test() {
         SDL.Tag root = new SDL.Tag("root").ReadString("person name=\"odv\" age=50");
         Tag p = root.GetChild("person");

         Assert.Equal(p["name"], "odv");
         Assert.Equal(p["age"], 50);
      }

//

//# a date time literal without a timezone
//here 2005/12/05 14:12:23.345

//# a date time literal with a timezone
//in_japan 2005/12/05 14:12:23.345-JST

   }
}
