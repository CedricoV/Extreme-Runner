using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
    public Text helpText;

    int i;

    #region singleton
    private static HelpText _instance;

    public static HelpText Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion


    public void ShowNext()
    {
        switch (i)
        {
            case 0:
                helpText.text = "Press <A> & <D> to turn left and right";
                break;
            case 1:
                helpText.text = "Hitting loose objects will reward points";
                break;
            case 2:
                helpText.text = "Press <Space> to jump (longer jumps will reward points)";
                break;
            case 3:
                helpText.text = "Press <C> to slide (doging objects will reward points)";
                break;
            case 4:
                helpText.text = "Losing too much speed or falling off will be game over";
                break;
            case 5:
                helpText.text = "Good luck!";
                Destroy(gameObject, 2f);
                break;
            default:
                break;
        }

        i++;
    }
}
