namespace SDL {
   public class Preprocessor {
      private readonly System.IO.TextReader reader;
      public Preprocessor(System.IO.TextReader reader) {
         this.reader = reader;
      }

      public System.IO.TextReader Process() {
         var writer = new System.Text.StringBuilder();
         Process(reader, writer);
         return new System.IO.StringReader(writer.ToString());
      }

      private void Process(System.IO.TextReader r, System.Text.StringBuilder w) {
         string line = string.Empty;
         while((line = r.ReadLine())!= null) {
            if (line.StartsWith("#include ")) {
               string file = GetFileName(line);
               if (System.IO.File.Exists(file)) {
                  var includeStream = new System.IO.StreamReader(file, System.Text.Encoding.UTF8);
                  Process(includeStream, w);
               }
            } else {
               w.AppendLine(line);
            }
         }
      }

      private string GetFileName(string line) {
         return line.Remove(0, 9).Trim();
      }
   }
}

