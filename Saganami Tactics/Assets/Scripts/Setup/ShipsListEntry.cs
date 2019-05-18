using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ST
{
    public class ShipsListEntry : MonoBehaviour
    {
        public Ship Ship;

#pragma warning disable 0649
        [SerializeField] private TMP_Text shipName;
        [SerializeField] private Button deleteButton;
#pragma warning restore

        public UnityEvent OnDelete { get { return deleteButton.onClick; } }

        private void Start()
        {
            shipName.text = Ship.Name;
        }
    }
}
