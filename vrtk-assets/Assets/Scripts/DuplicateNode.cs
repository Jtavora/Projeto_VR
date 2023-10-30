using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DuplicateNode : MonoBehaviour
{
    public GameObject node;
    public TMP_InputField userInputField; // Reference to the Unity InputField where the user types the text
    public TMP_Text validationMessage; // Reference to the Unity Text where the validation message is displayed
    public LinkedListOptions linkedListOptionsScript; // Reference to the MyScript script

    private Vector3 firstNodeVector;
    private List<GameObject> nodeList = new List<GameObject>();

    public enum Operation
    {
        FIRST,
        LAST,
        ORDERED
    }

    void Start()
    {
        firstNodeVector = new Vector3(node.transform.position.x,
            node.transform.position.y, node.transform.position.z);
    }

    void Update()
    {
        
    }

    private void UpdateNodePosition()
    {
        nodeList[0].transform.position = new Vector3(firstNodeVector.x, nodeList[0].transform.position.y, nodeList[0].transform.position.z);
        for (int i = 1; i < nodeList.Count; i++)
        {
            nodeList[i].transform.position = new Vector3(nodeList[i - 1].transform.position.x - 0.659f, nodeList[i].transform.position.y, nodeList[i].transform.position.z);
        }
        
        // print("UpdateNodePosition["+0+"]: " + nodeList[0].name + " " + nodeList[0].transform.position.x + "\n"
        //      + "UpdateNodePosition["+(nodeList.Count-1)+"]: " + nodeList[nodeList.Count-1].name + " " + nodeList[nodeList.Count-1].transform.position.x + "\n");
    }

    public void CreateNewNode(Operation OperationType)
    {
        // input validation
        int newNodeNumber;
        if (string.IsNullOrEmpty(userInputField.text) || !int.TryParse(userInputField.text, out newNodeNumber))
        {
            validationMessage.text = "Please enter a valid number";
            return;
        }
       
        if (nodeList.Count == 10)
        {
            validationMessage.text = "The list is full";
            return;
        }
        
        validationMessage.text = "";

        GameObject newNode = Instantiate(node, firstNodeVector, Quaternion.identity);
        newNode.SetActive(true);
        newNode.transform.parent = transform;
        newNode.name = "Node" + nodeList.Count;

        newNode.GetComponentInChildren<TMP_Text>().text = userInputField.text;
        userInputField.text = "";
        int index = 0;
        switch (OperationType)
        {
            case Operation.FIRST:
                nodeList.Insert(0, newNode);
                break;
            case Operation.LAST:
                nodeList.Add(newNode);
                break;
            case Operation.ORDERED:
                for (; index < nodeList.Count; index++) 
                {
                    int nodeNumber = int.Parse(nodeList[index].GetComponentInChildren<TMP_Text>().text);
                    if (newNodeNumber < nodeNumber)
                    {
                        break;
                    }
                }
                nodeList.Insert(index, newNode);
                break;
                
        }
        UpdateNodePosition();
    }

    public void deleteNode2(int index)
    {
        if (nodeList.Count == 0)
        {
            validationMessage.text = "The list is empty";
            return;
        }
        if(index < 0 || index >= nodeList.Count)
        {
            validationMessage.text = "Please enter a valid index";
            return;
        }
        validationMessage.text = "";
        Destroy(nodeList[index]);
        nodeList.RemoveAt(index);
        UpdateNodePosition();
    }

    public void deleteNode(Operation OperationType)
    {
        // exception handling
        if (nodeList.Count == 0)
        {
            validationMessage.text = "The list is empty";
            return;
        }

        switch(OperationType)
        {
            case Operation.FIRST:
                Destroy(nodeList[0]);
                nodeList.RemoveAt(0);
                break;
            case Operation.LAST:
                Destroy(nodeList[nodeList.Count - 1]);
                nodeList.RemoveAt(nodeList.Count - 1);
                break;
        }
        UpdateNodePosition();
    }

    public void updatenode2(int index)
    {
        if (nodeList.Count == 0)
        {
            validationMessage.text = "The list is empty";
            return;
        }
        if (index < 0 || index >= nodeList.Count)
        {
            validationMessage.text = "Please enter a valid index";
            return;
        }
        validationMessage.text = "";
        nodeList[index].GetComponentInChildren<TMP_Text>().text = userInputField.text;
        userInputField.text = "";
    }
    
    public void updatenode(Operation OperationType)
    {
        // exception handling
        if (nodeList.Count == 0)
        {
            validationMessage.text = "The list is empty";
            return;
        }

        switch (OperationType)
        {
            case Operation.FIRST:
                nodeList[0].GetComponentInChildren<TMP_Text>().text = userInputField.text;
                userInputField.text = "";
                break;
            case Operation.LAST:
                nodeList[nodeList.Count - 1].GetComponentInChildren<TMP_Text>().text = userInputField.text;
                userInputField.text = "";
                break;
        }
    }
    public void ResetNodes()
    {
        if(nodeList.Count == 0)
        {
            validationMessage.text = "The list is already empty";
            return;
        }

        foreach (GameObject node in nodeList)
        {
            Destroy(node);
        }
        nodeList.Clear();
    }
}
