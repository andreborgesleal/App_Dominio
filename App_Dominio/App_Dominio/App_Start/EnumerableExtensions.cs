using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Dominio.App_Start
{
    public static class EnumerableExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
            typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            if (attribs != null)
                return attribs.Length > 0 ? attribs[0].StringValue : null;

            return null;
        }

        public static string GetStringValue(Type type, string value)
        {
            // Get fieldinfo for this type
            System.Reflection.FieldInfo fieldInfo = type.GetField(value);

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
            typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            if (attribs != null)
                return attribs.Length > 0 ? attribs[0].StringValue : null;

            return null;
        }

        public static string GetStringDescription(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringdescription attributes
            StringDescriptionAttribute[] attribs = fieldInfo.GetCustomAttributes(
            typeof(StringDescriptionAttribute), false) as StringDescriptionAttribute[];

            // Return the first if there was a match.
            if (attribs != null)
                return attribs.Length > 0 ? attribs[0].StringDescription : null;
            return null;
        }

        public static string GetStringDescription(Type type, string value)
        {
            // Get fieldinfo for this type
            System.Reflection.FieldInfo fieldInfo = type.GetField(value);

            // Get the stringdescription attributes
            StringDescriptionAttribute[] attribs = fieldInfo.GetCustomAttributes(
            typeof(StringDescriptionAttribute), false) as StringDescriptionAttribute[];

            // Return the first if there was a match.
            if (attribs != null)
                return attribs.Length > 0 ? attribs[0].StringDescription : null;
            return null;
        }

        public static string GetName(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Return the Enum's name
            return type.GetField(value.ToString()).Name;
        }

        public static object GetValue(this Enum value)
        {
            object output = null;
            int enumValue;
            if (int.TryParse(value.ToString("D"), out enumValue))
                output = enumValue;

            // Return the Enum's value
            return output;
        }

    }

    public class StringValueAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
        #endregion
    }

    public class StringDescriptionAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Holds the stringdescription for a value in an enum.
        /// </summary>
        public string StringDescription { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor used to init a StringDescription Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringDescriptionAttribute(string value)
        {
            this.StringDescription = value;
        }
        #endregion
    }
}