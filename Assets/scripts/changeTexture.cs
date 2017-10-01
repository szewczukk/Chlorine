using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeTexture : MonoBehaviour
{
	public void setTexture(string name)
	{
		GetComponent<RawImage>().texture = Resources.Load(name) as Texture;
	}
}