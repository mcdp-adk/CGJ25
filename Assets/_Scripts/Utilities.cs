using System.Collections;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public static class Utilities
    {
        public static IEnumerator DestroyAfterDelay(GameObject gameObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (gameObject) Object.Destroy(gameObject);
        }

        public static void SetTalkText(string text)
        {
            var talkText = GameObject.FindWithTag("TalkText");
            if (!talkText) return;
            var textMesh = talkText.GetComponent<TextMeshProUGUI>();
            if (!textMesh) return;
            textMesh.text = text;
        }
    }
}