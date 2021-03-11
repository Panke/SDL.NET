using System.ComponentModel;
namespace SDL {
   public static class TypeExtensions {
      public static T ChangeType<T>(this object source, T returnValueIfException) {
         try {
            return source.ChangeType<T>();
         } catch {
            return returnValueIfException;
         }
      }

      public static T ChangeType<T>(this object source) {
         if (source is T) {
            return (T)source;
         } else {
            var destinationType = typeof(T);
            if (destinationType.IsNullable()) {
               destinationType = new NullableConverter(destinationType).UnderlyingType;
            }
            return (T)System.Convert.ChangeType(source, destinationType);
         }
      }

      public static bool IsNullable(this System.Type type) {
        return type.IsGenericType
               && (type.GetGenericTypeDefinition() == typeof(System.Nullable<>));
      }
   }
}
