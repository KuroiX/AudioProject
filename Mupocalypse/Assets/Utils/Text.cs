using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public static class TextUtil
    {
        public static IEnumerator Type(Text textField, string sentence, float timeBetweenLetters = 0)
        {
            textField.text = "";
            foreach (var c in sentence.ToCharArray())
            {
                textField.text += c;
                yield return new WaitForSeconds(timeBetweenLetters);
            }
        }
    }
}