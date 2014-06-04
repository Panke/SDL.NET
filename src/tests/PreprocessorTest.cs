using SCG = System.Collections.Generic;
using NSubstitute;
using NUF = NUnit.Framework;
namespace SDL.Test {
   /**
     $make test TF='-run:SDL.Test.SimpleTagTest'
     */
   [NUF.TestFixture] public class PreprocessorTest {
      [NUF.Test]public void Int_test() {
         var stream = new System.IO.StreamReader("../root.sdl", System.Text.Encoding.UTF8);
         var pp = new Preprocessor(stream);
         string outTxt = pp.Process().ReadToEnd();
         string txt = "main_1"
            + "\nfoo_1"
            + "\nfoo_2"
            + "\nbar_1"
            + "\nbar_2"
            + "\nbar_3"
            + "\nfoo_last"
            + "\n#include"
            + "\nmain_2"
            + "\nmain_last\n";
         NUF.Assert.That(outTxt, NUF.Is.EqualTo(txt));
      }
   }
}
