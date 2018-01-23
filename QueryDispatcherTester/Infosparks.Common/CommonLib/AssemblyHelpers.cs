using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TenK.InfoSparks.Common
{
    public class AssemblyHelpers
    {
        public static Stream GetStream(string resourceName, Assembly containingAssembly)
        {
            var text = containingAssembly.GetManifestResourceNames();
            string fullResourceName = containingAssembly.GetName().Name + "." + resourceName;
            Stream result = containingAssembly.GetManifestResourceStream(fullResourceName);
            if (result == null)
            {
                // throw not found exception
            }

            return result;
        }


        public static string GetResourceString(string resourceName, Assembly containingAssembly)
        {
            string result = String.Empty;
            Stream sourceStream = GetStream(resourceName, containingAssembly);

            if (sourceStream != null)
            {
                using (StreamReader streamReader = new StreamReader(sourceStream))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            if (resourceName != null)
            {
                return result;
            }
            return "";
        }
    }
}
