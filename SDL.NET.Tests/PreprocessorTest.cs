using Xunit;
using SCG = System.Collections.Generic;

using SDL.NET;

namespace SDL.NET.Tests {

   public class PreprocessorTest {
      [Fact]
      public void Int_test() {
         var stream = new System.IO.StreamReader("./testdata/root.sdl", System.Text.Encoding.UTF8);
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
         Assert.Equal(outTxt, txt);
      }
   }
}
