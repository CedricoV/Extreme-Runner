using UnityEngine;

public class ChaosArea : MonoBehaviour
{
    public int chaosAmount;

    public string chaosName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ChaosController.Instance.SetChaosText(this);
        }
    }
}
