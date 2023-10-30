using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject toggle;
    void Start()
    {
        Debug.Log("ToggleUI Start");
        // toggle = GameObject.Find("ToggleableObjects").GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Game Object Active? " + toggle.activeInHierarchy);
            toggle.SetActive(!toggle.activeInHierarchy);
        }
    }
}
