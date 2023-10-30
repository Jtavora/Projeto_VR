using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Required for accessing Toggle class
using TMPro;

public class LinkedListOptions : MonoBehaviour
{
    // Reference to the Unity Toggle button
    public Toggle begginingToggle;
    public Toggle endingToggle;
    public Toggle orderedToggle;
    public Toggle indexToggle;

    // Reference to the Unity Dropdown menu
    public TMP_Dropdown operationsDropdownMenu;
    public TMP_InputField nodeInputField; // Reference to the Unity InputField where the node types the text
    public TMP_InputField indexInputField; // Reference to the Unity InputField where the index types the text

    public EventSystem eventSystem; // Reference to the EventSystem game object
    private DuplicateNode duplicateNodeScript; // Reference to the DuplicateNode script
    
    void Start()
    {
        // Debug.Log("LinkedListOptions Start");
        // insertion options as default
        begginingToggle.gameObject.SetActive(true);
        begginingToggle.isOn = true;
        endingToggle.gameObject.SetActive(true);
        orderedToggle.gameObject.SetActive(true);
        nodeInputField.gameObject.SetActive(true);
        indexInputField.gameObject.SetActive(false);
        indexToggle.gameObject.SetActive(false);
        indexInputField.gameObject.SetActive(false);

        duplicateNodeScript = eventSystem.GetComponent<DuplicateNode>();
        // Add listener for when the state of the Toggle changes, to take action
        begginingToggle.onValueChanged.AddListener((value) => {
            if (value)
            {
                // Debug.Log("Beggining Toggle is on");
                endingToggle.isOn = false;
                orderedToggle.isOn = false;
                indexToggle.isOn = false;
            }
        });
        endingToggle.onValueChanged.AddListener((value) => {
            if (value)
            {
                // Debug.Log("Ending Toggle is on");
                begginingToggle.isOn = false;
                orderedToggle.isOn = false;
                indexToggle.isOn = false;
            }
        });
        orderedToggle.onValueChanged.AddListener((value) => {
            if (value)
            {
                // Debug.Log("Ordered Toggle is on");
                begginingToggle.isOn = false;
                endingToggle.isOn = false;
            }
        });
        indexToggle.onValueChanged.AddListener((value) => {
            indexInputField.gameObject.SetActive(value);
            if (value)
            {
                // Debug.Log("Index Toggle is on");
                begginingToggle.isOn = false;
                endingToggle.isOn = false;
            }
        });

        operationsDropdownMenu.onValueChanged.AddListener((value) => {
            if (value == 0)
            {
                // Debug.Log("Insertion");
                begginingToggle.gameObject.SetActive(true);
                begginingToggle.isOn = true;
                endingToggle.gameObject.SetActive(true);
                orderedToggle.gameObject.SetActive(true);
                nodeInputField.gameObject.SetActive(true);
                indexInputField.gameObject.SetActive(false);
                indexToggle.gameObject.SetActive(false);
                indexInputField.gameObject.SetActive(false);
            }
            else if (value == 1)
            {
                // Debug.Log("Empty List");
                begginingToggle.gameObject.SetActive(false);
                endingToggle.gameObject.SetActive(false);
                orderedToggle.gameObject.SetActive(false);
                indexToggle.gameObject.SetActive(false);
                nodeInputField.gameObject.SetActive(false);
                duplicateNodeScript.ResetNodes();
                indexInputField.gameObject.SetActive(false);
            }
            else if (value == 2)
            {
                // Debug.Log("Deletion");
                begginingToggle.gameObject.SetActive(true);
                indexToggle.isOn = true;
                endingToggle.gameObject.SetActive(true);
                indexToggle.gameObject.SetActive(true);
                orderedToggle.gameObject.SetActive(false);
                nodeInputField.gameObject.SetActive(false);
                indexInputField.gameObject.SetActive(true);
            }
            else if(value == 3)
            {
                Debug.Log("Update node");
                begginingToggle.gameObject.SetActive(true);
                indexToggle.isOn = true;
                endingToggle.gameObject.SetActive(true);
                indexToggle.gameObject.SetActive(true);
                orderedToggle.gameObject.SetActive(false);
                nodeInputField.gameObject.SetActive(true);
                indexInputField.gameObject.SetActive(true);
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        // insertion
        if(operationsDropdownMenu.value == 0 && Input.GetKeyDown(KeyCode.Return))
        {
            // remove the return from nodeInputField
            nodeInputField.text = nodeInputField.text.Replace("\r", "").Replace("\n", "");
            if(begginingToggle.isOn)
            {
                duplicateNodeScript.CreateNewNode(DuplicateNode.Operation.FIRST);
            }
            else if(endingToggle.isOn)
            {
                duplicateNodeScript.CreateNewNode(DuplicateNode.Operation.LAST);
            }
            else if(orderedToggle.isOn)
            {
                duplicateNodeScript.CreateNewNode(DuplicateNode.Operation.ORDERED);
            }
            else
            {
                duplicateNodeScript.validationMessage.text = "Please select an insertion type";
            }
        } 
        else if (operationsDropdownMenu.value == 2 && Input.GetKeyDown(KeyCode.Return))
        {
            // remove the return from indexInputField
            indexInputField.text = indexInputField.text.Replace("\r", "").Replace("\n", "");
            if (begginingToggle.isOn)
            {
                duplicateNodeScript.deleteNode(DuplicateNode.Operation.FIRST);
            }
            else if (endingToggle.isOn)
            {
                duplicateNodeScript.deleteNode(DuplicateNode.Operation.LAST);
            }
            else if(indexToggle.isOn)
            {
                duplicateNodeScript.deleteNode2(int.Parse(indexInputField.text));
                indexInputField.text = "";
            }
            else
            {
                duplicateNodeScript.validationMessage.text = "Please select a deletion type";
            }
        }
        else if (operationsDropdownMenu.value == 3 && Input.GetKeyDown(KeyCode.Return))
        {
            // remove the return from indexInputField
            indexInputField.text = indexInputField.text.Replace("\r", "").Replace("\n", "");
            if (begginingToggle.isOn)
            {
                duplicateNodeScript.updatenode(DuplicateNode.Operation.FIRST);
            }
            else if (endingToggle.isOn)
            {
                duplicateNodeScript.updatenode(DuplicateNode.Operation.LAST);
            }
            else if (indexToggle.isOn)
            {
                duplicateNodeScript.updatenode2(int.Parse(indexInputField.text));
                indexInputField.text = "";
            }
            else
            {
                duplicateNodeScript.validationMessage.text = "Please select a deletion type";
            }
        }
    }
}
