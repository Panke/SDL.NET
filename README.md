An [SDL (Simple Declarative Language)](http://sdl.ikayzo.org/display/SDL/Language+Guide) library for C#.

The Simple Declarative Language provides an easy way to describe lists, maps, and trees of typed data in a compact, easy to read representation. The simple and intuitive API allows you to read, write, and access all the datastructures using a single class. For property files, configuration files, logs, and simple serialization requirements, SDL provides a compelling alternative to XML and Properties files. Implementations are available for Java and .NET; a port to C++ is in the works, with more languages on the way.

This is what SDL looks like (some of these examples, and more, are from [the SDL site](http://sdl.ikayzo.org/display/SDL/Language+Guide)):

```
first "Joe"
last "Coder"

numbers 12 53 2 635
names "Sally" "Frank N. Stein"
pets chihuahua="small" dalmation="hyper" mastiff="big"

mixed 34.7f "Tim" somedate=2010/08/14
```

```
folder "myFiles" color="yellow" protection=on {
    folder "my images" {
        file "myHouse.jpg" color=true date=2005/11/05
        file "myCar.jpg" color=false date=2002/01/05
    }
    folder "my documents" {
        document "resume.pdf"
    }
}
```

Tags are of this form:
```
[tag name] [values] [attributes] [children]
```

Tag and attribute names can optionally include a namespace prefix (ie, ```namespace:name```). All parts are optional, the only exception being that an anonymous (ie, no name) tag must have at least one value.

Also:
* Tags are separated by either newline or semicolon.
* Whitespace and indentation is not significant (other than newlines).
* The line-continuation operator is ```\``` (backslash). This can be used to split a tag across multiple lines.
* Line comments start with either ```#```, ```//``` or ```--```.
* Block comments start with ```/*``` and end with ```*/```.


Documentation
-------------
* [SDL site](http://sdl.ikayzo.org/display/SDL/Language+Guide)):
* [License]()

