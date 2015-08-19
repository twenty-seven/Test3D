using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IpValidator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		InputField inputField = gameObject.GetComponent<InputField>();

		
		inputField.onValidateInput += ValidateInput;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	 
	public char ValidateInput(string text, int charIndex, char addedChar)
	{
		Debug.Log (text);
		Debug.Log (addedChar);
		return text [charIndex];
	}
}
