using UnityEngine;
using System;
using System.Collections;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: Firanel


namespace Utils
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string conditionalSourceField;
        public int enumIndex;
        public bool invertBool;

        public ConditionalHideAttribute(string boolVariableName, bool invert = false)
        {
            conditionalSourceField = boolVariableName;
            invertBool = invert;
        }

        public ConditionalHideAttribute(string enumVariableName, int enumIndex)
        {
            conditionalSourceField = enumVariableName;
            this.enumIndex = enumIndex;
        }

    }

}