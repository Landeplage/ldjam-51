using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
	// Singleton
	private static Singleton instance;
	public static Singleton Instance
	{
		get
		{
			return instance;
		}
	}

	void OnEnable()
	{
		if (instance != null && instance != this) {
			Destroy(gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
    }
}
