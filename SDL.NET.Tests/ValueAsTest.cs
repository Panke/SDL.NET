using Xunit;
using SCG = System.Collections.Generic;

namespace SDL.Test {

   public class ValueAsTest {

      [Fact]
      public void get_int_child_as_T_sould_return_valid_value() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size 4");
         Assert.Equal(root.GetChildAs<int>("size", 0), 4);
         Assert.Equal(root.GetChildAs<double>("size", 0), 4D);
         Assert.Equal(root.GetChildAs<string>("size", "x"), "4");
         Assert.True(root.GetChildAs<bool>("size", false));

         Assert.Equal(root.GetChildAs<int>("no_value", 1), 1);
         Assert.Equal(root.GetChildAs<double>("no_value", 0), 0);
         Assert.Equal(root.GetChildAs<string>("no_value", "x"), "x");
         Assert.False(root.GetChildAs<bool>("no_value", false));
      }

      [Fact]
      public void get_string_child_as_T_sould_return_valid_value() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size `a`");
         Assert.Equal(root.GetChildAs<int>("size", 0), 0);
         Assert.Equal(root.GetChildAs<double>("size", 0), 0);
         Assert.Equal(root.GetChildAs<string>("size", "x"), "a");
         Assert.False(root.GetChildAs<bool>("size", false));

         root = new SDL.Tag("root").ReadString("size `8`");
         Assert.Equal(root.GetChildAs<int>("size", 0), 8);
         Assert.Equal(root.GetChildAs<double>("size", 0), 8);
         Assert.Equal(root.GetChildAs<string>("size", "x"), "8");
         Assert.False(root.GetChildAs<bool>("size", false));
      }
   }
}
