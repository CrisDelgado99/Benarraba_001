using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Attributes
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int objectsQuantityOnStart;

    private List<GameObject> objectsPool = new List<GameObject>();
    #endregion

    #region Main Methods
    void Start()
    {
        CreateObjects(objectsQuantityOnStart);
    }
    #endregion

    #region Event Functions
    /// <summary>
    /// This method creates a quantity of objects
    /// </summary>
    /// <param name="quantityOfObjects"></param>
    private void CreateObjects(int quantityOfObjects)
    {
        //Create objects
        for(int i = 0; i < quantityOfObjects; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// This method instanciates a new object and adds it to the list
    /// </summary>
    /// <returns>A created new object</returns>
    private GameObject CreateNewObject()
    {
        //Instantiate anywhere
        GameObject newObject = Instantiate(objectPrefab);
        //Deactivate the object
        newObject.SetActive(false);
        //Add to the list
        objectsPool.Add(newObject);
        return newObject;
    }

    /// <summary>
    /// This method takes from the list an available object and creates
    /// a new one if it does not exist
    /// </summary>
    /// <returns></returns>
    public GameObject GetGameObject()
    {
        //Find in the objectsPool an object which is inactive in the game hierarchy
        GameObject theObject = objectsPool.Find(objectInList => objectInList.activeInHierarchy == false);

        //If list is empty, create an object
        if(theObject == null)
        {
            theObject = CreateNewObject();
        }

        theObject.SetActive(true);

        return theObject;
    }
    #endregion

}
