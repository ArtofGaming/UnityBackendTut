using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    Action<string> _createItemsCallback;
    // Start is called before the first frame update
    void Start()
    {
        _createItemsCallback = (JsonArrayString) => {
            StartCoroutine(CreateItemsRoutine(JsonArrayString));
        };

        CreateItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateItems()
    {
        string userId = Main.Instance.UserInfo.UserID;
        StartCoroutine(Main.Instance.Web.GetItemIDs(userId, _createItemsCallback));
    }

    IEnumerator CreateItemsRoutine(string JsonArrayString)
    {
        JSONArray jsonArray = JSON.Parse(JsonArrayString) as JSONArray;

        for (int i = 0; i < jsonArray.Count; i++) 
        {
            bool isDone = false;
            string itemid = jsonArray[i].AsObject["itemID"];
            JSONObject itemInfoJson = new JSONObject();

            Action<string> getItemInfoCallback = (itemInfo) =>
            {
                isDone = true;
                JSONArray tempArray = JSON.Parse(itemInfo) as JSONArray;
                itemInfoJson = tempArray[0].AsObject;
            };

            StartCoroutine(Main.Instance.Web.GetItem(itemid, getItemInfoCallback));

            yield return new WaitUntil(() => isDone = true);

            GameObject item = Instantiate(Resources.Load("Prefabs/Item") as GameObject);
            item.transform.SetParent(this.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            item.transform.Find("Name").GetComponent<Text>().text = itemInfoJson["name"];
            item.transform.Find("Price").GetComponent<Text>().text = itemInfoJson["price"];
            item.transform.Find("Description").GetComponent<Text>().text = itemInfoJson["description"];


        }

    }
}
