

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class GeneratingUniqueNames
    {
        private static int Counter = 0;

        // Note: we use '.' to make sure this attribute is not colliding with
        // any property defined by the user.
        public static readonly XName UniqueNameAttribute = GeneratingCode.xNamespace.GetName("__.UniqueName.__");

        public static void ProcessDocument(XDocument doc)
        {
            TraverseNextElement(doc.Root, "");
        }

        private static void TraverseNextElement(XElement currentElement, string path)
        {
            // If the current element is an object (rather than a property)
            if (!currentElement.Name.LocalName.Contains("."))
            {
                // Generate unique name
                string uniqueName = currentElement.Name.LocalName + "_" + path;

                // Assign unique name
                currentElement.SetAttributeValue(UniqueNameAttribute, uniqueName);
            }

            var counter = 0;
            // Recursion:
            foreach (var childElements in currentElement.Elements())
            {
                TraverseNextElement(childElements, path + "_" + counter);
                counter++;
            }
        }

        internal static string GenerateUniqueName(string str)
        {
            ReadOnlySpan<char> prefix = str.AsSpan();

            if (prefix.Length > 30)
            {
                prefix = prefix.Slice(0, 30);
            }

            // Because of f# warning, makes the first letter lower case
            return $"{char.ToLower(prefix[0])}{prefix.Slice(1)}_{Counter++}"; // Example: Button_4541C363579C48A981219C392BF8ACD5
        }
    }
}
