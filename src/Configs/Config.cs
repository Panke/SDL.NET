using SCG = System.Collections.Generic;
using System.IO;
namespace SDL {
   public interface IOptions {
      void Initialize(Tag info);
   }

   public class Config<T> where T: IOptions, new() {
      public T GetDefaultOption() {
         string filename = GetSdlFileName("default");

         if (File.Exists(filename)) {
            return GetOption("default");
         } else {
            return new T();
         }
      }

      public T GetOption(string[] args) {
         if (args.Length > 0) {
            return this.GetOption(args[0]);
         } else {
            return this.GetDefaultOption();
         }
      }

      public T GetOption(string key) {
         if (string.IsNullOrEmpty(key)) {
            throw new System.ArgumentException(key);
         }
         string filename = GetSdlFileName(key);
         var root = new Tag("root").ReadFile(filename);
         var opt = new T();
         opt.Initialize(root);
         return opt;
      }

      private string GetSdlFileName(string key) {
         return Path.Combine("../config", key) + ".sdl";
      }
   }
}
