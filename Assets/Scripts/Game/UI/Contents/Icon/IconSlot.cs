using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSlot : MonoBehaviour
{
    public void SetIconSlot(string iconName, string imgName)
    {
        ResourcesManager.LoadAddressableAsset<Sprite>(imgName, result =>
        {
	        var image = result;
	        GameObject iconObj = GameObject.Find(iconName + "/img_slot_value");
	        if (iconObj == null)
	        {
		        DebugManager.LogError("iconObj is null");
	        }
	        Image img = iconObj.GetComponent<Image>();
	        if (img == null)
	        {
		        DebugManager.LogError("Image Component is null");
	        }
	        
	        img.sprite = image;
        });
    }

    public void SetIconFrameColor(string iconName, AttributeType attribute)
    {
	    string frameName = GetAttributeFrameName(attribute);
	    
	    ResourcesManager.LoadAddressableAsset<Sprite>(frameName, result =>
	    {
		    var image = result;
		    GameObject iconObj = GameObject.Find(iconName + "/img_frame");
		    if (iconObj == null)
		    {
			    DebugManager.LogError("iconObj is null");
		    }
		    Image img = iconObj.GetComponent<Image>();
		    if (img == null)
		    {
			    DebugManager.LogError("Image Component is null");
		    }
	        
		    img.sprite = image;
	    });
    }

    private string GetAttributeFrameName(AttributeType attribute)
    {
	    string frameName = "ele_slot_frame_white";
	    
	    switch (attribute)
	    {
		    case AttributeType.Fire:
			    frameName = "ele_slot_frame_red";
			    break;
		    case AttributeType.Water:
			    frameName = "ele_slot_frame_blue";
			    break;
		    case AttributeType.Forest:
			    frameName = "ele_slot_frame_green";
			    break;
		    case AttributeType.Light:
			    frameName = "ele_slot_frame_gold";
			    break;
		    case AttributeType.Dark:
			    frameName = "ele_slot_frame_violet";
			    break;
	    }

	    return frameName;
    }
    
}
