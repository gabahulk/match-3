using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.MainMenu
{
    public class MenuAnimation : MonoBehaviour
    {
    
        [SerializeField] private TMP_Text title;
        [SerializeField] private List<Button> buttons;
        // Start is called before the first frame update
        void Start()
        {
            title.color = Color.clear; 
            title.DOColor(Color.white, 0.5f);
            var index = 0;
            var delay = .15f;
            foreach (var button in buttons)
            {
                var sequence = DOTween.Sequence();
                var finalPos = button.transform.position;
                var pos = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - button.GetComponent<RectTransform>().rect.size.x, finalPos.y);
                button.transform.position = pos;
                sequence.Append(button.transform.DOMove(finalPos, 0.5f));
                sequence.PrependInterval(index * delay);
                index++;
            }
        }

    }
}
