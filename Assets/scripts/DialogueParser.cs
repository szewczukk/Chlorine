using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour
{
	struct Reference
	{
		public string text;
		public int idTo;
	}

	struct Dialogue
	{
		public int id;
		public string name;
		public string quote;
		public List<Reference> options;
	}

	public string fileName;

	public int currentDialogue = 0;

	public Text personName;
	public Text personQuote;
	
	public Button buttonPrefab;

	private List<Dialogue> dialogues = new List<Dialogue>();

	// Use this for initialization
	void Start ()
	{
		using (StreamReader stream = new StreamReader("Assets/dialogues/" + fileName))
		{
			string line;

			while ((line = stream.ReadLine()) != null)
			{
				if(line.StartsWith("#"))
				{
					string fileName = line.Substring(1);

					var image = GameObject.Find("background");
					image.GetComponent<changeTexture>().setTexture(fileName);
				
					continue;
				}
				if (line.StartsWith("!"))
				{
					string fileName = line.Substring(1);

					var image = GameObject.Find("person");
					image.GetComponent<changeTexture>().setTexture(fileName);

					continue;
				}
				if (line.StartsWith("("))
				{
					Dialogue dialogue = new Dialogue();
					dialogue.options = new List<Reference>();

					string rawName = line.Split(')')[0];
					dialogue.name = rawName.Substring(rawName.LastIndexOf('(') + 1);

					string rawQuote = line.Split(']')[0];
					dialogue.quote = rawQuote.Substring(rawQuote.LastIndexOf('[') + 1);

					string rawId = line.Split('}')[0];
					int.TryParse(rawId.Substring(rawId.LastIndexOf('{') + 1), out dialogue.id);

					dialogues.Add(dialogue);

					continue;
				}
				if (line.StartsWith("<"))
				{
					Reference r = new Reference();

					string rawOption = line.Split('>')[0];
					r.text = rawOption.Substring(rawOption.LastIndexOf('<') + 1);

					string rawId = line.Split('}')[0];
					int id;
					int.TryParse(rawId.Substring(rawId.LastIndexOf('{') + 1), out id);
					r.idTo = id;

					dialogues[dialogues.Count - 1].options.Add(r);
				}
			}
		}

		setCurrent(currentDialogue);
	}

	void buttonOnClickReference(int id)
	{
		destroyButtons();
		setCurrent(id);
	}

	void destroyButtons()
	{
		var buttons = GameObject.FindGameObjectsWithTag("Button");

		foreach(var button in buttons)
		{
			Destroy(button);
		}
	}

	void setCurrent(int currentId)
	{
		personName.text = dialogues[currentId].name;
		personQuote.text = dialogues[currentId].quote;

		foreach (Reference option in dialogues[currentId].options)
		{
			Button button = Instantiate(buttonPrefab,
					new Vector3(dialogues[currentId].options.LastIndexOf(option) * 200.0f + 300, 125, 0),
					Quaternion.identity);
			button.tag = "Button";
			button.GetComponentInChildren<Text>().text = option.text;
			button.onClick.AddListener(delegate { buttonOnClickReference(option.idTo); });

			button.transform.SetParent(gameObject.transform);
		}
	}
}