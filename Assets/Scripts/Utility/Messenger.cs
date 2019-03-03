using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messenger : MonoBehaviour {

	private static Messenger instance = null;
	public static Messenger Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<Messenger>();
			}
			return instance;
		}
	}

	public Dictionary<string, object> messages = new Dictionary<string, object>();

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public bool GetMessage(string messageID, out object message) {
		if (messages.ContainsKey(messageID)) {
			message = messages[messageID];
			messages.Remove(messageID);
			return true;
		} else {
			message = null;
			return false;
		}
	}

	public void SendMessage(string messageID, object message) {
		messages.Add(messageID, message);
	}
}
