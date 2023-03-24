using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class EntryScript : MonoBehaviour
    {
        [SerializeField] private Animator portalAnimator, playerAnimator, entryAnimator;

        [Header("Disabled GameObjects")]
        [SerializeField] private GameObject[] disabledObjects;
        [SerializeField] private GameObject portal;
        [SerializeField] private GameObject player;

        private void Start()
        {
            entryAnimator.Play("Entry", 0);
            Invoke("EnablePortal", 9.5f);
        }

        private void EnablePortal()
        {
            //Disable Title/Black Card
            foreach (var obj in disabledObjects)
            {
                obj.SetActive(false);
            }

            portal.SetActive(true);
            portalAnimator.Play("Entry", 0);
            Invoke("EnablePlayer", 1f);
        }

        private void EnablePlayer()
        {
            player.SetActive(true);
            playerAnimator.Play("Entry", 0);
        }
    }
}