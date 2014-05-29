using SCG = System.Collections.Generic;
using NSubstitute;
using NUF = NUnit.Framework;
namespace SDL.Test {
   /**
     $make test TF='-run:SDL.Test.SimpleTagTest'
     */
   [NUF.TestFixture] public class IkayzoTest {
      [NUF.Test]public void attributes_should_consistently_ordered() {
         Tag t1 = new Tag("test");
         t1["foo"]="bar";
         t1["john"]="doe";

         Tag t2 = new Tag("test");
         t2["john"]="doe";
         t2["foo"]="bar";
         NUF.Assert.That(t1, NUF.Is.EqualTo(t2));
      }
   }
}

