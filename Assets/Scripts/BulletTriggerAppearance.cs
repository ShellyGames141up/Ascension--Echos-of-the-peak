using UnityEngine;

public class BulletTriggerAppearance : MonoBehaviour
{
    [Header("Target Settings")]
    public string targetTag = "Target"; 
    public GameObject objectToAppear;  
    [Header("Bullet Settings")]
    public string bulletTag = "Bullet"; 
    private void Start()
    {
        if (objectToAppear != null)
        {
            objectToAppear.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bulletTag))
        {
            if (gameObject.CompareTag(targetTag))
            {
                MakeObjectAppear();
            }
        }
    }
    private void MakeObjectAppear()
    {
        if (objectToAppear != null)
        {
            objectToAppear.SetActive(true);
            Debug.Log("object is here");
            Collider collider = objectToAppear.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            UnityEngine.UI.Button button = objectToAppear.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }
}