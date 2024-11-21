using TMPro;
using UnityEngine;

namespace Runtime
{
   public class GameVersion : MonoBehaviour
   {
      [SerializeField] private TextMeshProUGUI text;

      private void Start()
      {
         text.SetText($"v{Application.version}");
      }
   }
}
