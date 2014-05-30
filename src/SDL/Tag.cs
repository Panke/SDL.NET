/*
 * Simple Declarative Language (SDL) for .NET
 * Copyright 2005 Ikayzo, inc.
 *
 * This program is free software. You can distribute or modify it under the
 * terms of the GNU Lesser General Public License version 2.1 as published by
 * the Free Software Foundation.
 *
 * This program is distributed AS IS and WITHOUT WARRANTY. OF ANY KIND,
 * INCLUDING MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, contact the Free Software Foundation, Inc.,
 * 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SDL {
   /**
     SDL documents are composed of tags.  A tag is composed of a name and
     optionally a value list, attribute list, and child list.  Tags and
     attributes may specify namespaces.
     @author Daniel Leuck from Ikayzo
     */
   public class Tag {
      private string sdlNamespace;
      private string name;

      private List<object> values;
      private Dictionary<string, string> attributeToNamespace;
      private SortedDictionary<string, object> attributes;
      private List<Tag> children;

      /// Remove and replace with read only views once .NET adds proper
      /// read only generic lists and maps
      private List<object> valuesSnapshot;
      private bool valuesDirty;
      private SortedDictionary<string, object> attributesSnapshot;
      private bool attributesDirty;
      private Dictionary<string, string> attributeToNamespaceSnapshot;
      private List<Tag> childrenSnapshot;
      private bool childrenDirty;

      /**
       * Create an empty tag with a name and no namespace
       * @param name The name of this tag
       * @exception System.ArgumentException If the \cname is not a valid SDL identifier
       */
      public Tag(string name) : this("", name) {}

      /**
      * Create a tag with the given namespace and name
      * @param sdlNamespace The namespace for this tag.  null will be coerced to the empty string("")
      * @param name The name for this tag
      * @exception System.ArgumentException If <c>name</c> is not
      * a valid SDL identifier or <c>sdlNamespace</c> is not empty and is
      * not a valid SDL identifier
      */
      public Tag(string sdlNamespace, string name) {
         SDLNamespace = sdlNamespace;
         Name = name;

         values = new List<object>();
         attributeToNamespace = new Dictionary<string, string>();
         attributes = new SortedDictionary<string, object>();
         children = new List<Tag>();

         /// Remove and replace with read only views once .NET adds proper
         /// read only generic lists and maps
         valuesSnapshot = new List<object>();
         attributeToNamespaceSnapshot = new Dictionary<string, string>();
         attributesSnapshot = new SortedDictionary<string, object>();
         childrenSnapshot = new List<Tag>();
      }

      /**
       * The tag's name.  Must be a valid SDL identifier.
       */
      public string Name {
         set {
            name = SDLUtil.ValidateIdentifier(value);
         }
         get {
            return name;
         }
      }

      /**
       * The tag's namespace.  Must be a valid SDL identifier or empty.
       */
      public string SDLNamespace {
         set {
            if (value == null) {
               value = string.Empty;
            }

            if (value.Length != 0) {
               SDLUtil.ValidateIdentifier(value);
            }

            sdlNamespace = value;
         }
         get {
            return sdlNamespace;
         }
      }

      /**
      * A convenience property that sets and gets the first value in the
      * value list.
      * @exception If the value is not coercible to an SDL type
      */
      public object Value {
         set {
            value = SDLUtil.CoerceOrFail(value);

            if (values.Count == 0) {
               AddValue(value);
            } else {
               values[0] = value;
            }

            valuesDirty = true;
         }
         get {
            if (values.Count == 0) {
               return null;
            }
            return values[0];
         }
      }

      public T ValueAs<T>(T returnValueIfException) {
         try {
            return ValueAs<T>();
         } catch {
            return returnValueIfException;
         }
      }

      public T ValueAs<T>() {
         if (this.Value is T) {
            return (T)this.Value;
         } else {
            var destinationType = typeof(T);
            if (destinationType.IsNullable()) {
               destinationType = new System.ComponentModel.NullableConverter(destinationType).UnderlyingType;
            }
            return (T)System.Convert.ChangeType(this.Value, destinationType);
         }

      }
      /**
       * A property for the tag's children.  When read, this property returns
       * a copy of the children.
       */
      public IList<Tag> Children {
         set {
            childrenDirty = true;
            children = new List<Tag>(value);
         }

         get {
            if (childrenDirty) {
               childrenSnapshot = new List<Tag>(children);
            }

            return childrenSnapshot;
         }
      }

      /**
       * A property for the tag's values.  When read, this property returns
       * a copy of the values.
       */
      public IList<object> Values {
         set {
            valuesDirty = true;

            // we need to use this instead of a copy constructor because
            // validation is required for value (performed by AddValue(obj))
            values.Clear();
            foreach (object v in value) {
               AddValue(v);
            }
         }

         get {
            if (valuesDirty) {
               valuesSnapshot = new List<object>(values);
            }

            return valuesSnapshot;
         }
      }

      // HERE! TODO: From here down - validate all values using CoerceOrFail

      /**
       * A property for the tag's attributes.  When read, this property
       * returns a copy of the attributes.
       */
      public IDictionary<string, object> Attributes {
         set {
            attributesDirty = true;

            // we need to use this instead of a copy constructor because
            // validation is required for the key and the value
            // (performed by the indexer)
            attributes.Clear();
            foreach (string key in value.Keys) {
               this[key] = value[key];
            }
         }

         get {
            if (attributesDirty) {
               attributesSnapshot =
                  new SortedDictionary<string, object>(attributes);
            }

            return attributesSnapshot;
         }
      }

      /**
       * A property for the mapping of this tag's attributes to their
       * respective namespaces.  Attributes with no namespace are mapped to
       * an empty string ("")
       *
       * When read, this property returns a copy of the mapping.  It is
       * not write-through.
       *
       * This property is read only
       */
      public IDictionary<string, string> AttributeToNamespace {
         get {
            if (attributesDirty) {
               attributeToNamespaceSnapshot =
                  new Dictionary<string, string>(attributeToNamespace);
            }

            return attributeToNamespaceSnapshot;
         }
      }


      /**
       * An indexer for the tag's attributes.  This indexer sets the
       * attribute's namespace to an empty string ("")
       * @param key The key for this attribute
       * @returnThe value associated with the \ckey
       */
      public object this[string key] {
         get {
            return attributes[key];
         }

         set {
            this["", key] = value;
         }
      }

      /**
       * Set the <c>key</c> to the given value and sets the namespace.
       * @param sdlNamespace The namespace for this attribute
       * @param key The key for this attribute
       */
      public object this[string sdlNamespace, string key] {
         set {
            attributesDirty = true;
            attributes[SDLUtil.ValidateIdentifier(key)] =
               SDLUtil.CoerceOrFail(value);

            if (sdlNamespace == null) {
               sdlNamespace = "";
            }
            if (sdlNamespace.Length != 0) {
               SDLUtil.ValidateIdentifier(sdlNamespace);
            }

            attributeToNamespace[key] = sdlNamespace;
         }
      }

      /**
       * An indexer for the tag's values
       * @param index The <c>index</c> to get or set
       * @return The value at the given \cindex
       */
      public object this[int index] {
         get {
            return values[index];
         }

         set {
            valuesDirty = true;
            values[index] = SDLUtil.CoerceOrFail(value);
         }
      }


      /**
       * Add a child to this tag
       * @param child The child to add
       */
      public void AddChild(Tag child) {
         childrenDirty = true;
         children.Add(child);
      }

      /**
       * Remove a child from this tag
       * @param child The child to remove
       * @return true if the child is removed
       */
      public bool RemoveChild(Tag child) {
         childrenDirty = true;
         return children.Remove(child);
      }

      /**
       * Add a value to this tag
       * @param value The value to add
       */
      public void AddValue(object value) {
         valuesDirty = true;
         values.Add(SDLUtil.CoerceOrFail(value));
      }

      /**
       * Remove a child from this tag
       * @param child The child to remove
       * @return true if the child is removed
       */
      public bool RemoveValue(object value) {
         valuesDirty = true;
         return values.Remove(value);
      }

      /**
       * Get all the children of this tag optionally recursing through all
       * descendents.
       * @param recursively If true, recurse through all descendents
       * @return A snapshot of the children
       */
      public IList<Tag> GetChildren(bool recursively) {
         if (!recursively) {
            return this.Children;
         }

         List<Tag> kids = new List<Tag>();
         foreach (Tag t in this.Children) {
            kids.Add(t);

            if (recursively) {
               kids.AddRange(t.GetChildren(true));
            }
         }

         return kids;
      }

      /**
       * Get the first child with the given name.  The search is not
       * recursive.
       * @param childName The name of the child Tag
       * @return The first child tag having the given name or null if no
       * such child exists
       */
      public Tag GetChild(string childName) {
         return GetChild(childName, false);
      }

      public U GetChildAs<U>(string name, U defaultValue) {
         SDL.Tag tag = GetChild(name);
         return tag == null ? defaultValue : tag.ValueAs<U>(defaultValue);
      }

      /**
       * Get the first child with the given name, optionally using a
       * recursive search.
       * @param childName The name of the child Tag
       * @param recursive If the search should be recursive
       * @return The first child tag having the given name or null if no
       * such child exists
       */
      public Tag GetChild(string childName, bool recursive) {
         foreach (Tag t in children) {
            if (t.Name.Equals(childName)) {
               return t;
            }

            if (recursive) {
               Tag rc = t.GetChild(childName, true);
               if (rc != null) {
                  return rc;
               }
            }
         }

         return null;
      }

      /**
       * Get all children with the given name.  The search is not recursive.
       * @param childName The name of the children to fetch
       * @return All the child tags having the given name
       */
      public IList<Tag> GetChildren(string childName) {
         return GetChildren(childName, false);
      }

      /**
       * Get all the children with the given name (optionally searching
       * descendants recursively)
       * @param childName The name of the children to fetch
       * @param recursive If true search all descendents
       * @return All the child tags having the given name
       */
      public IList<Tag> GetChildren(string childName, bool recursive) {
         List<Tag> kids = new List<Tag>();
         foreach (Tag t in children) {
            if (t.Name.Equals(childName)) {
               kids.Add(t);
            }

            if (recursive) {
               kids.AddRange(t.GetChildren(childName, true));
            }
         }

         return kids;
      }

      /**
       * Get the values for all children with the given name.  If the child
       * has more than one value, all the values will be added as a list.  If
       * the child has no value, null will be added.  The search is not
       * recursive.
       * @param name The name of the children from which values are
       * retrieved
       * @return A list of values (or lists of values)
       */
      public IList<object> GetChildrenValues(string name) {
         List<object> results = new List<object>();
         IList<Tag> children = GetChildren(name);

         foreach (Tag t in children) {
            IList<object> values = t.Values;
            if (values.Count == 0) {
               results.Add(null);
            } else if (values.Count == 1) {
               results.Add(values[0]);
            } else {
               results.Add(values);
            }
         }

         return results;
      }

      /**
       * Get all children in the given namespace.  The search is not
       * recursive.
       * @param The namespace to search
       * @return All the child tags in the given namespace
       */
      public IList<Tag> GetChildrenForNamespace(string sdlNamespace) {
         return GetChildrenForNamespace(sdlNamespace, false);
      }

      /**
       * Get all the children in the given namespace (optionally searching
       * descendants recursively)
       * @param sdlNamespace The namespace of the children to
       * fetch
       * @param recursive If true search all descendents
       * @return All the child tags in the given namespace
       */
      public IList<Tag> GetChildrenForNamespace(string sdlNamespace,
            bool recursive) {

         List<Tag> kids = new List<Tag>();
         foreach (Tag t in children) {
            if (t.SDLNamespace.Equals(sdlNamespace)) {
               kids.Add(t);
            }

            if (recursive)
               kids.AddRange(t.GetChildrenForNamespace(sdlNamespace,
                        true));
         }

         return kids;
      }

      /**
       * Returns all attributes in the given namespace.
       * @param sdlNamespace The namespace to search
       * @return All attributes in the given namespace
       */
      public IDictionary<string, object> GetAttributesForNamespace(
            string sdlNamespace) {

         SortedDictionary<string, object> atts =
            new SortedDictionary<string, object>();

         foreach (string key in attributeToNamespace.Keys) {
            if (attributeToNamespace[key].Equals(sdlNamespace)) {
               atts[key] = attributes[key];
            }
         }

         return atts;
      }

      // Methods for reading in SDL input ////////////////////////////////////

      /**
       * Add all the tags specified in the file at the given URL to this Tag.
       * @param url url A UTF8 encoded .sdl file
       * @return This tag after adding all the children read from the reader

       * @exception System.IO.IOException If there is an IO problem
       while reading the source
       * @exception SDLParseException If the SDL input is malformed
       */
      public Tag ReadURL(string url) {
         WebRequest request = WebRequest.Create(url);
         Stream input = request.GetResponse().GetResponseStream();

         return Read(new StreamReader(input, System.Text.Encoding.UTF8));
      }

      /**
       * Add all the tags specified in the given file to this Tag.
       * @param file A UTF8 encoded .sdl file
       * @return This tag after adding all the children read from the reader
      ///
       * @exception System.IO.IOException If there is an IO problem
       * while reading the source
       * @exception SDLParseException If the SDL input is malformed
       */
      public Tag ReadFile(string file) {
         return Read(new StreamReader(file, System.Text.Encoding.UTF8));
      }

      /**
       * Add all the tags specified in the given String to this Tag.
       * @param text An SDL string
       * @return This tag after adding all the children read from the reader
       *
       * @exception SDLParseException If the SDL input is malformed
       *
       */
      public Tag ReadString(string text) {
         return Read(new StringReader(text));
      }

      /**
       * Add all the tags specified in the given Reader to this Tag.
       * @param reader A reader containing SDL source
       * @return This tag after adding all the children read from the reader
       *
       * @exception System.IO.IOException If there is an IO problem
       * while reading the source
       * @exception SDLParseException If the SDL input is malformed
       *
       */
      public Tag Read(TextReader reader) {
         IList<Tag> tags = new Parser(reader).Parse();
         foreach (Tag t in tags) {
            AddChild(t);
         }
         return this;
      }

      /**
       * Write this tag out to the given file.
       * @param file The file path to which we will write the children
       * of this tag.
       * @exception IOException If there is an IO problem during the
       * write operation
       */
      public void WriteFile(string file) {
         WriteFile(file, false);
      }

      /**
       * Write this tag out to the given file (optionally clipping the root.)
       * @param file The file path to which we will write this tag

       * @param includeRoot If true this tag will be included in the
       * file as the root element, if false only the children will be written

       * @exception IOException If there is an IO problem during the
       * write operation
       */
      public void WriteFile(string file, bool includeRoot) {
         Write(new StreamWriter(file, false, System.Text.Encoding.UTF8),
               includeRoot);

      }

      /**
       * Write this tag out to the given writer (optionally clipping the
       * root.)
       * @param writer The writer to which we will write this tag

       * @param includeRoot If true this tag will be written out as
       * the root element, if false only the children will be written
       * @exception IOException If there is an IO problem during the
       * write operation
       */
      public void Write(TextWriter writer, bool includeRoot) {
         //O-FIXME: sostituire con NewLine
         string newLine = "\r\n";

         if (includeRoot) {
            writer.Write(ToString());
         } else {
            for (int i = 0; i < children.Count; i++) {
               writer.Write(children[i].ToString());
               if (i < children.Count - 1) {
                  writer.Write(newLine);
               }
            }
         }

         writer.Close();
      }

      /**
       * Produces a full SDL "dump" of this tag.  The output is valid SDL.
       * @return SDL code describing this tag
       */
      public override string ToString() {
         return ToString("");
      }

      /**
       * Produces a full SDL "dump" of this tag using the given prefix before
       * each line.
       * @param linePrefix Text to be prefixed to each line
       * @return SDL code describing this tag
       */
      string ToString(string linePrefix) {
         StringBuilder sb = new StringBuilder();
         sb.Append(linePrefix);

         bool skipValueSpace = false;
         if (sdlNamespace.Length == 0 && name.Equals("content")) {
            skipValueSpace = true;
         } else {
            if (sdlNamespace.Length != 0) {
               sb.Append(sdlNamespace).Append(':');
            }
            sb.Append(name);
         }

         // output values
         if (values.Count != 0) {

            if (skipValueSpace == true) {
               skipValueSpace = false;
            } else {
               sb.Append(" ");
            }

            int size = values.Count;
            for (int i = 0; i < size; i++) {
               sb.Append(SDLUtil.Format(values[i]));
               if (i < size - 1) {
                  sb.Append(" ");
               }
            }
         }

         // output attributes
         if (attributes.Count != 0) {
            foreach (string key in attributes.Keys) {
               sb.Append(" ");

               string attNamespace = AttributeToNamespace[key];
               if (!attNamespace.Equals("")) {
                  sb.Append(attNamespace + ":");
               }
               sb.Append(key + "=");
               sb.Append(SDLUtil.Format(attributes[key]));
            }
         }

         // output children
         if (children.Count != 0) {
            sb.Append(" {\r\n");
            foreach (Tag t in children) {
               sb.Append(t.ToString(linePrefix + "    ") + "\r\n");
            }
            sb.Append(linePrefix + "}");
         }

         return sb.ToString();
      }

      /**
       * Returns a string containing an XML representation of this tag.
       * Values will be represented using _val0, _val1, etc.
       * @return An XML String describing this Tag
       */
      public string ToXMLString() {
         return ToXMLString("");
      }

      /**
       * Returns a string containing an XML representation of this tag.
       * Values will be represented using _val0, _val1, etc.
       * @param linePrefix A prefix to insert before every line.
       * @return An XML String describing this Tag
       */
      string ToXMLString(string linePrefix) {
         string newLine = "\r\n";

         if (linePrefix == null) {
            linePrefix = "";
         }

         StringBuilder builder = new StringBuilder(linePrefix + "<");
         if (!sdlNamespace.Equals("")) {
            builder.Append(sdlNamespace + ":");
         }
         builder.Append(name);

         // output values
         if (values.Count != 0) {
            int i = 0;
            foreach (object val in values) {
               builder.Append(" ");
               builder.Append("_val" + i + "=\"" + SDLUtil.Format(val,
                        false) + "\"");

               i++;
            }
         }

         // output attributes
         if (attributes.Count != 0) {
            foreach (string key in attributes.Keys) {
               builder.Append(" ");
               string attNamespace = attributeToNamespace[key];

               if (!attNamespace.Equals("")) {
                  builder.Append(attNamespace + ":");
               }
               builder.Append(key + "=");
               builder.Append("\"" + SDLUtil.Format(attributes[key], false)
                     + "\"");
            }
         }

         if (children.Count != 0) {
            builder.Append(">" + newLine);
            foreach (Tag t in children) {
               builder.Append(t.ToXMLString(linePrefix + "    ") + newLine);
            }

            builder.Append(linePrefix + "</");
            if (!sdlNamespace.Equals("")) {
               builder.Append(sdlNamespace + ":");
            }
            builder.Append(name + ">");
         } else {
            builder.Append("/>");
         }

         return builder.ToString();
      }

      /**
       * Tests for equality using \cToString()
       * @param obj The object to test
       * @return true if \cToString().Equals(obj.ToString)
       */
      public override bool Equals(object obj) {
         if (obj == null) {
            return false;
         }
         return ToString().Equals(obj.ToString());
      }

      /**
       * Generates a hashcode using <c>ToString().GetHashCode()</c>
       * @return A unique hashcode for this tag
       */
      public override int GetHashCode() {
         return ToString().GetHashCode();
      }
   }
}
