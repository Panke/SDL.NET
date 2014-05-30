using SCG = System.Collections.Generic;
using NSubstitute;
using NUF = NUnit.Framework;
namespace SDL.Test {
   /**
     $make test TF='-run:SDL.Test.SimpleTagTest'
     */
   [NUF.TestFixture] public class ValueAsTest {
      [NUF.Test]public void get_int_child_as_T_sould_return_valid_value() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size 4");
         NUF.Assert.That(root.GetChildAs<int>("size", 0), NUF.Is.EqualTo(4));
         NUF.Assert.That(root.GetChildAs<double>("size", 0), NUF.Is.EqualTo(4D));
         NUF.Assert.That(root.GetChildAs<string>("size", "x"), NUF.Is.EqualTo("4"));
         NUF.Assert.That(root.GetChildAs<bool>("size", false), NUF.Is.True);

         NUF.Assert.That(root.GetChildAs<int>("no_value", 1), NUF.Is.EqualTo(1));
         NUF.Assert.That(root.GetChildAs<double>("no_value", 0), NUF.Is.EqualTo(0));
         NUF.Assert.That(root.GetChildAs<string>("no_value", "x"), NUF.Is.EqualTo("x"));
         NUF.Assert.That(root.GetChildAs<bool>("no_value", false), NUF.Is.False);
      }

      [NUF.Test]public void get_string_child_as_T_sould_return_valid_value() {
         SDL.Tag root = new SDL.Tag("root").ReadString("size `a`");
         NUF.Assert.That(root.GetChildAs<int>("size", 0), NUF.Is.EqualTo(0));
         NUF.Assert.That(root.GetChildAs<double>("size", 0), NUF.Is.EqualTo(0));
         NUF.Assert.That(root.GetChildAs<string>("size", "x"), NUF.Is.EqualTo("a"));
         NUF.Assert.That(root.GetChildAs<bool>("size", false), NUF.Is.False);

         root = new SDL.Tag("root").ReadString("size `8`");
         NUF.Assert.That(root.GetChildAs<int>("size", 0), NUF.Is.EqualTo(8));
         NUF.Assert.That(root.GetChildAs<double>("size", 0), NUF.Is.EqualTo(8));
         NUF.Assert.That(root.GetChildAs<string>("size", "x"), NUF.Is.EqualTo("8"));
         NUF.Assert.That(root.GetChildAs<bool>("size", false), NUF.Is.False);
      }
   }
}
